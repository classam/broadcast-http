using System;

namespace broadcast.Errors
{
    public class BroadcastError: Exception
    {
        public BroadcastError()
        {
        }

        public BroadcastError(string message) : base(message)
        {
        }
    }
}