// See https://aka.ms/new-console-template for more information
using WoTConsole;

try
{
    Game.Instance.Start();
}
catch(Exception ex)
{
    Console.WriteLine(ex.ToString());
    Console.ReadLine();
}