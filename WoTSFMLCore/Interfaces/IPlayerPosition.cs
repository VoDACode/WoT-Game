using WoTSFMLCore.Models;

namespace WoTSFMLCore.Interfaces
{
    public interface IPlayerPosition : IPlayerId
    {
        public PositionF Position { get; set; }
    }
}
