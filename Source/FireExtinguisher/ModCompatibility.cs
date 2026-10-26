using System.Reflection;
using CompatUtils;
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
            [typeof(ThingWithComps)],
            true);
    }

    private static bool HasAmmo(ThingWithComps thingWithComps)
    {
        if ((object)CombatExtendedHasAmmoMethod != null)
        {
            return (bool)CombatExtendedHasAmmoMethod.Invoke(null, [thingWithComps]);
        }

        return true;
    }

    internal static bool CheckWeapon(ThingWithComps thingWithComps)
    {
        return thingWithComps != null && HasAmmo(thingWithComps);
    }
}