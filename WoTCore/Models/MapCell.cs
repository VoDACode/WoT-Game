using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.Linq;
using WoTCore.Modes;
using WoTCore.Modes.Resources;

namespace WoTCore.Models
{
    [Serializable]
    public class MapCell
    {
        private object? _content = default;
        private object? _background = default;
        public object Background
        {
            get => _background;
            set
            {
                if (!(value is ICell) && !(value is IEmptyObject) && value != default)
                    throw new ArgumentException($"value ('{value}') does not implement the interface '{typeof(ICell)}' or '{typeof(IEmptyObject)}'");
                _background = value;
            }
        }
        public object? Content
        {
            get => _content as object;
            set
            {
                if (!(value is ICell) && !(value is IPlayer) && !(value is IEmptyObject) && value != default)
                    throw new ArgumentException($"value ('{value}') does not implement the interface '{typeof(ICell)}' or '{typeof(IPlayer)}' or '{typeof(IEmptyObject)}'");
                _content = value;
            }
        }

        public MapCell(bool setToDefault = true)
        {
            if (setToDefault)
                Default();
        }

        public void Default()
        {
            Content = new EmptyObject();
            Background = new CellModel()
            {
                Icon = ' ',
                BackgroundColor = Color.Black,
                ForegroundColor = Color.White
            };
        }
    }
}
