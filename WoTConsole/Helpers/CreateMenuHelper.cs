using WoTConsole.Models;
using WoTCore.Models;

namespace WoTConsole.Helpers
{
    static class CreateMenuHelper
    {
        public static void ProgressBar(CellModel[,] arr, int yPos, int max, int val, int kX = 0, ConsoleColor color = ConsoleColor.Black, string title = default)
        {
            bool isTitle = !string.IsNullOrWhiteSpace(title);
            int rawCount = isTitle ? 4 : 3;
            string process = $"{val}/{max}";
            int textCenterIndex = (arr.GetLength(0) - 2 - process.Length - kX) / 2;
            for (int y = yPos; y < rawCount + yPos; y++)
                for (int x = kX; x < arr.GetLength(0); x++)
                {
                    arr[x, y].Default();
                    if (y == yPos || y == yPos + rawCount - 1)
                        arr[x, y].Icon = '-';
                    else if (x == 0 || x == arr.GetLength(0) - 1)
                        arr[x, y].Icon = '|';
                }
            if (isTitle)
            {
                int titleCenter = (arr.GetLength(0) - 2 - title.Length - kX) / 2;
                for (int x = titleCenter, p = 0; x < titleCenter + title.Length; x++, p++)
                    arr[x, yPos + 1].Icon = title[p];
            }
            int k = val == 0 ? 0 : ((arr.GetLength(0) - 2) * val)/max;
            int kY = isTitle ? 2 : 1;
            for (int x = 1 + kX, p = 0; x < arr.GetLength(0) - 1; x++)
            {
                if(x >= textCenterIndex && x< textCenterIndex + process.Length)
                {
                    arr[x, yPos + kY].Icon = process[p];
                    p++;
                }
                else if (x <= k)
                    arr[x, yPos + kY].Icon = '*';

                if(x <= k)
                    arr[x, yPos + kY].BackgroundColor = color;
            }
        }
    }
}
