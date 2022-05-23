using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoTConsole.Services
{
    abstract class Service
    {
        protected bool _isStart = false;
        protected Thread? thread;
        public bool IsStart => _isStart;

        public virtual void Stop()
        {
            _isStart = false;
            thread?.Join();
        }
        public virtual void Start()
        {
            if (IsStart)
                return;
            _isStart = true;
            thread = new Thread(new ThreadStart(Execute));
            thread.Start();
        }

        protected abstract void Execute();
    }
}
