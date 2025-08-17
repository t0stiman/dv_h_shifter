namespace dv_h_shifter;

public class Nothing: ShiftStrategy
{
	protected override int numberOfGears => 1;
	protected override void MoveGearLevers()
	{
		//nothing
	}

	protected override void CalculateGearDelta(int _)
	{
		//nothing
	}
}