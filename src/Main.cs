using System;
using System.Reflection;
using UnityEngine;
using UnityModManagerNet;
using HarmonyLib;

namespace dv_h_shifter
{
	[EnableReloading]
	static class Main
	{
		private static UnityModManager.ModEntry myModEntry;
		private static Harmony harmony;

		public static ShiftStrategy CurrentStrategy;
	
		//joy number
		private static int deviceNumber = 0;
		//unity starts counting at 1 instead of 0
		public static int UnityDeviceNumber => deviceNumber + 1;
		private static string[] joystickNames;

		//===========================================

		private static bool Load(UnityModManager.ModEntry modEntry)
		{
			try
			{
				joystickNames = Input.GetJoystickNames();
				for (int i = 0; i < joystickNames.Length; i++)
				{
					modEntry.Logger.Log(i+": "+joystickNames[i]);
				}
			
				myModEntry = modEntry;
				myModEntry.OnUpdate = OnUpdate;
				myModEntry.OnGUI = OnGUI;
				myModEntry.OnUnload = OnUnload;
				
				harmony = new Harmony(myModEntry.Info.Id);
				harmony.PatchAll(Assembly.GetExecutingAssembly());

				CurrentStrategy = new DM3();
			}
			catch (Exception ex)
			{
				myModEntry.Logger.LogException($"Failed to load {myModEntry.Info.DisplayName}:", ex);
				harmony?.UnpatchAll(myModEntry.Info.Id);
				return false;
			}

			Log("loaded");
			return true;
		}

		private static bool OnUnload(UnityModManager.ModEntry modEntry)
		{
			harmony?.UnpatchAll(myModEntry.Info.Id);
			return true;
		}

		private static void OnGUI(UnityModManager.ModEntry modEntry)
		{
			GUILayout.Label("Gear shift device");
			var previousDeviceNumber = deviceNumber;
			deviceNumber = GUILayout.Toolbar(deviceNumber, joystickNames);

			if (deviceNumber != previousDeviceNumber)
			{
				modEntry.Logger.Log("selecting ["+deviceNumber+"] "+joystickNames[deviceNumber]);
			}
		}

		private static void OnUpdate(UnityModManager.ModEntry modEntry, float idk)
		{
			CurrentStrategy.Update();
		}

		// Logger functions
		public static void Log(string message)
		{
			myModEntry.Logger.Log(message);
		}

		public static void Warning(string message)
		{
			myModEntry.Logger.Warning(message);
		}

		public static void Error(string message)
		{
			myModEntry.Logger.Error(message);
		}
	}
}