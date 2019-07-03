using System;
using System.Threading;

namespace broadcast.Concurrency
{
    internal class SupervisedThread
    {
        private readonly ThreadStart _thingToRun;
        private Thread _supervisedThread;
        private Thread _supervisorThread;
        private const int SUPERVISOR_INTERVAL = 25;
        
        public SupervisedThread(ThreadStart thingToRun)
        {
            _thingToRun = thingToRun;
        }

        private void Supervised()
        {
            _supervisedThread = new Thread(_thingToRun);
            _supervisedThread.IsBackground = true;
            
            _supervisedThread.Start();

            while (true)
            {
                if (_supervisedThread.IsAlive)
                {
                    Thread.Sleep(SUPERVISOR_INTERVAL);
                }
                else
                {
                    Console.WriteLine("Restarting thread!");
                    _supervisedThread = new Thread(_thingToRun);
                    _supervisedThread.IsBackground = true;
                    
                    _supervisedThread.Start();
                }
            }
        }

        public void Start()
        {
            _supervisorThread = new Thread(Supervised);
            _supervisorThread.IsBackground = true;
            
            _supervisorThread.Start();
        }
    }
}