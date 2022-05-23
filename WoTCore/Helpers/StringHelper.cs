using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoTCore.Helpers
{
    public static class StringHelper
    {
        private const string ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        public static string Random(int len)
        {
            Random random = new Random();
            StringBuilder str = new StringBuilder(len);
            for(int i = 0; i < len; i++)
                str.Append(ALPHABET[random.Next(ALPHABET.Length)]);
            return str.ToString();
        }
    }
}
