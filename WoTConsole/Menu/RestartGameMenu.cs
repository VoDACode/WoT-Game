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
    internal class RestartGameMenu : BaseMenu
    {
        public RestartGameMenu(int size) : base(size)
        {
        }
        protected override MenuOptions Options => new MenuOptions()
        {
            ShowMenu = true,
            Center = true,
            Marning = new Position(0, 3)
        };
        protected override List<MenuComponentModel> MenuComponents => new List<MenuComponentModel>
        {
            new MenuComponentModel("Restart!", RestartGame),
            new MenuComponentModel("Close", CloseGame)
        };

        protected override Position Magin => new Position(10, 10);
        public override string Name => MenuConfig.RestartGameMenu;
        protected override void Build()
        {
            WriteString(1, "Restart game?", isCeneter: true);
            NetworkServise.Instance.OnEndGame += OnEndGame;
        }

        private void OnEndGame(bool isWin)
        {
            for (int x = 0; x < Field.GetLength(0); x++)
                Field[x, 0].Default();
            WriteString(0, $"You {(isWin ? "won" : "lost")}!", isCeneter: true);
        }

        protected override void FirstEvent()
        {
            ControlService.Instance.OnBottom += this.EventDownKey;
            ControlService.Instance.OnTop += this.EventUpKey;
            ControlService.Instance.OnSelect += this.EventSpace;
            NetworkServise.Instance.OnRestartGame += OnRestartGame;
        }

        protected override void LastEvent()
        {
            ControlService.Instance.OnBottom -= this.EventDownKey;
            ControlService.Instance.OnTop -= this.EventUpKey;
            ControlService.Instance.OnSelect -= this.EventSpace;
            NetworkServise.Instance.OnRestartGame -= OnRestartGame;
        }

        private void OnRestartGame()
        {
            Response = new MenuResponse(new MenuOpenResponse(MenuConfig.GameMenu));
            Close();
        }

        private void CloseGame()
        {
            NetworkServise.Instance.StopHub().Wait();
            Response = new MenuResponse(new MenuOpenResponse(MenuConfig.StartMenu));
            Close();
        }

        private void RestartGame()
        {
            var res = NetworkServise.Instance.RestartGame();
            if(res.StatusCode != System.Net.HttpStatusCode.OK)
            {
                WriteError(res.Content.ReadAsStringAsync().Result);
                CloseGame();
                return;
            }
        }
    }
}
