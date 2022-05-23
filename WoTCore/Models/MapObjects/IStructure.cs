using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoTCore.Models.MapObjects
{
    public interface IStructure : IPosition, IUniversalId
    {
        public Dictionary<Position, IBlock> Blocks { get; }
    }
}
