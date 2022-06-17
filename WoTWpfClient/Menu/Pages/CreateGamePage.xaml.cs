using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WoTWpfClient.Models.Pages;

namespace WoTWpfClient.Menu.Pages
{
    /// <summary>
    /// Interaction logic for CreateGamePage.xaml
    /// </summary>
    public partial class CreateGamePage : Page
    {
        private static Regex _regex = new Regex("[^0-9.-]+");
        public event Action OnBack;
        public event Action OnCreateGame;
        public CreateGameInfo GameInfo { get; } = new CreateGameInfo();
        public CreateGamePage()
        {
            InitializeComponent();
            var rand = new Random();
            TextBox_Port.Text = rand.Next(55000, 62999).ToString();
            TextBox_GameName.Text = $"Game_{rand.Next(9999)}";
            TextBox_PlayerLimit.Text = "4";
        }

        private void TextBox_PlayerLimit_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = _regex.IsMatch(e.Text);
        }

        private void TextBox_PlayerLimit_TextChanged(object sender, TextChangedEventArgs e)
        {
            if((sender as TextBox).Text == "")
            {
                GameInfo.PlayerLimit = 1;
                (sender as TextBox).Text = "1";
                (sender as TextBox).CaretIndex = (sender as TextBox).Text.Length;
            }
            if(double.Parse((sender as TextBox).Text) > int.MaxValue)
            {
                (sender as TextBox).Text = (sender as TextBox).Text.Substring(0, (sender as TextBox).Text.Length - 1);
                (sender as TextBox).CaretIndex = (sender as TextBox).Text.Length;
            }
            GameInfo.PlayerLimit = int.Parse((sender as TextBox).Text);
        }

        private void TextBox_Port_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = _regex.IsMatch(e.Text);
        }

        private void TextBox_Port_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((sender as TextBox).Text == "")
            {
                GameInfo.Port = 0;
                (sender as TextBox).Text = "0";
                (sender as TextBox).CaretIndex = (sender as TextBox).Text.Length;
            }
            if (double.Parse((sender as TextBox).Text) > int.MaxValue)
            {
                (sender as TextBox).Text = (sender as TextBox).Text.Substring(0, (sender as TextBox).Text.Length - 1);
                (sender as TextBox).CaretIndex = (sender as TextBox).Text.Length;
            }
            GameInfo.Port = int.Parse((sender as TextBox).Text);
        }

        private void Butten_CreateGame_Click(object sender, RoutedEventArgs e)
        {
            GameInfo.Name = TextBox_GameName.Text;
            OnCreateGame?.Invoke();
        }

        private void Butten_Back_Click(object sender, RoutedEventArgs e)
        {
            OnBack?.Invoke();
        }

        private void TextBox_GameName_TextChanged(object sender, TextChangedEventArgs e)
        {
            var text = (sender as TextBox).Text;

            if (text.Length > 0 && text[0] == ' ' && !string.IsNullOrWhiteSpace(text))
            {
                int index = 0;
                for (int i = 0; i < text.Length; i++)
                {
                    if (text[i] != ' ')
                    {
                        index = i;
                        break;
                    }
                }
                text = text.Substring(index);
                (sender as TextBox).Text = text;
                (sender as TextBox).CaretIndex = text.Length;
            }
            Butten_CreateGame.IsEnabled = !string.IsNullOrWhiteSpace(text) && text.Length >= 2;
        }
    }
}
