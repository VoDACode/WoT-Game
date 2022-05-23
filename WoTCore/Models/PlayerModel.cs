﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoTCore.Helpers;
using WoTCore.Models.MapObjects;

namespace WoTCore.Models
{
    [Serializable()]
    public enum TurnObject
    {
        Left, Right, Top, Bottom
    }
    [Serializable()]
    public class PlayerModel: IPlayer
    {
        private char[] icons = new char[] { '<', '>', '^', '˅' };

        private DateTime _lastStep = DateTime.Now;
        private DateTime _lastShotTime;
        private string _name;
        private string _session;
        private string _uid;
        private int _maxLife = 50;
        private int _life = 50;
        private int _command = -1;
        private TurnObject _turn = TurnObject.Top;
        private Position _position;

        public DateTime LastStep
        {
            get => _lastStep;
            set
            {
                
                _lastStep = value;
            }
        }
        public int MaxLife
        {
            get => _maxLife;
            set
            {
                
                _maxLife = value;
            }
        }
        public int Life
        {
            get => _life;
            set
            {
                
                _life = value;
            }
        }
        public string Session
        {
            get => _session;
            set
            {
                
                _session = value;
            }
        }
        public int Command
        {
            get => _command;
            set
            {
                
                _command = value;
            }
        }
        public Position Position
        {
            get => _position;
            set
            {
                
                _position = value;
            }
        }
        public string Name
        {
            get => _name;
            set
            {
                
                _name = value;
            }
        }
        public DateTime LastShotTime
        {
            get => _lastShotTime;
            set
            {
                
                _lastShotTime = value;
            }
        }
        public TurnObject Turn
        {
            get => _turn;
            set
            {
                
                _turn = value;
            }
        }
        public string UID
        {
            get => _uid;
            set
            {
                _uid = value;
            }
        }

        public char Icon => icons[(int)Turn];
        public bool Killed => Life <= 0;

        public PlayerModel(Position position, string name)
        {
            Session = StringHelper.Random(32);
            Position = position;
            Name = name;
        }
        public PlayerModel(string name) : base()
        {
            Name = name;
        }
        public PlayerModel()
        {
            Session = StringHelper.Random(32);
            Position = new Position() { X = 0, Y = 0 };
        }

        public bool TryGoTo(TurnObject turn, int size, Map map)
        {
            map[Position].Content = default;
            switch (turn)
            {
                case TurnObject.Left:
                    if (Position.X - 1 < 0 || map.ExistContent(Position.X - 1, Position.Y))
                        return false;
                    Position.X--;
                    Position.Y = Position.Y;
                    break;
                case TurnObject.Right:
                    if (Position.X + 1 >= size || map.ExistContent(Position.X + 1, Position.Y))
                        return false;
                    Position.X++;
                    Position.Y = Position.Y;
                    break;
                case TurnObject.Top:
                    if (Position.Y - 1 < 0 || map.ExistContent(Position.X, Position.Y - 1))
                        return false;
                    Position.Y--;
                    Position.X = Position.X;
                    break;
                case TurnObject.Bottom:
                    if (Position.Y + 1 >= size || map.ExistContent(Position.X, Position.Y + 1))
                        return false;
                    Position.Y++;
                    Position.X = Position.X;
                    break;
            }
            map[Position].Content = this;
            Turn = turn;
            return true;
        }
    }
}
