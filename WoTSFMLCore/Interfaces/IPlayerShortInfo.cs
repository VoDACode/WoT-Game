using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoTSFMLCore.Interfaces
{
    public interface IPlayerShortInfo : IPlayerId
    {
        public string Name { get; }
        public int Murders { get; set; }
    }
}
