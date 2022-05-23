using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoTCore.Modes
{
    [Serializable()]
    public abstract class BaseEvents
    {
        public virtual void Setup() { }
        public virtual void Tick() { }
    }
}
