using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoTSFMLClient.Items;

namespace WoTSFMLClient.Pages
{
    public class GamePage : BasePage
    {
        GameLayer gameLayer;
        public GamePage()
        {
            gameLayer = new GameLayer(App.Window);
            _items.Add(gameLayer);
        }
    }
}
