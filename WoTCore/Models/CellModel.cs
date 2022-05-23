using System;

namespace WoTCore.Models
{
    [Serializable]
    public struct CellModel : ICell
    {
        public char Icon { get; set; }
        public ConsoleColor BackgroundColor { get; set; }
        public ConsoleColor ForegroundColor { get; set; }

        public string UID { get; set; }

        public void Default()
        {
            Icon = ' ';
            ForegroundColor = ConsoleColor.White;
            BackgroundColor = ConsoleColor.Black;
        }
    }
}
