using System;
using System.Drawing;
using WoTCore.Models;
using WoTCore.Modes;

namespace DefaultMode.Blocks
{
    [Serializable()]
    public class Sand : BaseBlock
    {
        public override int Durability { get; set; } = -1;
        public override Color BackgroundColor { get; set; } = Color.Yellow;
        public override Color ForegroundColor { get; set; } = Color.Yellow;
        public override char Icon { get; set; } = ' ';
        public override Position Position { get; set; }

        public override bool IsSpawnArea { get; set; } = true;
        public override bool IsInteractive { get; set; } = false;

        public override bool Generate(float val)
        {
            return val >= 55 && val < 75;
        }
    }
}
