using HarmonyLib;
using Verse;
using Verse.AI;

namespace FireExtinguisher;

[HarmonyPatch(typeof(Pawn_JobTracker), nameof(Pawn_JobTracker.EndCurrentJob))]
public static class Pawn_JobTracker_EndCurrentJob
{
    internal static bool Prefix(Pawn_JobTracker __instance, Pawn ___pawn)
    {
        if(__instance?.curJob?.def != JobDefOf_ExtinguishFire.ExtinguishFire || ___pawn == null)
        {
            return true;
        }
        InventoryUtils.UnEquipFireExtinguisher(___pawn);
        CastUtils.LastCheck.Remove(___pawn.thingIDNumber);
        return true;
    }
}

