using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoTCore.Views
{
    public class GameInfoView
    {
        public int Id { get; }
        public string Name { get; }
        public int PlayerLimits { get; }
        public int PlayerCount { get; }

        public GameInfoView(int id, string name, int playerLimits, int playerCount)
        {
            Id = id;
            Name = name;
            PlayerLimits = playerLimits;
            PlayerCount = playerCount;
        }

    }
}
