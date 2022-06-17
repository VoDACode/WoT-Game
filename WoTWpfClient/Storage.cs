using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoTWpfClient
{
    internal sealed class Storage
    {
        private static Storage instance;
        public static Storage Instance => instance ?? (instance = new Storage());
        private Storage() { }
        public string Nickname { get; set; }

        public const int H = 450;
        public const int W = 800;
    }
}
