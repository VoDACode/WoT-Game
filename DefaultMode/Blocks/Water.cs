using System;
using WoTCore.Models;
using WoTCore.Modes;

namespace DefaultMode.Blocks
{
    public class Water : BaseBlock
    {
        public override int Durability { get; set; } = -1;
        public override ConsoleColor BackgroundColor { get; set; } = ConsoleColor.Blue;
        public override ConsoleColor ForegroundColor { get; set; } = ConsoleColor.Blue;
        public override char Icon { get; set; } = ' ';
        public override Position Position { get; set; }

        public override bool IsSpawnArea { get; set; } = false;
        public override bool IsInteractive { get; set; } = false;

        public override bool Generate(float val)
        {
            return val > 0 && val < 70;
        }
    }
}
