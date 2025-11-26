using CompatUtils;
using System;
using System.Reflection;
using Verse;

namespace FireExtinguisher;

[StaticConstructorOnStartup]
internal static class ModCompatibility
{
    private static readonly MethodInfo CombatExtendedHasAmmoMethod;

    static ModCompatibility()
    {
        CombatExtendedHasAmmoMethod = Compatibility.GetConsistentMethod(
            "ceteam.combatextended",
            "CombatExtended.CE_Utility",
            "HasAmmo",
            new Type[1] { typeof(ThingWithComps) },
            logError: true);
    }

    private static bool HasAmmo(ThingWithComps thingWithComps)
    {
        if((object)CombatExtendedHasAmmoMethod != null)
        {
            return (bool)CombatExtendedHasAmmoMethod.Invoke(null, [thingWithComps]);
        }
        return true;
    }

    internal static bool CheckWeapon(ThingWithComps thingWithComps)
    {
        if(thingWithComps != null)
        {
            return HasAmmo(thingWithComps);
        }
        return false;
    }
}
