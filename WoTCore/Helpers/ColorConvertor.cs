using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class ColorConvertor
    {
        public static ConsoleColor ToConsoleColor(this Color color)
        {
            int index = (color.R > 128 | color.G > 128 | color.B > 128) ? 8 : 0; // Bright bit
            index |= (color.R > 64) ? 4 : 0; // Red bit
            index |= (color.G > 64) ? 2 : 0; // Green bit
            index |= (color.B > 64) ? 1 : 0; // Blue bit

            return (System.ConsoleColor)index;
        }
    }
}
