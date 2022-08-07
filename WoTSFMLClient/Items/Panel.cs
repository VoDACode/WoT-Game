using SFML.Graphics;
using SFML.System;
using SFML.Window;
using WoTSFMLClient.Models;

namespace WoTSFMLClient.Items
{
    public class Panel : BaseItem
    {
        private RectangleShape rectangle;

        public Panel(Window window, Color color) : base(window)
        {
            rectangle = new RectangleShape();
            rectangle.FillColor = color;
            rectangle.Size = new Vector2f(window.Size.X, window.Size.Y);
        }

        public override Vector2f Size
        {
            get => rectangle.Size;
            set => rectangle.Size = value;
        }
        public override PositionData Position
        {
            get => rectangle.Position;
            set
            {
                _position = Position;
                rectangle.Position = _positionInPixel;
            }
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(rectangle);
        }
    }
}
