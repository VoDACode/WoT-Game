using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Server.Models;
using System.Linq;

namespace Server.Controllers
{
    [Route("api/info")]
    [ApiController]
    public class InfoController : ControllerBase
    {
        IHubContext<GameHub> hub;
        private HubClient Client(string id) => Storage.Instance.HubClients.SingleOrDefault(p => p.ConnectionId == id);
        private GameModel Game(string id) => Client(id) == null ? null : Storage.Instance.Games.SingleOrDefault(p => p.ExistPlayer(Client(id).Player));

        public InfoController(IHubContext<GameHub> hub)
        {
            this.hub = hub;
        }

        [HttpGet("players")]
        public IActionResult GetPlayersList()
        {
            return Ok(Storage.Instance.HubClients.Select(p => p.Player));
        }

        [HttpGet("session")]
        public IActionResult GetSession(string id)
        {
            if (string.IsNullOrEmpty(id) || !Storage.Instance.HubClients.Any(p => p.ConnectionId == id))
                return NotFound();
            return Ok(Client(id).Player.Session);
        }
    }
}
