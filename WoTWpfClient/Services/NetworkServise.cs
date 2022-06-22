using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WoTCore;
using WoTCore.Helpers;
using WoTCore.Models;
using WoTCore.Views;

namespace WoTWpfClient.Services
{
    delegate void MapUpdateEvent(MapCell[,] map, Position absolutelyRegion, Position iRegion, Position leftTopRegion, bool endOfMessage, bool fistMessage);
    delegate void UpdateGamesList(List<GameInfoView> objects);
    delegate void UpdateCells(List<UpdateCellView> objects);
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

        public event MapUpdateEvent OnGetMap;
        public event UpdateGamesList OnUpdateGamesList;
        public event UpdateCells OnUpdateCells;
        public event Action<bool> OnEndGame;
        public event Action OnRestartGame;
        public event Action OnUpdatePlayerInfo;

        public string Host => _host;
        public int Port => _port;

        protected override void Execute()
        {
        }

        private async void UpdataPing()
        {
            await Task.Delay(500);
            while (hub == null || hub.State != HubConnectionState.Connected)
                await Task.Delay(100);
            await hub.InvokeAsync("Ping", pingData);
            pingStartTime = DateTime.Now;
        }

        public async Task StopHub() => await hub.StopAsync();
        public async Task Connect(string host, int port, int size)
        {
            if(hub != null && (hub.State == HubConnectionState.Connected || hub.State == HubConnectionState.Connecting))
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
            await Task.Run(() => UpdataPing());
            hub.On("Pong", (string data) =>
            {
                pingTime = DateTime.Now - pingStartTime;
                UpdataPing();
            });
            hub.On("GetMap", (byte[] date, Position absolutelyRegion, Position iRegion, Position leftTopRegion, bool endOfMessage, bool fistMessage) =>
            {
                try
                {
                    MapCell[,] map = ConvertTypes.ToObject<MapCell[,]>(date);
                    OnGetMap?.Invoke(map, absolutelyRegion, iRegion, leftTopRegion, endOfMessage, fistMessage);
                }catch(Exception ex)
                {
                    throw new Exception(ex.ToString());
                }
            });
            hub.On("UpdateMap", (byte[] data) =>
            {
                OnUpdateCells?.Invoke(ConvertTypes.ToObject<List<UpdateCellView>>(data));
            });
            hub.On("UpdateGamesList", (string listJson) =>
            {
                OnUpdateGamesList?.Invoke(JsonSerializer.Deserialize<List<GameInfoView>>(listJson));
            });
            hub.On("Chat", (string message) =>
            {
                OnNewMessage?.Invoke(message);
            });
            hub.On("EndGame", (bool isWin) => OnEndGame?.Invoke(isWin));
            hub.On("RestartGame", () => OnRestartGame?.Invoke());
            hub.On("UpdatePlayer", (PlayerModel player) =>
            {
                Storage.Instance.Player = player;
                OnUpdatePlayerInfo?.Invoke();
            });
            await hub.StartAsync();
            
            if (!Auth(size))
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
        public bool Auth(int size) =>
            Http.Post($"{api}game/auth?id={hub.ConnectionId}&key={Config.VersionKey}&s={size}").StatusCode == HttpStatusCode.OK;
        public HttpResponseMessage SetNick(string nick) => 
            Http.Post($"{api}game/set-nick?id={hub.ConnectionId}&n={nick}");
        public HttpResponseMessage CreateGame(string name, int playerLimits) =>
            Http.Post($"{api}game/create?id={hub.ConnectionId}&n={name}&pl={playerLimits}");
        public HttpResponseMessage Join(int gameId) =>
            Http.Post($"{api}game/join?id={hub.ConnectionId}&gId={gameId}");
        public async void GoTo(TurnObject turn) =>
            await hub.InvokeAsync("GoTo", turn);
        public async void Shot() =>
            await hub.InvokeAsync("Shot");
        public async void UpdateGameList(bool sub) =>
             await hub.InvokeAsync($"{(sub ? "S" : "Uns")}ubToUpdateGameList");

        public async void GetMap() =>
            await hub.InvokeAsync("GetMap");

        public HttpResponseMessage SendMessage(string message) =>
            Http.Post($"{api}game/chat?id={hub.ConnectionId}&m={message}");

        public HttpResponseMessage RestartGame() =>
            Http.Post($"{api}game/restart?id={hub.ConnectionId}");
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
