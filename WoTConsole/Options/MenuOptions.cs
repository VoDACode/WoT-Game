using WoTCore.Models;

namespace WoTConsole.Options
{
    public class MenuOptions
    {
        public bool ShowMenu { get; set; }
        public bool Center { get; set; } = true;
        public Position Marning { get; set; } = new Position();
    }
}
