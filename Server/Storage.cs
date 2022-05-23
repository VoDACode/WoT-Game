using Server.Extensions;
using Server.Models;
using System.Collections.Generic;
using WoTCore.Models;

namespace Server
{
    sealed class Storage
    {
        private static Storage instance;
        public static Storage Instance => instance ?? (instance = new Storage());
        private Storage() { }

        public List<string> WaitUsersConnection { get; } = new List<string>(100);
        public List<HubClient> HubClients { get; } = new List<HubClient>();
        public IdList<GameModel> Games { get; } = new IdList<GameModel>();
        public IdList<CommandModel> Commands { get; } = new IdList<CommandModel>();
        public List<ModeContent> Modes { get; } = new List<ModeContent>();
    }
}
