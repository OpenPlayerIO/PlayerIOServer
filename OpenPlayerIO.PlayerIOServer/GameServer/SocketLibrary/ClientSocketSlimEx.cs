using System;
using System.Net.Sockets;
using SocketSlim.ChannelWrapper;
using SocketSlim.Client;

namespace SocketSlim
{
    /// <summary>
    /// <see cref="ClientSocketSlim"/> which also allows you to send data through the same object
    /// once it has connected.
    /// </summary>
    public class ClientSocketSlimEx : ClientSocketSlim, IClientSocketSlimEx
    {
        private ISocketChannel channel;

        public ClientSocketSlimEx(AddressFamily? restrictedAddressFamily) : base(restrictedAddressFamily)
        {
        }

        public ClientSocketSlimEx(bool preallocate, AddressFamily? restrictedAddressFamily) : base(preallocate, restrictedAddressFamily)
        {
        }

        public ClientSocketSlimEx(int bufferSize, bool preallocate, AddressFamily? restrictedAddressFamily) : base(bufferSize, preallocate, restrictedAddressFamily)
        {
        }

        public ClientSocketSlimEx(int receiveBufferSize, int sendBufferSize, bool preallocate, AddressFamily? restrictedAddressFamily) : base(receiveBufferSize, sendBufferSize, preallocate, restrictedAddressFamily)
        {
        }

        protected override void OnChannelClosed(object o, EventArgs e)
        {
            channel = null;

            base.OnChannelClosed(o, e);
        }

        public override void Stop()
        {
            if (channel != null) {
                ChangeState(ChannelState.Disconnecting);
                channel.Close();
            } else {
                base.Stop();
            }
        }

        public void Send(byte[] bytes)
        {
            if (State != ChannelState.Connected || channel == null) {
                throw new InvalidOperationException("Can't send data when the socket is not open");
            }

            channel.Send(bytes);
        }

        protected override void RaiseConnected(ISocketChannel channel)
        {
            base.RaiseConnected(channel);

            channel.BytesReceived += OnChannelBytesReceived;
            channel.Error += OnChannelError;

            this.channel = channel;
        }

        private void OnChannelError(object o, ExceptionEventArgs e)
        {
            RaiseError(e);
        }

        private void OnChannelBytesReceived(ISocketChannel socket, byte[] message)
        {
            RaiseBytesReceived(message);
        }

        public event ChannelMessageHandler<ClientSocketSlim> BytesReceived;

        protected void RaiseBytesReceived(byte[] message)
        {
            ChannelMessageHandler<ClientSocketSlim> handler = BytesReceived;
            if (handler != null) {
                handler(this, message);
            }
        }
    }
}