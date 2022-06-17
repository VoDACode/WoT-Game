using WoTCore.Modes.Interfaces;
using System;
using System.Collections.Generic;
using WoTCore.Models;
using WoTCore.Models.MapObjects;
using System.Text.Json.Serialization;

namespace WoTCore.Modes.Resources
{
    [Serializable()]
    public class StructureResource : BaseStructure, IType
    {
        [JsonIgnore]
        [NonSerialized()]
        private object _item;
        public override Dictionary<Position, IBlock> Blocks { get; set; }
        public override Position Position { get; set; }

        public override void Setup()
        {
            Type().GetMethod("Setup").Invoke(GetObject(), null);
        }
        public override void Tick()
        {
            Type().GetMethod("Tick").Invoke(GetObject(), null);
        }
        public override bool OnTouch(object sender)
        {
            return (bool)Type().GetMethod("OnTouch", new Type[] { typeof(object) }).Invoke(GetObject(), new[] { sender });
        }

        public void Drow()
        {
            foreach (var block in Blocks)
            {
                (int x, int y) = (block.Key.X + Position.X, block.Key.Y + Position.Y);
                if (x < 0 || y < 0 || Console.WindowHeight < y || Console.WindowWidth < x)
                    continue;
                Console.SetCursorPosition(x, y);
                Console.BackgroundColor = block.Value.BackgroundColor.ToConsoleColor();
                Console.ForegroundColor = block.Value.ForegroundColor.ToConsoleColor();
                Console.Write(block.Value.Icon);
            }
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }

        public override bool Generate(float val)
        {
            return (bool)Type().GetMethod("Generate", new Type[] { typeof(float) }).Invoke(GetObject(), new object[] { val });
        }

        public object GetObject() => _item;

        public void SetObject(object value) => _item = value;

        public Type Type() => _item.GetType();
    }
}
