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
    public abstract class BaseStructure : BaseEventsMapElement, IStructure
    {
        public int Durability { get; set; }
        public abstract Dictionary<Position, IBlock> Blocks { get; set; }
        public abstract Position Position { get; set; }
        public string UID { get; set; }
    }
}
