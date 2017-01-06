using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using SocketSlim.Client;

namespace SocketSlim.ChannelWrapper
{
    /// <summary>
    /// Channel that's bound to a single open <see cref="Socket"/> used for data exchange.
    /// </summary>
    public class ImmutableChannel : ISocketChannel
    {
        // todo: provide data about local/remote endpoints for the channel

        private readonly ChannelWrapperBase wrapper;
        private readonly Socket channelSocket;

        private readonly object additionalData;

        public ImmutableChannel(Socket channelSocket, SocketAsyncEventArgs receiver, SocketAsyncEventArgs sender, DirectBytesReceivedEventArgs receiverArgs, MemoryStream senderWriter, object additionalData = null)
        {
            this.channelSocket = channelSocket;

            wrapper = new ChannelWrapperBase(channelSocket, receiver, receiverArgs, sender, senderWriter);
            wrapper.BytesReceived += OnBytesReceived;
            wrapper.Closed += OnChannelClosed;
            wrapper.DuplexChannelClosed += OnChannelError;

            this.additionalData = additionalData;
        }

        /// <summary>
        /// Call this after you've subscribed to all event so that you don't miss first events.
        /// </summary>
        public void Start()
        {
            wrapper.Start();
        }

        protected virtual void OnBytesReceived(object o, BytesReceivedEventArgs e)
        {
            byte[] msg = new byte[e.Size];

            Buffer.BlockCopy(e.Buffer, e.Offset, msg, 0, e.Size);

            RaiseBytesReceived(msg);

            e.Proceed();
        }

        protected virtual void OnChannelClosed(object o, EventArgs e)
        {
            RaiseClosed();
        }

        protected virtual void OnChannelError(object o, ChannelCloseEventArgs e)
        {
            if (e.Exception != null) {
                RaiseError(e.Exception);
            } else if (e.SocketError != null) {
                RaiseError(new SocketErrorException(e.SocketError.Value));
            }

            // when both are null, the socket is closed normally
        }

        public EndPoint RemoteEndPoint
        {
            get { return channelSocket.RemoteEndPoint; }
        }

        public EndPoint LocalEndPoint
        {
            get { return channelSocket.LocalEndPoint; }
        }

        public void Send(byte[] bytes)
        {
            wrapper.Send(bytes);
        }

        public object Tag { get; set; }

        public object AdditionalData
        {
            get { return additionalData; }
        }

        public event ChannelMessageHandler<ISocketChannel> BytesReceived;

        protected void RaiseBytesReceived(byte[] message)
        {
            BytesReceived?.Invoke(this, message);
        }

        public event EventHandler Closed;

        protected virtual void RaiseClosed()
        {
            Closed?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler<ExceptionEventArgs> Error;

        protected void RaiseError(Exception e)
        {
            Error?.Invoke(this, new ExceptionEventArgs(e));
        }

        public void Close()
        {
            wrapper.Close();
        }

        public IChannelSettingsAndStats SettingsAndStats
        {
            get { return wrapper; }
        }
    }
}