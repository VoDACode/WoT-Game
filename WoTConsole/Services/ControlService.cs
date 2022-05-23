using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoTConsole.Services
{ 
    delegate bool PressKey(ConsoleKeyInfo key);
    class ControlService: Service
    {
        private static ControlService? instance;
        public static ControlService Instance { get
            {
                if (instance == null)
                    instance = new ControlService();
                return instance;
            } 
        }

        public event Action OnLeft;
        public event Action OnRight;
        public event Action OnTop;
        public event Action OnBottom;
        public event Action OnClickButtonLeft;
        public event Action OnClickButtonRight;
        public event Action OnShot;
        public event Action OnSelect;
        public event PressKey OnKeyPress;

        private ControlService() { }

        protected override void Execute()
        {
            while (IsStart)
            {
                var key = Console.ReadKey(true);
                var res = OnKeyPress?.Invoke(key);
                if (res == false)
                    continue;
                if(key.Key == ConsoleKey.A)
                {
                    OnLeft?.Invoke();
                }
                else if(key.Key == ConsoleKey.D)
                {
                    OnRight?.Invoke();
                }
                else if(key.Key == ConsoleKey.W)
                {
                    OnTop?.Invoke();
                }
                else if(key.Key == ConsoleKey.S)
                {
                    OnBottom?.Invoke();
                }
                else if(key.Key == ConsoleKey.Spacebar)
                {
                    OnShot?.Invoke();
                    OnSelect?.Invoke();
                }
                else if(key.Key == ConsoleKey.Enter)
                {
                    OnSelect?.Invoke();
                }
            }
        }
    }
}
