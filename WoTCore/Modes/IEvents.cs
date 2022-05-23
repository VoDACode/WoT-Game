using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoTCore.Modes
{
    public interface IEvents
    {
        public void Tick();
        public void Setup();
        public bool OnTouch(object sender);
    }
}
