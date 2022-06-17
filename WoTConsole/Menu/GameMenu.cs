using System.Drawing;
using WoTConsole.Models;
using WoTConsole.Services;
using WoTCore.Models;
using WoTCore.Models.MapObjects;

namespace WoTConsole.Menu
{
    internal class GameMenu : BaseMenu
    {
        private PlayerModel Player => Game.Instance.Player;
        public GameMenu(int size) : base(size)
        {
            
        }

        public override string Name => MenuConfig.GameMenu;

        protected override void Build()
        {
            NetworkServise.Instance.OnEndGame += OnEndGame;
        }

        private void OnEndGame(bool obj)
        {
            Response = new MenuResponse(new MenuOpenResponse(MenuConfig.RestartGameMenu));
            Close();
        }

        protected override void FirstEvent()
        {
            ControlService.Instance.OnBottom += EventDownKey;
            ControlService.Instance.OnTop += EventUpKey;
            ControlService.Instance.OnRight += EventRightKey;
            ControlService.Instance.OnLeft += EventLeftKey;
            ControlService.Instance.OnShot += EventSpace;
            NetworkServise.Instance.OnMapUpdate += Tick;
        }

        protected override void LastEvent()
        {
            ControlService.Instance.OnBottom -= EventDownKey;
            ControlService.Instance.OnTop -= EventUpKey;
            ControlService.Instance.OnRight -= EventRightKey;
            ControlService.Instance.OnLeft -= EventLeftKey;
            ControlService.Instance.OnShot -= EventSpace;
            NetworkServise.Instance.OnMapUpdate -= Tick;
        }

        private void Tick(Dictionary<Position, MapCell> objects)
        {
            foreach(var obj in objects)
            {
                IUniversalId uid = obj.Value.Content as IUniversalId;
                Position position = obj.Key;

                if (obj.Value.Content != default)
                {
                    Position lastPos = (obj.Value.Content as IPosition).Position.PreliminaryState<Position>();
                    ICell backCell = obj.Value.Background as ICell;
                    if (backCell != null)
                    {
                        Field[lastPos.X, lastPos.Y].BackgroundColor = backCell.BackgroundColor;
                        Field[lastPos.X, lastPos.Y].ForegroundColor = backCell.ForegroundColor;
                        Field[lastPos.X, lastPos.Y].Icon = backCell.Icon;
                    }

                    if (obj.Value.Content is IPlayer)
                    {
                        IPlayer player = obj.Value.Content as IPlayer;
                        Field[position.X, position.Y].Icon = player.Icon;
                        if (player.Session == Player.Session)
                        {
                            Player.Life = player.Life;
                            Player.MaxLife = player.MaxLife;
                            DisplayService.Instance.AddToQueue(DisplayInfoZone.Instance.Tick);
                        }
                        if (player.Killed)
                        {
                            Field[position.X, position.Y].BackgroundColor = Color.Gray;
                            Field[position.X, position.Y].ForegroundColor = Color.Black;
                        }
                        else
                        {
                            if (player.Command != Player.Command)
                                Field[position.X, position.Y].BackgroundColor = Color.DarkRed;
                            else if (player.Command == Player.Command && Player.Session != player.Session)
                                Field[position.X, position.Y].BackgroundColor = Color.DarkGreen;
                            else if (player.Command == Player.Command && Player.Session == player.Session)
                                Field[position.X, position.Y].BackgroundColor = Color.YellowGreen;
                        }
                    }
                    else if(obj.Value.Content is IProjectile)
                    {
                        IProjectile projectile = obj.Value.Content as IProjectile;
                        Field[projectile.Position.X, projectile.Position.Y].Icon = projectile.Icon;
                        Field[projectile.Position.X, projectile.Position.Y].ForegroundColor = projectile.ForegroundColor;
                        Field[projectile.Position.X, projectile.Position.Y].BackgroundColor = projectile.BackgroundColor;
                    }
                    else if(obj.Value.Content is ICell)
                    {
                        ICell cell = obj.Value.Content as ICell;
                        Field[position.X, position.Y].ForegroundColor = cell.ForegroundColor;
                        Field[position.X, position.Y].BackgroundColor = cell.BackgroundColor;
                    }
                    else if(obj.Value.Content is IEmptyObject)
                    {
                        ICell back = obj.Value.Background as ICell;
                        if (back != null)
                        {
                            Field[position.X, position.Y].BackgroundColor = back.BackgroundColor;
                            Field[position.X, position.Y].ForegroundColor = back.ForegroundColor;
                            Field[position.X, position.Y].Icon = back.Icon;
                        }
                    }
                }
                else if(obj.Value.Background != default)
                {
                    ICell backCell = obj.Value.Background as ICell;
                    Field[position.X, position.Y].BackgroundColor = backCell.BackgroundColor;
                    Field[position.X, position.Y].ForegroundColor = backCell.ForegroundColor;
                    Field[position.X, position.Y].Icon = backCell.Icon;
                }
            }
            Updata();
        }

        #region Key events
        protected override void EventSpace()
        {
            NetworkServise.Instance.Shot();
        }
        protected override void EventUpKey()
        {
            NetworkServise.Instance.GoTo(TurnObject.Top);
        }
        protected override void EventDownKey()
        {
            NetworkServise.Instance.GoTo(TurnObject.Bottom);
        }
        protected override void EventLeftKey()
        {
            NetworkServise.Instance.GoTo(TurnObject.Left);
        }
        protected override void EventRightKey()
        {
            NetworkServise.Instance.GoTo(TurnObject.Right);
        }
        #endregion
    }
}
