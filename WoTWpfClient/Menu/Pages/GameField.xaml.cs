using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WoTCore.Models;
using WoTCore.Views;
using WoTWpfClient.Services;

namespace WoTWpfClient.Menu.Pages
{
    /// <summary>
    /// Interaction logic for GameField.xaml
    /// </summary>
    public partial class GameField : Page
    {
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);


        private Position LeftTopRegion = new Position(short.MaxValue, short.MaxValue);

        Timer updateMapTimer;
        public event Action OnBack;
        private bool _updateMap = false;
        const int _pixelSize = 12;
        const int _mapSize = 16 * 11;
        private Bitmap imageMap = new Bitmap(_pixelSize * _mapSize, _pixelSize * _mapSize);
        public GameField()
        {
            InitializeComponent();
            Display.Height = 500;
            Display.Width = 500;
        }

        public void Init()
        {
            NetworkServise.Instance.OnEndGame += OnEndGame;
            NetworkServise.Instance.OnGetMap += OnGetMap;
            NetworkServise.Instance.OnUpdateCells += OnUpdateCells;
            if (updateMapTimer == default)
                updateMapTimer = new Timer(UpdateGameMap, null, 0, 20);
            else
                updateMapTimer.Change(0, 20);
        }

        private void OnUpdateCells(List<UpdateCellView> objects)
        {
            foreach (var cell in objects)
            {
                var updataPos = cell.Position - LeftTopRegion;
                updataPos.Y -= 5;
                updataPos.X -= 5;
                if (cell.Cell.Content is IPlayer)
                {
                    writePixel(updataPos.X * _pixelSize, updataPos.Y * _pixelSize, System.Drawing.Color.Red);
                }
                else
                {
                    writePixel(updataPos.X * _pixelSize, updataPos.Y * _pixelSize,
                        (cell.Cell.Content as ICell).BackgroundColor);
                }
            }
            UpdateMap();
        }

        private void OnGetMap(MapCell[,] map, Position absolutelyRegion, Position iRegion, Position leftTopRegion, bool endOfMessage, bool fistMessage)
        {
            if (fistMessage)
            {
                ClearMap();
                LeftTopRegion = leftTopRegion;
            }
            for (int x = 0; x < map.GetLength(0); x++)
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    var color = System.Drawing.Color.Brown;
                    if (map[x, y].Content is IPlayer)
                    {
                        color = System.Drawing.Color.Red;
                    }
                    else if (map[x, y].Background is EmptyObject)
                    {
                        color = System.Drawing.Color.Black;
                    }
                    else
                    {
                        color = (map[x, y].Content == default || map[x, y].Content is EmptyObject ?
                           (map[x, y].Background as ICell) : (map[x, y].Content as ICell)).BackgroundColor;
                    }

                    writePixel((iRegion.X * (map.GetLength(0) - 1) * _pixelSize) + x * _pixelSize,
                        (iRegion.Y * (map.GetLength(1) - 1) * _pixelSize) + y * _pixelSize,
                        color);
                }
            if (endOfMessage)
                UpdateMap();
        }

        private void ClearMap()
        {
            for (int x = 0; x < imageMap.Width; x++)
                for (int y = 0; x < imageMap.Height; x++)
                {
                    imageMap.SetPixel(x, y, System.Drawing.Color.Black);
                }
            UpdateMap();
        }

        private void OnEndGame(bool obj)
        {
            NetworkServise.Instance.OnEndGame -= OnEndGame;
            NetworkServise.Instance.OnGetMap -= OnGetMap;
            updateMapTimer.Change(0, 100000);
            OnBack?.Invoke();
        }

        private void UpdateGameMap(object? state)
        {
            if (!_updateMap)
                return;
            Dispatcher.Invoke(() =>
            {
                Display.Source = ImageSourceFromBitmap(imageMap);
                _updateMap = false;
            });

        }

        public void KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.W:
                case Key.Up:
                    NetworkServise.Instance.GoTo(TurnObject.Top);
                    break;
                case Key.Down:
                case Key.S:
                    NetworkServise.Instance.GoTo(TurnObject.Bottom);
                    break;
                case Key.Left:
                case Key.A:
                    NetworkServise.Instance.GoTo(TurnObject.Left);
                    break;
                case Key.Right:
                case Key.D:
                    NetworkServise.Instance.GoTo(TurnObject.Right);
                    break;
            }
        }

        private void Page_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void UpdateMap()
        {
            _updateMap = true;
        }

        public ImageSource ImageSourceFromBitmap(Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally { DeleteObject(handle); }
        }

        void writePixel(int x, int y, System.Drawing.Color color)
        {
            if (x + _pixelSize >= imageMap.Width || y + _pixelSize >= imageMap.Height || x < 0 || y < 0)
                return;
            try
            {
                for (int ix = x; ix < _pixelSize + x; ix++)
                    for (var iy = y; iy < _pixelSize + y; iy++)
                    {
                        imageMap.SetPixel(ix, iy, color);
                    }
            }
            catch (Exception ex)
            {
                throw new Exception($"X: {x}, Y: {y}\n{ex}");
            }
        }
    }
}
