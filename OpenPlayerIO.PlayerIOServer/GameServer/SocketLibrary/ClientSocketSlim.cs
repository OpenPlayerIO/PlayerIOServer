using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using SocketSlim.ChannelWrapper;
using SocketSlim.Client;

namespace SocketSlim
{
    /// <summary> Socket which returns <see cref="ISocketChannel"/> on successful connection. </summary>
    public class ClientSocketSlim : SocketSlimBase<ChannelState>, IClientSocketSlim
    {
        public const int DefaultBufferSize = 8192;

        private IPAddress[] ipAddresses;
        private string host;

        private readonly AddressFamily? restrictedAddressFamily;

        private readonly TaskClientConnector connector;

        private readonly int receiveBufferSize;
        private readonly int sendBufferSize;

        private byte[] receiveBuffer;
        private byte[] sendBuffer;
        private SocketAsyncEventArgs receiver;
        private SocketAsyncEventArgs sender;
        private DirectBytesReceivedEventArgs receiverArgs;
        private MemoryStream senderWriter;

        private int state;

        public ClientSocketSlim(AddressFamily? restrictedAddressFamily = AddressFamily.InterNetwork)
            : this(false, restrictedAddressFamily)
        { }

        public ClientSocketSlim(bool preallocate, AddressFamily? restrictedAddressFamily = AddressFamily.InterNetwork)
            : this(DefaultBufferSize, preallocate, restrictedAddressFamily)
        { }

        public ClientSocketSlim(int bufferSize, bool preallocate, AddressFamily? restrictedAddressFamily = AddressFamily.InterNetwork)
            : this(bufferSize, bufferSize, preallocate, restrictedAddressFamily)
        { }

        public ClientSocketSlim(int receiveBufferSize, int sendBufferSize, bool preallocate, AddressFamily? restrictedAddressFamily = AddressFamily.InterNetwork)
        {
            this.receiveBufferSize = receiveBufferSize;
            this.sendBufferSize = sendBufferSize;
            this.restrictedAddressFamily = restrictedAddressFamily;

            if (preallocate) {
                AllocateCommunicationResources();
            }

            connector = new TaskClientConnector(SocketType.Stream, ProtocolType.Tcp, new SocketAsyncEventArgs());
            connector.Connected += OnConnectSucceeded;
            connector.Failed += OnConnectFailed;
        }

        protected virtual void OnConnectSucceeded(object o, SocketEventArgs e)
        {
            AllocateCommunicationResources();

            ISocketChannel channel = new ImmutableChannel(e.Socket, receiver, sender, receiverArgs, senderWriter);
            channel.Closed += OnChannelClosed;

            RaiseConnected(channel);
            ChangeState(ChannelState.Connected);

            channel.Start();
        }

        protected virtual void OnChannelClosed(object o, EventArgs e)
        {
            ChangeState(ChannelState.Disconnected);
        }

        protected virtual void OnConnectFailed(object o, ExceptionEventArgs e)
        {
            RaiseError(e);

            ChangeState(ChannelState.Disconnected);
        }

        private void AllocateCommunicationResources()
        {
            if (receiveBuffer != null) {
                return;
            }

            receiveBuffer = new byte[receiveBufferSize];
            sendBuffer = new byte[sendBufferSize];

            receiver = new SocketAsyncEventArgs();
            sender = new SocketAsyncEventArgs();
            receiver.SetBuffer(receiveBuffer, 0, receiveBuffer.Length);
            sender.SetBuffer(sendBuffer, 0, sendBuffer.Length);

            receiverArgs = new DirectBytesReceivedEventArgs(receiver);
            senderWriter = new MemoryStream(sendBuffer, true);
        }

        public ChannelState State
        {
            get { return (ChannelState)state; }
        }

        public string Host
        {
            get { return host; }
            set { host = value; }
        }

        public int Port
        {
            get { return connector.Port; }
            set { connector.Port = value; }
        }

        private void ResolveHostName()
        {
            IPAddress ipFromString; // string with ip address is a special case.
            if (IPAddress.TryParse(host, out ipFromString)) {
                ipAddresses = new[] { ipFromString };

                return;
            }

            ipAddresses = Dns.GetHostEntry(host).AddressList;

            if (restrictedAddressFamily != null) // filter ip addresses
            {
                ipAddresses = ipAddresses.Where(ip => ip.AddressFamily == restrictedAddressFamily.Value).ToArray();
            }
        }

        public void Start()
        {
            if (state != (int)ChannelState.Disconnected) {
                throw new InvalidOperationException("Can't open socket that's not diconnected");
            }

            ChangeState(ChannelState.Connecting);

            try {
                ResolveHostName();

                connector.Address = ipAddresses[0]; // todo: select random IP

                connector.Connect();
            }
            catch {
                ChangeState(ChannelState.Disconnected);
            }
        }

        public virtual void Stop()
        {
            if (State == ChannelState.Disconnected) {
                return;
            }

            ChangeState(ChannelState.Disconnecting);

            if (!connector.StopConnecting()) {
                ChangeState(ChannelState.Disconnected);
            }
        }

        protected void ChangeState(ChannelState newState)
        {
            ChannelState oldState = (ChannelState)Interlocked.Exchange(ref state, (int)newState);

            if (oldState == newState) {
                return;
            }

            RaiseStateChanged(new ChannelStateChangedEventArgs(oldState, newState));
        }
    }
}