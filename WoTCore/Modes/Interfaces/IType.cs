using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoTCore.Modes.Interfaces
{
    public interface IType
    {
        [field: NonSerialized]
        public Type ItemType { get; set; }
        [field: NonSerialized]
        public object Item { get; }
    }
}
