using WoTCore.Models;
using System;
using System.IO;

namespace Server.Managers
{
    public static class ChunkManager
    {
        public static string DataPath => Path.Combine(AppContext.BaseDirectory, "data");
        public static string GamesPath => Path.Combine(DataPath, "games");
        public static string GameRootPath(int gameId) => Path.Combine(GamesPath, gameId.ToString());
        public static string ChunkStorage(int gameId) => Path.Combine(GameRootPath(gameId), "chunks");

        static ChunkManager()
        {
            if(!Directory.Exists(DataPath))
                Directory.CreateDirectory(DataPath);
            if (!Directory.Exists(GamesPath))
                Directory.CreateDirectory(GamesPath);
        }

        public static void Save(int gameId, MapCell[,] chunk, Position position)
        {
            tryCreateGamePaths(gameId);
            using (var fs = new FileStream(Path.Combine(ChunkStorage(gameId), $"{position.X}.{position.Y}.bin"), FileMode.CreateNew))
            {
                var bytes = ConvertTypes.ToBytes(chunk);
                fs.Write(bytes);
            }
        }

        public static MapCell[,] Load(int gameId, Position position)
        {
            if (!File.Exists(Path.Combine(ChunkStorage(gameId), $"{position.X}.{position.Y}.bin")))
                throw new IOException("File not found!");
            using (var fs = new FileStream(Path.Combine(ChunkStorage(gameId), $"{position.X}.{position.Y}.bin"), FileMode.Open))
            {
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                return ConvertTypes.ToObject<MapCell[,]>(buffer);
            }
        }

        private static void tryCreateGamePaths(int gameId)
        {
            if (!Directory.Exists(GameRootPath(gameId)))
                Directory.CreateDirectory(GameRootPath(gameId));
            if (!Directory.Exists(ChunkStorage(gameId)))
                Directory.CreateDirectory(ChunkStorage(gameId));
        }
    }
}
