using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace FireExtinguisher;

internal static class CastUtils
{
    private const float DefaultMaxRangeFactor = 0.95f;

    internal static readonly Dictionary<int, float> LastCheck = new Dictionary<int, float>();

    internal static bool CanGotoCastPosition(Pawn actor, Thing thing, out IntVec3 intVec, bool fromWorkGiver)
    {
        intVec = default(IntVec3);
        Verb verb = null;
        if (InventoryUtils.GetFireExtinguisherFromEquipment(actor) == null && fromWorkGiver)
        {
            ThingWithComps fireExtinguisherFromInventory = InventoryUtils.GetFireExtinguisherFromInventory(actor);
            if (fireExtinguisherFromInventory != null)
            {
                verb = InventoryUtils.GetPrimaryVerb(fireExtinguisherFromInventory);
                if (verb != null)
                {
                    verb.caster = actor;
                }
            }
        }
        else
        {
            Job curJob = actor.jobs.curJob;
            if (curJob != null)
            {
                verb = curJob.verbToUse;
            }
            if (verb == null)
            {
                verb = actor.TryGetAttackVerb(thing);
            }
        }
        if (verb == null)
        {
            return false;
        }
        float value;
        float num = ((!fromWorkGiver && LastCheck.TryGetValue(actor.thingIDNumber, out value)) ? (value - 0.15f) : 0.95f);
        LastCheck.SetOrAdd(actor.thingIDNumber, num);
        return CastPositionFinder.TryFindCastPosition(
            new CastPositionRequest
            {
                caster = actor,
                target = thing,
                verb = verb,
                maxRangeFromTarget = Math.Max(verb.verbProps.range * num, 1.42f),
                wantCoverFromTarget = false
            },
            out intVec);
    }

    internal static Toil GotoCastPosition(TargetIndex targetInd)
    {
        Toil toil = ToilMaker.MakeToil("GotoCastPosition");
        toil.initAction = delegate
        {
            Pawn actor = toil.actor;
            Pawn_JobTracker jobs = actor.jobs;
            Pawn_PathFollower pather = actor.pather;
            Thing thing = jobs.curJob.GetTarget(targetInd).Thing;
            IntVec3 intVec;
            if (actor == thing || thing == null)
            {
                pather.StopDead();
                jobs.curDriver.ReadyForNextToil();
            }
            else if (!CanGotoCastPosition(actor, thing, out intVec, fromWorkGiver: false))
            {
                jobs.EndCurrentJob(JobCondition.Ongoing | JobCondition.Succeeded);
            }
            else
            {
                pather.StartPath(intVec, PathEndMode.OnCell);
                actor.Map.pawnDestinationReservationManager.Reserve(actor, jobs.curJob, intVec);
            }
        };
        toil.defaultCompleteMode = ToilCompleteMode.PatherArrival;
        return toil;
    }
}
