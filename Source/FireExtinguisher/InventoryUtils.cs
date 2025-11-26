using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace FireExtinguisher;

internal static class InventoryUtils
{
    private static readonly List<string> ExtinguishDamageDefNames = new List<string>
    {
        "VWE_Extinguish",
        "FExtExtinguish"
    };

    private static readonly Dictionary<int, ThingWithComps> PreviousWeapons = new Dictionary<int, ThingWithComps>();

    private static bool CanEquipFireExtinguisher(Pawn pawn, Thing extinguisher)
    {
        if(pawn != null && extinguisher?.def != null && !pawn.WorkTagIsDisabled(WorkTags.Firefighting))
        {
            if(pawn.WorkTagIsDisabled(WorkTags.Violent))
            {
                return !extinguisher.def.IsRangedWeapon;
            }
            return true;
        }
        return false;
    }

    private static bool EquipWeapon(Pawn pawn, ThingWithComps weapon, bool cachePrevious = false)
    {
        if(pawn == null || weapon == null || !UnEquipWeapon(pawn, cachePrevious))
        {
            return false;
        }
        if(weapon.stackCount > 1)
        {
            weapon = weapon.SplitOff(1) as ThingWithComps;
        }
        if(weapon?.holdingOwner != null)
        {
            weapon.holdingOwner.Remove(weapon);
        }
        pawn.equipment.AddEquipment(weapon);
        weapon?.def?.soundInteract?.PlayOneShot(new TargetInfo(pawn.Position, pawn.Map));
        return true;
    }

    private static bool UnEquipWeapon(Pawn pawn, bool cachePrevious = false)
    {
        if(pawn == null)
        {
            return false;
        }
        ThingWithComps primary = pawn.equipment.Primary;
        if(primary == null)
        {
            return true;
        }
        if(pawn.inventory.innerContainer.TryAddOrTransfer(primary))
        {
            if(cachePrevious)
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
        if(GetFireExtinguisherFromEquipment(pawn) == null)
        {
            return CanEquipFireExtinguisher(pawn, GetFireExtinguisherFromInventory(pawn));
        }
        return true;
    }

    internal static bool CanWeaponExtinguish(ThingWithComps weapon)
    {
        DamageDef damageDef = GetPrimaryVerb(weapon)?.GetDamageDef();
        if(damageDef != null)
        {
            if(damageDef != DamageDefOf.Extinguish)
            {
                return ExtinguishDamageDefNames.Contains(damageDef.defName);
            }
            return true;
        }
        return false;
    }

    internal static bool EquipFireExtinguisher(Pawn pawn, bool cachePrevious = true)
    {
        if(GetFireExtinguisherFromEquipment(pawn) == null)
        {
            return EquipWeapon(pawn, GetFireExtinguisherFromInventory(pawn), cachePrevious);
        }
        return true;
    }

    internal static Toil EquipFireExtinguisherToil()
    {
        Toil toil = ToilMaker.MakeToil("EquipFireExtinguisherToil");
        toil.initAction = delegate
        {
            Pawn actor = toil.actor;
            Pawn_JobTracker jobs = actor.jobs;
            if(GetFireExtinguisherFromEquipment(actor) == null)
            {
                EquipFireExtinguisher(actor, !PreviousWeapons.ContainsKey(actor.thingIDNumber));
            }
            if(GetFireExtinguisherFromEquipment(actor) == null)
            {
                jobs.EndCurrentJob(JobCondition.Ongoing | JobCondition.Succeeded);
            }
            jobs.curDriver.ReadyForNextToil();
        };
        return toil;
    }

    internal static ThingWithComps GetFireExtinguisherFromEquipment(Pawn pawn)
    {
        ThingWithComps thingWithComps = pawn?.equipment?.Primary;
        if(thingWithComps == null ||
            !CanWeaponExtinguish(thingWithComps) ||
            !ModCompatibility.CheckWeapon(thingWithComps))
        {
            return null;
        }
        return thingWithComps;
    }

    internal static ThingWithComps GetFireExtinguisherFromInventory(Pawn pawn)
    {
        return pawn?.inventory?.innerContainer?.FirstOrFallback(
            (Thing thing) => thing is ThingWithComps thingWithComps &&
            CanWeaponExtinguish(thingWithComps) &&
            ModCompatibility.CheckWeapon(thingWithComps)) as ThingWithComps;
    }

    internal static Verb GetPrimaryVerb(ThingWithComps weapon)
    { return weapon?.GetComp<CompEquippable>()?.PrimaryVerb; }

    internal static bool UnEquipFireExtinguisher(Pawn pawn)
    {
        if(GetFireExtinguisherFromEquipment(pawn) == null)
        {
            return true;
        }
        if(!PreviousWeapons.TryGetValue(pawn.thingIDNumber, out var value))
        {
            return true;
        }
        if(value == pawn.equipment?.Primary)
        {
            PreviousWeapons.Remove(pawn.thingIDNumber);
            return true;
        }
        if(UnEquipWeapon(pawn) && EquipWeapon(pawn, value))
        {
            return PreviousWeapons.Remove(pawn.thingIDNumber);
        }
        return false;
    }
}
