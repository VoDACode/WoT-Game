using SFML.Graphics;
using SFML.System;
using SFML.Window;
using WoTSFMLClient.Models;

namespace WoTSFMLClient.Items
{
    public delegate void SelectedProjectileDelegate(Projectile projectile);
    public class ListOfShells : BaseItem
    {
        private const int X_SIZE = 96;
        private const int Y_SIZE = 96;

        private RectangleShape mainBox;
        private RectangleShape itemBox;
        private RectangleShape selectedFrame;
        private RectangleShape processRecharge;
        private IReadOnlyList<Projectile> projectiles;
        private List<Sprite> projectileSprites = new List<Sprite>();

        private int pointer = 0;

        public event SelectedProjectileDelegate OnSelectedProjectile;

        public ListOfShells(Window window, IReadOnlyList<Projectile> projectiles) : base(window)
        {
            this.projectiles = projectiles;
            for (int i = 0; i < projectiles.Count; i++)
            {
                projectileSprites.Add(new Sprite(new Texture(projectiles[i].TextureFile)));
            }

            itemBox = new RectangleShape();
            mainBox = new RectangleShape();
            selectedFrame = new RectangleShape();
            processRecharge = new RectangleShape();

            processRecharge.Size = new Vector2f(X_SIZE - 10, -(Y_SIZE - 10) * Math.Min(1,(LocalStorage.Tank.RechargePercentage / 100)));
            processRecharge.FillColor = new Color(30, 30, 30, 64);

            itemBox.Size = new Vector2f(X_SIZE, Y_SIZE);
            itemBox.FillColor = new Color(50, 50, 50, 128);
            selectedFrame.Size = itemBox.Size;
            selectedFrame.FillColor = new Color(40, 40, 40, 128);
            mainBox.Size = new Vector2f(X_SIZE * projectiles.Count + 10, Y_SIZE + 10);
            mainBox.FillColor = new Color(65, 65, 65, 128);
            HorizontalAlign = HorizontalAlignType.Center;
            VerticalAlign = VerticalAlignType.Bottom;

            window.KeyPressed += Window_KeyPressed;
        }

        private void Window_KeyPressed(object? sender, KeyEventArgs e)
        {
            if(e.Code == Keyboard.Key.Num1)
            {
                pointer = 0;
                OnSelectedProjectile?.Invoke(projectiles[pointer]);
            }
            else if (e.Code == Keyboard.Key.Num2)
            {
                pointer = 1;
                OnSelectedProjectile?.Invoke(projectiles[pointer]);
            }
            else if (e.Code == Keyboard.Key.Num3)
            {
                pointer = 2;
                OnSelectedProjectile?.Invoke(projectiles[pointer]);
            }
            else if (e.Code == Keyboard.Key.Num4)
            {
                pointer = 3;
                OnSelectedProjectile?.Invoke(projectiles[pointer]);
            }
            else if (e.Code == Keyboard.Key.Num5)
            {
                pointer = 4;
                OnSelectedProjectile?.Invoke(projectiles[pointer]);
            }
            else if (e.Code == Keyboard.Key.Num6)
            {
                pointer = 5;
                OnSelectedProjectile?.Invoke(projectiles[pointer]);
            }
        }

        public override Vector2f Size
        {
            get => mainBox.Size;
            set => mainBox.Size = value;
        }
        public override PositionData Position
        {
            get => mainBox.Position;
            set
            {
                _position = value;
                mainBox.Position = _positionInPixel;
            }
        }

        public override void Loaded()
        {
            alignCalculate();
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(mainBox);
            for (int i = 0; i < projectiles.Count; i++)
            {
                itemBox.Position = new Vector2f(mainBox.Position.X + 5 + i * X_SIZE, mainBox.Position.Y + 5);
                projectileSprites[i].Scale = new Vector2f(0.75f, 0.75f);
                projectileSprites[i].Position = itemBox.Position;                 
                if (pointer == i)
                {
                    selectedFrame.Position = itemBox.Position;
                    itemBox.Size = new Vector2f(X_SIZE - 10, Y_SIZE - 10);
                    itemBox.Position = new Vector2f(mainBox.Position.X + 5 + i * X_SIZE + 5, mainBox.Position.Y + 5 + 5);
                    processRecharge.Size = new Vector2f(X_SIZE - 10, -(Y_SIZE - 10) * Math.Min(1, (LocalStorage.Tank.RechargePercentage / 100)));
                    processRecharge.Position = new Vector2f(mainBox.Position.X + 5 + i * X_SIZE + 5, mainBox.Position.Y + 5 + 5 + (Y_SIZE - 10));
                    target.Draw(selectedFrame);
                }
                else if(itemBox.Size.X != X_SIZE || itemBox.Size.Y != Y_SIZE)
                {
                    itemBox.Size = new Vector2f(X_SIZE, Y_SIZE);
                }
                target.Draw(itemBox);
                if(LocalStorage.Tank.RechargePercentage < 100)
                    target.Draw(processRecharge);
                target.Draw(projectileSprites[i]);
            }
        }
    }
}
