using System;
using System.Drawing;

namespace WoTCore.Models
{
    [Serializable]
    public class ProjectileModel : IProjectile
    {
        private short xK = 0;
        private short yK = 0;
        private char _icon = '*';
        private TurnObject _turn;
        public short Speed { get; set; } = 1;
        public Position Position { get; set; } = new Position();
        public char Icon => _icon;
        public short Damage { get; set; } = 10;
        public short Life { get; set; } = 25;
        public PlayerModel Owner { get; }
        public TurnObject Turn => _turn;
        public string UID { get; set; }
        public Color BackgroundColor { get; set; } = Color.White;
        public Color ForegroundColor { get; set; } = Color.White;

        public Position GetNextPos => new Position((short)(Position.X + Speed * xK), (short)(Position.Y + Speed * yK));

        public ProjectileModel(PlayerModel player, Position position, short damage, short life = 25)
        {
            Damage = damage;
            Life = life;
            Position.X = position.X;
            Position.Y = position.Y;
            _turn = player.Turn;
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
            Position.Y += (short)(Speed * yK);
            Position.X += (short)(Speed * xK);
        }
    }
}
