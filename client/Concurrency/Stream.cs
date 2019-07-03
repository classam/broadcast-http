using System;
using System.Collections.Concurrent;
using System.Text;

namespace broadcast.Concurrency
{
    public class Stream<T>:IStream<T, T>
    {
        private readonly Guid streamId;
        private readonly ConcurrentQueue<T> _queue;
        private readonly ConcurrentQueue<Exception> _error;
        private readonly ConcurrentQueue<string> _log;
        private const int QUEUE_FULL_AT = 1000;

        public Stream()
        {
            streamId = Guid.NewGuid();
            _queue = new ConcurrentQueue<T>();
            _error = new ConcurrentQueue<Exception>();
            _log = new ConcurrentQueue<string>();
        }

        public void In(T input)
        {
            _queue.Enqueue(input);
        }

        public void ErrIn(Exception error)
        {
            _error.Enqueue(error);
        }

        public void LogIn(string log)
        {
            _log.Enqueue(log);
        }

        public T Out()
        {
            if (!_queue.TryDequeue(out var returnVal))
            {
                return default(T);
            }

            return returnVal;
        }

        public Exception ErrOut()
        {
            if (!_error.TryDequeue(out var returnVal))
            {
                return null;
            }
            return returnVal;
        }

        public string LogOut()
        {
            if (!_log.TryDequeue(out var returnVal))
            {
                return null;
            }
            return returnVal;
        }

        public bool IsEmpty()
        {
            return _queue.IsEmpty;
        }

        public bool ErrIsEmpty()
        {
            return _error.IsEmpty;
        }

        public bool LogIsEmpty()
        {
            return _log.IsEmpty;
        }

        public bool IsFull()
        {
            return (_queue.Count >= QUEUE_FULL_AT);
        }

        public bool ErrIsFull()
        {
            return (_error.Count == QUEUE_FULL_AT);
        }

        public bool LogIsFull()
        {
            return (_log.Count >= QUEUE_FULL_AT);
        }

        public override string ToString()
        {
            return "<Stream " + streamId + " " + "Q:" + _queue.Count + " E:" + _error.Count + " L:" + _log.Count + " >";

        }

        public string DumpState()
        {
            var str = new StringBuilder();
            var items = _queue.ToArray();
            var errors = _error.ToArray();
            var logs = _log.ToArray();

            if (items.Length == 0 && errors.Length == 0)
            {
                return "[Stream: Empty]";
            }

            if (items.Length > 0)
            {
                str.Append("Items: ");
                foreach (var item in items)
                {
                    str.Append( "\t " + item.ToString() + ", ");
                }
            }

            if (errors.Length > 0)
            {
                str.Append("Errors: ");
                foreach (var error in errors)
                {
                    str.Append("\t " + error.Message + ", ");
                }
            }
            
            if (logs.Length > 0)
            {
                str.Append("Logs: ");
                foreach (var log in logs)
                {
                    str.Append("\t " + log + ", ");
                }
            }

            return "[Stream: " + str.ToString() + "]";

        }
    }
}