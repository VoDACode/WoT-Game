using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoTCore.Views
{
    public class JoinGameView
    {
        public int GameId { get; set; }
        public short Command { get; set; }
        public JoinGameView(int gameId, short command)
        {
            this.GameId = gameId;
            this.Command = command;
        }
    }
}
