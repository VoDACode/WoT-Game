using SFML.Window;

namespace WoTSFMLClient.Items.Projectiles
{
    public class Laser : Projectile
    {
        public Laser(Window window) 
            : base(window, "Laser", 3, int.MaxValue, TimeSpan.FromMilliseconds(500))
        {
        }
    }
}
