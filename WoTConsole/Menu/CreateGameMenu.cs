using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using WoTConsole.Models;
using WoTConsole.Options;
using WoTConsole.Services;
using WoTCore.Models;

namespace WoTConsole.Menu
{
    public class CreateGameMenu : BaseMenu
    {
        private int port = -1;
        private MenuResponseCreateGeme menuResponse = new MenuResponseCreateGeme();
        protected override MenuOptions Options => new MenuOptions
        {
            ShowMenu = true,
            Center = true,
            Marning = new Position(0, 2)
        };
        protected override Dictionary<string, string> MenuPatterns => new Dictionary<string, string>
        {
            {"name", menuResponse.Name},
            {"limit", menuResponse.PlayerLimits.ToString()},
            {"port", port.ToString()}
        };
        protected override List<MenuComponentModel> MenuComponents => new List<MenuComponentModel>
        {
            new MenuComponentModel("Name ('$name$')", SetName),
            new MenuComponentModel("Player limit ($limit$)", SetLimit),
            new MenuComponentModel("Port ($port$)", EditPort),
            new MenuComponentModel("Create!", CreateGame),
            new MenuComponentModel("Back", Back)
        };

        public override string Name => MenuConfig.CreateGameMenu;

        public CreateGameMenu(int size) : base(size)
        {
        }

        protected override void Build()
        {
            WriteString(0, "WoT", isCeneter: true);
            WriteLine(1);
            port = new Random().Next(55000, 62999);
        }
        protected override void Updata()
        {
            MenuPatterns["name"] = menuResponse.Name;
            MenuPatterns["limit"] = menuResponse.PlayerLimits.ToString();
            MenuPatterns["port"] = port.ToString();
            base.Updata();
        }
        private void EditPort()
        {
            string str = port.ToString();
            SetStringMenu(ref str, onlyNum: true);
            port = int.Parse(str);
        }
        private void SetName()
        {
            string str = menuResponse.Name;
            SetStringMenu(ref str);
            menuResponse.Name = str;
        }
        private void SetLimit()
        {
            string str = menuResponse.PlayerLimits.ToString();
            SetStringMenu(ref str, onlyNum: true);
            menuResponse.PlayerLimits = int.Parse(str);
        }
        private void CreateGame()
        {
            Task.Factory.StartNew(() => Server.Program.Main(new [] { $"port={port}", "--no-log" }));
            NetworkServise.Instance.OnConnect += OnConnect;
            NetworkServise.Instance.Connect("127.0.0.1", port);
            Console.Title = $"Game '{menuResponse.Name}' 127.0.0.1:{port}";
            WriteInfo($"You game port '{port}'!");
            void OnConnect()
            {
                var res = NetworkServise.Instance.CreateGame(menuResponse.Name, menuResponse.PlayerLimits);
                if (res.StatusCode != HttpStatusCode.OK)
                {
                    WriteError(res.GetString().Result);
                    Updata();
                    return;
                }
                dynamic obj = JsonConvert.DeserializeObject(res.GetString().Result);
                Game.Instance.Player.Command = (short)obj["command"];
                Game.Instance.GameId = (int)obj["gameId"];

                Response = new MenuResponse(new MenuOpenResponse(MenuConfig.GameMenu));
                Close();
                NetworkServise.Instance.OnConnect -= OnConnect;
            }
            Close();
        }
        private void Back()
        {
            Response = new MenuResponse(new MenuOpenResponse(MenuConfig.StartMenu));
            Close();
        }

        protected override void FirstEvent()
        {
            ControlService.Instance.OnBottom += this.EventDownKey;
            ControlService.Instance.OnTop += this.EventUpKey;
            ControlService.Instance.OnSelect += this.EventSpace;
        }
        protected override void LastEvent()
        {
            ControlService.Instance.OnBottom -= this.EventDownKey;
            ControlService.Instance.OnTop -= this.EventUpKey;
            ControlService.Instance.OnSelect -= this.EventSpace;
        }
    }
}
