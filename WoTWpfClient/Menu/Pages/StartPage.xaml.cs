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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WoTWpfClient.Menu.Pages
{
    /// <summary>
    /// Interaction logic for StartPage.xaml
    /// </summary>
    public partial class StartPage : Page
    {
        public event Action OnOpenGameCreatePage;
        public event Action OnOpenGameJoinPage;
        public StartPage()
        {
            InitializeComponent();
            setMode(false);
            TextBox_Nick.Text = "VoDA";
        }

        private void Button_Leave_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Button_JoinTheGame_Click(object sender, RoutedEventArgs e)
        {
            Storage.Instance.Nickname = TextBox_Nick.Text;
            OnOpenGameJoinPage?.Invoke();
        }

        private void Button_CreateGame_Click(object sender, RoutedEventArgs e)
        {
            Storage.Instance.Nickname = TextBox_Nick.Text;
            OnOpenGameCreatePage?.Invoke();
        }

        private void TextBox_Nick_TextChanged(object sender, TextChangedEventArgs e)
        {
            var text = (sender as TextBox)?.Text;
            
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
            setMode(!string.IsNullOrWhiteSpace(text) && text.Length >= 2);
        }

        private void setMode(bool enabled)
        {
            Button_CreateGame.IsEnabled = enabled;
            Button_JoinTheGame.IsEnabled = enabled;
        }
    }
}
