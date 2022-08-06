using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoTSFMLClient.Menus
{
    public abstract class BaseMenu
    {
        public IReadOnlyList<Drawable> Items => _items.AsReadOnly();
        protected List<Drawable> _items { get; } = new List<Drawable>();
        protected Window OwnerWindow { get; }
        public BaseMenu(Window window)
        {
            OwnerWindow = window;
        }
    }
}
