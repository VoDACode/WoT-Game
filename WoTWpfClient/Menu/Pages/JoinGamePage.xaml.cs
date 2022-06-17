using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WoTCore.Views;
using WoTWpfClient.Models.Pages;
using WoTWpfClient.Services;

namespace WoTWpfClient.Menu.Pages
{
    /// <summary>
    /// Interaction logic for JoinGamePage.xaml
    /// </summary>
    public partial class JoinGamePage : Page
    {
        private static Regex port = new Regex("[^0-9.-]+");
        private static Regex Ip = new Regex(@"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$");
        private static Regex Host = new Regex(@"^(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)*([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\-]*[A-Za-z0-9])$");
        public event Action OnBack;
        public event Action OnOpenGameField;

        public ConnectionInfo ConnectionInfo { get; } = new ConnectionInfo();
        public JoinGamePage()
        {
            InitializeComponent();
            TextBox_Port.Text = "5050";
            TextBox_Host.Text = "127.0.0.1";
            NetworkServise.Instance.OnUpdateGamesList += OnUpdateGamesList;
        }

        

        private void OnUpdateGamesList(List<GameInfoView> objects)
        {
            Dispatcher.Invoke(() =>
            {
                foreach(var item in objects)
                {
                    Grid gridContect = new Grid();
                    gridContect.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(15, GridUnitType.Star)});
                    gridContect.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(70, GridUnitType.Star)});
                    gridContect.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(15, GridUnitType.Star)});
                    var counter = new Label()
                    {
                        Content = $"{item.PlayerCount}/{item.PlayerLimits}"
                    };
                    var name = new Label()
                    {
                        Content = item.Name
                    };
                    var joinButton = new Button()
                    {
                        Content = "Join"
                    };
                    joinButton.Click += (s, o) => Button_JoinToGame(item.Id);
                    Grid.SetColumn(counter, 0);
                    gridContect.Children.Add(counter);
                    Grid.SetColumn(name, 1);
                    gridContect.Children.Add(name);
                    Grid.SetColumn(joinButton, 2);
                    gridContect.Children.Add(joinButton);
                    StackPanel_GameList.Children.Add(gridContect);
                }
            });
        }

        private void Button_JoinToGame(int gameId)
        {
            var res = NetworkServise.Instance.Join(gameId);
            if (res.StatusCode == HttpStatusCode.BadRequest)
            {
                MessageBox.Show(res.Content.ReadAsStringAsync().Result);
                return;
            }
            else if (res.StatusCode == HttpStatusCode.NotFound)
            {
                MessageBox.Show("Not found!");
                return;
            }
            else if (res.StatusCode == HttpStatusCode.OK)
            {
                NetworkServise.Instance.UpdateGameList(false);
                var data = Newtonsoft.Json.JsonConvert.DeserializeObject<JoinGameView>(res.Content.ReadAsStringAsync().Result);
                Game.Instance.Player.Command = data.Command;
                Game.Instance.GameId = data.GameId;
                OnOpenGameField?.Invoke();
                return;
            }
        }

        private void TextBox_Host_TextChanged(object sender, TextChangedEventArgs e)
        {
            Button_TryConnect.IsEnabled = Ip.IsMatch(TextBox_Host.Text) || Host.IsMatch(TextBox_Host.Text);
            ConnectionInfo.Host = TextBox_Host.Text;
        }

        private async void Button_TryConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await NetworkServise.Instance.Connect(ConnectionInfo.Host, ConnectionInfo.Port, 100);
                NetworkServise.Instance.UpdateGameList(true);
            }catch
            {
                MessageBox.Show("Incorrect host or port");
            }
        }

        private void Button_Back_Click(object sender, RoutedEventArgs e)
        {
            OnBack?.Invoke();
        }

        private void TextBox_Port_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = port.IsMatch(e.Text);
        }

        private void TextBox_Port_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((sender as TextBox).Text == "")
            {
                ConnectionInfo.Port = 1;
                (sender as TextBox).Text = "1";
                (sender as TextBox).CaretIndex = (sender as TextBox).Text.Length;
            }
            if (double.Parse((sender as TextBox).Text) > 65000)
            {
                (sender as TextBox).Text = (sender as TextBox).Text.Substring(0, (sender as TextBox).Text.Length - 1);
                (sender as TextBox).CaretIndex = (sender as TextBox).Text.Length;
            }
            ConnectionInfo.Port = int.Parse((sender as TextBox).Text);
        }
    }
}
