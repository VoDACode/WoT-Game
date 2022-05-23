using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoTConsole.Models
{
    public class MenuResponseCreateGeme
    {
        public string Name { get; set; } = "New Game";
        public int PlayerLimits { get; set; } = 5;
    }
}
