using System;

namespace WoTCore.Models
{
    [Serializable]
    public class MapCell
    {
        private object? _content = default;
        private object? _background = default;
        public object Background {
            get => _background;
            set
            {
                if (!(value is ICell) && !(value is IEmptyObject) && value != default)
                    throw new ArgumentException($"value does not implement the interface '{typeof(ICell)}'");
                _background = value;
            }
        }
        public object? Content
        {
            get => _content;
            set
            {
                if (!(value is ICell) && !(value is IPlayer) && !(value is IEmptyObject) && value != default)
                    throw new ArgumentException($"value does not implement the interface '{typeof(ICell)}' or '{typeof(IPlayer)}' or '{typeof(IEmptyObject)}'");
                _content = value;
            }
        }

        public MapCell()
        {
            Default();
        }

        public void Default()
        {
            Content = default;
            Background = new CellModel()
            {
                Icon = ' ',
                BackgroundColor = ConsoleColor.Black,
                ForegroundColor = ConsoleColor.White
            };
        }
    }
}
