using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoTCore.Helpers;
using WoTCore.Models.MapObjects;
using WoTCore.Modes;

namespace WoTCore.Models
{
    [Serializable]
    public enum TurnObject
    {
        Left, Right, Top, Bottom
    }
    [Serializable]
    public class PlayerModel : IPlayer
    {
        private char[] icons = new char[] { '<', '>', '^', '˅' };

        private DateTime _lastStep = DateTime.Now;
        private DateTime _lastShotTime;
        private string _name = "";
        private string _session = "";
        private string _uid = "";
        private short _maxLife = 100;
        private short _life = 100;
        private short _command = -1;
        private TurnObject _turn = TurnObject.Top;
        private Position _position;
        private byte _renderDistance;

        public byte RenderDistance
        {
            get => _renderDistance;
            set => _renderDistance = value;
        }
        public DateTime LastStep
        {
            get => _lastStep;
            set
            {

                _lastStep = value;
            }
        }
        public short MaxLife
        {
            get => _maxLife;
            set
            {

                _maxLife = value;
            }
        }
        public short Life
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
        public short Command
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
        [field: NonSerialized]
        public Position PositionInChunks
        {
            get
            {
                return new Position((short)(Position.X / Map.ChunkSize + (Position.X % Map.ChunkSize > 0 ? 1 : 0)),
                                    (short)(Position.Y / Map.ChunkSize + (Position.Y % Map.ChunkSize > 0 ? 1 : 0)));
            }
        }
        [field: NonSerialized]
        public Position TopLeftCorner
                    => new Position((short)(Position.X - RenderDistance * Map.ChunkSize),
                                    (short)(Position.Y - RenderDistance * Map.ChunkSize));
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

        public bool TryGoTo(TurnObject turn, Map map)
        {
            switch (turn)
            {
                case TurnObject.Left:
                    map[Position].Content = EmptyObject.Empty;
                    if (Position.X - 1 < 0)
                        return false;
                    if (map.ExistContent((short)(Position.X - 1), Position.Y))
                    {
                        if (map[(short)(Position.X - 1), Position.Y].Content is BaseBlock
                            && !(map[(short)(Position.X - 1), Position.Y].Content as BaseBlock).OnTouch(this, map[(short)(Position.X - 1), Position.Y]))
                            return false;
                        
                    }
                    Position.X--;
                    Position.Y = Position.Y;
                    break;
                case TurnObject.Right:
                    map[Position].Content = EmptyObject.Empty;
                    if (Position.X + 1 >= map.Size)
                        return false;
                    if (map.ExistContent((short)(Position.X + 1), Position.Y))
                    {
                        if (map[(short)(Position.X + 1), Position.Y].Content is BaseBlock
                            && !(map[(short)(Position.X + 1), Position.Y].Content as BaseBlock).OnTouch(this, map[(short)(Position.X + 1), Position.Y]))
                            return false;
                    }
                    Position.X++;
                    Position.Y = Position.Y;
                    break;
                case TurnObject.Top:
                    map[Position].Content = EmptyObject.Empty;
                    if (Position.Y - 1 < 0)
                        return false;
                    if (map.ExistContent(Position.X, (short)(Position.Y - 1)))
                    {
                        if (map[Position.X, (short)(Position.Y - 1)].Content is BaseBlock
                            && !(map[Position.X, (short)(Position.Y - 1)].Content as BaseBlock).OnTouch(this, map[Position.X, (short)(Position.Y - 1)]))
                            return false;
                    }
                    Position.Y--;
                    Position.X = Position.X;
                    break;
                case TurnObject.Bottom:
                    map[Position].Content = EmptyObject.Empty;
                    if (Position.Y + 1 >= map.Size)
                        return false;
                    if (map.ExistContent(Position.X, (short)(Position.Y + 1)))
                    {
                        if (map[Position.X, (short)(Position.Y + 1)].Content is BaseBlock
                            && !(map[Position.X, (short)(Position.Y + 1)].Content as BaseBlock).OnTouch(this, map[Position.X, (short)(Position.Y + 1)]))
                            return false;
                    }
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
