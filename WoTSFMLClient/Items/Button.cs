using SFML.Graphics;
using SFML.System;
using SFML.Window;
using WoTSFMLClient.Models;

namespace WoTSFMLClient.Items
{
    public enum VerticalAlignType { Custom, Top, Center, Bottom }
    public enum HorizontalAlignType { Custom, Left, Center, Right }
    public delegate void ButtonMouseDelegate(Button item, Vector2i mouse);
    public class Button : BaseItem
    {
        private static readonly Color _defaultBg = new Color(165, 165, 165);
        private static readonly Color _hoverBg = new Color(190, 190, 190);
        private static readonly Color _clickBg = new Color(200, 200, 200);
        private static readonly Cursor _cursorPointer = new Cursor(Cursor.CursorType.Hand);
        private static readonly Cursor _cursorDefault = new Cursor(Cursor.CursorType.Arrow);
        private bool _mouseDown = false;
        private bool _hover = false;
        private Font _font;
        private Text textItem;
        private RectangleShape rectangle;
        private CircleShape circle = new CircleShape();
        private CircleShape emptyCircle = new CircleShape();

        public string Text
        {
            get => textItem.DisplayedString;
            set
            {
                textItem.DisplayedString = value;
                textToCenter();
            }
        }
        public override Vector2f Size
        {
            get => rectangle.Size;
            set
            {
                rectangle.Size = value;
                textToCenter();
            }
        }
        public override PositionData Position
        {
            get => _position;
            set
            {
                _position = value;
                rectangle.Position = _positionInPixel;
                textToCenter();
            }
        }
        public event ButtonMouseDelegate OnHover;
        public event ButtonMouseDelegate OnClick;
        public event ButtonMouseDelegate OnMouseLeftClick;

        public float BorderRadius
        {
            get => circle.Radius;
            set
            {
                emptyCircle.Radius = value;
                circle.Radius = value;
            }
        }

        public uint FontSize
        {
            get => textItem.CharacterSize;
            set
            {
                textItem.CharacterSize = value;
                textToCenter();
            }
        }

        public Button(Window Window, Vector2f size, string text) : base(Window)
        {
            VerticalAlign = VerticalAlignType.Top;
            HorizontalAlign = HorizontalAlignType.Left;
            Init(new Vector2f(), size, text);
        }

        public Button(Window Window, PositionData position, Vector2f size, string text) : base(Window)
        {
            VerticalAlign = VerticalAlignType.Custom;
            HorizontalAlign = HorizontalAlignType.Custom;
            Init(position, size, text);
        }

        private void Init(PositionData position, Vector2f size, string text)
        {
            rectangle = new RectangleShape(size);
            rectangle.FillColor = _defaultBg;
            _position = position;
            rectangle.Position = _positionInPixel;
            _font = new Font("SourceSansPro-Regular.otf");
            textItem = new Text(text, _font, 28);
            textItem.FillColor = Color.White;
            textItem.Style = SFML.Graphics.Text.Styles.Bold;
            textToCenter();
            circle.FillColor = _defaultBg;
            emptyCircle.FillColor = new Color(235, 255, 255);
        }

        public override void Loaded()
        {
            _hover = false;
            _mouseDown = false;
            base.Loaded();
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            alignCalculate();
            var mousePos = Mouse.GetPosition(OwnerWindow);
            if (mousePos.X > _positionInPixel.X && mousePos.Y > _positionInPixel.Y &&
                mousePos.X < _positionInPixel.X + Size.X && mousePos.Y < _positionInPixel.Y + Size.Y)
            {
                OnHover?.Invoke(this, mousePos);
                if (IsEnabled)
                {
                    rectangle.FillColor = _hoverBg;
                    circle.FillColor = _hoverBg;
                    OwnerWindow.SetMouseCursor(_cursorPointer);
                }
                _hover = true;
            }
            else
            {
                rectangle.FillColor = _defaultBg;
                circle.FillColor = _defaultBg;
                OwnerWindow.SetMouseCursor(_cursorDefault);
                _hover = false;
                _mouseDown = false;
            }
            if (Mouse.IsButtonPressed(Mouse.Button.Left) && _hover && IsEnabled)
            {
                OnMouseLeftClick?.Invoke(this, mousePos);
                rectangle.FillColor = _clickBg;
                circle.FillColor = _clickBg;
                if (!_mouseDown)
                {
                    OnClick?.Invoke(this, mousePos);
                    _mouseDown = true;
                }
            }
            if (!IsEnabled)
            {
                rectangle.FillColor = _defaultBg;
                circle.FillColor = _defaultBg;
                textItem.FillColor = _clickBg;
            }
            else
            {
                textItem.FillColor = Color.White;
            }
            target.Draw(rectangle);
            if (BorderRadius > 0)
            {
                //LT
                emptyCircle.Position = new Vector2f(_positionInPixel.X - BorderRadius, _positionInPixel.Y - BorderRadius);
                circle.Position = _positionInPixel;
                target.Draw(emptyCircle);
                target.Draw(circle);
                //RT
                emptyCircle.Position = new Vector2f(_positionInPixel.X - BorderRadius + Size.X, _positionInPixel.Y - BorderRadius);
                circle.Position = new Vector2f(_positionInPixel.X + Size.X - BorderRadius * 2, _positionInPixel.Y);
                target.Draw(emptyCircle);
                target.Draw(circle);
                //LD
                emptyCircle.Position = new Vector2f(_positionInPixel.X - BorderRadius, _positionInPixel.Y - BorderRadius + Size.Y);
                circle.Position = new Vector2f(_positionInPixel.X, _positionInPixel.Y + Size.Y - BorderRadius * 2);
                target.Draw(emptyCircle);
                target.Draw(circle);
                //RD
                emptyCircle.Position = new Vector2f(_positionInPixel.X - BorderRadius + Size.X, _positionInPixel.Y - BorderRadius + Size.Y);
                circle.Position = new Vector2f(_positionInPixel.X + Size.X - BorderRadius * 2, _positionInPixel.Y + Size.Y - BorderRadius * 2);
                target.Draw(emptyCircle);
                target.Draw(circle);
            }
            target.Draw(textItem);
        }

        private void textToCenter()
        {
            Vector2f origin = new Vector2f();
            var bounds = textItem.GetLocalBounds();
            origin.X = bounds.Left + bounds.Width / 2;
            origin.Y = bounds.Top + bounds.Height / 2;
            textItem.Origin = origin;
            textItem.Position = new Vector2f(
                rectangle.Size.X / 2 + rectangle.Position.X,
                rectangle.Size.Y / 2 + rectangle.Position.Y);
        }
    }
}
