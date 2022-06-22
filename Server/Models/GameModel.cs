using Server.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using WoTCore.Models;
using WoTCore.Helpers;
using WoTCore.Models.MapObjects;
using Serilog;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.IO;

namespace Server.Models
{
    delegate void LeaveEvent(GameModel game, CommandModel command, PlayerModel player);
    class GameModel : IDisposable, IIdentificator
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PlayerLimits { get; set; }
        public int PlayerCount => commands.Sum(p => p.Players.Count);
        public bool IsFull => PlayerCount >= PlayerLimits;
        public event LeaveEvent OnLeave;
        public bool IsEmpty => PlayerCount == 0;
        public event EventHandler OnEmpty;
        private CommandModel[] commands = new CommandModel[2];
        public IReadOnlyCollection<CommandModel> Commands => commands;
        public IReadOnlyList<PlayerModel> Players
        {
            get
            {
                var result = commands[0].Players.ToList();
                result.AddRange(commands[1].Players);
                return result;
            }
        }

        public List<ProjectileModel> Projectiles = new List<ProjectileModel>();

        public Map Map { get; } = new Map(13);

        public GameModel()
        {
            commands[0] = Storage.Instance.Commands.Add(new CommandModel());
            commands[1] = Storage.Instance.Commands.Add(new CommandModel());
        }
        public GameModel(int id) : this()
        {
            Id = id;
            Map.GameId = id;
        }
        public GameModel(int id, string name, int playerLimits) : this()
        {
            Id = id;
            Name = name;
            PlayerLimits = playerLimits;
            Map.GameId = id;
        }

        public short AddPlayer(PlayerModel player)
        {
            short c = (short)(commands[0].Players.Count > commands[1].Players.Count ? 1 :
                    commands[0].Players.Count == commands[1].Players.Count ? 1 : 0);
            AddPlayer(player, c);
            return c;
        }

        public int AddPlayer(PlayerModel player, short commandNum)
        {
            if (commandNum > 1 || commandNum < 0)
                throw new ArgumentOutOfRangeException("Command num, was waiting for 0 or 1!");
            if (commands[0].Players.Any(p => p == player) || commands[1].Players.Any(p => p == player))
                return -1;
            Log.Debug($"[Player - '{Id}','{Name}'] Command = '{commandNum}'");
            player.Command = commandNum;
            {
                var pos = new Position();
                var rand = new Random();
                do
                {
                    Log.Debug($"[Player - '{Id}','{Name}'] Start rand pos...");
                    pos.X = (short)rand.Next(Map.Size);
                    if (commandNum == 0)
                        pos.Y = (short)rand.Next(Map.Size / 2 - 5);
                    else
                        pos.Y = (short)rand.Next(Map.Size / 2 - 5, Map.Size);
                } while (!((((Map[pos].Background is IBlock) && (Map[pos].Background as IBlock).IsSpawnArea) || Map[pos].Background is MapCell) && Map[pos].Content is EmptyObject));
                Log.Debug($"[Player - '{Id}','{Name}'] Position = '{pos}'");
                player.Position = pos;
                string uid = "";
                do
                {
                    uid = StringHelper.Random(64);
                } while (Map.UIDs.ContainsKey(uid));
                player.UID = uid;
            }
            commands[commandNum].AddPlayer(player);
            Map[player.Position].Content = player;
            Map.UIDs.Add(player.UID, Map[player.Position]);
            return commandNum;
        }

        public void Restart()
        {
            Map.Clear();
            Map.Generate(Storage.Instance.Modes);
            Projectiles.Clear();
            for (int i = 0; i < Players.Count; i++)
            {

                var pos = new Position();
                var rand = new Random();
                do
                {
                    pos.X = (short)rand.Next(Map.Size);
                    pos.Y = (short)rand.Next(Map.Size);
                } while (!((((Map[pos].Background is IBlock) && (Map[pos].Background as IBlock).IsSpawnArea) || Map[pos].Background is MapCell) && Map[pos].Content is EmptyObject));
                Players[i].Position = pos;
                Map[pos].Content = Players[i];
                Players[i].Life = Players[i].MaxLife;
            }
        }

        public IReadOnlyList<PlayerModel> GetPlayersInZone(int y, int x, int h, int w) =>
            Players.Where(p => p.Position.X >= x && p.Position.X <= w && p.Position.Y >= y && p.Position.Y <= h).ToList();

        public IReadOnlyList<ProjectileModel> GetProjectilesInZone(int y, int x, int h, int w) =>
           Projectiles.Where(p => p.Position.X >= x && p.Position.X <= w && p.Position.Y >= y && p.Position.Y <= h).ToList();

        public bool LeavePlayer(PlayerModel player)
        {
            CommandModel command;
            if (commands[0].Players.Any(p => p == player))
                command = commands[0];
            else if (commands[1].Players.Any(p => p == player))
                command = commands[1];
            else
                return false;
            if (command.Leave(player))
                OnLeave?.Invoke(this, command, player);
            if (IsEmpty)
                OnEmpty?.Invoke(this, null);
            Map[player.Position].Content = default;
            return true;
        }

        public bool ExistPlayer(PlayerModel player)
        {
            return commands[0].Players.Any(p => p == player) || commands[1].Players.Any(p => p == player);
        }

        public int CheckLose()
        {
            var commands = Commands.ToArray();
            for (int i = 0; i < commands.Length; i++)
                if (commands[i].Players.All(p => p.Killed))
                    return i;
            return -1;
        }

        public void SandRegionMap(IClientProxy client, Position playerPosition, byte renderDistance)
        {

            Position leftTop;
            var rendeRegion = Map.GetRenderChunks2D(playerPosition, renderDistance, out leftTop);
            for (short ax = (short)(leftTop.X / ServerMap.ChunkSize), ix = 0; ix < rendeRegion.GetLength(0); ax++, ix++)
                for (short ay = (short)(leftTop.Y / ServerMap.ChunkSize), iy = 0; iy < rendeRegion.GetLength(1); ay++, iy++)
                {
                    var item = rendeRegion[ix, iy];
                    try
                    {
                        var b = ConvertTypes.ToBytes(item);
                        var obj = ConvertTypes.ToObject<MapCell[,]>(b);
                        client.SendAsync("GetMap", b, new Position(ax, ay), new Position(ix, iy),
                           leftTop,
                           (ix + 1 == rendeRegion.GetLength(0) && iy + 1 == rendeRegion.GetLength(1)),
                           (ix == 0 && iy == 0));
                    }
                    catch (Exception ex)
                    {
                        File.WriteAllText($"{AppContext.BaseDirectory}crash_{DateTime.Now.Ticks}.log",
                            $"{ex}\n\n DATA[{ix}, {iy}]:\n{JsonConvert.SerializeObject(rendeRegion[ix, iy])}");
                        throw new Exception(ex.Message);
                    }
                }
            GC.Collect();

        }

        public void Dispose()
        {
            Storage.Instance.Commands.Remove(commands[0]);
            Storage.Instance.Commands.Remove(commands[1]);
        }
    }
}
