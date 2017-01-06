using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using SocketSlim.Client;

namespace SocketSlim.Server
{
    public class ServerAcceptor
    {
        private readonly SocketType socketType;
        private readonly ProtocolType protocolType;
        private readonly ISocketAsyncEventArgsPool acceptorPool;
        private readonly Func<SocketAsyncEventArgs> acceptorFactory;

        private Socket socket;
        private bool acceptStopped;

        private IMaxConnectionsEnforcer maxConnectionsEnforcer;
        private int maxPendingConnections = 100;
        private int maxSimultaneousConnections = -1;

        public ServerAcceptor(SocketType socketType, ProtocolType protocolType, ISocketAsyncEventArgsPool acceptorPool, Func<SocketAsyncEventArgs> acceptorFactory)
        {
            this.socketType = socketType;
            this.protocolType = protocolType;
            this.acceptorPool = acceptorPool;
            this.acceptorFactory = acceptorFactory;
        }

        // set properties below before calling Start.

        public IPAddress ListenAddress { get; set; }

        public int ListenPort { get; set; }

        public int MaxPendingConnections
        {
            get { return maxPendingConnections; }
            set { maxPendingConnections = value; }
        }

        public int MaxSimultaneousConnections
        {
            get { return maxSimultaneousConnections; }
            set { maxSimultaneousConnections = value; }
        }

        /// <summary>
        /// Gets or sets whether the socket should listen both on IPv6 and IPv4 when the listen
        /// address is an IPv6 address.
        /// </summary>
        public bool Ipv6Only { get; set; }

        /// <summary>
        /// Call this method when the socket we've sent you through <see cref="Accepted"/> event got closed.
        /// </summary>
        public void ReleaseOpenConnectionSlot()
        {
            maxConnectionsEnforcer.ReleaseOne();
        }

        public void Start()
        {
            // reset the stopping flag
            acceptStopped = false;

            // create connection count limiter
            maxConnectionsEnforcer = maxSimultaneousConnections < 0
                ? (IMaxConnectionsEnforcer)new NoMaxConnectionEnforcer()
                : new MaxConnectionsEnforcer(MaxSimultaneousConnections);

            // create listen socket
            socket = new Socket(ListenAddress.AddressFamily, socketType, protocolType);

#if !__MonoCS__
            if (ListenAddress.AddressFamily == AddressFamily.InterNetworkV6) {
                socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, Ipv6Only ? 1 : 0);
            }
#endif

            // bind it
            socket.Bind(new IPEndPoint(ListenAddress, ListenPort));

            // start listening
            socket.Listen(MaxPendingConnections);

            // start connection acceptin'
            StartAccept();
        }

        /// <summary>
        /// Stops server socket from accepting new connections and frees the port it's listening on.
        ///
        /// Does nothing to the client channels that are already opened.
        /// </summary>
        public virtual void Stop()
        {
            // signal that we're stopping
            if (acceptStopped) // if someone already stopping the socket, then we're doing nothing
            {
                return;
            }

            acceptStopped = true;

            // then close it
            socket.Close();
        }

        private void StartAccept()
        {
            if (acceptStopped) // break the loop, when socket is stopping
            {
                return;
            }

            // get new acceptor from pool quickly before dealing with the old one
            SocketAsyncEventArgs currentAcceptor;
            if (!acceptorPool.TryTake(out currentAcceptor)) {
                currentAcceptor = acceptorFactory();
            }

            currentAcceptor.Completed += OnAcceptorCompleted;

            // don't start accepting new connection until the number of simultaneous connections
            // falls below the set limit
            Task slotAvailable = maxConnectionsEnforcer.TakeOne();  // asynchronous pattern allows us to free the thread from waiting for connection count to go down
            if (slotAvailable == null || slotAvailable.IsCompleted) // and also to avoid blocking connection accepting logic which launches StartAccept before processing connected socket.
            {
                ContinueAccept(currentAcceptor);
            } else {
                slotAvailable.ContinueWith(t => ContinueAccept(currentAcceptor), TaskContinuationOptions.ExecuteSynchronously);
            }
        }

        private void ContinueAccept(SocketAsyncEventArgs currentAcceptor)
        {
            bool callbackPending;
            try {
                callbackPending = socket.AcceptAsync(currentAcceptor);
            }
            catch (ObjectDisposedException) {
                // either acceptor is disposed/corrupted, or socket is closed
                StartAccept(); // possible stack overflow if too many accepts end up bad

                HandleBadAcceptor(currentAcceptor);
                return;
            }

            if (!callbackPending) {
                ProcessAccept(currentAcceptor);
            }
        }

        private void OnAcceptorCompleted(object sender, SocketAsyncEventArgs e)
        {
            ProcessAccept(e);
        }

        private void ProcessAccept(SocketAsyncEventArgs acceptor)
        {
            // quickly start accepting new connections
            StartAccept();

            // then deal with this one

            // check if there's any error
            if (acceptor.SocketError != SocketError.Success) {
                Fail(new SocketErrorException(acceptor.SocketError));

                HandleBadAcceptor(acceptor);
                return;
            }

            // maybe listening socket was closed
            if (acceptor.AcceptSocket == null) {
                HandleBadAcceptor(acceptor);
                return;
            }

            Success(acceptor.AcceptSocket);
        }

        private void HandleBadAcceptor(SocketAsyncEventArgs acceptor)
        {
            // release connection slot
            maxConnectionsEnforcer.ReleaseOne();

            //This method closes the socket and releases all resources, both
            //managed and unmanaged. It internally calls Dispose.
            if (acceptor.AcceptSocket != null) {
                acceptor.AcceptSocket.Close();
            }

            // acceptor may be damaged if ConnectionReset, we will destroy it and add new one
            acceptorPool.Put(acceptor.SocketError == SocketError.ConnectionReset ? acceptorFactory() : acceptor);
        }

        private void Success(Socket s)
        {
            RaiseAccepted(s);
        }

        private void Fail(Exception e)
        {
            RaiseFailed(e);
        }

        public event EventHandler<SocketEventArgs> Accepted;

        protected virtual void RaiseAccepted(Socket s)
        {
            Accepted?.Invoke(this, new SocketEventArgs(s));
        }

        public event EventHandler<ExceptionEventArgs> AcceptFailed;

        protected virtual void RaiseFailed(Exception e)
        {
            AcceptFailed?.Invoke(this, new ExceptionEventArgs(e));
        }
    }
}