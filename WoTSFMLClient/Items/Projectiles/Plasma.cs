using SFML.Window;

namespace WoTSFMLClient.Items.Projectiles
{
    public class Plasma : Projectile
    {
        public Plasma(Window window) 
            : base(window, "Plasma", 2, int.MaxValue, TimeSpan.FromMilliseconds(250))
        {
        }
    }
}
