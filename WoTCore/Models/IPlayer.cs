using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoTCore.Models.MapObjects;

namespace WoTCore.Models
{
    public interface IPlayer : IPosition, IIcon, IUniversalId
    {
        public DateTime LastStep { get; set; }
        public DateTime LastShotTime { get; set; }
        public string Name { get; set; }
        public string Session { get; set; }
        public int MaxLife { get; set; }
        public int Life { get; set; }
        public int Command { get; set; }
        public TurnObject Turn { get; set; }
        public bool Killed { get; }
        public bool TryGoTo(TurnObject turn, int size, Map map);
    }
}
