using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoTConsole.Models
{
    public class MenuComponentModel
    {
        public string Text { get; set; }
        public Action Action { get; set; }

        public MenuComponentModel(string text, Action action)
        {
            Text = text;
            Action = action;
        }
    }
}
