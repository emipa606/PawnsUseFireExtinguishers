using RimWorld;
using Verse;
using Verse.AI;

namespace FireExtinguisher;

public class WorkGiver_ExtinguishFire : WorkGiver_Scanner
{
	public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(ThingDefOf.Fire);

	public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
	{
		if (!(t is Fire fire))
		{
			return false;
		}
		if (fire.parent is Pawn pawn2)
		{
			if (pawn2 == pawn)
			{
				return false;
			}
			if ((pawn2.Faction == pawn.Faction || pawn2.HostFaction == pawn.Faction || pawn2.HostFaction == pawn.HostFaction) && IntVec3Utility.ManhattanDistanceFlat(pawn.Position, pawn2.Position) > 15)
			{
				return false;
			}
		}
		else if (!pawn.Map.areaManager.Home[fire.Position])
		{
			JobFailReason.Is(WorkGiver_FixBrokenDownBuilding.NotInHomeAreaTrans);
			return false;
		}
		if (InventoryUtils.CanEquipFireExtinguisher(pawn))
		{
			IntVec3 intVec = pawn.Position - fire.Position;
			if ((intVec.LengthHorizontalSquared <= 225 || pawn.CanReserve(fire, 1, -1, null, forced)) && !FireIsBeingHandled(fire, pawn) && CastUtils.CanGotoCastPosition(pawn, fire, out intVec, fromWorkGiver: true))
			{
				return InventoryUtils.EquipFireExtinguisher(pawn);
			}
		}
		return false;
	}

	public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
	{
		return JobMaker.MakeJob(JobDefOf_ExtinguishFire.ExtinguishFire, t);
	}

	private static bool FireIsBeingHandled(Fire f, Pawn potentialHandler)
	{
		if (f.Spawned)
		{
			return f.Map.reservationManager.FirstRespectedReserver(f, potentialHandler) != null;
		}
		return false;
	}
}
