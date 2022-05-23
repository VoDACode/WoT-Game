using WoTCore.Models;

namespace Server.Models
{
    class HubClient
    {
        public string ConnectionId { get; }
        public int FieldSize { get; }
        public PlayerModel Player { get; } = new PlayerModel();

        public HubClient(string connectionId, int fieldSize)
        {
            ConnectionId = connectionId;
            FieldSize = fieldSize;
        }
    }
}
