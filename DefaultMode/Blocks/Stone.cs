using System;
using WoTCore.Models;
using WoTCore.Models.MapObjects;
using WoTCore.Modes;

namespace DefaultMode.Blocks
{
    [Serializable()]
    public class Stone : BaseBlock
    {
        public override int Durability { get; set; } = 100;
        public override ConsoleColor BackgroundColor { get; set; } = ConsoleColor.Gray;
        public override ConsoleColor ForegroundColor { get; set; } = ConsoleColor.Gray;
        public override char Icon { get; set; } = ' ';
        public override Position Position { get; set; } = new Position();
        public override bool IsSpawnArea { get; set; } = false;
        public override bool CanBeBroken { get; set; } = true;
        public override bool IsInteractive { get; set; } = true;
        public override ICell Background { get; set; } = new Bush();

        public override bool Generate(float val)
        {
            return val >= 220;
        }

        public override void Setup()
        {
            myFun();
        }

        private void myFun()
        {
            Console.WriteLine("Hi! I`m stone xD");
        }
    }
}
