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
        public int Command { get; set; }
        public JoinGameView(int gameId, int command)
        {
            this.GameId = gameId;
            this.Command = command;
        }
    }
}
