using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoTConsole.Models;
using WoTConsole.Options;
using WoTConsole.Services;
using WoTCore.Models;

namespace WoTConsole.Menu
{
    internal class StartMenu : BaseMenu
    {
        private PlayerModel Player => Game.Instance.Player;
        protected override MenuOptions Options => new MenuOptions()
        {
            ShowMenu = true,
            Marning = new Position(0, 2)
        };
        public StartMenu(int size) : base(size)
        {
        }

        protected override List<MenuComponentModel> MenuComponents => new List<MenuComponentModel>()
        {
            new MenuComponentModel("Your nickname ('$nick$')", EditNick),
            new MenuComponentModel("Create the game", CreateGame_Event),
            new MenuComponentModel("Join the game", JoinGame_Event),
            new MenuComponentModel("Leave", Leave_Event),
        };

        protected override Dictionary<string, string> MenuPatterns => new Dictionary<string, string>
        {
            {"nick", Player.Name }
        };

        public override string Name => MenuConfig.StartMenu;

        protected override void Build()
        {
            int line = 0;
            WriteString(line++, "WoT", isCeneter: true);
            WriteLine(line++);
        }
        protected override void Updata()
        {
            MenuPatterns["nick"] = Player.Name;
            base.Updata();
        }
        protected override void FirstEvent()
        {
            ControlService.Instance.OnBottom += this.EventDownKey;
            ControlService.Instance.OnTop += this.EventUpKey;
            ControlService.Instance.OnSelect += this.EventSpace;
            MenuPatterns["nick"] = Player.Name;
        }
        protected override void LastEvent()
        {
            ControlService.Instance.OnBottom -= this.EventDownKey;
            ControlService.Instance.OnTop -= this.EventUpKey;
            ControlService.Instance.OnSelect -= this.EventSpace;
        }
        private void Leave_Event()
        {
            Console.Clear();
            Close();
            Environment.Exit(0);
        }

        private void EditNick()
        {
            string str = Player.Name;
            SetStringMenu(ref str);
            if(str.Length > 20)
            {
                WriteError("nick > 20");
                return;
            }
            Player.Name = str;
            if (NetworkServise.Instance.IsConnected)
                NetworkServise.Instance.SetNick(Player.Name);
        }

        private void JoinGame_Event()
        {
            Response = new MenuResponse(new MenuOpenResponse(MenuConfig.GameListMenu));
            Close();
        }

        private void CreateGame_Event()
        {
            Response = new MenuResponse(new MenuOpenResponse(MenuConfig.CreateGameMenu));
            Close();
        }
    }
}
