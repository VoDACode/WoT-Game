using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoTWpfClient.Models.Pages
{
    public class CreateGameInfo
    {
        public int Port { get; set; }
        public string Name { get; set; }
        public int PlayerLimit { get; set; }
    }
}
