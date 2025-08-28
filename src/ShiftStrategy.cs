using System;
using System.Linq;
using DV.CabControls;
using DV.KeyboardInput;
using UnityEngine;

namespace dv_h_shifter;

public abstract class ShiftStrategy
{
	protected DateTime previousLeverMoveTime = new(0);
	protected abstract int numberOfGears { get; }
	
	protected LeverBase gearLeverA;
	protected LeverBase gearLeverB;

	protected ShiftStrategy(){}
	
	protected ShiftStrategy(TrainCar car)
	{
		var gearInputs = car.interior
			.GetComponentsInChildren<MouseScrollKeyboardInput>();
			
		var gearInputA = gearInputs 
			.FirstOrDefault(anInput => anInput.scrollAction.name == "GearAIncrement");
		
		if (!gearInputA)
		{
			Main.Error($"{nameof(gearInputA)} not found on {car.name}");
			return;
		}
		
		gearLeverA = gearInputA.gameObject.GetComponent<LeverBase>();
		
		var gearInputB = gearInputs
			.FirstOrDefault(anInput => anInput.scrollAction.name == "GearBIncrement");
		
		if (!gearInputB)
		{
			Main.Debug($"{nameof(gearInputB)} not found on {car.name}");
			return;
		}
		
		gearLeverB = gearInputB.gameObject.GetComponent<LeverBase>();
	}

	public void Update()
	{
		HandleInput();
		MoveGearLevers();
	}

	protected virtual void HandleInput()
	{
		for (int gearIndex = 0; gearIndex < numberOfGears; gearIndex++)
		{
			string joyString = GetJoyString(gearIndex);
			bool keyDown;
			try
			{
				keyDown = Input.GetKeyDown(joyString);
			}
			//ignore buttons that don't exist
			catch (ArgumentException)
			{
				continue;
			}

			if (!keyDown) continue;
			
			Main.Debug(joyString);
			CalculateGearDelta(gearIndex);
		}
	}

	public static string GetJoyString(int buttonIndex)
	{
		return $"joystick {Main.MySettings.UnityDeviceNumber} button {buttonIndex}";
	}

	/// <summary>
	/// Move the gear levers of the locomotive according to gearDelta, by simulating button presses. 
	/// </summary>
	protected abstract void MoveGearLevers();
	
	/// <summary>
	/// determines if and how far the gear levers should be moved
	/// </summary>
	/// <param name="selectedJoystickGear">the gear the player engaged in the h-pattern shifter (starts at 0)</param>
	protected abstract void CalculateGearDelta(int selectedJoystickGear);
}
