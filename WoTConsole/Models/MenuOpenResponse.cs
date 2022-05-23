using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoTConsole.Models
{
    public class MenuOpenResponse
    {
        public string Name { get; }

        public MenuOpenResponse(string name)
        {
            Name = name;
        }
    }
}
