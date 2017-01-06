using System;
using System.Net.Sockets;

namespace SocketSlim.ChannelWrapper
{
    /// <summary>
    /// <see cref="BytesReceivedEventArgs"/> implementation that gives event handler a direct access
    /// to the receive buffer. This makes calling receive events much more efficient, but places a
    /// burden on the event receiver to either read data in place or copy it himself, but in the same
    /// time allows receiver implementation to skip unnecessary memory copying.
    /// </summary>
    public class DirectBytesReceivedEventArgs : BytesReceivedEventArgs
    {
        private readonly SocketAsyncEventArgs receiver;
        private readonly ProceedReceiveEventArgs args = new ProceedReceiveEventArgs();

        public DirectBytesReceivedEventArgs(SocketAsyncEventArgs receiver)
        {
            this.receiver = receiver;
        }

        public override byte[] Buffer
        {
            get { return receiver.Buffer; }
        }

        public override int Offset
        {
            get { return receiver.Offset; }
        }

        public override int Size
        {
            get { return receiver.BytesTransferred; }
        }

        public override bool Proceed()
        {
            args.Closed = false;
            RaiseProceedReceive();
            return args.Closed;
        }

        public event EventHandler<ProceedReceiveEventArgs> ProceedReceive;

        public void RaiseProceedReceive()
        {
            EventHandler<ProceedReceiveEventArgs> handler = ProceedReceive;
            if (handler != null) {
                handler(this, args);
            }
        }
    }
}