using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace FireExtinguisher;

public class JobDriver_ExtinguishFire : JobDriver
{
    private const TargetIndex FireIndex = TargetIndex.A;

    private Toil CheckDestroyedToil(TargetIndex targetIndex)
    {
        var toil = ToilMaker.MakeToil();
        toil.initAction = delegate
        {
            var fire = (Fire)(Thing)pawn.CurJob.GetTarget(targetIndex);
            if (fire is { Destroyed: false })
            {
                return;
            }

            CastUtils.LastCheck.Remove(pawn.thingIDNumber);
            pawn.records.Increment(RecordDefOf.FiresExtinguished);
            pawn.jobs.EndCurrentJob(JobCondition.Succeeded);
        };
        return toil;
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        var equip = InventoryUtils.EquipFireExtinguisherToil();
        equip.EndOnDespawnedOrNull(FireIndex, JobCondition.Ongoing | JobCondition.Succeeded);
        yield return Toils_Combat.TrySetJobToUseAttackVerb(FireIndex);
        var checkDestroyed = CheckDestroyedToil(FireIndex);
        var toil = CastUtils.GotoCastPosition(FireIndex);
        toil.JumpIfDespawnedOrNull(FireIndex, checkDestroyed);
        yield return toil;
        var toil2 = Toils_Combat.CastVerb(FireIndex);
        toil2.JumpIfDespawnedOrNull(FireIndex, checkDestroyed);
        yield return toil2;
        yield return checkDestroyed;
        yield return Toils_Jump.Jump(equip);
    }

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return true;
    }
}