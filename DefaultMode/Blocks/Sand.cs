using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoTCore.Models;
using WoTCore.Modes;

namespace DefaultMode.Blocks
{
    public class Sand : BaseBlock
    {
        public override int Durability { get; set; } = -1;
        public override ConsoleColor BackgroundColor { get; set; } = ConsoleColor.Yellow;
        public override ConsoleColor ForegroundColor { get; set; } = ConsoleColor.Yellow;
        public override char Icon { get; set; } = ' ';
        public override Position Position { get; set; }

        public override bool IsSpawnArea { get; set; } = true;
        public override bool IsInteractive { get; set; } = false;

        public override bool Generate(float val)
        {
            return val >= 70 && val < 100;
        }
    }
}
