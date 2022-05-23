using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoTCore.Models
{
    public abstract class MapModel : IPosition, IIcon
    {
        public abstract char Icon { get; }

        public Position Position { get; set; }
        public abstract bool Broken { get; }
    }
}
