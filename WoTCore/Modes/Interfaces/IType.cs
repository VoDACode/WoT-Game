using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoTCore.Modes.Interfaces
{
    public interface IType
    {
        public object GetObject();
        public Type Type();
        public void SetObject(object value);
    }
}
