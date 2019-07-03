using System;

namespace broadcast.Events
{
    internal class BroadcastEvent<T> : EventArgs
    {
        public BroadcastEvent(T msg)
        {
            this.msg = msg;
        }

        public T msg { get; set; }
    }
}