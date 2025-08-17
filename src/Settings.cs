using System;
using UnityEngine;
using UnityModManagerNet;

namespace dv_h_shifter;

[Serializable]
public class Settings : UnityModManager.ModSettings
{
	public bool DebugLogging;
	
	//joy number
	public int DeviceNumber = 0;
	//unity starts counting at 1 instead of 0
	public int UnityDeviceNumber => DeviceNumber + 1;

	public void Draw(UnityModManager.ModEntry modEntry)
	{
		GUILayout.Label("Gear shift device");
		var previousDeviceNumber = DeviceNumber;
		DeviceNumber = GUILayout.Toolbar(DeviceNumber, Main.JoystickNames);

		if (DeviceNumber != previousDeviceNumber)
		{
			modEntry.Logger.Log("selecting ["+DeviceNumber+"] "+Main.JoystickNames[DeviceNumber]);
		}
		
		DebugLogging = GUILayout.Toggle(DebugLogging, "Debug logs");
	}

	public void OnChange()
	{
		// nothing
	}

	public override void Save(UnityModManager.ModEntry modEntry)
	{
		Save(this, modEntry);
	}
}