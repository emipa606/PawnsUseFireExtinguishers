using System.Reflection;
using HarmonyLib;
using Verse;

namespace FireExtinguisher;

[StaticConstructorOnStartup]
internal static class HarmonyPatches
{
    static HarmonyPatches()
    {
        new Harmony("pointfeev.fireextinguisher").PatchAll(Assembly.GetExecutingAssembly());
    }
}