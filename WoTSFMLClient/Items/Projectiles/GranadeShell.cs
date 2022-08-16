using SFML.Window;

namespace WoTSFMLClient.Items.Projectiles
{
    public class GranadeShell : Projectile
    {
        public GranadeShell(Window window) 
            : base(window, "Granade_Shell", 15, 50, TimeSpan.FromSeconds(4))
        {
        }
    }
}
