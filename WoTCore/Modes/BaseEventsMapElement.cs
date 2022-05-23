using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoTCore.Modes
{
    [Serializable()]
    public abstract class BaseEventsMapElement : BaseEvents, IGenerate
    {
        public abstract bool Generate(float val);
        public virtual bool OnTouch(object sender) => false;
    }
}
