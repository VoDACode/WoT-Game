using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WoTSFMLClient.Items;
using WoTSFMLClient.Models;

namespace WoTSFMLClient.Menus
{
    public class StartMenu : BaseMenu
    {
        private const int margin = 8;
        private const int startPos = 35;
        private TextBox nicknameBox;
        private Button createGameButton;
        private Button joinToGameButton;
        private Button exitButton;

        public StartMenu(Window window) : base(window)
        {
            nicknameBox = new TextBox(window,
                new PositionData(1, startPos, PositionUnits.Percentage),
                new Vector2f(250, 50),
                String.Empty,
                "Nickname"
                );
            nicknameBox.MaxLength = 16;
            nicknameBox.HorizontalAlign = HorizontalAlignType.Center;
            nicknameBox.OnChange += NicknameBox_OnChange;
            createGameButton = new Button(window,
                new PositionData(1, startPos + margin, PositionUnits.Percentage),
                new Vector2f(250, 75),
                "Create new game");
            createGameButton.IsEnabled = false;
            createGameButton.BorderRadius = 6;
            createGameButton.HorizontalAlign = HorizontalAlignType.Center;
            joinToGameButton = new Button(window,
                new PositionData(1, startPos + margin * 2, PositionUnits.Percentage),
                new Vector2f(250, 75),
                "Join to the game");
            joinToGameButton.IsEnabled = false;
            joinToGameButton.BorderRadius = 6;
            joinToGameButton.HorizontalAlign = HorizontalAlignType.Center;
            exitButton = new Button(window,
                new PositionData(1, startPos + margin * 3, PositionUnits.Percentage),
                new Vector2f(250, 75),
                "Exit");
            exitButton.BorderRadius = 6;
            exitButton.HorizontalAlign = HorizontalAlignType.Center;
            exitButton.OnClick += (e, s) => Environment.Exit(0);
            _items.Add(nicknameBox);
            _items.Add(createGameButton);
            _items.Add(joinToGameButton);
            _items.Add(exitButton);
            Mouse.SetPosition(new Vector2i((int)(window.Size.X / 2), (int)(window.Size.Y / 2)), window);
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
