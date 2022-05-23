using Server.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using WoTCore.Models;
using WoTCore.Helpers;
using SimplexNoise;
using WoTCore.Models.MapObjects;

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

        public Map Map { get; } = new Map(40);

        public GameModel()
        {
            commands[0] = Storage.Instance.Commands.Add(new CommandModel());
            commands[1] = Storage.Instance.Commands.Add(new CommandModel());
        }
        public GameModel(int id) : this()
        {
            Id = id;
        }
        public GameModel(int id, string name, int playerLimits) : this()
        {
            Id = id;
            Name = name;
            PlayerLimits = playerLimits;
        }

        public int AddPlayer(PlayerModel player)
        {
            int c = commands[0].Players.Count > commands[1].Players.Count ? 1 :
                    commands[0].Players.Count == commands[1].Players.Count ? 1 : 0;
            AddPlayer(player, c);
            return c;
        }

        public int AddPlayer(PlayerModel player, int commandNum)
        {
            if (commandNum > 1 || commandNum < 0)
                throw new ArgumentOutOfRangeException("Command num, was waiting for 0 or 1!");
            if (commands[0].Players.Any(p => p == player) || commands[1].Players.Any(p => p == player))
                return -1;
            player.Command = commandNum;
            {
                var pos = new Position();
                var rand = new Random();
                do
                {
                    pos.X = rand.Next(Map.Size);
                    if (commandNum == 0)
                        pos.Y = rand.Next(Map.Size / 2 - 5);
                    else
                        pos.Y = rand.Next(Map.Size / 2 - 5, Map.Size);
                    var a = Map[pos];
                } while (!(Map[pos].Content == default && ((Map[pos].Background is IBlock) && (Map[pos].Background as IBlock).IsSpawnArea)));
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
                    pos.X = rand.Next(Map.Size);
                    pos.Y = rand.Next(Map.Size);
                } while (!(Map[pos].Content == default && (Map[pos].Background is IBlock) && (Map[pos].Background as IBlock).IsSpawnArea));
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

        public void Dispose()
        {
            Storage.Instance.Commands.Remove(commands[0]);
            Storage.Instance.Commands.Remove(commands[1]);
        }
    }
}
