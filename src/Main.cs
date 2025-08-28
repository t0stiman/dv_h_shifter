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
		private static Harmony myHarmony;
		public static Settings MySettings;

		public static ShiftStrategy CurrentStrategy;
		
		public static string[] JoystickNames;

		//===========================================

		private static bool Load(UnityModManager.ModEntry modEntry)
		{
			try
			{
				MySettings = Settings.Load<Settings>(modEntry);
				modEntry.OnGUI = entry => MySettings.Draw(entry);
				modEntry.OnSaveGUI = entry => MySettings.Save(entry);
				
				JoystickNames = Input.GetJoystickNames();
				for (int i = 0; i < JoystickNames.Length; i++)
				{
					modEntry.Logger.Log(i+": "+JoystickNames[i]);
				}
			
				myModEntry = modEntry;
				modEntry.OnUpdate = OnUpdate;

				modEntry.OnUnload = OnUnload;
				
				myHarmony = new Harmony(modEntry.Info.Id);
				myHarmony.PatchAll(Assembly.GetExecutingAssembly());

				CurrentStrategy = new Nothing();
			}
			catch (Exception ex)
			{
				modEntry.Logger.LogException($"Failed to load {modEntry.Info.DisplayName}:", ex);
				myHarmony?.UnpatchAll(modEntry.Info.Id);
				return false;
			}

			Log("loaded");
			return true;
		}

		private static bool OnUnload(UnityModManager.ModEntry modEntry)
		{
			myHarmony?.UnpatchAll(modEntry.Info.Id);
			return true;
		}

		private static void OnUpdate(UnityModManager.ModEntry modEntry, float idk)
		{
			CurrentStrategy.Update();
		}

		// Logger functions
		public static void Debug(string message)
		{
			if(!MySettings.DebugLogging) return;
			myModEntry.Logger.Log($"[DEBUG] {message}");
		}
		
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