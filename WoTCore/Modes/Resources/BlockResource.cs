using System;
using System.Drawing;
using System.Text.Json.Serialization;
using WoTCore.Models;
using WoTCore.Modes;
using WoTCore.Modes.Interfaces;

namespace WoTCore.Modes.Resources
{
    [Serializable]
    public class BlockResource : BaseBlock, IType, ICopy<BlockResource>
    {
        [JsonIgnore]
        [NonSerialized()] 
        private object _item;
        public override int Durability { get; set; }
        public override Color BackgroundColor { get; set; }
        public override Color ForegroundColor { get; set; }
        public override char Icon { get; set; } = ' ';
        public override Position Position { get; set; }

        public override bool IsSpawnArea { get; set; }
        public override bool IsInteractive { get; set; }

        public override void Setup()
        {
            Type().GetMethod("Setup").Invoke(GetObject(), null);
        }
        public override void Tick()
        {
            Type().GetMethod("Tick").Invoke(GetObject(), null);
        }
        public override bool OnTouch(object sender, MapCell cell)
        {
            return (bool)Type().GetMethod("OnTouch", new Type[] { typeof(object), typeof(MapCell) }).Invoke(GetObject(), new[] { sender, cell });
        }

        public override bool Generate(float val)
        {
            return (bool)Type().GetMethod("Generate", new Type[] { typeof(float) }).Invoke(GetObject(), new object [] { val });
        }

        public BlockResource Copy()
        {
            var instance = new BlockResource();
            foreach (var property in this.GetType().GetProperties())
            {
                if(property.CanRead && property.CanWrite)
                    instance.GetType().GetProperty(property.Name).SetValue(instance, property.GetValue(this));
            }
            instance.SetObject(Activator.CreateInstance(_item.GetType()));
            return instance;
        }

        public object GetObject() => _item;

        public void SetObject(object value) => _item = value;

        public Type Type() => _item.GetType();

    }
}
