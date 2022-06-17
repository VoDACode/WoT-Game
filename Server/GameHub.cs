using Microsoft.AspNetCore.SignalR;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WoTCore.Models;
using WoTCore.Models.MapObjects;
using WoTCore.Views;

namespace Server
{
    public class GameHub : Hub
    {
        private HubClient Client => Storage.Instance.HubClients.SingleOrDefault(p => p.ConnectionId == Context.ConnectionId);
        private GameModel Game => Client == null ? null : Storage.Instance.Games.SingleOrDefault(p => p.ExistPlayer(Client.Player));
        public override Task OnConnectedAsync()
        {
            Storage.Instance.WaitUsersConnection.Add(Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (Storage.Instance.WaitUsersConnection.Any(p => p == Context.ConnectionId))
                Storage.Instance.WaitUsersConnection.Remove(Context.ConnectionId);
            if (Client != null)
            {
                if (Game != null)
                {
                    var game = Game;
                    Game.LeavePlayer(Client.Player);
                    await Clients.Group($"GAME_{game.Id}_CHAT").SendAsync("Chat", $"'{Client.Player.Name}' left the game!");
                    //Game.SandAllMap(Clients);
                }
                Storage.Instance.HubClients.Remove(Client);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task GoTo(TurnObject turn)
        {
            if (Client == null || Client.Player.Killed)
                return;
            if (DateTime.Now - Client.Player.LastStep < TimeSpan.FromMilliseconds(20))
                return;
            var lastChunk = Client.Player.PositionInChunks;
            if (Client.Player.TryGoTo(turn, Game.Map))
            {
                Client.Player.LastStep = DateTime.Now;
                if (!lastChunk.Equals(Client.Player.PositionInChunks))
                {
                    Game.SandRegionMap(Clients.Caller, Client.Player.Position, 5);
                    await updateMap(Game, new List<object> { Client.Player },
                        exceptClients: new[] { Client.ConnectionId });
                }
                else
                {
                    await updateMap(Game, new List<object> { Client.Player });
                }
            }
            return;
        }

        public Task Shot()
        {
            if (Client == null || Client.Player.Killed)
                return Task.CompletedTask;
            if (DateTime.Now - Client.Player.LastShotTime < TimeSpan.FromSeconds(1))
                return Task.CompletedTask;
            Client.Player.LastShotTime = DateTime.Now;
            var client = Client;
            var game = Game;
            var hub = Clients;
            Task.Factory.StartNew(async () =>
            {
                var projectile = new ProjectileModel(client.Player, client.Player.Position, 10);
                if (projectile.Position.Normalize(game.Map.Size, game.Map.Size))
                    return;
                game.Projectiles.Add(projectile);
                Position lastPos = new Position();
                while (projectile.Life > 0)
                {
                    lastPos = projectile.Position.Copy();
                    projectile.Tick();
                    if (game.Players.Any(p => p.Position.Equals(projectile.Position)))
                    {
                        var shotPlayer = game.Players.FirstOrDefault(p => p.Position.Equals(projectile.Position));
                        if (shotPlayer.Killed || shotPlayer.Command == client.Player.Command)
                        {
                            projectile.Life = -1;
                            break;
                        }
                        shotPlayer.Life -= projectile.Damage;
                        projectile.Life = -1;
                        if (shotPlayer.Killed)
                        {
                            await hub.Group($"GAME_{game.Id}_CHAT").SendAsync("Chat", $"'{shotPlayer.Name}' was killed '{client.Player.Name}'.");
                            var winCommand = game.CheckLose();
                            if (winCommand >= 0)
                            {
                                if (winCommand == 1)
                                {
                                    await hub.Group($"COMMAND_{game.Id}_0").SendAsync("EndGame", true);
                                    await hub.Group($"COMMAND_{game.Id}_1").SendAsync("EndGame", false);
                                }
                                else if (winCommand == 0)
                                {
                                    await hub.Group($"COMMAND_{game.Id}_0").SendAsync("EndGame", false);
                                    await hub.Group($"COMMAND_{game.Id}_1").SendAsync("EndGame", true);
                                }
                            }
                        }
                        break;
                    }
                    if (!projectile.GetNextPos.Normalize((short)(game.Map.Size - 1), (short)(game.Map.Size - 1)) ||
                        (game.Map[projectile.GetNextPos].Content != default &&
                        (game.Map[projectile.GetNextPos].Content is IBlock) && (game.Map[projectile.GetNextPos].Content as IBlock).Durability > 0))
                    {
                        var content = game.Map[projectile.GetNextPos].Content;
                        if (content is IBlock && (content as IBlock).CanBeBroken)
                        {
                            (content as IBlock).Damage(projectile.Damage);
                            projectile.Life = -1;
                            game.Map[projectile.GetNextPos].Background = (content as IBlock).Background;
                            game.Map[projectile.GetNextPos].Content = default;
                            Dictionary<Position, MapCell> cells = new Dictionary<Position, MapCell>();
                            cells.Add(projectile.GetNextPos, game.Map[projectile.GetNextPos]);
                            await hub.Group($"GAME_{game.Id}").SendAsync("GetMap", ConvertTypes.ToBytes(cells));
                            break;
                        }
                    }
                    await updateMap(game, new List<object> { projectile }, hub, client);
                    if (projectile.Position.Normalize((short)(game.Map.Size - 1), (short)(game.Map.Size - 1)))
                    {
                        projectile.Life = -1;
                        break;
                    }
                    Thread.Sleep(75);
                }
                await updateMap(game, new List<object> { new EmptyObject(){
                    Position = projectile.Position
                } }, hub, client);
                game.Projectiles.Remove(projectile);
            });
            return Task.CompletedTask;
        }

        public async Task SubToUpdateGameList()
        {
            if (Client == null)
                return;
            await Groups.AddToGroupAsync(Context.ConnectionId, "getGameList");
            await Clients.Caller.SendAsync("UpdateGamesList",
                                                JsonSerializer.Serialize(
                                                    Storage.Instance.Games.Select(
                                                        p => new GameInfoView(p.Id, p.Name, p.PlayerLimits, p.PlayerCount)
                                                        )
                                                    )
                                                );
        }

        public async Task UnsubToUpdateGameList()
        {
            if (Client == null)
                return;
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "getGameList");
        }

        public async Task Ping(string data) => await Clients.Caller.SendAsync("Pong", data);

        private async Task updateMap(GameModel game, List<object> entities,
            IHubCallerClients clients = default, HubClient hub = default,
            string[] exceptClients = null)
        {
            if (entities == null)
                return;
            clients = clients == default ? Clients : clients;
            List<UpdateCellView> cells = new List<UpdateCellView>();
            foreach (var entity in entities)
            {
                var pos = entity as IPosition;
                cells.Add(new UpdateCellView()
                {
                    Cell = new MapCell()
                    {
                        Content = entity,
                        Background = game.Map[pos.Position].Background
                    },
                    Position = pos.Position
                });
            }

            if (exceptClients != null && exceptClients.Length > 0)
            {
                await clients.GroupExcept($"GAME_{game.Id}", exceptClients)
                    .SendAsync("UpdateMap", ConvertTypes.ToBytes(cells));
            }
            else
            {
                await clients.Group($"GAME_{game.Id}")
                    .SendAsync("UpdateMap", ConvertTypes.ToBytes(cells));
            }
        }
    }
}
