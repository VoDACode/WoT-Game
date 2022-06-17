using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoTCore.Game;
using WoTWpfClient.Services;

namespace WoTWpfClient
{
    internal class Game : GameContext
    {
        private static Game? _instance;
        public static Game Instance => _instance ?? (_instance = new Game());
        public override void Start()
        {
            NetworkServise.Instance.Start();
        }
    }
}
