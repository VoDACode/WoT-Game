using SFML.Window;

namespace WoTSFMLClient.Items.Projectiles
{
    public class HeavyShell : Projectile
    {
        public HeavyShell(Window window) 
            : base(window, "Heavy_Shell", 50, 15, TimeSpan.FromSeconds(5))
        {
        }
    }
}
