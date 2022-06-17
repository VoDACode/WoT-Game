using System;
using System.Drawing;
using WoTCore.Models.MapObjects;

namespace WoTCore.Models
{
    public interface ICell : IIcon, IUniversalId
    {
        public Color BackgroundColor { get; set; }
        public Color ForegroundColor { get; set; }
    }
}
