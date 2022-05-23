using System;
using WoTCore.Models;
using WoTCore.Modes;
using WoTCore.Modes.Interfaces;

namespace WoTCore.Modes.Resources
{
    [Serializable()]
    public class BlockResource : BaseBlock, IType, IDrow, ICopy<BlockResource>
    {
        [NonSerialized()] private Type _type;
        [NonSerialized()] private object _item;
        public override int Durability { get; set; }
        public override ConsoleColor BackgroundColor { get; set; }
        public override ConsoleColor ForegroundColor { get; set; }
        public override char Icon { get; set; }
        public override Position Position { get; set; }
        public Type ItemType { get => _type; set => _type = value; }
        public object Item
        {
            get
            {
                if(this._item == null)
                    _item = Activator.CreateInstance(ItemType);
                return _item;
            }
        }

        public override bool IsSpawnArea { get; set; }
        public override bool IsInteractive { get; set; }

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
            Console.SetCursorPosition(Position.X, Position.Y);
            Console.BackgroundColor = BackgroundColor;
            Console.ForegroundColor = ForegroundColor;
            Console.Write(Icon);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }
        public void DrowTo(CellModel[,] cells)
        {
            cells[Position.X, Position.Y].Icon = Icon;
            cells[Position.X, Position.Y].BackgroundColor = BackgroundColor;
            cells[Position.X, Position.Y].ForegroundColor = ForegroundColor;
        }

        public override bool Generate(float val)
        {
            return (bool)ItemType.GetMethod("Generate", new Type[] { typeof(float) }).Invoke(Item, new object [] { val });
        }

        public BlockResource Copy()
        {
            var instance = new BlockResource();
            foreach (var property in this.GetType().GetProperties())
            {
                if(property.CanRead && property.CanWrite)
                    instance.GetType().GetProperty(property.Name).SetValue(instance, property.GetValue(this));
            }
            return instance;
        }
    }
}
