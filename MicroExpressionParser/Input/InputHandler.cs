using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGEngine.Input
{
    using System.Collections.Concurrent;
    using System.Threading;

    using RPGEngine.Discord;

    public class InputHandler
    {
        const int SLEEP_TIME = 1000;
        private Thread workerThread;

        private ConcurrentQueue<Command> messageQueue;

        private bool running;

        public InputHandler()
        {
            messageQueue = new ConcurrentQueue<Command>();
            workerThread=new Thread(Run);
            running = false;
        }

        public void AddCommand(Command command)
        {
            messageQueue.Enqueue(command);
        }
        public void Start()
        {
            running = true;
            workerThread.Start();
        }

        public void Stop()
        {
            running = false;

        }

        private void Run()
        {
            while (running)
            {
                if (!messageQueue.IsEmpty)
                {
                    Command command;
                    bool result = messageQueue.TryDequeue(out command);
                    if (result)
                    {
                        Console.WriteLine(command);
                    }
                }
                Thread.Sleep(SLEEP_TIME);
            }
        }

    }
}
