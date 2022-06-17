using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoTCore.Models;

namespace WoTCore.Views
{
    [Serializable]
    public class UpdateCellView : IPosition
    {
        public Position Position { get; set; }
        public MapCell Cell { get; set; } = new MapCell(false);
    }
}
