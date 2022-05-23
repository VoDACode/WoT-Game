using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoTCore.Models.MapObjects;

namespace WoTCore.Models
{
    [Serializable]
    public class EmptyObject : IEmptyObject
    {
        public Position Position { get; set; }
    }
}
