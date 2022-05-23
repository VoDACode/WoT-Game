using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WoTCore.Models
{
    public interface ICopy<T> where T : class, new()
    {
        public T Copy();
    }
}
