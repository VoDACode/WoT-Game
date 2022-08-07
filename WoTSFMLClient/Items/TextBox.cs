using SFML.Graphics;
using SFML.System;
using SFML.Window;
using WoTSFMLClient.Models;

namespace WoTSFMLClient.Items
{
    public delegate void TextBoxChangeDelegat(TextBox sender, string text);
    public delegate bool TextBoxPreviewChangeDelegat(TextBox sender, string unicode);
    public class TextBox : BaseItem
    {
        private static readonly Color _borderBg = new Color(175, 175, 175);
        private static readonly Color _boxBg = new Color(240, 240, 240);
        private RectangleShape _border;
        private RectangleShape _box;
        private Text _content;
        private Text _placeholder;
        private RectangleShape _selector;
        private Font _font;

        private bool _focus = false;
        private bool _loaed = false;

        public event TextBoxChangeDelegat OnChange;
        public event TextBoxPreviewChangeDelegat OnPreviewChange;

        public int MaxLength { get; set; } = -1;

        public string Text
        {
            get => _content.DisplayedString;
            set
            {
                _content.DisplayedString = value;
                calcTextPos();
            }
        }

        public string Placeholder
        {
            get => _placeholder.DisplayedString;
            set
            {
                _placeholder.DisplayedString = value;
                calcTextPos();
            }
        }

        public override Vector2f Size
        {
            get => _border.Size;
            set
            {
                _border.Size = value;
                _box.Size = new Vector2f(value.X - 10, value.Y - 10);
                calcTextPos();
            }
        }

        public override PositionData Position
        {
            get => _position;
            set
            {
                _position = value;
                _border.Position = _positionInPixel;
                _box.Position = new Vector2f(_border.Position.X + 5, _border.Position.Y + 5);
                calcTextPos();
            }
        }

        public uint FontSize
        {
            get => _content.CharacterSize;
            set
            {
                _placeholder.CharacterSize = value;
                _content.CharacterSize = value;
                calcTextPos();
            }
        }

        public TextBox(Window window, Vector2f size) : this(window, new Vector2f(), size, string.Empty, string.Empty)
        { }

        public TextBox(Window window, PositionData position, Vector2f size, string text, string placeholder) : base(window)
        {
            _font = new Font("SourceSansPro-Regular.otf");
            _content = new Text(text, _font, 24);
            _placeholder = new Text(placeholder, _font, 24);
            _border = new RectangleShape();
            _box = new RectangleShape();
            _selector = new RectangleShape();

            _border.FillColor = _borderBg;
            _position = position;
            _border.Position = _positionInPixel;
            _border.Size = size;

            _box.FillColor = _boxBg;
            _box.Position = new Vector2f(_positionInPixel.X + 5, _positionInPixel.Y + 5);
            _box.Size = new Vector2f(size.X - 10, size.Y - 10);

            calcTextPos();
        }

        public override void Loaded()
        {
            OwnerWindow.TextEntered += TextEntered;
            base.Loaded();
        }

        public override void Unloaded()
        {
            OwnerWindow.TextEntered -= TextEntered;
            base.Unloaded();
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            var window = target as Window;
            alignCalculate();
            if (IsEnabled)
            {
                var mousePos = Mouse.GetPosition(window);
                if (mousePos.X > _positionInPixel.X && mousePos.Y > _positionInPixel.Y &&
                    mousePos.X < _positionInPixel.X + Size.X && mousePos.Y < _positionInPixel.Y + Size.Y
                    )
                {
                    if (Mouse.IsButtonPressed(Mouse.Button.Left))
                    {
                        var bound = _content.GetLocalBounds();
                        _focus = true;
                        _selector.Position = new Vector2f(_positionInPixel.X + 15 + (bound.Width + bound.Left), _positionInPixel.Y + 10);
                    }
                }
                else
                {
                    if (Mouse.IsButtonPressed(Mouse.Button.Left))
                        _focus = false;
                }
            }

            target.Draw(_border);
            target.Draw(_box);
            if (string.IsNullOrEmpty(Text) && !_focus)
                target.Draw(_placeholder);
            else
            {
                target.Draw(_content);
                if (_focus)
                    target.Draw(_selector);
            }
        }

        private void TextEntered(object? sender, TextEventArgs e)
        {
            if (!_focus || (!OnPreviewChange?.Invoke(this, e.Unicode) == false && e.Unicode != "\b") || !IsEnabled)
                return;
            if (e.Unicode == "\b")
            {
                if (Text.Length == 0)
                    return;
                Text = Text.Substring(0, Text.Length - 1);
            }
            else
            {
                if (MaxLength >= 0 && MaxLength < Text.Length + 1)
                    return;
                Text += e.Unicode;
            }
            OnChange?.Invoke(this, Text);
        }

        private void calcTextPos()
        {
            var bounds = _placeholder.GetLocalBounds();
            _placeholder.Origin = new Vector2f(0, bounds.Top + bounds.Height / 2);
            _placeholder.Position = new Vector2f(
                _box.Position.X + 10,
                _box.Size.Y / 2 + _box.Position.Y);
            bounds = _content.GetLocalBounds();
            _content.Origin = new Vector2f(0, bounds.Top + bounds.Height / 2);
            _content.Position = new Vector2f(
                _box.Position.X + 10,
                _box.Size.Y / 2 + _box.Position.Y);

            _content.FillColor = Color.Black;
            _placeholder.FillColor = new Color(0, 0, 0, 128);

            _selector.FillColor = Color.Black;
            _selector.Size = new Vector2f(2, _box.Size.Y * 0.8f);
            _selector.Position = new Vector2f(_positionInPixel.X + 15 + (bounds.Width + bounds.Left), _positionInPixel.Y + 10);
        }
    }
}
