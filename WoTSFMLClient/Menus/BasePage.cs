using SFML.Graphics;
using WoTSFMLClient.Items;

namespace WoTSFMLClient.Pages
{
    public abstract class BasePage
    {
        public IReadOnlyList<Drawable> Items => _items.AsReadOnly();
        protected List<BaseItem> _items { get; } = new List<BaseItem>();
        public virtual void Loaded()
        {
            foreach (var item in _items)
                item.Loaded();
        }
        public virtual void Unloaded()
        {
            foreach (var item in _items)
                item.Unloaded();
        }
    }
}
