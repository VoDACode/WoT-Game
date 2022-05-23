using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoTConsole.Helpers;
using WoTCore.Models;

namespace WoTConsole.Services
{
    public class DisplayInfoZone
    {
        private static DisplayInfoZone _instance;
        public static DisplayInfoZone Instance => _instance ?? (_instance = new DisplayInfoZone());
        private CellModel[,] Field { get; set; }
        private bool openedChat { get; set; } = false;
        private string chatMessage { get; set; } = "";
        private const int chatMessageLimit = 40;
        public Position Marning { get; private set; }
        private List<string> logBlock = new List<string>(10);
        public int Size { get; private set; }



        private DisplayInfoZone()
        {
        }

        public void Init(Position marning, int size)
        {
            Marning = marning;
            Size = size;
            Field = new CellModel[size, Console.WindowHeight];
            for (int x = 0; x < Field.GetLength(0); x++)
                for (int y = 0; y < Field.GetLength(1); y++)
                    Field[x, y].Default();
            NetworkServise.Instance.OnNewMessage += OnNewMessage;
            ControlService.Instance.OnKeyPress += OnKeyPress;
        }

        private bool OnKeyPress(ConsoleKeyInfo key)
        {
            if (!openedChat && key.Key == ConsoleKey.T)
            {
                openChat();
                DisplayService.Instance.AddToQueue(Tick);
                return false;
            }
            if (!openedChat)
                return true;
            if (key.Key == ConsoleKey.Escape)
            {
                closeChat();
                return false;
            }
            else if (key.Key == ConsoleKey.Enter)
            {
                NetworkServise.Instance.SendMessage(chatMessage);
                closeChat();
                return false;
            }
            else if (key.Key == ConsoleKey.Backspace)
            {
                if (chatMessage.Length > 0)
                    chatMessage = chatMessage.Remove(chatMessage.Length - 1);
                DisplayService.Instance.AddToQueue(Tick);
                return false;
            }
            if(chatMessage.Length >= chatMessageLimit)
                return false;
            if(key.Key == ConsoleKey.Spacebar && chatMessage.Length > 0)
            {
                chatMessage += ' ';
                return false;
            }
            if (!((int)key.Key >= 48 && (int)key.Key <= 90) && !((int)key.Key >= 96 && (int)key.Key <= 105) && !((int)key.Key >= 186 && (int)key.Key <= 226))
                return false;
            chatMessage += key.KeyChar;
            DisplayService.Instance.AddToQueue(Tick);
            return false;
        }

        private void openChat()
        {
            openedChat = true;
            chatMessage = "";
        }

        private void closeChat()
        {
            openedChat = false;
            chatMessage = "";
            DisplayService.Instance.AddToQueue(Tick);
        }

        private void OnNewMessage(string obj)
        {
            if (logBlock.Count >= 5)
                logBlock.Remove(logBlock.First());
            logBlock.Add(obj);
            DisplayService.Instance.AddToQueue(Tick);
        }

        public void Tick()
        {
            if (Game.Instance.Player.Command >= 0)
            {
                CreateMenuHelper.ProgressBar(Field, 0, Game.Instance.Player.MaxLife, Game.Instance.Player.Life, color: ConsoleColor.DarkGreen, title: "HP");
                writeChat();
            }
            if (NetworkServise.Instance.IsConnected)
                writePing();
            drawArr(false);
        }

        private void writePing()
        {
            string text = $"Ping: {NetworkServise.Instance.Ping.Milliseconds} ms";
            for (int x = 0; x < Field.GetLength(0); x++)
                Field[x, 5].Default();
            for (int p = 0; p < text.Length; p++)
                Field[p, 5].Icon = text[p];
        }
        private void writeChat()
        {
            for (int x = 0; x < Field.GetLength(0); x++)
                for (int y = 5; y < Field.GetLength(1); y++)
                    Field[x, y].Default();
            int lastRow = Field.GetLength(1) - 5;
            for (int i = logBlock.Count - 1; i >= 0; i--)
            {
                string item = logBlock[i];
                int pointer = 0;
                int pointerInField = 0;
                int rowLen = rowCounter(item);
                lastRow -= rowLen;
                while (pointer < item.Length)
                {
                    if (pointerInField >= Field.GetLength(0))
                    {
                        pointerInField = 0;
                        lastRow++;
                    }
                    Field[pointerInField, lastRow].Icon = item[pointer];
                    pointer++;
                    pointerInField++;
                }
                lastRow = lastRow - rowLen - 1;
                for (int x = 0; x < Field.GetLength(0); x++)
                    Field[x, lastRow].Icon = '-';
                lastRow--;
            }

            // write chat text
            {
                lastRow = Field.GetLength(1) - 4;
                string txt = $"{chatMessage.Length}/{chatMessageLimit}: ";
                for (int x = 0; x < Field.GetLength(0); x++)
                {
                    Field[x, lastRow].Icon = '-';
                    Field[x, lastRow + 3].Icon = '-';
                }
                for (int x = 0; x < txt.Length; x++)
                {
                    Field[x, lastRow + 1].Icon = txt[x];
                    if (openedChat)
                    {
                        Field[x, lastRow + 1].BackgroundColor = ConsoleColor.White;
                        Field[x, lastRow + 1].ForegroundColor = ConsoleColor.Black;
                    }
                }
                for(int p = 0, x = txt.Length, r = 1; p < chatMessage.Length; p++, x++)
                {
                    if(x >= Field.GetLength(0))
                    {
                        x = 0;
                        r++;
                    }
                    Field[x, lastRow + r].Icon = chatMessage[p];
                }

            }

            int rowCounter(string text)
            {
                int counter = -1;
                int len = text.Length;
                while (len > 0)
                {
                    len = len / Field.GetLength(0);
                    counter++;
                }
                return counter;
            }
        }
        private void drawArr(bool ignoreSpace = true)
        {
            for (int y = 0; y < Field.GetLength(1); y++)
            {
                Console.SetCursorPosition(Marning.X, y);
                Console.Write('|');
            }
            for (int x = 0; x < Field.GetLength(0); x++)
            {
                for (int y = 0; y < Field.GetLength(1); y++)
                {
                    if (Field[x, y].Icon == ' ' && ignoreSpace)
                        continue;
                    var pos = new Position(x, y);
                    if (Field[x, y].Icon != ' ')
                    {
                        Console.BackgroundColor = Field[x, y].BackgroundColor;
                        Console.ForegroundColor = Field[x, y].ForegroundColor;
                    }
                    Console.SetCursorPosition(x + Marning.X + 1, y + Marning.Y);
                    Console.Write(Field[x, y].Icon);
                    if (Field[x, y].Icon != ' ')
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
            }
        }
    }
}
