using HarmonyLib;
using RimWorld;
using Verse;

namespace FireExtinguisher;

[HarmonyPatch(typeof(CompShield), nameof(CompShield.CompAllowVerbCast))]
public static class CompShield_CompAllowVerbCast
{
    internal static void Postfix(ref bool __result, Verb verb)
    {
        if(__result)
        {
            return;
        }
        if(InventoryUtils.CanWeaponExtinguish(verb?.EquipmentSource))
        {
            __result = true;
        }
    }
}

