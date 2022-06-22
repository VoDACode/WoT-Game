using System;
using System.Collections.Generic;
using System.Drawing;
using SimplexNoise;
using WoTCore.Helpers;

namespace WoTCore.Models
{
    [Serializable]
    public class Map
    {
        protected virtual MapCell[,] map { get; }
        public Dictionary<string, MapCell> UIDs { get; } = new Dictionary<string, MapCell>();
        public short Size { get; }
        public const byte ChunkSize = 16;
        public int GameId { get; set; }

        public Action Generated;

        public virtual MapCell this[short x, short y]
        {
            get => map[x, y];
            set => map[x, y] = value;
        }

        public MapCell this[Position pos]
        {
            get => this[pos.X, pos.Y];
            set => this[pos.X, pos.Y] = value;
        }

        public Map(short inСhunks)
        {
            map = new MapCell[inСhunks * ChunkSize, inСhunks * ChunkSize];
            Size = (short)(inСhunks * ChunkSize);
            for (short x = 0; x < map.GetLength(0); x++)
            {
                for (short y = 0; y < map.GetLength(1); y++)
                {
                    map[x, y] = new MapCell();
                }
            }
        }

        public bool TryGetValue(Position pos, out MapCell val)
        {
            if (pos.X < 0 || pos.X >= Size || pos.Y < 0 || pos.Y >= Size)
            {
                val = null;
                return false;
            }
            val = this[pos];
            return true;
        }

        public bool ExistContent(short x, short y)
        {
            if (x < 0 && y < 0 && x >= Size && y >= Size)
                return false;
            var data = this[x, y].Content;
            if (data is EmptyObject)
                return false;
            if (data is null)
                return false;
            return true;
        }

        public MapCell[,] GetRegion2D(short x, short y, short w, short h)
        {
            if (x + w - 1 >= Size || y + h - 1 >= Size)
                throw new IndexOutOfRangeException();
            var list = new MapCell[w, h];
            for (short ix = x; ix < x + w; ix++)
                for (short iy = y; iy < y + h; iy++)
                {
                    list[ix - x, iy - y] = this[ix, iy];
                }
            return list;
        }

        public MapCell[,] GetRenderRegion2D(Position playerPosition, byte renderDistance, out Position leftTop)
            => GetRenderRegion2D(playerPosition.X, playerPosition.Y, renderDistance, out leftTop);

        public MapCell[,] GetRenderRegion2D(short playerX, short playerY, byte renderDistance, out Position leftTop)
        {
            short renderToPixels() => (short)(renderDistance * ChunkSize);
            leftTop = new Position(x: (short)(playerX - renderToPixels()), 
                                   y: (short)(playerY - renderToPixels()));
            Position rightBottom = new Position(x: (short)(playerX + renderToPixels()),
                                                y: (short)(playerY + renderToPixels()));

            MapCell[,] cells = new MapCell[renderToPixels() * 2, renderToPixels() * 2];

            for (short ix = leftTop.X, x = 0; ix < rightBottom.X; ix++, x++)
                for (short iy = leftTop.Y, y = 0; iy < rightBottom.Y; iy++, y++)
                {
                    if(ix < 0 || iy < 0 || ix >= Size || iy >= Size)
                        cells[x, y] = new MapCell() { Background = EmptyObject.Empty };
                    else
                        cells[x, y] = this[ix, iy];
                }
            return cells;
        }

        public MapCell[,][,] GetRenderChunks2D(Position playerPosition, byte renderDistance, out Position leftTop)
            => GetRenderChunks2D(playerPosition.X, playerPosition.Y, renderDistance, out leftTop);
        public MapCell[,][,] GetRenderChunks2D(short playerX, short playerY, byte renderDistance, out Position leftTop)
        {
            var region = GetRenderRegion2D(playerX, playerY, renderDistance, out leftTop);
            MapCell[,][,] chunks = new MapCell[region.GetLength(0) / Map.ChunkSize, region.GetLength(1) / Map.ChunkSize][,];
            for (int cx = 0; cx < chunks.GetLength(0); cx++)
                for (int cy = 0; cy < chunks.GetLength(1); cy++)
                {
                    chunks[cx, cy] = new MapCell[Map.ChunkSize, Map.ChunkSize];
                    for (int x = 0; x < Map.ChunkSize; x++)
                        for (int y = 0; y < Map.ChunkSize; y++)
                            chunks[cx, cy][x, y] = region[cx * Map.ChunkSize + x, cy * Map.ChunkSize + y];
                }
            return chunks;
        }

        public void Clear()
        {
            UIDs.Clear();
            for (short x = 0; x < Size; x++)
                for (short y = 0; y < Size; y++)
                    this[x, y].Default();
        }

        public void Generate(List<ModeContent> modes)
        {
            if (modes != null && modes.Count > 0)
                generateModeMap(modes);
            else
                generateDefaultMap();
            Generated?.Invoke();
        }
        private void generateDefaultMap()
        {
            for (short x = 0; x < Size; x++)
            {
                for (short y = 0; y < Size; y++)
                {
                    this[x, y].Background = new CellModel()
                    {
                        BackgroundColor = Color.Black,
                        ForegroundColor = Color.White,
                        Icon = ' '
                    };
                }
            }
        }
        private void generateModeMap(List<ModeContent> modes)
        {
            Noise.Seed = new Random().Next(int.MaxValue);
            //float[,] noiseMap = Noise.Calc2D(Size, Size, 0.0235f);
            float[,] noiseMap = Noise.Calc2D(Size, Size, 0.005f);
            for (short x = 0; x < Size; x++)
            {
                for (short y = 0; y < Size; y++)
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
            noiseMap = null;
            GC.Collect();
        }
    }
}
