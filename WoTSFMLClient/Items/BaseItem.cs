using SFML.Graphics;
using SFML.System;
using SFML.Window;
using WoTSFMLClient.Models;

namespace WoTSFMLClient.Items
{
    public abstract class BaseItem : Drawable
    {
        public Window OwnerWindow { get; }
        public bool IsEnabled { get; set; } = true;
        public abstract void Draw(RenderTarget target, RenderStates states);
        public abstract Vector2f Size { get; set; }
        public abstract PositionData Position { get; set; }
        protected PositionData _position { get; set; }
        protected Vector2f _positionInPixel
        {
            get
            {
                if (_position.Units == PositionUnits.Pixel)
                    return new Vector2f(_position.X, _position.Y);
                return new Vector2f(_position.X * OwnerWindow.Size.X / 100, _position.Y * OwnerWindow.Size.Y / 100);
            }
        }
        public VerticalAlignType VerticalAlign { get; set; }
        public HorizontalAlignType HorizontalAlign { get; set; }

        public BaseItem(Window window)
        {
            OwnerWindow = window;
        }

        public virtual void Loaded() { }
        public virtual void Unloaded() { }

        protected void alignCalculate()
        {
            var px = _positionInPixel;
            if (VerticalAlign == VerticalAlignType.Top)
            {
                Position = new PositionData(px.X, 0);
            }
            else if (VerticalAlign == VerticalAlignType.Bottom)
            {
                Position = new PositionData(px.X, OwnerWindow.Size.Y - Size.Y);
            }
            else if (VerticalAlign == VerticalAlignType.Center)
            {
                Position = new PositionData(px.X, (OwnerWindow.Size.Y - Size.Y) / 2);
            }
            if (HorizontalAlign == HorizontalAlignType.Left)
            {
                Position = new PositionData(0, px.Y);
            }
            else if (HorizontalAlign == HorizontalAlignType.Right)
            {
                Position = new PositionData(OwnerWindow.Size.X - Size.X, px.Y);
            }
            else if (HorizontalAlign == HorizontalAlignType.Center)
            {
                Position = new PositionData((OwnerWindow.Size.X - Size.X) / 2, px.Y);
            }
        }
    }
}
