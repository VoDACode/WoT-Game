using System;
using System.Drawing;
using WoTCore.Models;
using WoTCore.Modes;

namespace DefaultMode.Blocks
{
    [Serializable()]
    public class Bush : BaseBlock
    {
        public override int Durability { get; set; } = 5;
        public override Color BackgroundColor { get; set; } = Color.FromArgb(56, 199, 0);
        public override Color ForegroundColor { get; set; } = Color.FromArgb(56, 199, 0);
        public override char Icon { get; set; } = ' ';
        public override Position Position { get; set; } = new Position();

        public override bool IsSpawnArea { get; set; } = true;
        public override bool IsInteractive { get; set; } = false;

        public override bool Generate(float val)
        {
            return (val >= 75 && val < 160) || (val >= 210 && val < 220);
        }
    }
}
