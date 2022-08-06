using SFML.System;

namespace WoTSFMLClient.Models
{
    public enum PositionUnits { Pixel, Percentage }
    public struct PositionData
    {
        public PositionUnits Units;
        public float X;
        public float Y;
        public PositionData()
        {
            X = 0;
            Y = 0;
            Units = PositionUnits.Pixel;
        }
        public PositionData(float x, float y)
        {
            X = x;
            Y = y;
            Units = PositionUnits.Pixel;
        }
        public PositionData(float x, float y, PositionUnits units)
        {
            X = x;
            Y = y;
            Units = units;
        }

        public static implicit operator PositionData(Vector2f vector)
            => new PositionData(vector.X, vector.Y, PositionUnits.Pixel);
    }
}
