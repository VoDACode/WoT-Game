using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System.Net;
using WoTCore;
using WoTCore.Helpers;
using WoTCore.Models;
using WoTCore.Views;

namespace WoTConsole.Services
{
    delegate void MapUpdateEvent(Dictionary<Position, MapCell> objects);
    delegate void UpdateGamesList(List<GameInfoView> objects);
    internal class NetworkServise : Service
    {
        private string _host = "127.0.0.1";
        private int _port = 5050;
        private string api => $"https://{_host}:{_port}/api/";
        private HubConnection hub { get; set; }

        private static NetworkServise instance;
        private string pingData { get; } = StringHelper.Random(32);
        private DateTime pingStartTime { get; set; }
        private TimeSpan pingTime { get; set; }
        public TimeSpan Ping => pingTime;
        public event Action OnConnect;
        public event Action<string> OnNewMessage;
        public bool IsConnected => hub != null && hub.State == HubConnectionState.Connected;
        public static NetworkServise Instance
        {
            get
            {
                if (instance == null)
                    instance = new NetworkServise();
                return instance;
            }
        }

        public event MapUpdateEvent OnMapUpdate;
        public event UpdateGamesList OnUpdateGamesList;
        public event Action<bool> OnEndGame;
        public event Action OnRestartGame;

        public string Host => _host;
        public int Port => _port;

        protected override void Execute()
        {
            Task.Factory.StartNew(() => UpdataPing());
        }

        private void UpdataPing()
        {
            Thread.Sleep(500);
            while (hub == null || hub.State != HubConnectionState.Connected)
                Thread.Sleep(100);
            hub.InvokeAsync("Ping", pingData);
            pingStartTime = DateTime.Now;
        }

        public async Task StopHub() => await hub.StopAsync();
        public async Task Connect(string host, int port)
        {
            if (hub != null && (hub.State == HubConnectionState.Connected || hub.State == HubConnectionState.Connecting))
            {
                await hub.StopAsync(CancellationToken.None);
                await hub.DisposeAsync();
            }
            hub = new HubConnectionBuilder()
                .WithUrl($"https://{host}:{port}/hub/game", (opt) =>
                {
                    opt.HttpMessageHandlerFactory = (m) =>
                    {
                        if (m is HttpClientHandler clientHandler)
                            clientHandler.ServerCertificateCustomValidationCallback +=
                            (s, ce, ch, sslPE) => true;
                        return m;
                    };
                })
                .WithAutomaticReconnect()
                .Build();
            _host = host;
            _port = port;
            hub.Closed += Hub_Closed;
            hub.Reconnected += Hub_Reconnected;
            hub.Reconnecting += Hub_Reconnecting;
            hub.On("Pong", (string data) =>
            {
                pingTime = DateTime.Now - pingStartTime;
                UpdataPing();
            });
            hub.On("GetMap", (byte[] date) =>
            {
                try
                {
                    OnMapUpdate?.Invoke(ConvertTypes.ToObject<Dictionary<Position, MapCell>>(date));
                }
                catch (Exception ex)
                {
                    throw new Exception($"");
                }
            });
            hub.On("UpdateGamesList", (string listJson) =>
            {
                OnUpdateGamesList?.Invoke(JsonConvert.DeserializeObject<List<GameInfoView>>(listJson));
            });
            hub.On("Chat", (string message) =>
            {
                OnNewMessage?.Invoke(message);
            });
            hub.On("EndGame", (bool isWin) => OnEndGame?.Invoke(isWin));
            hub.On("RestartGame", () => OnRestartGame?.Invoke());
            await hub.StartAsync();

            if (!Auth())
            {
                Console.Clear();
                Console.WriteLine("Old version!");
                Environment.Exit(0);
            }
            Game.Instance.Player.Session = GetSession;
            SetNick(Game.Instance.Player.Name);
            OnConnect?.Invoke();
        }

        #region Funcions
        public string GetSession =>
            Http.Get($"{api}info/session?id={hub.ConnectionId}").Result.Replace("\"", "");
        public bool Auth() =>
            Http.Post($"{api}game/auth?id={hub.ConnectionId}&key={Config.VersionKey}&s={DisplayService.Instance.Size}").Result.StatusCode == HttpStatusCode.OK;
        public HttpResponseMessage SetNick(string nick) =>
            Http.Post($"{api}game/set-nick?id={hub.ConnectionId}&n={nick}").Result;
        public HttpResponseMessage CreateGame(string name, int playerLimits) =>
            Http.Post($"{api}game/create?id={hub.ConnectionId}&n={name}&pl={playerLimits}").Result;
        public HttpResponseMessage Join(int gameId) =>
            Http.Post($"{api}game/join?id={hub.ConnectionId}&gId={gameId}").Result;
        public async void GoTo(TurnObject turn) =>
            await hub.InvokeAsync("GoTo", turn);
        public async void Shot() =>
            await hub.InvokeAsync("Shot");
        public async void UpdateGameList(bool sub) =>
             await hub.InvokeAsync($"{(sub ? "S" : "Uns")}ubToUpdateGameList");

        public HttpResponseMessage SendMessage(string message) =>
            Http.Post($"{api}game/chat?id={hub.ConnectionId}&m={message}").Result;

        public HttpResponseMessage RestartGame() =>
            Http.Post($"{api}game/restart?id={hub.ConnectionId}").Result;
        #endregion

        private Task Hub_Reconnecting(Exception? arg)
        {
            return Task.CompletedTask;
        }

        private Task Hub_Reconnected(string? arg)
        {
            OnConnect?.Invoke();
            return Task.CompletedTask;
        }

        private Task Hub_Closed(Exception? arg)
        {
            return Task.CompletedTask;
        }
    }
}
