using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WoTWpfClient.Menu.Pages;
using System.Net;
using Newtonsoft.Json;
using WoTWpfClient.Services;
using System.Diagnostics;
using WoTCore.Models;
using WoTCore.Helpers;

namespace WoTWpfClient.Menu
{
    /// <summary>
    /// Interaction logic for MenuWindow.xaml
    /// </summary>
    public partial class MenuWindow : Window
    {
        private StartPage startPage { get; } = new StartPage();
        private CreateGamePage createGamePage { get; } = new CreateGamePage();
        private GameField gameField { get; } = new GameField();
        private JoinGamePage joinGamePage { get; } = new JoinGamePage();
        public MenuWindow()
        {
            InitializeComponent();

            startPage.OnOpenGameCreatePage += StartPage_OnOpenGameCreatePage;
            startPage.OnOpenGameJoinPage += StartPage_OnOpenGameJoinPage;

            createGamePage.OnBack += EventOnBack;
            createGamePage.OnCreateGame += CreateGamePage_OnCreateGame;

            joinGamePage.OnBack += EventOnBack;
            joinGamePage.OnOpenGameField += JoinGamePage_OnOpenGameField;

            gameField.OnBack += EventOnBack;

            this.Content = startPage;
        }

        private void JoinGamePage_OnOpenGameField()
        {
            gameField.Init();
            this.Content = gameField;
        }

        private async void CreateGamePage_OnCreateGame()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo($"{AppContext.BaseDirectory}Server.exe", $"port={createGamePage.GameInfo.Port} --debug");
            var info = Process.Start(startInfo);

            NetworkServise.Instance.OnConnect += OnConnect;
            await NetworkServise.Instance.Connect("127.0.0.1", createGamePage.GameInfo.Port, 400);
            void OnConnect()
            {
                var res = NetworkServise.Instance.CreateGame(createGamePage.GameInfo.Name, createGamePage.GameInfo.PlayerLimit);
                if (res.StatusCode != HttpStatusCode.OK)
                {
                    MessageBox.Show(res.GetString().Result);
                    return;
                }
                dynamic? obj = JsonConvert.DeserializeObject(res.GetString().Result);
                Game.Instance.Player.Command = (short)obj?["command"];
                Game.Instance.GameId = (int)obj?["gameId"];

                NetworkServise.Instance.OnConnect -= OnConnect;
                this.Content = gameField;
                gameField.Init();
            }
        }

        private void EventOnBack()
        {
            this.Content = startPage;
        }

        private void StartPage_OnOpenGameJoinPage()
        {
            this.Content = joinGamePage;
        }

        private void StartPage_OnOpenGameCreatePage()
        {
            this.Content = createGamePage;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            gameField.KeyDown(sender, e);
        }
    }
}
