using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoTConsole.Models
{
    public class MenuSetString
    {
        public string Data { get; }
        public bool OnlyInt { get; }
        public string Sender { get; }
        public string Val { get; }
        public MenuSetString(string data, string sender, string val, bool onlyInt = false)
        {
            Data = data;
            OnlyInt = onlyInt;
            Sender = sender;
            Val = val;
        }
    }
}
