using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoTCore.Models;

namespace WoTCore.Game
{
    public abstract class GameContext
    {
        protected List<ModeContent> modes = new List<ModeContent>();
        public IReadOnlyList<ModeContent> Modes => modes;
        public PlayerModel Player { get; } = new PlayerModel();
        public int GameId { get; set; } = 0;
        protected virtual short gameFieldSize => 40;
        public Dictionary<string, object> ChangeValus { get; } = new Dictionary<string, object>();

        public abstract void Start();
    }
}
