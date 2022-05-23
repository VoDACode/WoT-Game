using System;
using System.Collections.Generic;
using SimplexNoise;
using WoTCore.Helpers;

namespace WoTCore.Models
{
    [Serializable]
    public sealed class Map
    {
        private MapCell[,] map { get; }
        public Dictionary<string, MapCell> UIDs { get; } = new Dictionary<string, MapCell>();
        public int Size { get; }

        public MapCell this[int x, int y]
        {
            get => map[x, y];
            set => map[x, y] = value;
        }

        public MapCell this[Position pos]
        {
            get => map[pos.X, pos.Y];
            set => map[pos.X, pos.Y] = value;
        }

        public Dictionary<Position, MapCell> GetList
        {
            get
            {
                Dictionary<Position, MapCell> list = new Dictionary<Position, MapCell>();
                for(int x = 0; x < Size; x++)
                    for(int y = 0; y < Size; y++)
                        list.Add(new Position(x, y), this[x, y]);
                return list;
            }
        }

        public Map(int size)
        {
            map = new MapCell[size, size];
            Size = size;
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    map[x, y] = new MapCell();
                }
            }
        }

        public bool ExistContent(int x, int y)
        {
            return (x >= 0 && y >= 0 && x < Size && y < Size && this[x, y].Content != default);
        }

        public bool ExistContentSquared(Position pos)
        {
            if (pos.X >= map.GetLength(0) || pos.Y >= map.GetLength(1) || pos.Y < 0 || pos.X < 0)
                return false;
            /* xy
             * --   0-  +-
             * -0   00  +0
             * -+   0+  ++
             */
            #region if
            if (pos.X - 1 >= 0 && pos.Y - 1 >= 0 && this[pos.X - 1, pos.Y - 1].Content != default)//--
                return true;
            else if (pos.X >= 0 && pos.Y - 1 >= 0 && this[pos.X, pos.Y - 1].Content != default)//0-
                return true;
            else if (pos.X + 1 >= 0 && pos.Y - 1 >= 0 && this[pos.X + 1, pos.Y - 1].Content != default)//+-
                return true;
            else if (pos.X - 1 >= 0 && pos.Y + 1 >= 0 && this[pos.X - 1, pos.Y + 1].Content != default)//-+
                return true;
            else if (pos.X - 1 >= 0 && pos.Y >= 0 && this[pos.X - 1, pos.Y].Content != default)//-0
                return true;
            else if (pos.X >= 0 && pos.Y + 1 >= 0 && this[pos.X, pos.Y + 1].Content != default)//0+
                return true;
            else if (pos.X + 1 >= 0 && pos.Y >= 0 && this[pos.X + 1, pos.Y].Content != default)//+0
                return true;
            else if (pos.X + 1 >= 0 && pos.Y + 1 >= 0 && this[pos.X + 1, pos.Y + 1].Content != default)//++
                return true;
            #endregion
            return false;
        }

        public Dictionary<Position, object> GetContentSquared(Position pos)
        {
            Dictionary<Position, object> list = new Dictionary<Position, object>();
            if (pos.X - 1 >= 0 && pos.Y - 1 >= 0 && this[pos.X - 1, pos.Y - 1].Content != default)//--
                list.Add(new Position(pos.X - 1, pos.Y - 1), this[pos.X - 1, pos.Y - 1].Content);
            if (pos.X >= 0 && pos.Y - 1 >= 0 && this[pos.X, pos.Y - 1].Content != default)//0-
                list.Add(new Position(pos.X, pos.Y - 1), this[pos.X, pos.Y - 1].Content);
            if (pos.X + 1 >= 0 && pos.Y - 1 >= 0 && this[pos.X + 1, pos.Y - 1].Content != default)//+-
                list.Add(new Position(pos.X + 1, pos.Y - 1), this[pos.X + 1, pos.Y - 1].Content);
            if (pos.X - 1 >= 0 && pos.Y + 1 >= 0 && this[pos.X - 1, pos.Y + 1].Content != default)//-+
                list.Add(new Position(pos.X - 1, pos.Y + 1), this[pos.X - 1, pos.Y + 1].Content);
            if (pos.X - 1 >= 0 && pos.Y >= 0 && this[pos.X - 1, pos.Y].Content != default)//-0
                list.Add(new Position(pos.X - 1, pos.Y), this[pos.X - 1, pos.Y].Content);
            if (pos.X >= 0 && pos.Y + 1 >= 0 && this[pos.X, pos.Y + 1].Content != default)//0+
                list.Add(new Position(pos.X, pos.Y + 1), this[pos.X, pos.Y + 1].Content);
            if (pos.X + 1 >= 0 && pos.Y >= 0 && this[pos.X + 1, pos.Y].Content != default)//+0
                list.Add(new Position(pos.X + 1, pos.Y), this[pos.X + 1, pos.Y].Content);
            if (pos.X + 1 >= 0 && pos.Y + 1 >= 0 && this[pos.X + 1, pos.Y + 1].Content != default)//++
                list.Add(new Position(pos.X + 1, pos.Y + 1), this[pos.X + 1, pos.Y + 1].Content);
            return list;
        }

        public void Clear()
        {
            UIDs.Clear();
            for (int x = 0; x < Size; x++)
                for (int y = 0; y < Size; y++)
                    this[x, y].Default();
        }

        public void Generate(List<ModeContent> modes)
        {
            if(modes != null && modes.Count > 0)
                generateModeMap(modes);
            else
                generateDefaultMap();
        }
        private void generateDefaultMap()
        {
            for (int x = 0; x < Size; x++)
            {
                for (int y = 0; y < Size; y++)
                {
                    map[x, y].Background = new CellModel()
                    {
                        BackgroundColor = ConsoleColor.Black,
                        ForegroundColor = ConsoleColor.White,
                        Icon = ' '
                    };
                }
            }
        }
        private void generateModeMap(List<ModeContent> modes)
        {
            Noise.Seed = new Random().Next(int.MaxValue);
            float[,] noiseMap = Noise.Calc2D(Size, Size, 0.05f);
            for (int x = 0; x < Size; x++)
            {
                for (int y = 0; y < Size; y++)
                {
                    foreach (var mode in modes)
                    {
                        bool isAdded = false;
                        foreach (var block in mode.Blocks)
                        {
                            if (block.Generate(noiseMap[x, y]))
                            {
                                var obj = block.Copy();
                                obj.Position = new Position(x, y);
                                string uid = "";
                                do
                                {
                                    uid = StringHelper.Random(64);
                                } while (UIDs.ContainsKey(uid));
                                obj.UID = uid;
                                if (block.IsInteractive)
                                {
                                    this[x, y].Content = obj;
                                    UIDs.Add(uid, this[x, y]);
                                }
                                else
                                    this[x, y].Background = obj;
                                isAdded = true;
                                break;
                            }
                        }
                        if (isAdded)
                            break;
                    }
                }
            }
        }
    }
}
