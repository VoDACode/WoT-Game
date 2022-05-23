using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoTConsole.Menu;
using WoTCore.Models;

namespace WoTConsole.Models
{
    public class MenuResponse
    {
        public object Data { get; }
        public MenuResponse(object data)
        {
            Data = data;
        }
    }
}
