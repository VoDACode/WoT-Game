using SFML.Graphics;
using SFML.System;
using System.Text.RegularExpressions;
using WoTSFMLClient.Items;
using WoTSFMLClient.Models;

namespace WoTSFMLClient.Pages
{
    public class CreateGamePage : BasePage
    {
        private const int margin = 8;
        private const int startPos = 35;
        private static Regex _regex = new Regex("[^0-9.-]+");
        private TextBox nameBox;
        private TextBox limitBox;
        private TextBox portBox;
        private Button createButton;
        private Button backButton;
        private Panel bg = new Panel(App.Window, new Color(235, 255, 255));

        public CreateGamePage()
        {
            bg.Size = new Vector2f(App.Window.Size.X, App.Window.Size.Y);
            nameBox = new TextBox(App.Window,
                new PositionData(1, startPos, PositionUnits.Percentage),
                new Vector2f(250, 75),
                String.Empty,
                "Game name");
            nameBox.FontSize = 36;
            nameBox.HorizontalAlign = HorizontalAlignType.Center;
            nameBox.OnChange += NameBox_OnChange;
            nameBox.MaxLength = 12;
            limitBox = new TextBox(App.Window,
                new PositionData(1, startPos + margin, PositionUnits.Percentage),
                new Vector2f(250, 75),
                String.Empty,
                "Player limit");
            limitBox.FontSize = 36;
            limitBox.HorizontalAlign = HorizontalAlignType.Center;
            limitBox.OnPreviewChange += LimitBox_OnPreviewChange;
            limitBox.OnChange += LimitBox_OnChange;
            limitBox.MaxLength = 12;
            portBox = new TextBox(App.Window,
                new PositionData(1, startPos + margin * 2, PositionUnits.Percentage),
                new Vector2f(250, 75),
                String.Empty,
                "Port");
            portBox.FontSize = 36;
            portBox.HorizontalAlign = HorizontalAlignType.Center;
            portBox.OnPreviewChange += PortBox_OnPreviewChange;
            portBox.OnChange += PortBox_OnChange;
            createButton = new Button(App.Window,
                new PositionData(1, startPos + margin * 3, PositionUnits.Percentage),
                new Vector2f(250, 75),
                "Create!");
            createButton.IsEnabled = false;
            createButton.BorderRadius = 6;
            createButton.HorizontalAlign = HorizontalAlignType.Center;
            backButton = new Button(App.Window,
                new PositionData(1, startPos + margin * 4, PositionUnits.Percentage),
                new Vector2f(250, 75),
                "Back");
            backButton.HorizontalAlign = HorizontalAlignType.Center;
            backButton.OnClick += (e, s) => App.Navigator.Pop();
            backButton.BorderRadius = 6;
            _items.Add(bg);
            _items.Add(nameBox);
            _items.Add(limitBox);
            _items.Add(portBox);
            _items.Add(createButton);
            _items.Add(backButton);
        }

        private void PortBox_OnChange(TextBox sender, string text)
        {
            if (!string.IsNullOrEmpty(sender.Text) && double.Parse(sender.Text) > int.MaxValue)
            {
                sender.Text = sender.Text.Substring(0, sender.Text.Length - 1);
            }
        }

        private bool PortBox_OnPreviewChange(TextBox sender, string unicode)
            => _regex.IsMatch(sender.Text + unicode);

        private void LimitBox_OnChange(TextBox sender, string text)
        {
            if (!string.IsNullOrEmpty(sender.Text) && double.Parse(sender.Text) > int.MaxValue)
            {
                sender.Text = sender.Text.Substring(0, sender.Text.Length - 1);
            }
        }

        private bool LimitBox_OnPreviewChange(TextBox sender, string unicode)
            => _regex.IsMatch(sender.Text + unicode);

        private void NameBox_OnChange(TextBox sender, string text)
        {
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
                sender.Text = text;
            }
            createButton.IsEnabled = !string.IsNullOrWhiteSpace(text) && text.Length >= 2;
        }
    }
}
