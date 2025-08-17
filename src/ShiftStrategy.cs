using System;
using UnityEngine;

namespace dv_h_shifter;

public abstract class ShiftStrategy
{
	protected DateTime previousLeverMoveTime = new(0);
	protected abstract int numberOfGears { get; }

	public void Update()
	{
		HandleInput();
		MoveGearLevers();
	}

	private void HandleInput()
	{
		for (int gearNumber = 0; gearNumber < numberOfGears; gearNumber++)
		{
			bool keyDown;
			try
			{
				keyDown = Input.GetKeyDown("joystick " + Main.MySettings.UnityDeviceNumber + " button " + gearNumber);
			}
			//ignore buttons that don't exist
			catch (ArgumentException)
			{
				continue;
			}

			if (keyDown)
			{
				Main.LogDebug("joystick " + Main.MySettings.UnityDeviceNumber + " button " + gearNumber);
				CalculateGearDelta(gearNumber);
			}
		}
	}

	/// <summary>
	/// Move the gear levers of the locomotive according to gearDelta, by simulating button presses. 
	/// </summary>
	protected abstract void MoveGearLevers();
	
	/// <summary>
	/// determines if and how far the gear levers should be moved
	/// </summary>
	/// <param name="wantedGear">the gear the player requested</param>
	protected abstract void CalculateGearDelta(int wantedGear);
}
