using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
        private Timer updatePing;
        private Position LeftTopRegion = new Position(short.MaxValue, short.MaxValue);
        public event Action OnBack;
        const int _mapSize = 16 * 10;
        private WriteableBitmap imageMap = new WriteableBitmap(_mapSize - 9, _mapSize - 9, 96, 96, PixelFormats.Bgr32, null);
        public GameField()
        {
            InitializeComponent();
            Display.Source = imageMap;
            updatePing = new Timer(UpdatePing, null, 1000, 750);
        }

        public void Init()
        {
            NetworkServise.Instance.OnEndGame += OnEndGame;
            NetworkServise.Instance.OnGetMap += OnGetMap;
            NetworkServise.Instance.OnUpdateCells += OnUpdateCells;
            NetworkServise.Instance.OnNewMessage += Instance_OnNewMessage;
            NetworkServise.Instance.OnUpdatePlayerInfo += Instance_OnUpdatePlayerInfo;
            NetworkServise.Instance.SetNick(Storage.Instance.Player.Name);
            Title = $"{NetworkServise.Instance.Host}:{NetworkServise.Instance.Port}";
        }

        private void UpdatePing(object o)
        {
            if (!NetworkServise.Instance.IsConnected)
                return;
            Dispatcher.Invoke(() =>
            {
                PingLable.Content = NetworkServise.Instance.Ping.Milliseconds;
            });
        }

        private void Instance_OnUpdatePlayerInfo()
        {
            var percentage = Math.Round((double)((Storage.Instance.Player.Life * 100) / Storage.Instance.Player.MaxLife), 2);
            LableHP.Content = $"{percentage}% ({Storage.Instance.Player.Life}/{Storage.Instance.Player.MaxLife})";
            ProgressBarHP.Value = percentage;
        }

        private void Instance_OnNewMessage(string obj)
        {
            if (StackPanelChatData.Children.Count > 7)
            {
                StackPanelChatData.Children.Remove(StackPanelChatData.Children[0]);
            }
            StackPanelChatData.Children.Add(new Label() { Content = obj });
        }

        private void OnUpdateCells(List<UpdateCellView> objects)
        {
            Task.Run(() =>
            {
                foreach (var cell in objects)
                {
                    var updataPos = cell.Position - LeftTopRegion.GetNormalize(_mapSize, _mapSize);
                    updataPos.Y += (short)((LeftTopRegion.Y < 0 && LeftTopRegion.X > 0 ? LeftTopRegion.Y * -1 : 0) - 5);
                    updataPos.X += (short)((LeftTopRegion.X < 0 ? LeftTopRegion.X * -1 : 0) - 5);
                    var lastPos = cell.LastPosition - LeftTopRegion.GetNormalize(_mapSize, _mapSize);
                    lastPos.Y += (short)((LeftTopRegion.Y < 0 && LeftTopRegion.X > 0 ? LeftTopRegion.Y * -1 : 0) - 5);
                    lastPos.X += (short)((LeftTopRegion.X < 0 ? LeftTopRegion.X * -1 : 0) - 5);
                    bool status = true;

                    Dispatcher.Invoke(() =>
                    {
                        if (updataPos.Y >= imageMap.Height || updataPos.Y < 0 || updataPos.X >= imageMap.Width || updataPos.X < 0)
                            status = false;
                        else if (lastPos.Y >= imageMap.Height || lastPos.Y < 0 || lastPos.X >= imageMap.Width || lastPos.X < 0)
                            status = false;
                    });

                    if (!status)
                        continue;
                    if (cell.Cell.Content is IPlayer)
                    {
                        writePixel(lastPos.X, lastPos.Y, (cell.Cell.Background as ICell).BackgroundColor);
                        System.Drawing.Color color;
                        var p = cell.Cell.Content as IPlayer;
                        if (p.Killed)
                        {
                            color = System.Drawing.Color.Gray;
                        }
                        else
                        {
                            if (p.UID != Storage.Instance.Player.UID)
                                if (p.Command == Storage.Instance.Player.Command)
                                    color = System.Drawing.Color.FromArgb(0, 255, 0);
                                else
                                    color = System.Drawing.Color.Red;
                            else
                                color = System.Drawing.Color.Orange;
                        }
                        writePixel(updataPos.X, updataPos.Y, color);
                    }
                    else if (cell.Cell.Content is IProjectile)
                    {
                        writePixel(lastPos.X, lastPos.Y, (cell.Cell.Background as ICell).BackgroundColor);
                        writePixel(updataPos.X, updataPos.Y, System.Drawing.Color.White);
                    }
                    else if (cell.Cell.Content is IEmptyObject)
                    {
                        writePixel(updataPos.X, updataPos.Y, (cell.Cell.Background as ICell).BackgroundColor);
                    }
                    else
                    {
                        writePixel(updataPos.X, updataPos.Y,
                            (cell.Cell.Content as ICell).BackgroundColor);
                    }
                }
            });
        }

        private void OnGetMap(MapCell[,] map, Position absolutelyRegion, Position iRegion, Position leftTopRegion, bool endOfMessage, bool fistMessage)
        {
            if (fistMessage)
            {
                ClearMap();
                LeftTopRegion = leftTopRegion;
            }
            Task.Run(() =>
            {
                for (int x = 0; x < map.GetLength(0); x++)
                    for (int y = 0; y < map.GetLength(1); y++)
                    {
                        var color = System.Drawing.Color.Brown;
                        if (map[x, y].Content is IPlayer)
                        {
                            var p = map[x, y].Content as IPlayer;
                            if (p.Killed)
                            {
                                color = System.Drawing.Color.Gray;
                            }
                            else
                            {
                                if (p.UID != Storage.Instance.Player.UID)
                                    if (p.Command == Storage.Instance.Player.Command)
                                        color = System.Drawing.Color.FromArgb(0, 255, 0);
                                    else
                                        color = System.Drawing.Color.Red;
                                else
                                    color = System.Drawing.Color.Orange;
                            }
                        }
                        else if (map[x, y].Background is EmptyObject)
                        {
                            color = x % 2 == 0 && y % 2 == 0 ? System.Drawing.Color.LightGray : System.Drawing.Color.DarkGray;
                        }
                        else
                        {
                            color = (map[x, y].Content is null || map[x, y].Content is EmptyObject ?
                               (map[x, y].Background as ICell) : (map[x, y].Content as ICell)).BackgroundColor;
                        }

                        writePixel((iRegion.X * (map.GetLength(0) - 1)) + x,
                            (iRegion.Y * (map.GetLength(1) - 1)) + y,
                            color);
                    }
            });
        }

        private void ClearMap()
        {
            for (int x = 0; x < imageMap.Width; x++)
                for (int y = 0; x < imageMap.Height; x++)
                {
                    writePixel(x, y, System.Drawing.Color.Black);
                }
        }

        private void OnEndGame(bool obj)
        {
            NetworkServise.Instance.RestartGame();
            NetworkServise.Instance.OnEndGame -= OnEndGame;
            NetworkServise.Instance.OnGetMap -= OnGetMap;
            OnBack?.Invoke();
        }

        public void DownKey(object sender, KeyEventArgs e)
        {
            if (!NetworkServise.Instance.IsConnected)
                return;
            if (TextBoxChatInput.IsEnabled && !(e.Key == Key.Enter || e.Key == Key.Escape))
            {
                var c = (char)KeyInterop.VirtualKeyFromKey(e.Key);
                TextBoxChatInput.Text += e.IsUp ? c.ToString() : c.ToString().ToLower();
                e.Handled = true;
                return;
            }
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
                case Key.Space:
                    NetworkServise.Instance.Shot();
                    break;
                case Key.T:
                    if (!TextBoxChatInput.IsEnabled)
                    {
                        TextBoxChatInput.IsEnabled = true;
                    }
                    break;
                case Key.Escape:
                    TextBoxChatInput.IsEnabled = false;
                    TextBoxChatInput.Text = string.Empty;
                    break;
                case Key.Enter:
                    if (TextBoxChatInput.IsEnabled)
                    {
                        if (!string.IsNullOrEmpty(TextBoxChatInput.Text))
                            NetworkServise.Instance.SendMessage(TextBoxChatInput.Text);
                        TextBoxChatInput.Text = string.Empty;
                        TextBoxChatInput.IsEnabled = false;
                    }
                    break;
            }
        }

        private void Page_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        void writePixel(int x, int y, System.Drawing.Color color)
        {
            Dispatcher.Invoke(() =>
            {
                try
                {
                    // Reserve the back buffer for updates.
                    imageMap.Lock();

                    unsafe
                    {
                        // Get a pointer to the back buffer.
                        IntPtr pBackBuffer = imageMap.BackBuffer;

                        // Find the address of the pixel to draw.
                        pBackBuffer += y * imageMap.BackBufferStride;
                        pBackBuffer += x * 4;

                        // Compute the pixel's color.
                        int color_data = color.R << 16;
                        color_data |= color.G << 8;
                        color_data |= color.B << 0;

                        // Assign the color data to the pixel.
                        *((int*)pBackBuffer) = color_data;
                    }

                    // Specify the area of the bitmap that changed.
                    imageMap.AddDirtyRect(new Int32Rect(x, y, 1, 1));
                }
                finally
                {
                    // Release the back buffer and make it available for display.
                    imageMap.Unlock();
                }
            });
        }

        public System.Drawing.Color GetPixelColor(int x, int y)
        {
            System.Drawing.Color color = System.Drawing.Color.Black;
            unsafe
            {
                // Get a pointer to the back buffer.
                IntPtr pBackBuffer = imageMap.BackBuffer;

                // Find the address of the pixel to draw.
                pBackBuffer += y * imageMap.BackBufferStride;
                pBackBuffer += x * 4;

                // Compute the pixel's color.
                int color_data = *((int*)pBackBuffer);
                color = System.Drawing.Color.FromArgb(
                    (byte)(color_data >> 16),
                    (byte)(color_data >> 8),
                    (byte)(color_data >> 0));
            }
            return color;
        }

        private void ButtonChatSend_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TextBoxChatInput.Text))
                NetworkServise.Instance.SendMessage(TextBoxChatInput.Text);
            TextBoxChatInput.IsEnabled = false;
            TextBoxChatInput.Text = string.Empty;
            TextBoxChatInput.Focusable = false;
        }
    }
}
