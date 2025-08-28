using System;
using UnityEngine;

namespace dv_h_shifter;

public class GT26: DM1U
{
	protected override int numberOfGears => 3;

	public GT26(TrainCar car) : base(car) {}

	private const int STICK_FORWARD_INDEX = 2;
	private const int STICK_BACKWARD_INDEX = 3;
	
	protected override void HandleInput()
	{
		int wantedGear;

		if (GetJoyButtonUpDown(STICK_FORWARD_INDEX, false))
		{
			Main.Debug($"button {STICK_FORWARD_INDEX} down");
			wantedGear = 2;
		}
		else if (GetJoyButtonUpDown(STICK_BACKWARD_INDEX, false))
		{
			Main.Debug($"button {STICK_BACKWARD_INDEX} down");
			wantedGear = 0;
		}
		else if (GetJoyButtonUpDown(STICK_FORWARD_INDEX, true) || GetJoyButtonUpDown(STICK_BACKWARD_INDEX, true))
		{
			Main.Debug("some button up");
			wantedGear = 1;
		}
		else
		{
			return;
		}
		
		CalculateGearDelta(wantedGear);
	}

	private bool GetJoyButtonUpDown(int buttonIndex, bool up)
	{
		var joyString = GetJoyString(buttonIndex);
		try
		{
			return up ? Input.GetKeyUp(joyString) : Input.GetKeyDown(joyString);
		}
		//ignore buttons that don't exist
		catch (ArgumentException)
		{
			return false;
		}
	}
}