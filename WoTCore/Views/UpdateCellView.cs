using System;
using WoTCore.Models;

namespace WoTCore.Views
{
    [Serializable]
    public class UpdateCellView : IPosition
    {
        public Position LastPosition { get; set; } 
        public Position Position { get; set; }
        public MapCell Cell { get; set; } = new MapCell(false);
    }
}
