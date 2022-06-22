using System.Drawing;
using WoTCore.Models;
using WoTCore.Modes;

namespace DefaultMode.Blocks
{
    public class Tree : BaseBlock
    {
        public override int Durability { get; set; } = 1;
        public override Color BackgroundColor { get; set; } = Color.FromArgb(6, 171, 0);
        public override Color ForegroundColor { get; set; } = Color.FromArgb(6, 171, 0);
        public override Position Position { get; set; }
        public override bool IsSpawnArea { get; set; } = false;
        public override bool IsInteractive { get; set; } = true;
        public override ICell Background { get; set; } = new Bush();
        public override bool CanBeBroken { get; set; } = true;
        public override char Icon { get; set; } = '*';

        public override bool Generate(float val)
        {
            return val >= 160 && val < 210;
        }

        public override bool OnTouch(object sender, MapCell cell)
        {
            Damage(50);
            this.BackgroundColor = Background.BackgroundColor;
            this.ForegroundColor = Background.ForegroundColor;
            cell.Background = Background;
            return true;
        }
    }
}
