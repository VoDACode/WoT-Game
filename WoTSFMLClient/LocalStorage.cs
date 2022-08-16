using WoTSFMLClient.Items;

namespace WoTSFMLClient
{
    internal static class LocalStorage
    {
        public static List<Projectile> Projectiles { get; } = new List<Projectile>();
        public static Tank Tank { get; set; }
    }
}
