using System;
using System.Threading;
using System.Threading.Tasks;

namespace broadcast.Concurrency
{
    public abstract class Actor<T1, T2>: IStream<T1, T2>
    {
        private readonly Stream<T1> _input;
        private readonly Stream<T2> _output;
        private SupervisedThread _pipelineThread;
        private SupervisedThread _errorPipelineThread;
        private SupervisedThread _logPipelineThread;
        protected const int BACKPRESSURE_INTERVAL = 10;
        
        public Actor()
        {
            _input = new Stream<T1>();
            _output = new Stream<T2>();

            Go();
        }

        public void LogStreams()
        {
            Console.WriteLine("Input:" + _input.ToString());
            Console.WriteLine("Output: " + _output.ToString());
        }
        
        public void In(T1 input)
        {
            _input.In(input);
        }

        public void ErrIn(Exception error)
        {
            _input.ErrIn(error);
        }

        public void LogIn(string log)
        {
            _input.LogIn(log);
        }

        public T2 Out()
        {
            return _output.Out();
        }

        public Exception ErrOut()
        {
            return _output.ErrOut();
        }

        public string LogOut()
        {
            return _output.LogOut();
        }

        public bool IsEmpty()
        {
            return _output.IsEmpty();
        }

        public bool ErrIsEmpty()
        {
            return _output.ErrIsEmpty();
        }

        public bool LogIsEmpty()
        {
            return _output.LogIsEmpty();
        }

        public bool IsFull()
        {
            return _input.IsFull();
        }

        public bool ErrIsFull()
        {
            return _input.ErrIsFull();
        }

        public bool LogIsFull()
        {
            return _input.LogIsFull();
        }

        protected virtual async Task<T2> DoSomethingToItem(T1 a)
        {
            var o = (object) a;
            return (T2) o;
        }
        
        protected virtual async void ProcessItem(T1 a)
        {
            if (a == null)
            {
                return;
            }

            var value = await DoSomethingToItem(a);
            if (value != null)
            {
                _output.In(value); 
            }
        }

        protected virtual Exception DoSomethingToError(Exception error)
        {
            return error;
        }

        protected virtual void ProcessErrorItem(Exception error)
        {
            if (error == null)
            {
                return;
            }
            _output.ErrIn(DoSomethingToError(error));
        }

        protected virtual string DoSomethingToLog(string log)
        {
            return log;
        }

        protected virtual void ProcessLogItem(string log)
        {
            if (String.IsNullOrEmpty(log))
            {
                return;
            }
            _output.LogIn(DoSomethingToLog(log));
        }

        protected virtual void ProcessPipe()
        {
            try
            {
                while(true)
                {
                    if (_input.IsEmpty())
                    {
                        Thread.Sleep(BACKPRESSURE_INTERVAL);
                    }
                    else if (_output.IsFull())
                    {
                        Thread.Sleep(BACKPRESSURE_INTERVAL);
                    }
                    else
                    {
                        var item = _input.Out();
                        if (item != null)
                        {
                            ProcessItem(item);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _output.ErrIn(e);
            }
        }

        protected virtual void ProcessError()
        {
            try
            {
                while(true)
                {
                    if (_input.ErrIsEmpty())
                    {
                        Thread.Sleep(BACKPRESSURE_INTERVAL);
                    }
                    if (_output.ErrIsFull())
                    {
                        Thread.Sleep(BACKPRESSURE_INTERVAL);
                    }
                    else
                    {
                        var err = _input.ErrOut();
                        if (err != null)
                        {
                            ProcessErrorItem(err);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                _output.ErrIn(e);
            }
            
        }
        
        protected virtual void ProcessLog()
        {
            try
            {
                while(true)
                {
                    if (_input.LogIsEmpty())
                    {
                        Thread.Sleep(BACKPRESSURE_INTERVAL);
                    }
                    if (_output.LogIsFull())
                    {
                        Thread.Sleep(BACKPRESSURE_INTERVAL);
                    }
                    else
                    {
                        var log = _input.LogOut();
                        if (!String.IsNullOrEmpty(log))
                        {
                            ProcessLogItem(log);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                _output.ErrIn(e);
            }
            
        }

        private void Go()
        {
            _pipelineThread = new SupervisedThread(ProcessPipe);
            _pipelineThread.Start();
            
            _errorPipelineThread = new SupervisedThread(ProcessError);
            _errorPipelineThread.Start();
            
            _logPipelineThread = new SupervisedThread(ProcessLog);
            _logPipelineThread.Start();
        }
    }
}