using SFML.Graphics;
using SFML.System;
using System.Text.RegularExpressions;
using WoTSFMLClient.Items;
using WoTSFMLClient.Models;

namespace WoTSFMLClient.Pages
{
    public class JoinToGamePage : BasePage
    {
        private const int margin = 8;
        private const int startPos = 35;
        private static Regex port = new Regex("[^0-9.-]+");
        private static Regex Ip = new Regex(@"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$");
        private static Regex Host = new Regex(@"^(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)*([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\-]*[A-Za-z0-9])$");
        private Panel bg = new Panel(App.Window, new Color(235, 255, 255));
        private TextBox hostBox;
        private TextBox portBox;
        private Button joinButton;
        private Button backButton;

        public JoinToGamePage()
        {
            bg.Size = new Vector2f(App.Window.Size.X, App.Window.Size.Y);
            hostBox = new TextBox(App.Window,
                new PositionData(1, startPos, PositionUnits.Percentage),
                new Vector2f(250, 75),
                String.Empty,
                "Host");
            hostBox.HorizontalAlign = HorizontalAlignType.Center;
            hostBox.OnChange += HostBox_OnChange;
            hostBox.MaxLength = 64;
            portBox = new TextBox(App.Window,
                new PositionData(1, startPos + margin, PositionUnits.Percentage),
                new Vector2f(250, 75),
                String.Empty,
                "Port");
            portBox.HorizontalAlign = HorizontalAlignType.Center;
            portBox.OnChange += PortBox_OnChange;
            portBox.OnPreviewChange += PortBox_OnPreviewChange;
            portBox.MaxLength = 12;
            joinButton = new Button(App.Window,
                new PositionData(1, startPos + margin * 2, PositionUnits.Percentage),
                new Vector2f(250, 75),
                "Join!");
            joinButton.IsEnabled = false;
            joinButton.BorderRadius = 6;
            joinButton.HorizontalAlign = HorizontalAlignType.Center;
            backButton = new Button(App.Window,
                new PositionData(1, startPos + margin * 3, PositionUnits.Percentage),
                new Vector2f(250, 75),
                "Back");
            backButton.HorizontalAlign = HorizontalAlignType.Center;
            backButton.OnClick += (e, s) => App.Navigator.Pop();
            backButton.BorderRadius = 6;
            _items.Add(bg);
            _items.Add(hostBox);
            _items.Add(portBox);
            _items.Add(joinButton);
            _items.Add(backButton);
        }

        private bool PortBox_OnPreviewChange(TextBox sender, string unicode)
            => port.IsMatch(sender.Text + unicode);

        private void PortBox_OnChange(TextBox sender, string text)
        {
            if (!string.IsNullOrEmpty(sender.Text) && double.Parse(sender.Text) > int.MaxValue)
            {
                sender.Text = sender.Text.Substring(0, sender.Text.Length - 1);
            }
        }

        private void HostBox_OnChange(TextBox sender, string text)
        {
            joinButton.IsEnabled = Host.IsMatch(sender.Text) || Ip.IsMatch(sender.Text);
        }
    }
}
