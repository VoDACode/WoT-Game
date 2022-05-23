using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using WoTCore;
using WoTCore.Models;
using WoTCore.Views;

namespace Server.Controllers
{
    [Route("api/game")]
    [ApiController]
    public class GameController : ControllerBase
    {
        IHubContext<GameHub> hub;
        private HubClient Client(string id) => Storage.Instance.HubClients.SingleOrDefault(p => p.ConnectionId == id);
        private GameModel Game(string id) => Client(id) == null ? null : Storage.Instance.Games.SingleOrDefault(p => p.ExistPlayer(Client(id).Player));

        public GameController(IHubContext<GameHub> hub)
        {
            this.hub = hub;
        }

        [HttpPost("auth")]
        public IActionResult Auth(string key, string id, int s)
        {
            if (key == Config.VersionKey && Storage.Instance.WaitUsersConnection.Any(p => p == id) && s > 5)
            {
                Storage.Instance.WaitUsersConnection.Remove(id);
                Storage.Instance.HubClients.Add(new HubClient(id, s));
                return Ok();
            }
            return NotFound();
        }

        [HttpPost("set-nick")]
        public IActionResult SetNick(string id, string n)
        {
            if (string.IsNullOrEmpty(id) || !Storage.Instance.HubClients.Any(p => p.ConnectionId == id))
                return NotFound();
            if (string.IsNullOrEmpty(n) || !(n.Length <= 20 && n.Length > 1))
                return BadRequest("1 > n.Length > 20");
            Client(id).Player.Name = n;
            return Ok();
        }

        [HttpPost("create")]
        public IActionResult CreateGame(string id, string n, int pl)
        {
            if (string.IsNullOrEmpty(id) || !Storage.Instance.HubClients.Any(p => p.ConnectionId == id))
                return NotFound();
            if (string.IsNullOrEmpty(n) || !(n.Length <= 16 && n.Length > 1) || pl < 2)
                return BadRequest();
            var game = Storage.Instance.Games.Add(new GameModel(0, n, pl));
            game.OnEmpty += Game_OnEmpty;
            game.OnLeave += Game_OnLeave;
            game.Map.Clear();
            game.Map.Generate(Storage.Instance.Modes);
            hub.Groups.AddToGroupAsync(id, $"GAME_{game.Id}");
            var command = game.AddPlayer(Client(id).Player);
            Client(id).Player.Command = command;
            hub.Groups.AddToGroupAsync(id, $"COMMAND_{game.Id}_{command}");
            hub.Clients.Group("getGameList").SendAsync("UpdataGamesList", 
                                                            JsonConvert.SerializeObject(
                                                                Storage.Instance.Games.Select(
                                                                    p => new GameInfoView(p.Id, p.Name, p.PlayerLimits, p.PlayerCount)
                                                                    )
                                                                )
                                                            );

            hub.Clients.Group($"GAME_{game.Id}").SendAsync("GetMap", game.Map.GetList.ToBytes());
            hub.Groups.AddToGroupAsync(id, $"GAME_{game.Id}_CHAT");
            hub.Clients.Client(id).SendAsync("Chat", $"'{Client(id).Player.Name}' welcome!");
            return Ok(new
            {
                gameId = game.Id,
                command= command
            });
        }

        [HttpPost("join")]
        public IActionResult Join(string id, int gId)
        {
            if (string.IsNullOrEmpty(id) || !Storage.Instance.HubClients.Any(p => p.ConnectionId == id))
                return NotFound();
            if (Game(id) != null || !Storage.Instance.Games.Any(p => p.Id == gId))
                return BadRequest();

            var game = Storage.Instance.Games.SingleOrDefault(p => p.Id == gId);
            if (game.IsFull)
                return BadRequest("Game is full!");

            var command = game.AddPlayer(Client(id).Player);
            hub.Groups.AddToGroupAsync(id, $"GAME_{gId}");
            hub.Groups.AddToGroupAsync(id, $"COMMAND_{gId}_{command}");
            hub.Clients.Group($"GAME_{game.Id}_CHAT").SendAsync("Chat", $"'{Client(id).Player.Name}' join the game!");
            hub.Groups.AddToGroupAsync(id, $"GAME_{game.Id}_CHAT");
            hub.Clients.Client(id).SendAsync("Chat", $"'{Client(id).Player.Name}' welcome!");
            hub.Clients.Group($"GAME_{game.Id}").SendAsync("GetMap", game.Map.GetList.ToBytes());

            return Ok(new JoinGameView(game.Id, command));
        }

        [HttpGet("info/{gameId}")]
        public IActionResult GetGameInfo(int gameId)
        {
            if(!Storage.Instance.Games.Any(p => p.Id == gameId))
                return NotFound();
            return Ok(Storage.Instance.Games.SingleOrDefault(p => p.Id == gameId));
        }

        [HttpPost("chat")]
        public IActionResult PostMessageToChat(string id, string m)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(m) || !Storage.Instance.HubClients.Any(p => p.ConnectionId == id))
                return NotFound();
            if (Game(id) == null)
                return BadRequest();
            hub.Clients.Group($"GAME_{Game(id).Id}_CHAT").SendAsync("Chat", $"{Client(id).Player.Name}: {m}");
            return Ok();
        }

        [HttpPost("restart")]
        public IActionResult ResttartGame(string id)
        {
            if (string.IsNullOrEmpty(id) || !Storage.Instance.HubClients.Any(p => p.ConnectionId == id))
                return NotFound();
            var game = Game(id);
            if(game == null)
                return NotFound("Not found game!");

            game.Restart();
            hub.Clients.Group($"GAME_{Game(id).Id}").SendAsync("RestartGame");
            return Ok();
        }

        private void Game_OnEmpty(object sender, EventArgs e)
        {
            var game = sender as GameModel;
            game = Storage.Instance.Games.GetForId(game.Id);
            Storage.Instance.Games.Remove(game);
        }
        private void Game_OnLeave(GameModel game, CommandModel command, PlayerModel player)
        {
            HubClient client = Storage.Instance.HubClients.SingleOrDefault(p => p.Player == player);
            if (client == null)
                return;
            hub.Groups.RemoveFromGroupAsync(client.ConnectionId, $"GAME_{game.Id}");
        }
    }
}
