using System;
using System.Collections.Generic;
using DV.CabControls;
using UnityEngine;

namespace dv_h_shifter;

public class DM3: ShiftStrategy
{
	/**
	Gear lever positions (A-B) from lowest to highest gear. 1 means pulled towards player, 3 the opposite.
	1-1
	1-2
	2-1
	2-2
	3-1
	3-2
	2-3
	3-3
	 */
	private static readonly List<DM3GearLeversState> gearLeversPositions = new
	(new[] {
		new DM3GearLeversState (1, 1),
		new DM3GearLeversState (1, 2),
		new DM3GearLeversState (2, 1),
		new DM3GearLeversState (2, 2),
		new DM3GearLeversState (3, 1),
		new DM3GearLeversState (3, 2),
		new DM3GearLeversState (2, 3),
		new DM3GearLeversState (3, 3)
	});
	
	// difference between the current GearLeversState and the state we want to achieve
	private DM3GearLeversState gearDelta = new();

	protected override int numberOfGears => 8; 
	
	public DM3(TrainCar car) : base(car) {}

	protected override void MoveGearLevers()
	{
		if (gearDelta.posA == 0 && gearDelta.posB == 0)
		{
			return;
		}
		
		// moving the lever 2 notches doesn't work if we do it too quickly
		if ((DateTime.UtcNow - previousLeverMoveTime).TotalMilliseconds < 100)
		{
			return;
		}
		
		if (gearDelta.posA > 0)
		{
			Main.Debug("shifting A up");
			gearLeverA.Scroll(ScrollAction.ScrollUp);
			gearDelta.posA--;
		}
		else if (gearDelta.posA < 0)
		{
			Main.Debug("shifting A down");
			gearLeverA.Scroll(ScrollAction.ScrollDown);
			gearDelta.posA++;
		}
		
		if (gearDelta.posB > 0)
		{
			Main.Debug("shifting B up");
			gearLeverB.Scroll(ScrollAction.ScrollUp);
			gearDelta.posB--;
		}
		else if (gearDelta.posB < 0)
		{
			Main.Debug("shifting B down");
			gearLeverB.Scroll(ScrollAction.ScrollDown);
			gearDelta.posB++;
		}
		
		previousLeverMoveTime = DateTime.UtcNow;
	}
	
	protected override void CalculateGearDelta(int wantedGear)
	{
		//no neutral in the DM3 so we start at 1
		wantedGear++;
		Main.Debug("engage gear "+wantedGear);

		var currentState = new DM3GearLeversState(GetGearLeverPosition(gearLeverA), GetGearLeverPosition(gearLeverB));
		var wantedState = gearLeversPositions[wantedGear - 1];
		gearDelta = wantedState - currentState;
	}

	private static int GetGearLeverPosition(LeverBase gearLever)
	{
		// 0 -> 1, 0,5 -> 2, 1 -> 3
		return Mathf.RoundToInt(gearLever.Value * 2 + 1);
	}
}
