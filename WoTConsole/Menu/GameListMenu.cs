using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WoTConsole.Models;
using WoTConsole.Options;
using WoTConsole.Services;
using WoTCore.Models;
using WoTCore.Views;

namespace WoTConsole.Menu
{
    internal class GameListMenu : BaseMenu
    {
        private List<GameInfoView> games = new List<GameInfoView>();
        public GameListMenu(int size) : base(size)
        {
        }
        protected override MenuOptions Options => new MenuOptions
        {
            Marning = new Position(2, 2),
            ShowMenu = true,
            Center = false,
        };
        public override string Name => MenuConfig.GameListMenu;

        protected override void Build()
        {
            Console.Clear();
            WriteString(0, "WoT Games", isCeneter: true);
            WriteLine(1);
            NetworkServise.Instance.OnUpdateGamesList += OnUpdateGamesList;
            NetworkServise.Instance.OnConnect += () =>
            {
                NetworkServise.Instance.UpdateGameList(true);
            };
        }

        private void OnUpdateGamesList(List<GameInfoView> objects)
        {
            games.Clear();
            games.AddRange(objects);
            this.MenuComponents.Clear();
            MenuComponents.Add(new MenuComponentModel($"Host: {NetworkServise.Instance.Host}:{NetworkServise.Instance.Port}", EditHost));
            MenuComponents.Add(new MenuComponentModel("Back", Back));
            for (int i = 0; i < objects.Count; i++)
                this.MenuComponents.Add(new MenuComponentModel($"{objects[i].Name}\t{objects[i].PlayerCount}/{objects[i].PlayerLimits}", OnJoinTheGame));
            if (VirtualPointerPosition.Y >= objects.Count + 2)
            {
                this.VirtualPointerPosition.Y = 0;
                this.PointerPosition.Y = this.StartPointerPosition.Y;
            }
            Updata();
        }

        private async void EditHost()
        {
            string host = $"{NetworkServise.Instance.Host}:{NetworkServise.Instance.Port}";
            SetStringMenu(ref host);
            string[] tmp = host.Split(':');
            if (tmp.Length < 2)
            {
                WriteError("Incorrect format!");
                return;
            }
            int port;
            if (tmp.Length != 2 || !int.TryParse(tmp[1], out port) || tmp[0].Any(p => p == ' '))
            {
                WriteError("Incorrect format!");
            }
            else
            {
                try
                {
                    await NetworkServise.Instance.Connect(tmp[0], port);
                }
                catch
                {
                    WriteError("Incorrect host!", TimeSpan.FromSeconds(3));
                }
            }
            Updata();
        }

        private void Back()
        {
            Response = new MenuResponse(new MenuOpenResponse(MenuConfig.StartMenu));
            Close();
        }

        private void OnJoinTheGame()
        {
            var itemIndex = VirtualPointerPosition.Y - 2;
            if (itemIndex < 0 && itemIndex >= games.Count)
                return;
            var item = games[itemIndex];
            var res = NetworkServise.Instance.Join(item.Id);
            if(res.StatusCode == HttpStatusCode.BadRequest)
            {
                WriteError(res.Content.ReadAsStringAsync().Result);
                return;
            }else if(res.StatusCode == HttpStatusCode.NotFound)
            {
                WriteError("Not found!");
                return;
            }else if(res.StatusCode == HttpStatusCode.OK)
            {
                NetworkServise.Instance.UpdateGameList(false);
                var data = Newtonsoft.Json.JsonConvert.DeserializeObject<JoinGameView>(res.Content.ReadAsStringAsync().Result);
                Game.Instance.Player.Command = data.Command;
                Game.Instance.GameId = data.GameId;
                Response = new MenuResponse(new MenuOpenResponse(MenuConfig.GameMenu));
                Close();
                return;
            }
        }

        protected override void FirstEvent()
        {
            this.MenuComponents.Clear();
            MenuComponents.Add(new MenuComponentModel($"Host: {NetworkServise.Instance.Host}:{NetworkServise.Instance.Port}", EditHost));
            MenuComponents.Add(new MenuComponentModel("Back", Back));
            ControlService.Instance.OnBottom += this.EventDownKey;
            ControlService.Instance.OnTop += this.EventUpKey;
            ControlService.Instance.OnSelect += this.EventSpace;
            if (NetworkServise.Instance.IsConnected)
                NetworkServise.Instance.UpdateGameList(true);
            Updata();
        }

        protected override void LastEvent()
        {
            ControlService.Instance.OnBottom -= this.EventDownKey;
            ControlService.Instance.OnTop -= this.EventUpKey;
            ControlService.Instance.OnSelect -= this.EventSpace;
            if(NetworkServise.Instance.IsConnected)
                NetworkServise.Instance.UpdateGameList(false);
        }
    }
}
