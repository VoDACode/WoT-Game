using WoTCore.Models;
using Server.Managers;
using System;

namespace Server.Models
{
    public class ServerMap : Map
    {
        public override MapCell this[short x, short y]
        {
            get
            {
                if (base[x, y] == null)
                    Load(x, y);
                return base[x, y];
            }
            set => base[x, y] = value;
        }

        public ServerMap(short inСhunks) : base(inСhunks)
        {
            Generated += afterGenerat;
        }

        private void afterGenerat()
        {
            for (short x = 0; x < Size / ChunkSize; x++)
                for (short y = 0; y < Size / ChunkSize; y++)
                {
                    var sx = (short)(x * ChunkSize);
                    var sy = (short)(y * ChunkSize);
                    var chunk = GetRegion2D(sx, sy, ChunkSize, ChunkSize);
                    ChunkManager.Save(GameId, chunk, Position.Parse(x, y));
                }
            for (short x = 0; x < Size; x++)
                for (short y = 0; y < Size; y++)
                    this[x, y] = null;
            GC.Collect();
        }

        private void Load(int x, int y)
        {
            var cx = (short)(x / ChunkSize + (x % ChunkSize == 0 ? 0 : 1) - 1);
            var cy = (short)(y / ChunkSize + (y % ChunkSize == 0 ? 0 : 1) - 1);
            var chunk = ChunkManager.Load(GameId, Position.Parse(cx, cy));
            for (short xp = (short)(cx * ChunkSize), ix = 0; ix < ChunkSize; xp++, ix++)
                for (short yp = (short)(cy * ChunkSize), iy = 0; iy < ChunkSize; yp++, iy++)
                {
                    this[xp, yp] = chunk[ix, iy];
                }
        }
    }
}
