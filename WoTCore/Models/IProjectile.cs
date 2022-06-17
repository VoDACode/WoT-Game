using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoTCore.Models.MapObjects;

namespace WoTCore.Models
{
    public interface IProjectile : IPosition, IIcon, IUniversalId, ICell
    {
        public short Speed { get; set; }
        public short Damage { get; set; }
        public short Life { get; set; }
        public PlayerModel Owner { get; }
        public TurnObject Turn { get; }
    }
}
