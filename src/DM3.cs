using System;
using System.Collections.Generic;
using DV.CabControls.NonVR;
using Mono.Collections.Generic;
using UnityEngine;
using WindowsInput.Native;

namespace dv_h_shifter
{
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
		private static readonly IList<DM3GearLeversState> gearLeversPositions = new ReadOnlyCollection<DM3GearLeversState>
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
				Main.Log($"pressing A up");
				inputSim.Keyboard.KeyPress(VirtualKeyCode.NUMPAD5);
				gearDelta.posA--;
			}
			else if (gearDelta.posA < 0)
			{
				Main.Log($"pressing A down");
				inputSim.Keyboard.KeyPress(VirtualKeyCode.NUMPAD2);
				gearDelta.posA++;
			}
			
			if (gearDelta.posB > 0)
			{
				Main.Log($"pressing B up");
				inputSim.Keyboard.KeyPress(VirtualKeyCode.NUMPAD6);
				gearDelta.posB--;
			}
			else if (gearDelta.posB < 0)
			{
				Main.Log($"pressing B down");
				inputSim.Keyboard.KeyPress(VirtualKeyCode.NUMPAD3);
				gearDelta.posB++;
			}

			previousLeverMoveTime = DateTime.UtcNow;
		}
		
		protected override void CalculateGearDelta(int wantedGear)
		{
			//no neutral in the DM3 so we start at 1
			wantedGear++;
			Main.Log("engage gear "+wantedGear);

			var currentState = new DM3GearLeversState(GetGearLeverPosition('A'), GetGearLeverPosition('B'));
			var wantedState = gearLeversPositions[wantedGear - 1];
			gearDelta = wantedState - currentState;
		}

		private static int GetGearLeverPosition(char gearBoxLetter)
		{
			var box = GameObject.Find("C_Gearbox" + gearBoxLetter);
			if (box is null)
			{
				Main.Error($"Can't find {nameof(DM3)} gearbox {gearBoxLetter}");
				return 0;
			}
			var value = box.GetComponent<LeverNonVR>().Value;
			
			// 0 -> 1, 0,5 -> 2, 1 -> 3
			if (value < 0.1)
			{
				return 1;
			}
			if (value < 0.6)
			{
				return 2;
			}

			return 3;
		}
		
		/// <summary>
		/// convert a GearLeversState to a gear number
		/// </summary>
		private static int StateToGear(DM3GearLeversState state)
		{
			// Main.Log($"state: {state.posA}, {state.posB}");
			for (var index = 0; index < gearLeversPositions.Count; index++)
			{
				if (state.Equals_(gearLeversPositions[index]))
				{
					return index + 1;
				}
			}

			return 0;
		}
	}
}