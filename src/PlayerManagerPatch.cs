using DV.ThingTypes;
using HarmonyLib;

namespace dv_h_shifter;

[HarmonyPatch(typeof(PlayerManager))]
[HarmonyPatch(nameof(PlayerManager.SetCar))]
public class PlayerManager_SetCar_Patch
{
	private static TrainCarType currentCarType = TrainCarType.NotSet;
	
	private static void Postfix(TrainCar newCar)
	{
		Main.Log(nameof(PlayerManager_SetCar_Patch));
		
		if (!newCar || newCar.carType == currentCarType) return;
		
		switch (newCar.carType)
		{
			case TrainCarType.LocoDM3:
				Main.Log($"Entering {nameof(DM3)}");
				Main.CurrentStrategy = new DM3();
				break;
			case TrainCarType.LocoDM1U:
				Main.Log($"Entering {nameof(DM1U)}");
				Main.CurrentStrategy = new DM1U();
				break;
		}
	}
}