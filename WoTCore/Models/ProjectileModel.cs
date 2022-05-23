using System;

namespace WoTCore.Models
{
    [Serializable]
    public class ProjectileModel : IProjectile
    {
        private int xK = 0;
        private int yK = 0;
        public int Speed { get; set; } = 1;
        public Position Position { get; set; } = new Position();
        public char Icon { get; } = '*';
        public int Damage { get; set; } = 10;
        public int Life { get; set; } = 25;
        public PlayerModel Owner { get; }
        public TurnObject Turn { get; }
        public string UID { get; set; }
        public ConsoleColor BackgroundColor { get; set; } = ConsoleColor.White;
        public ConsoleColor ForegroundColor { get; set; } = ConsoleColor.White;

        public Position GetNextPos => new Position(Position.X + Speed * xK, Position.Y + Speed * yK);

        public ProjectileModel(PlayerModel player, Position position, int damage, int life = 25)
        {
            Damage = damage;
            Life = life;
            Position.X = position.X;
            Position.Y = position.Y;
            Turn = player.Turn;
            Owner = player;
            switch (Turn)
            {
                case TurnObject.Left:
                    xK = -1;
                    Position.X--;
                    break;
                case TurnObject.Right:
                    xK = 1;
                    Position.X++;
                    break;
                case TurnObject.Top:
                    yK = -1;
                    Position.Y--;
                    break;
                case TurnObject.Bottom:
                    yK = 1;
                    Position.Y++;
                    break;
            }
        }

        public void Tick()
        {
            Life--;        
            Position.Y += Speed*yK;
            Position.X += Speed*xK;
        }

    }
}
