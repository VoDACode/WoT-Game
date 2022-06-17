using Newtonsoft.Json;
using System;
using System.Drawing;

namespace WoTCore.Models
{
    [Serializable]
    public struct CellModel : ICell
    {
        public char Icon { get; set; }
        public Color BackgroundColor { get; set; }
        public Color ForegroundColor { get; set; }

        public string UID { get; set; }

        public void Default()
        {
            Icon = ' ';
            ForegroundColor = Color.White;
            BackgroundColor = Color.Black;
            UID = " ";
        }
    }
}
