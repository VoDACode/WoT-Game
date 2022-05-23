using System.Text;
using WoTConsole.Models;
using WoTConsole.Helpers;
using WoTCore.Models;
using WoTConsole.Menu;

namespace WoTConsole.Services
{
    class DisplayService : Service
    {
        private static DisplayService instance;
        private List<BaseMenu> menus = new List<BaseMenu>();
        private List<Action> drawQueue { get; } = new List<Action>();
        private BaseMenu MenuNow;

        public int Size { get; private set; }

        public static DisplayService Instance {get
        {
            if(instance == null)
                instance = new DisplayService();
            return instance;
        } }

        private DisplayService() { }

        protected override void Execute()
        {
            AddToQueue(printMenu);
            while (_isStart)
            {
                while (drawQueue.Count == 0 && !MenuNow.IsUpdate) Thread.Sleep(10);
                if(MenuNow.IsUpdate)
                    AddToQueue(printMenu);
                for (int i = 0; i < drawQueue.Count; i++)
                {
                    drawQueue[i]();
                }
                drawQueue.Clear();
                Thread.Sleep(20);
            }
        }

        private void printMenu()
        {
            var res = MenuNow.Draw();
            if (res.Data != null)
            {
                if (res.Data is MenuOpenResponse)
                {
                    SetMenu(res);
                    AddToQueue(printMenu);
                }
                else
                {

                }
            }
        }

        public void AddToQueue(Action action)
        {
            drawQueue.Add(action);
        }

        public void InitGameField(int size)
        {
            Size = size;
            Console.OutputEncoding = Encoding.UTF8;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.CursorVisible = false;
            Console.SetWindowSize(size + 25 + 1, size);
            
            DisplayInfoZone.Instance.Init(new Position(size, 0), 25);
            ConsoleHelper.DisableConsoleMouseInput();
            ConsoleHelper.DesableResize();
            menus.Add(new StartMenu(size));
            menus.Add(new CreateGameMenu(size));
            menus.Add(new GameMenu(size));
            menus.Add(new GameListMenu(size));
            menus.Add(new RestartGameMenu(20));
            MenuNow = menus[0];
            MenuNow.Select();
        }

        private void SetMenu(MenuResponse response)
        {
            var data = response.Data as MenuOpenResponse;
            if(data == null || string.IsNullOrWhiteSpace(data.Name) || !menus.Any(p => p.Name == data.Name))
                return;
            MenuNow = menus.Single(p => p.Name == data.Name);
            MenuNow.Select();
        }
    }
}
