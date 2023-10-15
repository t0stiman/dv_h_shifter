namespace dv_h_shifter
{
    public class GearLeversState
    {
        public int posA;
        public int posB;
        private const int numberOfPositions = 3;

        public GearLeversState()
        {
            posA = 0;
            posB = 0;
        }
        
        public GearLeversState(int posA, int posB)
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
        
        public static GearLeversState operator -(GearLeversState a) => new GearLeversState(-a.posA, -a.posB);
        
        public static GearLeversState operator +(GearLeversState a, GearLeversState b)
            => new GearLeversState(a.posA + b.posA, a.posB + b.posB);

        public static GearLeversState operator -(GearLeversState a, GearLeversState b)
            => a + (-b);

        public bool Equals_(GearLeversState other)
        {
            return posA == other.posA && posB == other.posB;
        }

        public override string ToString()
        {
            return $"posA: {posA}, posB: {posB}";
        }
    }
}