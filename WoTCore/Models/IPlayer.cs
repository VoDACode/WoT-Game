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
        public short MaxLife { get; set; }
        public short Life { get; set; }
        public short Command { get; set; }
        public TurnObject Turn { get; set; }
        public bool Killed { get; }
        public Position PositionInChunks { get; }
        public bool TryGoTo(TurnObject turn, Map map);
    }
}
