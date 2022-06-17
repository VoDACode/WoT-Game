using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoTCore.Models;
using WoTCore.Models.MapObjects;

namespace WoTCore.Modes
{
    [Serializable()]
    public abstract class BaseBlock : BaseEventsMapElement, IBlock
    {
        public abstract int Durability { get; set; }
        public abstract Color BackgroundColor { get; set; }
        public abstract Color ForegroundColor { get; set; }
        public virtual char Icon { get; set; } = ' ';
        public abstract Position Position { get; set; }
        public string UID { get; set; } = " ";
        public abstract bool IsSpawnArea { get; set; }
        public virtual bool CanBeBroken { get; set; } = true;
        public abstract bool IsInteractive { get; set; }
        public virtual ICell Background { get; set; }

        public void Damage(int damage)
        {
            if (Durability >= damage)
                Durability -= damage;
            else
                Durability = 0;
        }

    }
}
