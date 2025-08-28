using System;
using UnityEngine;

namespace dv_h_shifter;

public class DM1U: ShiftStrategy
{
	protected int gearDelta;

	//7 gears including neutral
	protected override int numberOfGears => 7;
	
	public DM1U(TrainCar car) : base(car) {}

	protected override void MoveGearLevers()
	{
		if (gearDelta == 0)
		{
			return;
		}
		
		// moving the lever 2 notches doesn't work if we do it too quickly
		if ((DateTime.UtcNow - previousLeverMoveTime).TotalMilliseconds < 100)
		{
			return;
		}
			
		if (gearDelta > 0)
		{
			Main.Debug($"shifting up");
			gearLeverA.Scroll(ScrollAction.ScrollUp);
			gearDelta--;
		}
		else if (gearDelta < 0)
		{
			Main.Debug($"shifting down");
			gearLeverA.Scroll(ScrollAction.ScrollDown);
			gearDelta++;
		}
		
		previousLeverMoveTime = DateTime.UtcNow;
	}

	protected override void CalculateGearDelta(int wantedGear)
	{
		Main.Debug("engage gear "+wantedGear);

		var currentGear = GetSelectedGear();
		gearDelta = wantedGear - currentGear;
	}
	
	private int GetSelectedGear()
	{
		// 0 - 1 -> 0 - numberOfGears-1
		return Mathf.RoundToInt(gearLeverA.Value * (numberOfGears-1));
	}
}