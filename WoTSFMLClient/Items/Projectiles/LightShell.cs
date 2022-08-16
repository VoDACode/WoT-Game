using SFML.Window;

namespace WoTSFMLClient.Items.Projectiles
{
    public class LightShell : Projectile
    {
        public LightShell(Window window) 
            : base(window, "Light_Shell", 5, 50, TimeSpan.FromMilliseconds(500))
        {
        }
    }
}
