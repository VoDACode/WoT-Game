using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoTCore.Models
{
    [Serializable]
    public class Position : NotifyPropertyChangedContext
    {
        private short x, y;
        public short X
        {
            get => x;
            set
            {
                NotifyPropertyChanged();
                x = value;
            }
        }
        public short Y
        {
            get => y;
            set
            {
                NotifyPropertyChanged();
                y = value;
            }
        }
        public Position()
        {
            X = 0;
            Y = 0;
        }
        public Position(short x, short y)
        {
            X = x;
            Y = y;
        }

        public Position Copy()
        {
            return new Position(X, Y);
        }

        public bool Normalize(short xLimit, short yLimit)
        {
            if (X <= 0)
            {
                X = 0;
                return true;
            }
            else if (Y <= 0)
            {
                Y = 0;
                return true;
            }
            else if (X >= xLimit)
            {
                X = (short)(xLimit - 1);
                return true;
            }
            else if (Y >= yLimit)
            {
                Y = (short)(yLimit - 1);
                return true;
            }
            return false;
        }
        public Position GetNormalize(short xLimit, short yLimit)
        {
            var pos = new Position(X, Y);
            pos.Normalize(xLimit, yLimit);
            return pos;
        }
        public bool Equals(Position obj)
        {
            return this.X == obj.X && this.Y == obj.Y;
        }

        public Position AddX(short x)
            => new Position((short)(X + x), Y);
        public Position AddY(short y)
            => new Position(X, (short)(Y + y));
        public static Position Parse(short x, short y)
            => new Position(x, y);
        public static Position operator +(Position a, Position b)
            => new Position((short)(a.X + b.X), (short)(a.Y + b.Y));
        public static Position operator -(Position a, Position b)
            => new Position((short)(a.X - b.X), (short)(a.Y - b.Y));
        public static Position operator *(Position a, Position b)
            => new Position((short)(a.X * b.X), (short)(a.Y * b.Y));
        public static Position operator /(Position a, Position b)
        {
            if (b.X == 0 || b.Y == 0)
                throw new DivideByZeroException();
            return new Position((short)(a.X / b.X), (short)(a.Y / b.Y));
        }
        public static Position operator +(Position a, short b)
            => new Position((short)(a.X + b), (short)(a.Y + b));
        public static Position operator -(Position a, short b)
            => new Position((short)(a.X - b), (short)(a.Y - b));
        public static Position operator *(Position a, short b)
            => new Position((short)(a.X * b), (short)(a.Y * b));
        public static short operator %(Position a, short b)
            => (short)(a.X % b + a.Y % b);
        public static Position operator /(Position a, short b)
        {
            if (b == 0)
                throw new DivideByZeroException();
            return new Position((short)(a.X / b), (short)(a.Y / b));
        }

        public static bool operator ==(Position a, Position b)
        {
            if (a is null && b is null)
                return true;
            if((!(a is null) && b is null) || (a is null && !(b is null)))
                return false;
            return a.X == b.X && a.Y == b.Y;
        }
        public static bool operator !=(Position a, Position b)
            => !(a == b);

        public static bool operator >(Position a, Position b)
        {
            if (a is null)
                return false;
            if (b is null)
                return true;
            return a.X > b.X && a.Y > b.Y;
        }
        public static bool operator <(Position a, Position b)
        {
            if (a is null)
                return false;
            if (b is null)
                return true;
            return a.X < b.X && a.Y < b.Y;
        }
        public static bool operator >=(Position a, Position b)
            => a == b || a > b;
        public static bool operator <=(Position a, Position b)
            => a == b || a < b;
    }
}
