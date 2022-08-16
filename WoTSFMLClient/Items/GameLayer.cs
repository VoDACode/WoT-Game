using SFML.Graphics;
using SFML.System;
using SFML.Window;
using WoTSFMLClient.Items.Projectiles;
using WoTSFMLClient.Models;

namespace WoTSFMLClient.Items
{
    public class GameLayer : BaseItem
    {
        ListOfShells listOfShells;
        ListOfPlayers ListOfPlayersLeft;
        ListOfPlayers ListOfPlayersRight;
        HealthBar healthBar;
        private int _zoom = 10;
        private float Zoom => _zoom / 10f;
        private bool pressingShift = false;
        List<BaseItem> items = new List<BaseItem>();
        List<BaseItem> ui = new List<BaseItem>();
        Panel bg;
        public GameLayer(Window window) : base(window)
        {
            bg = new Panel(window, new Color(104, 107, 105));

            var tank = new Tank(App.Window, 0, 1, 5, new List<Projectile>
            {
                new Laser(App.Window),
                new HeavyShell(App.Window),
                new LightShell(App.Window),
                new GranadeShell(App.Window),
                new MediumShell(App.Window),
                new Plasma(App.Window)
            });
            tank.Controlled = true;
            tank.HorizontalAlign = HorizontalAlignType.Center;

            LocalStorage.Tank = tank;

            listOfShells = new ListOfShells(window, new List<Projectile>
            {
                new Laser(App.Window),
                new HeavyShell(App.Window),
                new LightShell(App.Window),
                new GranadeShell(App.Window),
                new MediumShell(App.Window),
                new Plasma(App.Window)
            });
            listOfShells.OnSelectedProjectile += (Projectile projectile) =>
            {
                tank.SelectedProjectile = projectile;
            };

            ListOfPlayersLeft = new ListOfPlayers(window);
            ListOfPlayersLeft.HorizontalAlign = HorizontalAlignType.Left;

            ListOfPlayersRight = new ListOfPlayers(window);
            ListOfPlayersRight.HorizontalAlign = HorizontalAlignType.Right;

            healthBar = new HealthBar(window);
            healthBar.HorizontalAlign = HorizontalAlignType.Center;
            healthBar.VerticalAlign = VerticalAlignType.Top;
            healthBar.Size = new Vector2f(window.Size.X / 3, healthBar.Size.Y);
            healthBar.Health = 20;
            healthBar.Margin = new FloatRect(0, 10, 0, 0);

            tank.VerticalAlign = VerticalAlignType.Center;
            items.Add(tank);
            ui.Add(listOfShells);
            ui.Add(ListOfPlayersLeft);
            ui.Add(ListOfPlayersRight);
            ui.Add(healthBar);
        }

        private void Window_MouseWheelScrolled(object? sender, MouseWheelScrollEventArgs e)
        {
            if (pressingShift)
            {
                var val = _zoom + (e.Delta > 0 ? 1 : -1);
                if (val >= 10 && val <= 20)
                {
                    _zoom = val;
                    foreach (BaseItem item in items)
                    {
                        //item.Size = new Vector2f(Zoom * item.FixedSize.X, Zoom * item.FixedSize.Y);
                    }
                }
            }
        }

        private void Window_KeyPressed(object? sender, KeyEventArgs e)
        {
            pressingShift = e.Shift;
        }

        public override Vector2f Size
        {
            get => new Vector2f(App.Window.Size.X, App.Window.Size.Y);
            set => throw new NotImplementedException();
        }
        public override PositionData Position
        {
            get => new PositionData();
            set => throw new NotImplementedException();
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(bg);
            foreach (BaseItem item in items)
                target.Draw(item);
            foreach (Drawable drawable in LocalStorage.Projectiles)
                target.Draw(drawable);
            foreach (BaseItem item in ui)
                target.Draw(item);
        }

        private float normalizeVal(float val, float max, float min = 0)
            => val > max ? max : val <= min ? min : val;

        public override void Loaded()
        {
            foreach (BaseItem item in items)
                item.Loaded();
            foreach (BaseItem item in ui)
                item.Loaded();
            base.Loaded();
        }
    }
}
