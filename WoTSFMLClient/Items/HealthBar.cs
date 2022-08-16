using SFML.Graphics;
using SFML.System;
using SFML.Window;
using WoTSFMLClient.Models;

namespace WoTSFMLClient.Items
{
    public class HealthBar : BaseItem
    {
        private RectangleShape mainBox;
        private RectangleShape bar;
        private Text text;

        private float _health = 0;

        public float Health
        {
            get => _health;
            set
            {
                _health = value;
                text.DisplayedString = $"{((value * 10) % 10 == 0 ? $"{value}.00" : Math.Round(value, 2))}%";
                calcProgress();
                textToCenter();
            }
        }

        public HealthBar(Window window) : base(window)
        {
            mainBox = new RectangleShape();
            bar = new RectangleShape();
            bar.FillColor = new Color(0, 255, 0);
            mainBox.FillColor = Color.Transparent;
            mainBox.OutlineColor = Color.Black;
            mainBox.OutlineThickness = 1;
            text = new Text(string.Empty, new Font("SourceSansPro-Regular.otf"), 16);
            text.Color = Color.Black;
            Size = new Vector2f(125, 24);
            bar.Position = new Vector2f(Position.X + 1, Position.Y + 1);
            textToCenter();
            calcProgress();
        }

        public override Vector2f Size
        {
            get => mainBox.Size;
            set
            {
                mainBox.Size = value;
            }
        }
        public override PositionData Position
        {
            get => mainBox.Position;
            set
            {
                _position = value;
                mainBox.Position = _positionInPixel;
                bar.Size = new Vector2f(0, mainBox.Size.Y - 2);
                bar.Position = new Vector2f(Position.X + 1, Position.Y + 1);
                textToCenter();
                calcProgress();
            }
        }

        public override void Loaded()
        {
            alignCalculate();
            base.Loaded();
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(mainBox);
            target.Draw(bar);
            target.Draw(text);
        }
        private void textToCenter()
        {
            Vector2f origin = new Vector2f();
            var bounds = text.GetLocalBounds();
            origin.X = bounds.Left + bounds.Width / 2;
            origin.Y = bounds.Top + bounds.Height / 2;
            text.Origin = origin;
            text.Position = new Vector2f(
                mainBox.Size.X / 2 + mainBox.Position.X,
                mainBox.Size.Y / 2 + mainBox.Position.Y);
        }

        private void calcProgress()
        {
            bar.Size = new Vector2f(((mainBox.Size.X - 2) * _health) / 100f, mainBox.Size.Y - 2);
            byte r = _health <= 50 ? byte.MaxValue : (byte)(byte.MaxValue - ((_health * byte.MaxValue) / 50));
            byte g = _health >= 50 ? byte.MaxValue : (byte)((_health * byte.MaxValue) / 50);
            bar.FillColor = new Color(r, g, 0);
            bar.Position = new Vector2f(Position.X + 1, Position.Y + 1);
        }
    }
}
