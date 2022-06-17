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
        private static EmptyObject _emptyObject;
        public static EmptyObject Empty => _emptyObject ?? (_emptyObject = new EmptyObject());
        public Position Position { get; set; } = new Position();
    }
}
