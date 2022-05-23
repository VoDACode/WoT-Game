using Server.Extensions;
using System.Collections.Generic;
using WoTCore.Models;

namespace Server.Models
{
    class CommandModel : IIdentificator
    {
        public int Id { get; set; }
        private List<PlayerModel> _players = new List<PlayerModel>();
        public IReadOnlyList<PlayerModel> Players => _players;
        public CommandModel() { }
        public CommandModel(int id)
        {
            Id = id;
        }

        public void AddPlayer(PlayerModel player)
        {
            _players.Add(player);
        }

        public bool Leave(PlayerModel player)
        {
            bool status = _players.Remove(player);
            return status;
        }
    }
}
