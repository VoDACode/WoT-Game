using WoTCore.Modes.Interfaces;
using System;
using System.Collections.Generic;
using WoTCore.Models;
using WoTCore.Models.MapObjects;

namespace WoTCore.Modes.Resources
{
    [Serializable()]
    public class StructureResource : BaseStructure, IType
    {
        [field: NonSerialized]
        private object _item;
        [field: NonSerialized]
        public Type ItemType { get; set; }
        public override Dictionary<Position, IBlock> Blocks { get; set; }
        public override Position Position { get; set; }
        [field: NonSerialized]
        public object Item
        {
            get
            {
                if (this._item == null)
                    _item = Activator.CreateInstance(ItemType);
                return _item;
            }
        }

        public override void Setup()
        {
            ItemType.GetMethod("Setup").Invoke(Item, null);
        }
        public override void Tick()
        {
            ItemType.GetMethod("Tick").Invoke(Item, null);
        }
        public override bool OnTouch(object sender)
        {
            return (bool)ItemType.GetMethod("OnTouch", new Type[] { typeof(object) }).Invoke(Item, new[] { sender });
        }

        public void Drow()
        {
            foreach (var block in Blocks)
            {
                (int x, int y) = (block.Key.X + Position.X, block.Key.Y + Position.Y);
                if (x < 0 || y < 0 || Console.WindowHeight < y || Console.WindowWidth < x)
                    continue;
                Console.SetCursorPosition(x, y);
                Console.BackgroundColor = block.Value.BackgroundColor;
                Console.ForegroundColor = block.Value.ForegroundColor;
                Console.Write(block.Value.Icon);
            }
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }

        public override bool Generate(float val)
        {
            return (bool)ItemType.GetMethod("Generate", new Type[] { typeof(float) }).Invoke(Item, new object[] { val });
        }
    }
}
