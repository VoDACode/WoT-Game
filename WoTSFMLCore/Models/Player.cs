using WoTSFMLCore.Interfaces;

namespace WoTSFMLCore.Models
{
    public class Player : IPlayerShortInfo, IPlayerPosition
    {
        public string Name { get; }
        public int Murders { get; set; }
        public string Id { get; }

        public float MaxHealth { get; set; }
        public float Health { get; set; }

        public float HealthInPercentages => (100 * Health) / MaxHealth;

        public PositionF Position { get; set; }

        public Player(string name, float maxHealth, PositionF position)
        {
            Name = name;
            Id = Guid.NewGuid().ToString();
            MaxHealth = maxHealth;
            Position = position;
        }

        public Player(string name, float maxHealth) : this(name, maxHealth, new PositionF())
        { }
    }
}
