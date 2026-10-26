using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace FireExtinguisher;

internal static class CastUtils
{
    private const float DefaultMaxRangeFactor = 0.95f;

    internal static readonly Dictionary<int, float> LastCheck = new();

    internal static bool CanGotoCastPosition(Pawn actor, Thing thing, out IntVec3 intVec, bool fromWorkGiver)
    {
        intVec = default;
        Verb verb = null;
        if (InventoryUtils.GetFireExtinguisherFromEquipment(actor) == null && fromWorkGiver)
        {
            var fireExtinguisherFromInventory = InventoryUtils.GetFireExtinguisherFromInventory(actor);
            if (fireExtinguisherFromInventory != null)
            {
                verb = InventoryUtils.GetPrimaryVerb(fireExtinguisherFromInventory);
                verb?.caster = actor;
            }
        }
        else
        {
            var curJob = actor.jobs.curJob;
            if (curJob != null)
            {
                verb = curJob.verbToUse;
            }

            verb ??= actor.TryGetAttackVerb(thing);
        }

        if (verb == null)
        {
            return false;
        }

        var num = !fromWorkGiver && LastCheck.TryGetValue(actor.thingIDNumber, out var value)
            ? value - 0.15f
            : DefaultMaxRangeFactor;
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
        var toil = ToilMaker.MakeToil();
        toil.initAction = delegate
        {
            var actor = toil.actor;
            var jobs = actor.jobs;
            var pather = actor.pather;
            var thing = jobs.curJob.GetTarget(targetInd).Thing;
            if (actor == thing || thing == null)
            {
                pather.StopDead();
                jobs.curDriver.ReadyForNextToil();
            }
            else if (!CanGotoCastPosition(actor, thing, out var intVec, false))
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