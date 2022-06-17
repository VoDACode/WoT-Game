using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Server.Models;
using System;
using System.Collections.Generic;
using WoTCore.Models;
using WoTCore.Modes;
using WoTCore.Modes.Resources;

namespace Server
{
    public class Program
    {
        private static bool _running = false;
        private static bool _noLog = false;
        private static bool _debug = false;
        private static bool _createNewGame = false;
        private static int Port => int.Parse(Args["port"]);
        private static Dictionary<string, string> Args = new Dictionary<string, string>();
        public static void Main(string[] args)
        {
            if (_running)
                return;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--no-log")
                {
                    _noLog = true;
                    continue;
                }
                else if (args[i] == "--debug")
                {
                    _debug = true;
                    continue;
                }
                else if (args[i] == "--creare-new-game")
                {
                    _createNewGame = true;
                    continue;
                }
                var tmp = args[i].Split('=');
                Args.Add(tmp[0], tmp[1]);
            }
            if (!Args.ContainsKey("port"))
                Args.Add("port", "5050");
            if (!_noLog)
            {
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.Console()
                    .MinimumLevel.Is(_debug ? Serilog.Events.LogEventLevel.Debug : Serilog.Events.LogEventLevel.Information)
                    .CreateBootstrapLogger();
                if (_debug)
                    Log.Information("DEBUG MODE");
            }
            else
            {
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.Debug()
                    .CreateBootstrapLogger();
            }
            _running = true;

            try
            {
                Storage.Instance.Modes.AddRange(new List<ModeContent>
                {
                    new ModeContent(new WoTCore.Modes.AssemblyInfo()
                    {
                        Description = DefaultMode.AssemblyInfo.Description,
                        AddedResources = DefaultMode.AssemblyInfo.AddedResources,
                        Author = DefaultMode.AssemblyInfo.Author,
                        Name = DefaultMode.AssemblyInfo.Name,
                        Version = DefaultMode.AssemblyInfo.Version
                    })
                    {
                        Blocks = new List<BlockResource>()
                        {
                            blockToBlockResource(new DefaultMode.Blocks.Bush()),
                            blockToBlockResource(new DefaultMode.Blocks.Sand()),
                            blockToBlockResource(new DefaultMode.Blocks.Stone()),
                            blockToBlockResource(new DefaultMode.Blocks.Water())
                        }
                    },
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                Console.WriteLine("Press Enter to close.");
                Console.ReadLine();
                return;
            }
            Log.Information($"Loaded {Storage.Instance.Modes.Count} modes.");

            if (_createNewGame)
            {
                var game = Storage.Instance.Games.Add(new GameModel(0, "TEST_GAME", 50));
                game.Map.Clear();
                game.Map.Generate(Storage.Instance.Modes);
            }

            CreateHostBuilder(args).Build().RunAsync().Wait();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls($"https://*:{Port}");
                });
        private static BlockResource blockToBlockResource<T>(T block) where T : BaseBlock
        {
            BlockResource resulr = new BlockResource();
            var iType = block.GetType();
            var iObj = Activator.CreateInstance(iType);
            foreach (var m in iType.GetProperties())
            {
                var p = resulr.GetType().GetProperty(m.Name);
                if (p == null)
                    continue;
                p.SetValue(resulr, m.GetValue(iObj));
            }
            resulr.SetObject(iObj);
            return resulr;
        }
    }
}
