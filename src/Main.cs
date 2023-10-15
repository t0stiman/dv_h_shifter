using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityModManagerNet;
using DV.CabControls.NonVR;
using HarmonyLib;
using WindowsInput;
using WindowsInput.Native;

namespace dv_h_shifter
{
    [EnableReloading]
    static class Main
    {
        public static bool enabled;
        public static UnityModManager.ModEntry myModEntry;
        
        //joy number
        private static int deviceNumber = 0;
        private static InputSimulator inputSim;
        
        //gear number -> button number. 8 gears so the array is 8 long. 
        private static int[] gearToButtonMap = {0, 1, 2, 3, 4, 5, 6, 7};
        private const int numberOfGears = 8;
        
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
        private static readonly IList<GearLeversState> gearLeversPositions = new ReadOnlyCollection<GearLeversState>
        (new[] {
            new GearLeversState (1, 1),
            new GearLeversState (1, 2),
            new GearLeversState (2, 1),
            new GearLeversState (2, 2),
            new GearLeversState (3, 1),
            new GearLeversState (3, 2),
            new GearLeversState (2, 3),
            new GearLeversState (3, 3)
        });

        private static string[] joystickNames = Array.Empty<string>();
        // difference between the current GearLeversState and the state we want to achieve
        private static GearLeversState gearDelta;
        private static DateTime previousLeverMoveTime = new DateTime(0);

        //================================================================

        private static bool Load(UnityModManager.ModEntry modEntry)
        {
            gearDelta = new GearLeversState();
            inputSim = new InputSimulator();

            joystickNames = Input.GetJoystickNames();
            for (int i = 0; i < joystickNames.Length; i++)
            {
                modEntry.Logger.Log(i+": "+joystickNames[i]);
            }
            
            var harmony = new Harmony(modEntry.Info.Id);
            harmony.PatchAll();

            myModEntry = modEntry;
            modEntry.OnToggle = OnToggle;
            modEntry.OnUpdate = OnUpdate;
            modEntry.OnGUI = OnGUI;
            
            if (gearToButtonMap.Length != numberOfGears)
            {
                modEntry.Logger.Error("make gearToButtonMap "+numberOfGears+" long pls");
                return false;
            }

            modEntry.Logger.Log("loaded");

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

        private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value) 
        {
            enabled = value;
            string msg = enabled ? "hello!" : "goodbye!";
            modEntry.Logger.Log(msg);

            return true;
        }

        private static void OnUpdate(UnityModManager.ModEntry modEntry, float idk)
        {
            HandleInput(modEntry);
            MoveGearLevers();
        }

        /// <summary>
        /// Move the gear levers of the locomotive according to gearDelta, by simulating button presses. 
        /// </summary>
        private static void MoveGearLevers()
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
                myModEntry.Logger.Log($"pressing A up");
                //KeyBindings.increaseGearAKeys[0]
                inputSim.Keyboard.KeyPress(VirtualKeyCode.NUMPAD5);
                gearDelta.posA--;
            }
            else if (gearDelta.posA < 0)
            {
                myModEntry.Logger.Log($"pressing A down");
                //KeyBindings.decreaseGearAKeys[0]
                inputSim.Keyboard.KeyPress(VirtualKeyCode.NUMPAD2);
                gearDelta.posA++;
            }
            
            if (gearDelta.posB > 0)
            {
                myModEntry.Logger.Log($"pressing B up");
                //KeyBindings.increaseGearBKeys[0]
                inputSim.Keyboard.KeyPress(VirtualKeyCode.NUMPAD6);
                gearDelta.posB--;
            }
            else if (gearDelta.posB < 0)
            {
                myModEntry.Logger.Log($"pressing B down");
                //KeyBindings.decreaseGearBKeys[0]
                inputSim.Keyboard.KeyPress(VirtualKeyCode.NUMPAD3);
                gearDelta.posB++;
            }

            previousLeverMoveTime = DateTime.UtcNow;
        }

        private static void HandleInput(UnityModManager.ModEntry modEntry)
        {
            for (int gearNumber = 1; gearNumber <= numberOfGears; gearNumber++)
            {
                var unity_deviceNumber = deviceNumber + 1; //unity starts counting at 1 instead of 0
                
                bool keyDown;
                try
                {
                    keyDown = Input.GetKeyDown("joystick " + unity_deviceNumber + " button " +
                                               gearToButtonMap[gearNumber - 1]);
                }
                //ignore buttons that don't exist
                catch (ArgumentException e)
                {
                    //get rid of the unused variable warning
                    e.ToString();
                    continue;
                }
                
                if (keyDown)
                {
                    modEntry.Logger.Log("joystick " + unity_deviceNumber + " button " +
                                        gearToButtonMap[gearNumber - 1]);
                    CalculateGearDelta(gearNumber);
                }
            }
        }

        /// <summary>
        /// determines if and how far the gear levers should be moved
        /// </summary>
        /// <param name="wantedGear">the gear the player requested</param>
        static void CalculateGearDelta(int wantedGear)
        {
            myModEntry.Logger.Log("engage gear "+wantedGear);

            var currentState = new GearLeversState(GetGearLeverPosition('A'), GetGearLeverPosition('B'));
            var wantedState = gearLeversPositions[wantedGear - 1];
            gearDelta = wantedState - currentState;
        }

        static int GetGearLeverPosition(char gearBoxLetter)
        {
            var box = GameObject.Find("C_Gearbox" + gearBoxLetter);
            if (box is null)
            {
                myModEntry.Logger.Error("Cant find gearbox "+gearBoxLetter);
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
        private static int StateToGear(GearLeversState state)
        {
            // myModEntry.Logger.Log($"state: {state.posA}, {state.posB}");
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