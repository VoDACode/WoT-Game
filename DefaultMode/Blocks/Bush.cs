using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoTCore.Models;
using WoTCore.Modes;

namespace DefaultMode.Blocks
{
    [Serializable()]
    public class Bush : BaseBlock
    {
        public override int Durability { get; set; } = 5;
        public override ConsoleColor BackgroundColor { get; set; } = ConsoleColor.Green;
        public override ConsoleColor ForegroundColor { get; set; } = ConsoleColor.Green;
        public override char Icon { get; set; } = ' ';
        public override Position Position { get; set; } = new Position();

        public override bool IsSpawnArea { get; set; } = true;
        public override bool IsInteractive { get; set; } = false;

        public override bool Generate(float val)
        {
            return val >= 100 && val < 220;
        }
    }
}
