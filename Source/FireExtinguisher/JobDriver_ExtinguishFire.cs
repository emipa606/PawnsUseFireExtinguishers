using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace FireExtinguisher;

public class JobDriver_ExtinguishFire : JobDriver
{
    private const TargetIndex FireIndex = TargetIndex.A;

    private Toil CheckDestroyedToil(TargetIndex targetIndex)
    {
        Toil toil = ToilMaker.MakeToil("CheckDestroyedToil");
        toil.initAction = delegate
        {
            Fire fire = (Fire)(Thing)pawn.CurJob.GetTarget(targetIndex);
            if(fire == null || fire.Destroyed)
            {
                CastUtils.LastCheck.Remove(pawn.thingIDNumber);
                pawn.records.Increment(RecordDefOf.FiresExtinguished);
                pawn.jobs.EndCurrentJob(JobCondition.Succeeded);
            }
        };
        return toil;
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        Toil equip = InventoryUtils.EquipFireExtinguisherToil();
        equip.EndOnDespawnedOrNull(TargetIndex.A, JobCondition.Ongoing | JobCondition.Succeeded);
        yield return Toils_Combat.TrySetJobToUseAttackVerb(TargetIndex.A);
        Toil checkDestroyed = CheckDestroyedToil(TargetIndex.A);
        Toil toil = CastUtils.GotoCastPosition(TargetIndex.A);
        toil.JumpIfDespawnedOrNull(TargetIndex.A, checkDestroyed);
        yield return toil;
        Toil toil2 = Toils_Combat.CastVerb(TargetIndex.A);
        toil2.JumpIfDespawnedOrNull(TargetIndex.A, checkDestroyed);
        yield return toil2;
        yield return checkDestroyed;
        yield return Toils_Jump.Jump(equip);
    }

    public override bool TryMakePreToilReservations(bool errorOnFailed) { return true; }
}
