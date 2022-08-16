using SFML.Graphics;
using SFML.System;
using SFML.Window;
using WoTSFMLClient.Models;

namespace WoTSFMLClient.Items
{
    public enum ProjectileType { Shell, Laser, Plasma }
    public class Projectile : BaseItem
    {
        private string _projectileFile;
        private DateTime lastAnimationTime = DateTime.MinValue;
        private Sprite projectile;
        public string TextureFile { get; }

        public override Vector2f Size
        {
            get => new Vector2f(projectile.Texture.Size.X, projectile.Texture.Size.Y);
            set => throw new NotImplementedException();
        }
        public override PositionData Position
        {
            get => projectile.Position;
            set
            {
                _position = value;
                projectile.Position = _positionInPixel;
            }
        }

        public Vector2f Origin
        {
            get => projectile.Origin;
            set => projectile.Origin = value;
        }

        public float Rotation
        {
            get => projectile.Rotation;
            set => projectile.Rotation = value;
        }

        public float Damage { get; set; }
        public int Life { get; set; }
        public TimeSpan RechargeSpeed { get; set; }

        public event Action OnEndAnimation;

        public Projectile(Window window, string projectile, float damage, int life, TimeSpan rechargeSpeed) : base(window)
        {
            TextureFile = $@"Sources\PNG\Effects\{projectile}.png";
            this.projectile = new Sprite(new Texture(TextureFile));
            Damage = damage;
            Life = life;
            RechargeSpeed = rechargeSpeed;
            _projectileFile = projectile;
        }

        public Projectile Copy()
        {
            return new Projectile(OwnerWindow, _projectileFile, Damage, Life, RechargeSpeed);
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            
            if (lastAnimationTime == DateTime.MinValue)
                lastAnimationTime = DateTime.Now;
            else if (lastAnimationTime + TimeSpan.FromMilliseconds(85) > DateTime.Now)
            {
                float radians = (float)Math.PI / 180 * Rotation;
                Position = new PositionData(
                    Position.X + (float)(15 * Math.Sin(radians)),
                    Position.Y - (float)(15 * Math.Cos(radians)));
                target.Draw(projectile);
                return;
            }
            target.Draw(projectile);
            lastAnimationTime = DateTime.Now;
        }
    }
}
