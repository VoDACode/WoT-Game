using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Server.Extensions;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server
{
    public class Program
    {
        private static bool _running = false;
        private static int Port => int.Parse(Args["port"]);
        private static Dictionary<string, string> Args = new Dictionary<string, string>();
        public static void Main(string[] args)
        {
            if(_running)
                return;
            for (int i = 0; i < args.Length; i++)
            {
                var tmp = args[i].Split('=');
                Args.Add(tmp[0], tmp[1]);
            }
            if (!Args.ContainsKey("port"))
                Args.Add("port", "5050");
            _running = true;

            Storage.Instance.Modes.AddRange(ModesLoader.ModesLoader.Load());
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls($"https://*:{Port}");
                });
    }
}
