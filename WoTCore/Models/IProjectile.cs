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
        public int Speed { get; set; }
        public int Damage { get; set; }
        public int Life { get; set; }
        public PlayerModel Owner { get; }
        public TurnObject Turn { get; }
    }
}
