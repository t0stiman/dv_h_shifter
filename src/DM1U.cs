using System;
using DV.CabControls.NonVR;
using UnityEngine;
using WindowsInput.Native;

namespace dv_h_shifter;

public class DM1U: ShiftStrategy
{
	private static LeverNonVR gearLever;
	private int gearDelta;

	//7 gears including neutral
	protected override int numberOfGears => 7;

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
			Main.Log($"pressing up");
			inputSim.Keyboard.KeyPress(VirtualKeyCode.NUMPAD5);
			gearDelta--;
		}
		else if (gearDelta < 0)
		{
			Main.Log($"pressing down");
			inputSim.Keyboard.KeyPress(VirtualKeyCode.NUMPAD2);
			gearDelta++;
		}
		
		previousLeverMoveTime = DateTime.UtcNow;
	}

	protected override void CalculateGearDelta(int wantedGear)
	{
		Main.Log("engage gear "+wantedGear);

		var currentGear = GetSelectedGear();
		gearDelta = wantedGear - currentGear;
	}
	
	private int GetSelectedGear()
	{
		if (!gearLever)
		{
			var gearSelect = GameObject.Find("C_GearSelect");
			if (gearSelect is null)
			{
				Main.Error($"Can't find {nameof(DM1U)} gearbox");
				return 0;
			}

			gearLever = gearSelect.GetComponent<LeverNonVR>();
		}
		
		// 0 - 1 -> 0 - 6
		return Mathf.RoundToInt(gearLever.Value * (numberOfGears-1));
	}
}