using SFML.Graphics;
using SFML.System;
using SFML.Window;
using WoTSFMLClient.Items.Projectiles;
using WoTSFMLClient.Models;

namespace WoTSFMLClient.Items
{
    public class Tank : BaseItem
    {
        private bool stepFlagLeft = false;
        private bool stepFlagRight = false;
        private bool? pressedLeftMouseButton = null;
        private bool drawProjectile = false;
        private Dictionary<char, bool> statusControlButtons { get; } = new Dictionary<char, bool>
        {
            {'A', false},{'W', false},{'S', false},{'D', false},
        };
        private Vector2f TrackLeftOrigin;
        private Vector2f TrackRightOrigin;
        private Effect shotEffectA;
        private Effect shotEffectB;
        private Effect shotEffect => _selectedProjectile is Laser || _selectedProjectile is Plasma ? shotEffectA : shotEffectB;
        private Sprite Body;
        private Sprite TrackA;
        private Sprite TrackB;
        private Sprite Weapon;
        private Projectile? _selectedProjectile;
        private HealthBar healthBar;
        private DateTime lastFireTime = DateTime.Now;

        public float Health => healthBar?.Health ?? 0;

        public float Speed { get; set; }

        public float RechargePercentage
        {
            get
            {
                /*      100     SelectedProjectile.RechargeSpeed.Ticks + lastFireTime.Ticks
                 * 
                 *       x      DateTime.Now.Ticks
                 * 
                 * 
                 * 
                 */
                if (SelectedProjectile == null)
                    return 0;
                return (DateTime.Now.Ticks - lastFireTime.Ticks) * 100f / SelectedProjectile.RechargeSpeed.Ticks;
            }
        }

        public IReadOnlyList<Projectile>? Projectiles { get; }
        public Projectile? SelectedProjectile
        {
            get => _selectedProjectile;
            set
            {
                _selectedProjectile = value;
                lastFireTime = DateTime.Now;
            }
        }

        private char counter(int i) => (char)(65 + i);

        public bool Controlled { get; set; } = false;

        public Tank(Window window, int color, int tankType, float speed = 5, IReadOnlyList<Projectile>? projectiles = null) : base(window)
        {
            Speed = speed;
            int track = tankType % 4;
            track = track == 0 ? 4 : track;

            Projectiles = projectiles;
            if (projectiles != null)
                SelectedProjectile = projectiles.FirstOrDefault();

            shotEffectA = new Effect(window, getEffectName(ProjectileType.Laser), CounterType.Numbers);
            shotEffectB = new Effect(window, getEffectName(ProjectileType.Shell), CounterType.Numbers);

            Body = new Sprite(new Texture(@$"Sources\PNG\Hulls_Color_{counter(color)}\Hull_0{tankType}.png"));
            TrackA = new Sprite(new Texture(@$"Sources\PNG\Tracks\Track_{track}_A.png"));
            TrackB = new Sprite(new Texture(@$"Sources\PNG\Tracks\Track_{track}_B.png"));
            Weapon = new Sprite(new Texture(@$"Sources\PNG\Weapon_Color_{counter(color)}\Gun_0{tankType}.png"));

            healthBar = new HealthBar(window);
            healthBar.Health = 100;

            Position = new PositionData();
            window.KeyPressed += Window_KeyPressed;
            window.MouseMoved += Window_MouseMoved;
            window.KeyReleased += Window_KeyReleased;
            Body.Origin = new Vector2f(Body.Texture.Size.X / 2, Body.Texture.Size.Y / 2 + 30);
            if (tankType == 1)
            {
                TrackLeftOrigin = new Vector2f(Body.Origin.X - TrackA.Texture.Size.X * 0.8f, Body.Origin.Y - 5);
                TrackRightOrigin = new Vector2f(Body.Origin.X - TrackA.Texture.Size.X - Body.Texture.Size.X / 2 - TrackA.Texture.Size.X * 0.2f,
                    Body.Origin.Y - 5);
                Weapon.Origin = new Vector2f(Body.Origin.X - 80, Body.Origin.Y);
            }
            else if (tankType == 2)
            {
                TrackLeftOrigin = new Vector2f(Body.Origin.X - TrackA.Texture.Size.X * 0.9f, Body.Origin.Y - 5);
                TrackRightOrigin = new Vector2f(Body.Origin.X - TrackA.Texture.Size.X - Body.Texture.Size.X / 2 - TrackA.Texture.Size.X * 0.1f,
                    Body.Origin.Y - 5);
                Weapon.Origin = new Vector2f(Body.Origin.X - 84, Body.Origin.Y - 10);
            }
            else if (tankType == 3)
            {
                TrackLeftOrigin = new Vector2f(Body.Origin.X - TrackA.Texture.Size.X * 0.9f - 18, Body.Origin.Y - 5);
                TrackRightOrigin = new Vector2f(Body.Origin.X - TrackA.Texture.Size.X - Body.Texture.Size.X / 2 - TrackA.Texture.Size.X * 0.1f + 18,
                    Body.Origin.Y - 5);
                Weapon.Origin = new Vector2f(Body.Origin.X - 94.5f, Body.Origin.Y - 20);
            }
            else if (tankType == 4)
            {
                TrackA.Scale = new Vector2f(1, 0.83f);
                TrackB.Scale = TrackA.Scale;
                TrackLeftOrigin = new Vector2f(Body.Origin.X - TrackA.Texture.Size.X * 0.99f - 18, Body.Origin.Y - 15);
                TrackRightOrigin = new Vector2f(Body.Origin.X - TrackA.Texture.Size.X - Body.Texture.Size.X / 2 - TrackA.Texture.Size.X * 0.01f + 18,
                    Body.Origin.Y - 15);
                Weapon.Origin = new Vector2f(Body.Origin.X - 80, Body.Origin.Y - 13.5f);
            }
            else if (tankType == 5)
            {
                TrackA.Scale = new Vector2f(1, 1.05f);
                TrackB.Scale = TrackA.Scale;
                TrackLeftOrigin = new Vector2f(Body.Origin.X - TrackA.Texture.Size.X * 0.99f - 3, Body.Origin.Y - 5);
                TrackRightOrigin = new Vector2f(Body.Origin.X - TrackA.Texture.Size.X - Body.Texture.Size.X / 2 - TrackA.Texture.Size.X * 0.01f + 3,
                    Body.Origin.Y - 5);
                Weapon.Origin = new Vector2f(Body.Origin.X - 88.5f, Body.Origin.Y - 0.5f);
            }
            else if (tankType == 6)
            {
                TrackA.Scale = new Vector2f(1, 1.08f);
                TrackB.Scale = TrackA.Scale;
                TrackLeftOrigin = new Vector2f(Body.Origin.X - TrackA.Texture.Size.X * 0.99f, Body.Origin.Y - 5);
                TrackRightOrigin = new Vector2f(Body.Origin.X - TrackA.Texture.Size.X - Body.Texture.Size.X / 2 - TrackA.Texture.Size.X * 0.01f,
                    Body.Origin.Y - 5);
                Weapon.Origin = new Vector2f(Body.Origin.X - 91.5f, Body.Origin.Y - 25.5f);
            }
            else if (tankType == 7)
            {
                TrackA.Scale = new Vector2f(1, 1.08f);
                TrackB.Scale = TrackA.Scale;
                TrackLeftOrigin = new Vector2f(Body.Origin.X - TrackA.Texture.Size.X * 0.9f - 15, Body.Origin.Y - 8);
                TrackRightOrigin = new Vector2f(Body.Origin.X - TrackA.Texture.Size.X - Body.Texture.Size.X / 2 - TrackA.Texture.Size.X * 0.1f + 15,
                    Body.Origin.Y - 8);
                Weapon.Origin = new Vector2f(Body.Origin.X - 84.5f, Body.Origin.Y + 1);
            }
            else if (tankType == 8)
            {
                TrackA.Scale = new Vector2f(1, 0.85f);
                TrackB.Scale = TrackA.Scale;
                TrackLeftOrigin = new Vector2f(Body.Origin.X - TrackA.Texture.Size.X * 0.97f - 18, Body.Origin.Y - 5);
                TrackRightOrigin = new Vector2f(Body.Origin.X - TrackA.Texture.Size.X - Body.Texture.Size.X / 2 - TrackA.Texture.Size.X * 0.03f + 18,
                    Body.Origin.Y - 5);
                Weapon.Origin = new Vector2f(Body.Origin.X - 88.5f, Body.Origin.Y - 13.5f);
            }
            shotEffectA.Origin = new Vector2f(Body.Texture.Size.X / 4, Weapon.Origin.Y + Weapon.Texture.Size.Y / 2);
            shotEffectB.Origin = new Vector2f(Body.Texture.Size.X / 4, Weapon.Origin.Y + Weapon.Texture.Size.Y / 2);
        }

        public override Vector2f Size
        {
            get => Body.Scale;
            set => throw new NotImplementedException();
        }

        public float Rotation
        {
            get => Body.Rotation;
            set
            {
                Body.Rotation = value;
                TrackA.Rotation = value;
                TrackB.Rotation = value;
            }
        }

        public override PositionData Position
        {
            get => Body.Position;
            set
            {
                _position = value;
                Body.Position = _positionInPixel;
                TrackA.Position = _positionInPixel;
                TrackB.Position = _positionInPixel;
                Weapon.Position = _positionInPixel;
                shotEffectA.Position = _positionInPixel;
                shotEffectB.Position = _positionInPixel;
                healthBar.Position = new PositionData(Body.Position.X - healthBar.Size.X / 2, Body.Position.Y);
            }
        }

        public override void Loaded()
        {
            alignCalculate();
            pressedLeftMouseButton = false;
            lastFireTime = DateTime.Now;
        }

        public void Fire()
        {
            Fire(SelectedProjectile);
        }

        public void Fire(Projectile projectile)
        {
            var copy = projectile.Copy();
            if (lastFireTime + copy.RechargeSpeed > DateTime.Now)
                return;
            drawProjectile = true;
            shotEffect.ReloadEffect();
            copy.Origin = shotEffect.Origin;
            copy.Rotation = Weapon.Rotation;
            copy.Position = Weapon.Position;
            SelectedProjectile = projectile;
            copy.OnEndAnimation += SelectedProjectile_OnEndAnimation;
            LocalStorage.Projectiles.Add(copy);
            lastFireTime = DateTime.Now;
        }

        private void SelectedProjectile_OnEndAnimation()
        {
            drawProjectile = false;
            SelectedProjectile.OnEndAnimation -= SelectedProjectile_OnEndAnimation;
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            if (statusControlButtons['A'])
            {
                Rotation -= 1;
                stepFlagRight = !stepFlagRight;
            }
            if (statusControlButtons['D'])
            {
                Rotation += 1;
                stepFlagLeft = !stepFlagLeft;
            }
            if (statusControlButtons['W'] || statusControlButtons['S'])
            {
                stepFlagLeft = !stepFlagLeft;
                stepFlagRight = !stepFlagRight;
                float radians = (float)Math.PI / 180 * Rotation;
                if (statusControlButtons['W'])
                {
                    Position = new PositionData(
                        Position.X + (float)(Speed * Math.Sin(radians)),
                        Position.Y - (float)(Speed * Math.Cos(radians)));
                }
                else
                {
                    Position = new PositionData(
                        Position.X - (float)(Speed / 2 * Math.Sin(radians)),
                        Position.Y + (float)(Speed / 2 * Math.Cos(radians)));
                }
                Weapon.Rotation = (float)(Math.Atan2(Weapon.Position.Y - Mouse.GetPosition().Y, Weapon.Position.X - Mouse.GetPosition().X) * 180 / Math.PI) - 90;
                shotEffect.Rotation = Weapon.Rotation;
            }

            if (Controlled && Mouse.IsButtonPressed(Mouse.Button.Left) && pressedLeftMouseButton == false)
            {
                pressedLeftMouseButton = true;
                Fire();
            }
            else if (Controlled && !Mouse.IsButtonPressed(Mouse.Button.Left) && pressedLeftMouseButton == true)
            {
                pressedLeftMouseButton = false;
            }
            if (stepFlagLeft)
            {
                TrackA.Origin = TrackLeftOrigin;
                target.Draw(TrackA);
            }
            else
            {
                TrackB.Origin = TrackLeftOrigin;
                target.Draw(TrackB);
            }
            if (stepFlagRight)
            {
                TrackA.Origin = TrackRightOrigin;
                target.Draw(TrackA);
            }
            else
            {
                TrackB.Origin = TrackRightOrigin;
                target.Draw(TrackB);
            }
            target.Draw(Body);
            target.Draw(Weapon);
            if (drawProjectile)
            {
                target.Draw(shotEffect);
            }
            if(!Controlled)
                target.Draw(healthBar);
        }

        private void Window_MouseMoved(object? sender, MouseMoveEventArgs e)
        {
            if (!Controlled)
                return;
            Weapon.Rotation = (float)(Math.Atan2(Weapon.Position.Y - e.Y, Weapon.Position.X - e.X) * 180 / Math.PI) - 90;
            shotEffectA.Rotation = Weapon.Rotation;
            shotEffectB.Rotation = Weapon.Rotation;
        }

        private void Window_KeyPressed(object? sender, KeyEventArgs e)
        {
            if (!Controlled)
                return;
            if (e.Code == Keyboard.Key.W)
            {
                statusControlButtons['W'] = true;
            }
            else if (e.Code == Keyboard.Key.S)
            {
                statusControlButtons['S'] = true;
            }
            else if (e.Code == Keyboard.Key.A)
            {
                statusControlButtons['A'] = true;
            }
            else if (e.Code == Keyboard.Key.D)
            {
                statusControlButtons['D'] = true;
            }
        }

        private void Window_KeyReleased(object? sender, KeyEventArgs e)
        {
            if (!Controlled)
                return;
            if (e.Code == Keyboard.Key.A)
            {
                statusControlButtons['A'] = false;
            }
            else if (e.Code == Keyboard.Key.W)
            {
                statusControlButtons['W'] = false;
            }
            else if (e.Code == Keyboard.Key.S)
            {
                statusControlButtons['S'] = false;
            }
            else if (e.Code == Keyboard.Key.D)
            {
                statusControlButtons['D'] = false;
            }
        }

        private static string getEffectName(ProjectileType projectileType)
            => projectileType switch
            {
                ProjectileType.Shell => "Flash_A",
                ProjectileType.Plasma => "",
                ProjectileType.Laser => "Flash_B"
            };
    }
}
