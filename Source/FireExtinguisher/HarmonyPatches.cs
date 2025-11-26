using HarmonyLib;
using RimWorld;
using System.Reflection;
using Verse;
using Verse.AI;

namespace FireExtinguisher;

[StaticConstructorOnStartup]
internal static class HarmonyPatches
{
    static HarmonyPatches()
    {
        new Harmony("pointfeev.fireextinguisher").PatchAll(Assembly.GetExecutingAssembly());
    }
}

