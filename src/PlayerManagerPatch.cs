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

		if (TryVanilla(newCar, out var newShiftStrategy) ||
		    TryCCL(newCar, out newShiftStrategy))
		{
			Main.CurrentStrategy = newShiftStrategy;
		}
		else
		{
			Main.CurrentStrategy = new Nothing();
		}

		Main.Debug($"Entering {Main.CurrentStrategy.GetType()}");
	}

	private static bool TryVanilla(TrainCar newCar, out ShiftStrategy newShiftStrategy)
	{
		switch (newCar.carType)
		{
			case TrainCarType.LocoDM3:
				newShiftStrategy = new DM3(newCar);
				return true;
			case TrainCarType.LocoDM1U:
				newShiftStrategy = new DM1U(newCar);
				return true;
			default:
				newShiftStrategy = null;
				return false;
		}
	}
	
	private static bool TryCCL(TrainCar newCar, out ShiftStrategy newShiftStrategy)
	{
		switch (newCar.carLivery.parentType.id)
		{
			case "YF_GT26CW-2":
				newShiftStrategy = new GT26(newCar);
				return true;
			default:
				newShiftStrategy = null;
				return false;
		}
	}
}