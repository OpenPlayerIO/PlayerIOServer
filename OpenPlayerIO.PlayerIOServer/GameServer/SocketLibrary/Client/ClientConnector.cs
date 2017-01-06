using System;
using System.Net;
using System.Net.Sockets;

namespace SocketSlim.Client
{
    /// <summary>
    /// This class implements asynchronous client connection.
    ///
    /// Typical workflow with this class should be as follows:
    /// * Create an object with the constructor.
    /// * Fill in <see cref="Address"/> and <see cref="Port"/> and subscribe to <see
    ///   cref="Connected"/> and <see cref="Failed"/> events.
    /// * Call the <see cref="Connect"/> method to initiate connection.
    /// * You can interrupt connection operation anytime by calling <see cref="StopConnecting"/> method.
    /// * If connection has succeded, the <see cref="Connected"/> event will be raised with an opened
    ///   data socket as an argument.
    /// * If connection failed, the <see cref="Failed"/> event willl be raised with the reason for
    ///   the failure.
    /// * After either success or failure, this object can be reused to open another connections to
    ///   different hosts by changing <see cref="Address"/> and <see cref="Port"/> properties and
    ///   calling <see cref="Connect"/> method again.
    ///
    /// This class represents a low level building block for the socket. Feel free to use it in your
    /// own designs, but avoid using it directly in the user code.
    /// </summary>
    public class ClientConnector : IDisposable
    {
        private readonly SocketType socketType;
        private readonly ProtocolType protocolType;
        private readonly SocketAsyncEventArgs connector;

        private Socket socket;

        /// <summary> Creates client connector. </summary>
        public ClientConnector(SocketType socketType, ProtocolType protocolType, SocketAsyncEventArgs connector)
        {
            this.socketType = socketType;
            this.protocolType = protocolType;
            this.connector = connector;

            connector.Completed += OnConnectCompleted;
        }

        /// <summary> Gets or sets remote ip address to connect to. </summary>
        public IPAddress Address { get; set; }

        /// <summary> Gets or sets remote port to connect to. </summary>
        public int Port { get; set; }

        /// <summary> Starts connection process. </summary>
        public void Connect()
        {
            if (socket != null) {
                throw new InvalidOperationException("We're already connecting.");
            }

            socket = new Socket(Address.AddressFamily, socketType, protocolType) {
                NoDelay = true
            };

            StartConnect();
        }

        /// <summary>
        /// Interrupts connection process, assuming there's anything to interrupt.
        ///
        /// Doesn't close the data socket if it has already been connected.
        /// </summary>
        /// <returns>
        /// True if the connection process was active and was successfully closed, otherwise false.
        /// </returns>
        public virtual bool StopConnecting()
        {
            Socket s = socket;
            if (s == null) {
                return false;
            }

            socket = null;

            try {
                s.Close();
            }
            catch (Exception e) {
                Fail(e);
                return false;
            }

            return true;
        }

        /// <summary> Starts asynchronous connection process. </summary>
        private void StartConnect()
        {
            connector.RemoteEndPoint = new IPEndPoint(Address, Port);

            bool callbackPending;
            try {
                callbackPending = socket.ConnectAsync(connector);
            }
            catch (Exception e) {
                Fail(e);
                return;
            }

            if (!callbackPending) {
                ProcessConnect();
            }
        }

        private void OnConnectCompleted(object sender, SocketAsyncEventArgs e)
        {
            ProcessConnect();
        }

        /// <summary> Handles async connection results. </summary>
        private void ProcessConnect()
        {
            if (connector.SocketError != SocketError.Success) {
                Fail(new SocketErrorException(connector.SocketError));
                return;
            }

            Success(connector.ConnectSocket);
        }

        private void Success(Socket s)
        {
            socket = null;

            RaiseConnected(s);
        }

        private void Fail(Exception e)
        {
            socket = null;

            RaiseFailed(e);
        }

        public event EventHandler<SocketEventArgs> Connected;

        protected virtual void RaiseConnected(Socket s)
        {
            EventHandler<SocketEventArgs> handler = Connected;
            if (handler != null) {
                handler(this, new SocketEventArgs(s));
            }
        }

        public event EventHandler<ExceptionEventArgs> Failed;

        protected virtual void RaiseFailed(Exception e)
        {
            EventHandler<ExceptionEventArgs> handler = Failed;
            if (handler != null) {
                handler(this, new ExceptionEventArgs(e));
            }
        }

        public void Dispose()
        {
            connector.Completed -= OnConnectCompleted;
        }
    }
}