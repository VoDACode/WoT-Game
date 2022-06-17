using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoTConsole.Models;
using WoTConsole.Options;
using WoTConsole.Services;
using WoTCore.Models;

namespace WoTConsole.Menu
{
    public abstract class BaseMenu
    {
        private short _minMenuLen = 1000;
        private MenuOptions deffaultOptions = new MenuOptions();
        private bool isUpdate = true;
        private bool isEnd = false;
        private bool _isStopUpdate = false;
        private bool _isInit = false;
        public bool IsUpdate => isUpdate;
        private CellModel[,] EditStringField { get; set; }
        public abstract string Name { get; }
        protected int Size { get; }
        protected CellModel[,] Field { get; set; }
        protected virtual MenuOptions Options => deffaultOptions;
        protected virtual MenuResponse Response { get; set; }
        protected MenuRequest Request { get; set; }
        protected Position VirtualPointerPosition { get; set; } = new Position();
        protected Position PointerPosition { get; set; } = new Position();
        protected Position StartPointerPosition { get; set; } = new Position();
        protected virtual Position Magin { get; } = new Position();
        protected virtual List<MenuComponentModel> MenuComponents { get; } = new List<MenuComponentModel>();
        protected virtual Dictionary<string, string> MenuPatterns { get; } = new Dictionary<string, string>();
        protected virtual void EventUpKey()
        {
            if (Options.ShowMenu)
            {
                Field[PointerPosition.X, PointerPosition.Y].Icon = ' ';
                this.VirtualPointerPosition.Y--;
                this.PointerPosition.Y--;
                if (this.VirtualPointerPosition.Y < 0)
                    this.VirtualPointerPosition.Y = (short)(MenuComponents.Count - 1);
                if (this.PointerPosition.Y < this.StartPointerPosition.Y)
                    this.PointerPosition.Y = (short)(this.StartPointerPosition.Y + MenuComponents.Count - 1);
                Field[PointerPosition.X, PointerPosition.Y].Icon = '>';
            }
            Updata();
        }
        protected virtual void EventDownKey()
        {
            if (Options.ShowMenu)
            {
                Field[PointerPosition.X, PointerPosition.Y].Icon = ' ';
                this.VirtualPointerPosition.Y++;
                this.PointerPosition.Y++;
                if (this.VirtualPointerPosition.Y >= MenuComponents.Count)
                    this.VirtualPointerPosition.Y = 0;
                if (this.PointerPosition.Y >= this.StartPointerPosition.Y + MenuComponents.Count)
                    this.PointerPosition.Y = this.StartPointerPosition.Y;
                Field[PointerPosition.X, PointerPosition.Y].Icon = '>';
            }
            Updata();
        }
        protected virtual void EventLeftKey() => Updata();
        protected virtual void EventRightKey() => Updata();
        protected virtual void EventSpace()
        {
            if (Options.ShowMenu)
            {
                MenuComponents[this.VirtualPointerPosition.Y].Action();
            }
            Updata();
        }
        protected virtual void EventRightClick() => Updata();
        protected virtual void EventLeftClick() => Updata();
        protected virtual void EventEnter() => Updata();
        protected virtual void AfterDraw() { }
        protected virtual void BeforeDraw() { }

        protected event Action<CellModel, Position> OnAfterWriteItem;
        protected event Action<CellModel, Position> OnBeforeWriteItem;
        protected abstract void Build();
        protected abstract void FirstEvent();
        protected abstract void LastEvent();

        public void Select()
        {
            _isInit = true;
            Response = new MenuResponse(null);
        }

        protected void Close()
        {
            isEnd = true;
            _isInit = false;
            Updata();
        }
        public BaseMenu(int size)
        {
            Size = size;
            Field = new CellModel[size, size];
            EditStringField = new CellModel[size, size];
            for (int x = 0; x < size; x++)
                for (int y = 0; y < size; y++)
                {
                    Field[x, y].Default();
                    EditStringField[x, y].Default();
                }
            if (Response == null)
                Response = new MenuResponse(null);
            WriteString(EditStringField, 0, "WoT", isCeneter: true);
            WriteLine(EditStringField, 1);
            Build();
        }

        public void SetRequest(ref MenuRequest data)
        {
            Request = data;
        }

        public MenuResponse Draw()
        {
            if (!isEnd && _isInit)
            {
                FirstEvent();
                _isInit = false;
            }
            if (isUpdate)
            {
                BeforeDraw();
                _showMenu();
                drawArr(false);
                isUpdate = false;
                AfterDraw();
            }
            if (isEnd)
            {
                isEnd = false;
                LastEvent();
                isUpdate = false;
            }
            return Response;
        }

        protected void SetStringMenu(ref string str, bool onlyNum = false)
        {
            Console.Clear();
            _isStopUpdate = true;
            for (int i = 0; i < EditStringField.GetLength(1); i++)
                EditStringField[i, 2].Icon = ' ';
            string text = $"Enter {(onlyNum ? "number" : "string")} ({str}): ";
            WriteString(EditStringField, 2, text, isCeneter: false);
            WriteString(EditStringField, 4, "Clear string and press enter for close this menu.", isCeneter: false);

            drawArr(EditStringField, false);

            Console.SetCursorPosition(text.Length, 2);
            Console.CursorVisible = true;
            string data = Console.ReadLine();
            Console.CursorVisible = false;
            if (data == null)
            {
                _isStopUpdate = false;
                return;
            }
            if (onlyNum)
            {
                int num = 0;
                if (!int.TryParse(data, out num))
                {
                    Console.Clear();
                    Console.Write($"'{data}' isn`t number!");
                    Thread.Sleep(1250);
                    SetStringMenu(ref str, onlyNum);
                    _isStopUpdate = false;
                    return;
                }
            }
            str = data;
            _isStopUpdate = false;
            Updata();
        }

        protected void WriteError(string message) => WriteError(message, TimeSpan.FromSeconds(5));
        protected void WriteError(string message, TimeSpan displayTime)
        {
            _isStopUpdate = true;
            Console.Clear();
            var defaultColor = Console.BackgroundColor;
            Console.BackgroundColor = ConsoleColor.Red;
            Console.Write("ERROR: ");
            Console.BackgroundColor = defaultColor;
            Console.Write(message);
            Thread.Sleep(displayTime);
            _isStopUpdate = false;
            Updata();
        }

        protected void WriteInfo(string message)
        {
            _isStopUpdate = true;
            Console.Clear();
            Console.WriteLine(message);
            Thread.Sleep(2000);
            _isStopUpdate = false;
            Updata();
        }
        protected void WriteInfo(string message, TimeSpan time)
        {
            _isStopUpdate = true;
            Console.Clear();
            Console.Write($"Info: {message}");
            Thread.Sleep(time);
            _isStopUpdate = false;
            Updata();
        }

        protected virtual void Updata()
        {
            isUpdate = !_isStopUpdate;
        }
        private void _showMenu()
        {
            if (!Options.ShowMenu)
                return;
            for (int y = 0; y < MenuComponents.Count; y++)
                for (int x = 0; x < Field.GetLength(1); x++)
                    Field[x, y + Options.Marning.Y].Icon = ' ';
            int lastMin = _minMenuLen;
            short min = short.MaxValue;
            for (short i = 0; i < MenuComponents.Count; i++)
            {
                var text = MenuComponents[i].Text;
                foreach (var p in MenuPatterns)
                    text = text.Replace($"${p.Key}$", p.Value);
                short pos = WriteString((short)(i + Options.Marning.Y), text, isCeneter: Options.Center, setPosX: Options.Marning.X);
                if (pos < min)
                    min = pos;
            }
            if (lastMin != min)
            {
                _minMenuLen = min;
                StartPointerPosition.Y = Options.Marning.Y;
                StartPointerPosition.X = (short)(_minMenuLen - 1);
                PointerPosition.Y = Options.Marning.Y;
                PointerPosition.X = (short)(_minMenuLen - 1);
            }
            Field[_minMenuLen - 1, PointerPosition.Y].Icon = '>';
        }

        protected short WriteString(short row, string text, bool isCeneter = false, short setPosX = 0) =>
            WriteString(Field, row, text, isCeneter, setPosX);
        private short WriteString(CellModel[,] arr, short row, string text, bool isCeneter = false, short setPosX = 0)
        {
            short pos = (short)(isCeneter ? (arr.GetLength(1) - text.Length) / 2 + setPosX : setPosX);
            short startPos = pos;
            for (int i = 0, p = 0; i < text.Length; i++, p++)
            {
                if (p + pos >= arr.GetLength(1))
                {
                    row++;
                    pos = (short)(isCeneter ? (arr.GetLength(1) - text.Length - i) / 2 + setPosX : setPosX);
                    p = 0;
                }
                arr[p + pos, row].Icon = text[i];
            }
            return pos;
        }
        protected void WriteLine(int row) => WriteLine(Field, row);
        private void WriteLine(CellModel[,] arr, int row)
        {
            for (int i = 0; i < arr.GetLength(1); i++)
                arr[i, row].Icon = '-';
        }
        private void drawArr(bool ignoreSpace = true) =>
            drawArr(Field, ignoreSpace);
        private void drawArr(CellModel[,] arr, bool ignoreSpace = true)
        {
            for (short x = 0; x < arr.GetLength(0); x++)
            {
                for (short y = 0; y < arr.GetLength(1); y++)
                {
                    if (arr[x, y].Icon == ' ' && ignoreSpace)
                        continue;
                    var pos = new Position(x, y);
                    Console.BackgroundColor = arr[x, y].BackgroundColor.ToConsoleColor();
                    Console.ForegroundColor = arr[x, y].ForegroundColor.ToConsoleColor();
                    OnBeforeWriteItem?.Invoke(arr[x, y], pos);
                    Console.SetCursorPosition(x + Magin.X, y + Magin.Y);
                    Console.Write(arr[x, y].Icon);
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                    OnAfterWriteItem?.Invoke(arr[x, y], pos);
                }
            }
        }
    }
}
