using SFML.Window;

namespace WoTSFMLClient.Items.Projectiles
{
    public class MediumShell : Projectile
    {
        public MediumShell(Window window) 
            : base(window, "Medium_Shell", 10, 45, TimeSpan.FromSeconds(1.5))
        {
        }
    }
}
