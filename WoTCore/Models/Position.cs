using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoTCore.Models
{
    [Serializable()]
    public class Position : NotifyPropertyChangedContext
    {
        private int x, y;
        public int X
        {
            get => x;
            set
            {
                NotifyPropertyChanged();
                x = value;
            }
        }
        public int Y
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
        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Position Copy()
        {
            return new Position(X, Y);
        }

        public bool Normalize(int xLimit, int yLimit)
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
                X = xLimit - 1;
                return true;
            }
            else if (Y >= yLimit)
            {
                Y = yLimit - 1;
                return true;
            }
            return false;
        }
        public bool Equals(Position obj)
        {
            return this.X == obj.X && this.Y == obj.Y;
        }
    }
}
