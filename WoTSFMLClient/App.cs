using SFML.Graphics;
using SFML.Window;
using WoTSFMLClient.Pages;

namespace WoTSFMLClient
{
    public static class App
    {
        private static bool _isRunning = false;
        private static IReadOnlyList<Drawable> drawables = new List<Drawable>();
        public static RenderWindow Window { get; }
        public static bool IsRunning => _isRunning;
        
        static App()
        {
            Window = new RenderWindow(new VideoMode(1000, 1000), "WoT SFML Client", Styles.Fullscreen);
            Window.SetVerticalSyncEnabled(true);
            Window.KeyPressed += Window_KeyPressed;
        }

        private static void Window_KeyPressed(object? sender, KeyEventArgs e)
        {
            if (e.Alt && e.Code == Keyboard.Key.F4)
                Exit();
        }

        public static void Run()
        {
            _isRunning = true;;
            while (Window.IsOpen && IsRunning)
            {
                Window.Clear();
                foreach (Drawable drawable in drawables)
                    Window.Draw(drawable);
                Window.Display();
                Window.DispatchEvents();
            }
            Window.Close();
        }

        public static void Exit()
        {
            _isRunning = false;
        }

        public static class Navigator
        {
            private static int _pointer = 0;
            private static List<BasePage> menus = new List<BasePage>();
            public static int Pointer => _pointer;
            public static void Push<T>(T menu) where T : BasePage
            {
                menus.Insert(_pointer, menu);
                _pointer++;
                menu.Loaded();
                drawables = menu.Items;
            }

            public static void Pop()
            {
                if (Pointer - 1 < 0)
                {
                    menus[0].Unloaded();
                    _pointer = menus.Count;
                    menus.RemoveAt(0);
                }
                else
                {
                    _pointer--;
                    menus[_pointer].Unloaded();
                    menus.RemoveAt(_pointer);
                }
                menus[_pointer - 1].Loaded();
                drawables = menus[_pointer - 1].Items;           
            }

            public static void Back()
            {
                if (Pointer - 1 < 0)
                {
                    menus[0].Unloaded();
                    _pointer = menus.Count;
                }
                else
                {
                    menus[_pointer].Unloaded();
                    _pointer--;
                }
                menus[_pointer].Loaded();
                drawables = menus[_pointer].Items;
            }
        }
    }
}
