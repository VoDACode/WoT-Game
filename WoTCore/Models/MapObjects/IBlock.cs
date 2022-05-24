namespace WoTCore.Models.MapObjects
{
    public interface IBlock : ICell, IPosition
    {
        public bool IsSpawnArea { get; set; }
        public int Durability { get; set; }
        public bool CanBeBroken { get; set; }
        public bool IsInteractive { get; set; }
        public ICell Background { get; set; }
        public void Damage(int damage);
    }
}
