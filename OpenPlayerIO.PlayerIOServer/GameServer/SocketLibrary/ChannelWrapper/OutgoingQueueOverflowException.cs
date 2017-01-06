using System;

namespace SocketSlim.ChannelWrapper
{
    public class OutgoingQueueOverflowException : Exception
    {
        private readonly int maxLimit;

        public OutgoingQueueOverflowException(int maxLimit)
        {
            this.maxLimit = maxLimit;
        }

        public int MaxLimit
        {
            get { return maxLimit; }
        }
    }
}