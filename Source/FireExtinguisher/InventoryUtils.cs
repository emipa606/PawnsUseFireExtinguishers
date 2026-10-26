using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace FireExtinguisher;

internal static class InventoryUtils
{
    private static readonly List<string> ExtinguishDamageDefNames =
    [
        "VWE_Extinguish",
        "FExtExtinguish"
    ];

    private static readonly Dictionary<int, ThingWithComps> PreviousWeapons = new();

    private static bool CanEquipFireExtinguisher(Pawn pawn, Thing extinguisher)
    {
        if (pawn == null || extinguisher?.def == null || pawn.WorkTagIsDisabled(WorkTags.Firefighting))
        {
            return false;
        }

        if (pawn.WorkTagIsDisabled(WorkTags.Violent))
        {
            return !extinguisher.def.IsRangedWeapon;
        }

        return true;
    }

    private static bool EquipWeapon(Pawn pawn, ThingWithComps weapon, bool cachePrevious = false)
    {
        if (pawn == null || weapon == null || !UnEquipWeapon(pawn, cachePrevious))
        {
            return false;
        }

        if (weapon.stackCount > 1)
        {
            weapon = weapon.SplitOff(1) as ThingWithComps;
        }

        weapon?.holdingOwner?.Remove(weapon);

        pawn.equipment.AddEquipment(weapon);
        weapon?.def?.soundInteract?.PlayOneShot(new TargetInfo(pawn.Position, pawn.Map));
        return true;
    }

    private static bool UnEquipWeapon(Pawn pawn, bool cachePrevious = false)
    {
        if (pawn == null)
        {
            return false;
        }

        var primary = pawn.equipment.Primary;
        if (primary == null)
        {
            return true;
        }

        if (pawn.inventory.innerContainer.TryAddOrTransfer(primary))
        {
            if (cachePrevious)
            {
                PreviousWeapons.SetOrAdd(pawn.thingIDNumber, primary);
            }

            return true;
        }

        Log.Error($"[FireExtinguisher] Failed to un-equip weapon for {pawn.LabelShort}: {primary.Label}");
        return false;
    }

    internal static bool CanEquipFireExtinguisher(Pawn pawn)
    {
        return GetFireExtinguisherFromEquipment(pawn) != null ||
               CanEquipFireExtinguisher(pawn, GetFireExtinguisherFromInventory(pawn));
    }

    internal static bool CanWeaponExtinguish(ThingWithComps weapon)
    {
        var damageDef = GetPrimaryVerb(weapon)?.GetDamageDef();
        if (damageDef == null)
        {
            return false;
        }

        return damageDef == DamageDefOf.Extinguish || ExtinguishDamageDefNames.Contains(damageDef.defName);
    }

    internal static bool EquipFireExtinguisher(Pawn pawn, bool cachePrevious = true)
    {
        return GetFireExtinguisherFromEquipment(pawn) != null ||
               EquipWeapon(pawn, GetFireExtinguisherFromInventory(pawn), cachePrevious);
    }

    internal static Toil EquipFireExtinguisherToil()
    {
        var toil = ToilMaker.MakeToil();
        toil.initAction = delegate
        {
            var actor = toil.actor;
            var jobs = actor.jobs;
            if (GetFireExtinguisherFromEquipment(actor) == null)
            {
                EquipFireExtinguisher(actor, !PreviousWeapons.ContainsKey(actor.thingIDNumber));
            }

            if (GetFireExtinguisherFromEquipment(actor) == null)
            {
                jobs.EndCurrentJob(JobCondition.Ongoing | JobCondition.Succeeded);
            }

            jobs.curDriver.ReadyForNextToil();
        };
        return toil;
    }

    internal static ThingWithComps GetFireExtinguisherFromEquipment(Pawn pawn)
    {
        var thingWithComps = pawn?.equipment?.Primary;
        if (thingWithComps == null ||
            !CanWeaponExtinguish(thingWithComps) ||
            !ModCompatibility.CheckWeapon(thingWithComps))
        {
            return null;
        }

        return thingWithComps;
    }

    internal static ThingWithComps GetFireExtinguisherFromInventory(Pawn pawn)
    {
        return pawn?.inventory?.innerContainer?.FirstOrFallback(thing => thing is ThingWithComps thingWithComps &&
                                                                         CanWeaponExtinguish(thingWithComps) &&
                                                                         ModCompatibility.CheckWeapon(thingWithComps))
            as ThingWithComps;
    }

    internal static Verb GetPrimaryVerb(ThingWithComps weapon)
    {
        return weapon?.GetComp<CompEquippable>()?.PrimaryVerb;
    }

    internal static void UnEquipFireExtinguisher(Pawn pawn)
    {
        if (GetFireExtinguisherFromEquipment(pawn) == null)
        {
            return;
        }

        if (!PreviousWeapons.TryGetValue(pawn.thingIDNumber, out var value))
        {
            return;
        }

        if (value != pawn.equipment?.Primary && (!UnEquipWeapon(pawn) || !EquipWeapon(pawn, value)))
        {
            return;
        }

        PreviousWeapons.Remove(pawn.thingIDNumber);
    }
}