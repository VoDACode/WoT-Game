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

namespace WoTConsole
{
    public class Game : IDisposable
    {

        private static Game _instance;
        public static Game Instance => _instance ?? (_instance = new Game());
        private List<ModeContent> modes = new List<ModeContent>();
        public IReadOnlyList<ModeContent> Modes => modes;
        public PlayerModel Player { get; } = new PlayerModel();
        public int GameId { get; set; } = 0;
       
        private int gameFieldSize = 40;
        public Dictionary<string, object> ChangeValus { get; } = new Dictionary<string, object>();

        private Game()
        {
        }

        public void Start()
        {
            modes = ModesLoader.ModesLoader.Load();
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, sslPolicyErrors) => true;
            DisplayService.Instance.InitGameField(gameFieldSize);
            DisplayService.Instance.Start();
            ControlService.Instance.Start();
            NetworkServise.Instance.Start();
        }

        public void Dispose()
        {
            
        }
    }
}
