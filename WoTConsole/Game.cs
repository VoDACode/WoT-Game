using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoTConsole.Services;
using WoTConsole.Models;
using WoTCore.Models;
using Server;
using System.Diagnostics;
using System.Net;
using WoTCore.Game;

namespace WoTConsole
{
    public class Game : GameContext
    {

        private static Game _instance;
        public static Game Instance => _instance ?? (_instance = new Game());

        private Game()
        {
        }

        public override void Start()
        {
            modes = ModesLoader.ModesLoader.Load();
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, sslPolicyErrors) => true;
            DisplayService.Instance.InitGameField(gameFieldSize);
            DisplayService.Instance.Start();
            ControlService.Instance.Start();
            NetworkServise.Instance.Start();
        }
    }
}
