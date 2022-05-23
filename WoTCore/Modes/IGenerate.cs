using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoTCore.Models;

namespace WoTCore.Modes
{
    [interface: NonSerialized]
    public interface IGenerate
    {
        public bool Generate(float val);
    }
}
