using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using SocketSlim.ChannelWrapper;
using SocketSlim.Client;
using SocketSlim.Server;
using SocketSlim.Util;

namespace SocketSlim
{
    public class ServerSocketSlim : SocketSlimBase<ServerState>, IServerSocketSlim
    {
        private readonly ServerAcceptor serverAcceptor;

        private readonly IBufferManager bufferManager;
        private readonly ObjectPool<PreallocatedChannelData> channelDataPool = new ObjectPool<PreallocatedChannelData>();

        private int state;

        /// <summary> Creates server socket without preallocated client data. </summary>
        /// [Obsolete("As allocating the send/receive structs is expensive, it shouldn't be done on
        /// the fly.")]
        /// <param name="sendReceiveBufferSize">
        /// size of the both send and receive buffers in bytes
        /// </param>
        /// <param name="acceptorPoolSize">
        /// size of the acceptor structures pool (expensive to allocate on the fly)
        /// </param>
        /// <param name="maxPendingConnections"> sets the size of the listen socket OS backlog </param>
        public ServerSocketSlim(int sendReceiveBufferSize, int acceptorPoolSize = 100, int maxPendingConnections = 100)
            : this(sendReceiveBufferSize, -1, acceptorPoolSize, maxPendingConnections)
        { }

        /// <summary>
        /// Creates server socket with non-contiguous send and receive buffers and preallcates
        /// <paramref name="preallocatedDataCount"/> of each.
        /// </summary>
        /// <param name="sendReceiveBufferSize">
        /// size of the both send and receive buffers in bytes
        /// </param>
        /// <param name="preallocatedDataCount">
        /// number of instances of send/receive resources to preallocate on socket creation
        /// </param>
        /// <param name="acceptorPoolSize">
        /// size of the acceptor structures pool (expensive to allocate on the fly)
        /// </param>
        /// <param name="maxPendingConnections"> sets the size of the listen socket OS backlog </param>
        public ServerSocketSlim(int sendReceiveBufferSize, int preallocatedDataCount, int acceptorPoolSize = 100, int maxPendingConnections = 100)
            : this(new SimpleBufferManager { BufferBytesAllocatedForEachSocket = sendReceiveBufferSize }, -1, preallocatedDataCount, acceptorPoolSize, maxPendingConnections)
        { }

        /// <summary>
        /// Creates server socket with the upper limit on accepted connections and a contiguous
        /// buffer block for send/receive buffers, if specified.
        ///
        /// All the send/receive buffers are allocated on creation of the server socket.
        /// </summary>
        /// <param name="contiguous">
        /// if true, all the buffers for send and receive are allocated as one contiguous byte array.
        /// This prevents memory fragmentation as the socket receive buffers become unmovable on
        /// heap, once the socket is opened. You really want to set this to true, if you're using
        /// this constructor.
        /// </param>
        /// <param name="sendReceiveBufferSize">
        /// size of the both send and receive buffers in bytes
        /// </param>
        /// <param name="maxSimultaneousConnections">
        /// maximum number of accepted client connections at any point in time. The next connection
        /// is accepted when one of the previous ones is closed.
        /// </param>
        /// <param name="acceptorPoolSize">
        /// size of the acceptor structures pool (expensive to allocate on the fly)
        /// </param>
        /// <param name="maxPendingConnections"> sets the size of the listen socket OS backlog </param>
        public ServerSocketSlim(bool contiguous, int sendReceiveBufferSize, int maxSimultaneousConnections, int acceptorPoolSize = 100, int maxPendingConnections = 100)
            : this(contiguous, sendReceiveBufferSize, maxSimultaneousConnections, maxSimultaneousConnections, acceptorPoolSize, maxPendingConnections)
        { }

        /// <summary>
        /// Creates server socket with the upper limit on accepted connections and a contiguous
        /// buffer block for send/receive buffers, if specified.
        ///
        /// All the send/receive buffers are allocated on creation of the server socket.
        ///
        /// This constructor allows to preallocate a larger number of resources that there will be
        /// simultaneous connections available, because some of the resources could be
        /// unrecoverble/in a brief use and so the buffer is welcome.
        /// </summary>
        /// <param name="contiguous">
        /// if true, all the buffers for send and receive are allocated as one contiguous byte array.
        /// This prevents memory fragmentation as the socket receive buffers become unmovable on
        /// heap, once the socket is opened. You really want to set this to true, if you're using
        /// this constructor.
        /// </param>
        /// <param name="sendReceiveBufferSize">
        /// size of the both send and receive buffers in bytes
        /// </param>
        /// <param name="maxSimultaneousConnections">
        /// maximum number of accepted client connections at any point in time. The next connection
        /// is accepted when one of the previous ones is closed.
        /// </param>
        /// <param name="preallocatedDataCount">
        /// number of instances of send/receive resources to preallocate on socket creation
        /// </param>
        /// <param name="acceptorPoolSize">
        /// size of the acceptor structures pool (expensive to allocate on the fly)
        /// </param>
        /// <param name="maxPendingConnections"> sets the size of the listen socket OS backlog </param>
        public ServerSocketSlim(bool contiguous, int sendReceiveBufferSize, int maxSimultaneousConnections, int preallocatedDataCount, int acceptorPoolSize = 100, int maxPendingConnections = 100)
            : this(
                contiguous
                    ? new BigBufferManager { BufferBytesAllocatedForEachSocket = sendReceiveBufferSize, TotalBytesInBufferBlock = sendReceiveBufferSize * preallocatedDataCount * 2 }
                    : (IBufferManager)new SimpleBufferManager { BufferBytesAllocatedForEachSocket = sendReceiveBufferSize },
                maxSimultaneousConnections,
                preallocatedDataCount,
                acceptorPoolSize,
                maxPendingConnections
            )
        { }

        public ServerSocketSlim(IBufferManager bufferManager, int maxSimultaneousConnections, int preallocatedDataCount, int acceptorPoolSize = 100, int maxPendingConnections = 100)
        {
            this.bufferManager = bufferManager;
            // set up acceptor
            ISocketAsyncEventArgsPool acceptorPool = CreateAcceptorPool(acceptorPoolSize);

            serverAcceptor = new ServerAcceptor(SocketType.Stream, ProtocolType.Tcp, acceptorPool, CreateAcceptor) {
                MaxSimultaneousConnections = maxSimultaneousConnections,
                MaxPendingConnections = maxPendingConnections
            };

            serverAcceptor.Accepted += OnServerAcceptorAccepted;
            serverAcceptor.AcceptFailed += OnServerAcceptorAcceptFailed;

            // remove data pool when no preallocation is happening
            if (preallocatedDataCount <= 0) {
                channelDataPool = null;
                return;
            }

            // preallocate data for channels
            List<PreallocatedChannelData> preallocatedChannelData = new List<PreallocatedChannelData>();
            for (int i = 0; i < preallocatedDataCount; i++) {
                preallocatedChannelData.Add(new PreallocatedChannelData());
            }

            // set up receivers and senders
            foreach (PreallocatedChannelData channelData in preallocatedChannelData) {
                channelData.InitReceiver(bufferManager);
            }
            foreach (PreallocatedChannelData channelData in preallocatedChannelData) {
                channelData.InitSender(bufferManager);
            }

            // and add them into the pool
            foreach (PreallocatedChannelData channelData in preallocatedChannelData) {
                channelDataPool.PutObject(channelData);
            }
        }

        public ServerState State
        {
            get { return (ServerState)state; }
        }

        protected void ChangeState(ServerState newState)
        {
            ServerState oldState = (ServerState)Interlocked.Exchange(ref state, (int)newState);

            if (oldState == newState) {
                return;
            }

            RaiseStateChanged(new ServerStateChangedEventArgs(oldState, newState));
        }

        public IPAddress ListenAddress
        {
            get { return serverAcceptor.ListenAddress; }
            set { serverAcceptor.ListenAddress = value; }
        }

        public int ListenPort
        {
            get { return serverAcceptor.ListenPort; }
            set { serverAcceptor.ListenPort = value; }
        }

        public bool Ipv6Only
        {
            get { return serverAcceptor.Ipv6Only; }
            set { serverAcceptor.Ipv6Only = value; }
        }

        public void Start()
        {
            if (state != (int)ServerState.Stopped) {
                throw new InvalidOperationException("Server is already listening");
            }

            ChangeState(ServerState.Starting);

            try {
                serverAcceptor.Start();

                ChangeState(ServerState.Listening);
            }
            catch {
                ChangeState(ServerState.Stopped);
                throw;
            }
        }

        public virtual void Stop()
        {
            if (State == ServerState.Stopped) {
                return;
            }

            ChangeState(ServerState.Stopping);

            try {
                serverAcceptor.Stop();
            }
            finally {
                ChangeState(ServerState.Stopped);
            }
        }

        private PreallocatedChannelData CreateChannelData()
        {
            PreallocatedChannelData channelData = new PreallocatedChannelData();
            channelData.InitReceiver(bufferManager);
            channelData.InitSender(bufferManager);

            return channelData;
        }

        private PreallocatedChannelData GetChannelData()
        {
            PreallocatedChannelData preallocatedData;
            if (channelDataPool == null || !channelDataPool.TryTake(out preallocatedData)) {
                preallocatedData = CreateChannelData();
            }

            return preallocatedData;
        }

        private void OnServerAcceptorAccepted(object sender, SocketEventArgs e)
        {
            PreallocatedChannelData channelResources = GetChannelData();

            ISocketChannel channel = new ImmutableChannel(e.Socket, channelResources.Receiver, channelResources.Sender, channelResources.ReceiverArgs, channelResources.SenderWriter, channelResources);
            channel.Closed += OnChannelClosed;

            RaiseConnected(channel);

            channel.Start();
        }

        private void OnChannelClosed(object sender, EventArgs e)
        {
            if (channelDataPool != null) {
                PreallocatedChannelData channelResources = (PreallocatedChannelData)((ImmutableChannel)sender).AdditionalData;
                channelDataPool.PutObject(channelResources);
            } else if (state == (int)ServerState.Stopped) // perform expensive ter down only if server doesn't listen anymore
              {                                            // this way you can stop listener and then close all the channels to free resources
                PreallocatedChannelData channelResources = (PreallocatedChannelData)((ImmutableChannel)sender).AdditionalData;

                channelResources.Receiver.TearDown(false);
                channelResources.Sender.TearDown();
            }

            serverAcceptor.ReleaseOpenConnectionSlot();
        }

        private void OnServerAcceptorAcceptFailed(object sender, ExceptionEventArgs e)
        {
            RaiseError(e);
        }

        private static ISocketAsyncEventArgsPool CreateAcceptorPool(int size)
        {
            QueueSocketAsyncEventArgsPool pool = new QueueSocketAsyncEventArgsPool();

            for (int i = 0; i < size; i++) {
                pool.Put(CreateAcceptor());
            }

            return pool;
        }

        private static SocketAsyncEventArgs CreateAcceptor()
        {
            return new SocketAsyncEventArgs();
        }

        private class PreallocatedChannelData
        {
            private readonly SocketAsyncEventArgs receiver;
            private readonly SocketAsyncEventArgs sender;
            private readonly DirectBytesReceivedEventArgs receiverArgs;
            private MemoryStream senderWriter;

            public PreallocatedChannelData()
            {
                receiver = new SocketAsyncEventArgs();
                sender = new SocketAsyncEventArgs();

                receiverArgs = new DirectBytesReceivedEventArgs(receiver);
            }

            /// <summary>
            /// Initializes receiver buffer.
            ///
            /// We provide separate initialization methods to allow grouping of the send and receive buffers.
            /// </summary>
            public void InitReceiver(IBufferManager bufferManager)
            {
                bufferManager.SetBuffer(receiver);
            }

            /// <summary>
            /// Initializes sender buffer.
            ///
            /// We provide separate initialization methods to allow grouping of the send and receive buffers.
            /// </summary>
            public void InitSender(IBufferManager bufferManager)
            {
                bufferManager.SetBuffer(sender);

                senderWriter = new MemoryStream(sender.Buffer, sender.Offset, sender.Count);
            }

            public SocketAsyncEventArgs Receiver
            {
                get { return receiver; }
            }

            public SocketAsyncEventArgs Sender
            {
                get { return sender; }
            }

            public DirectBytesReceivedEventArgs ReceiverArgs
            {
                get { return receiverArgs; }
            }

            public MemoryStream SenderWriter
            {
                get { return senderWriter; }
            }
        }
    }
}