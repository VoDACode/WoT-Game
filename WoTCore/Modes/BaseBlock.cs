using System;
using System.Collections.Generic;
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
        public abstract ConsoleColor BackgroundColor { get; set; }
        public abstract ConsoleColor ForegroundColor { get; set; }
        public abstract char Icon { get; set; }
        public abstract Position Position { get; set; }
        public string UID { get; set; }
        public abstract bool IsSpawnArea { get; set; }
        public virtual bool CanBeBroken { get; set; } = true;
        public abstract bool IsInteractive { get; set; }

        public void Damage(int damage)
        {
            if (Durability >= damage)
                Durability -= damage;
            else
                Durability = 0;
        }
    }
}
