using SFML.Graphics;
using SFML.System;
using SFML.Window;

using WoTSFMLClient.Items;
using WoTSFMLClient.Models;

namespace WoTSFMLClient.Pages
{
    public class StartPage : BasePage
    {
        private readonly Color bgColor = new Color(235, 255, 255);
        private const int margin = 8;
        private const int startPos = 35;
        private TextBox nicknameBox;
        private Button createGameButton;
        private Button joinToGameButton;
        private Button exitButton;
        private Panel bg = new Panel(App.Window, new Color(235, 255, 255));

        public StartPage()
        {
            bg.Size = new Vector2f(App.Window.Size.X, App.Window.Size.Y);
            nicknameBox = new TextBox(App.Window,
                new PositionData(1, startPos, PositionUnits.Percentage),
                new Vector2f(250, 60),
                String.Empty,
                "Nickname"
                );
            nicknameBox.FontSize = 36;
            nicknameBox.MaxLength = 16;
            nicknameBox.HorizontalAlign = HorizontalAlignType.Center;
            nicknameBox.OnChange += NicknameBox_OnChange;
            createGameButton = new Button(App.Window,
                new PositionData(1, startPos + margin, PositionUnits.Percentage),
                new Vector2f(250, 75),
                "Create new game");
            createGameButton.IsEnabled = false;
            createGameButton.BorderRadius = 6;
            createGameButton.HorizontalAlign = HorizontalAlignType.Center;
            createGameButton.OnClick += CreateGameButton_OnClick;
            joinToGameButton = new Button(App.Window,
                new PositionData(1, startPos + margin * 2, PositionUnits.Percentage),
                new Vector2f(250, 75),
                "Join to the game");
            joinToGameButton.IsEnabled = false;
            joinToGameButton.BorderRadius = 6;
            joinToGameButton.HorizontalAlign = HorizontalAlignType.Center;
            exitButton = new Button(App.Window,
                new PositionData(1, startPos + margin * 3, PositionUnits.Percentage),
                new Vector2f(250, 75),
                "Exit");
            exitButton.BorderRadius = 6;
            exitButton.HorizontalAlign = HorizontalAlignType.Center;
            exitButton.OnClick += (e, s) => App.Exit();
            _items.Add(bg);
            _items.Add(nicknameBox);
            _items.Add(createGameButton);
            _items.Add(joinToGameButton);
            _items.Add(exitButton);
            Mouse.SetPosition(new Vector2i((int)(App.Window.Size.X / 2), (int)(App.Window.Size.Y / 2)), App.Window);
        }

        private void CreateGameButton_OnClick(Button item, Vector2i mouse)
        {
            App.Navigator.Push(new CreateGamePage());
        }

        private void NicknameBox_OnChange(TextBox sender, string text)
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
            createGameButton.IsEnabled = !string.IsNullOrWhiteSpace(text) && text.Length >= 2;
            joinToGameButton.IsEnabled = !string.IsNullOrWhiteSpace(text) && text.Length >= 2;
        }
    }
}
