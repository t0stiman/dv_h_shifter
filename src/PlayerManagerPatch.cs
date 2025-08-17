using DV.ThingTypes;
using HarmonyLib;

namespace dv_h_shifter;

/// <summary>
/// Player enters a different train car -> switch the shifting strategy
/// </summary>
[HarmonyPatch(typeof(PlayerManager))]
[HarmonyPatch(nameof(PlayerManager.SetCar))]
public class PlayerManager_SetCar_Patch
{
	private static TrainCarType currentCarType = TrainCarType.NotSet;
	
	private static void Postfix(TrainCar newCar)
	{
		if (!newCar || newCar.carType == currentCarType) return;
		
		switch (newCar.carType)
		{
			case TrainCarType.LocoDM3:
				Main.CurrentStrategy = new DM3(newCar);
				break;
			case TrainCarType.LocoDM1U:
				Main.CurrentStrategy = new DM1U(newCar);
				break;
			default:
				Main.CurrentStrategy = new Nothing();
				break;
		}
		
		Main.LogDebug($"Entering {Main.CurrentStrategy.GetType()}");
	}
}