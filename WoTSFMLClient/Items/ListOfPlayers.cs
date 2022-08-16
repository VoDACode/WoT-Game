using SFML.Graphics;
using SFML.System;
using SFML.Window;
using WoTSFMLClient.Models;
using WoTSFMLCore.Interfaces;

namespace WoTSFMLClient.Items
{
    public class ListOfPlayers : BaseItem
    {
        private const int X_SIZE = 150;
        private const int Y_SIZE = 27;
        private const int MARGIN = 10;
        private RectangleShape mainBox;
        private Text text;
        private List<IPlayerShortInfo> playerList;
        public IReadOnlyList<IPlayerShortInfo> Players => playerList.AsReadOnly();
        public ListOfPlayers(Window window, List<IPlayerShortInfo>? players = null) : base(window)
        {
            mainBox = new RectangleShape();
            mainBox.FillColor = new Color(50, 50, 50, 100);
            text = new Text(string.Empty, new Font("SourceSansPro-Regular.otf"), 24);
            playerList = players ?? new List<IPlayerShortInfo>();
            Position = new PositionData(0, 10);
            calcSize();
        }

        public void AddPlayer(IPlayerShortInfo item)
        {
            playerList.Add(item);
            calcSize();
        }

        public bool RemovePlayer(IPlayerId id)
        {
            var item = playerList.SingleOrDefault(p => p.Id == id.Id);
            if (item != null)
            {
                playerList.Remove(item);
                calcSize();
                return true;
            }
            return false;
        }

        public override Vector2f Size
        {
            get => mainBox.Size;
            set
            {
                mainBox.Size = value;
            }
        }
        public override PositionData Position
        {
            get => mainBox.Position;
            set
            {
                _position = value;
                mainBox.Position = _positionInPixel;
            }
        }

        public override void Loaded()
        {
            alignCalculate();
            base.Loaded();
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(mainBox);
            for(int i = 0; i < playerList.Count; i++)
            {
                text.DisplayedString = $"[{playerList[i].Murders}] {playerList[i].Name}";
                text.Position = new Vector2f(Position.X + MARGIN, Position.Y + Y_SIZE * i + MARGIN * (i + 1));
                target.Draw(text);
            }
        }

        private void calcSize()
        {
            mainBox.Size = new Vector2f(          
                X_SIZE + MARGIN * 2,
                (playerList.Count + 1) * MARGIN + playerList.Count * Y_SIZE
                );
        }
    }
}
