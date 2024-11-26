namespace dv_h_shifter
{
	public class DM3GearLeversState
	{
		public int posA;
		public int posB;
		private const int numberOfPositions = 3;

		public DM3GearLeversState()
		{
			posA = 0;
			posB = 0;
		}
		
		public DM3GearLeversState(int posA, int posB)
		{
			this.posA = posA;
			this.posB = posB;
		}

		public bool IsValidState()
		{
			return posA >= 1 &&
			       posA <= numberOfPositions &&
			       posB >= 1 &&
			       posB <= numberOfPositions;
		}
		
		public static DM3GearLeversState operator -(DM3GearLeversState a) => new (-a.posA, -a.posB);
		
		public static DM3GearLeversState operator +(DM3GearLeversState a, DM3GearLeversState b)
			=> new (a.posA + b.posA, a.posB + b.posB);

		public static DM3GearLeversState operator -(DM3GearLeversState a, DM3GearLeversState b)
			=> a + (-b);

		public bool Equals_(DM3GearLeversState other)
		{
			return posA == other.posA && posB == other.posB;
		}

		public override string ToString()
		{
			return $"posA: {posA}, posB: {posB}";
		}
	}
}