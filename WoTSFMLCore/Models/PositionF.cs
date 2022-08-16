namespace WoTSFMLCore.Models
{
    public struct PositionF
    {
        public float X;
        public float Y;
        public PositionF(float x, float y)
        {
            X = x;
            Y = y;
        }

        public PositionF()
        {
            X = 0;
            Y = 0;
        }
    }
}
