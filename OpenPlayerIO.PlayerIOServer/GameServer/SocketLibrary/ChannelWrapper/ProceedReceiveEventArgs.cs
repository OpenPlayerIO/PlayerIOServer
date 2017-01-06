using System;

namespace SocketSlim.ChannelWrapper
{
    public class ProceedReceiveEventArgs : EventArgs
    {
        public bool Closed { get; set; }
    }
}