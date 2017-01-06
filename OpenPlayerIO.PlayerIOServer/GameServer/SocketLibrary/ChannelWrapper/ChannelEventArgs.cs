using System;

namespace SocketSlim.ChannelWrapper
{
    public class ChannelEventArgs : EventArgs
    {
        private readonly ISocketChannel channel;

        public ChannelEventArgs(ISocketChannel channel)
        {
            this.channel = channel;
        }

        public ISocketChannel Channel
        {
            get { return channel; }
        }
    }
}