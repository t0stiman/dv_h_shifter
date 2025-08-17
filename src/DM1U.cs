using System;
using System.Linq;
using DV.CabControls;
using DV.KeyboardInput;
using UnityEngine;

namespace dv_h_shifter;

public class DM1U: ShiftStrategy
{
	private LeverBase gearLever;
	private int gearDelta;

	//7 gears including neutral
	protected override int numberOfGears => 7;
	
	public DM1U(TrainCar car)
	{
		var gearInput = car.interior
			.GetComponentsInChildren<MouseScrollKeyboardInput>()
			.FirstOrDefault(anInput => anInput.scrollAction.name == "GearAIncrement");
		
		if (!gearInput)
		{
			Main.Error($"DM1U {nameof(gearInput)} not found");
			return;
		}
		
		gearLever = gearInput.gameObject.GetComponent<LeverBase>();
	}

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
			Main.LogDebug($"shifting up");
			gearLever.Scroll(ScrollAction.ScrollUp);
			gearDelta--;
		}
		else if (gearDelta < 0)
		{
			Main.LogDebug($"shifting down");
			gearLever.Scroll(ScrollAction.ScrollDown);
			gearDelta++;
		}
		
		previousLeverMoveTime = DateTime.UtcNow;
	}

	protected override void CalculateGearDelta(int wantedGear)
	{
		Main.LogDebug("engage gear "+wantedGear);

		var currentGear = GetSelectedGear();
		gearDelta = wantedGear - currentGear;
	}
	
	private int GetSelectedGear()
	{
		// 0 - 1 -> 0 - 6
		return Mathf.RoundToInt(gearLever.Value * (numberOfGears-1));
	}
}