using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace SocketSlim.ChannelWrapper
{
    /// <summary>
    /// This class wraps a socket and allows for sending and receiving of the data on that socket.
    ///
    /// The socket which is taken by the constructor is presumed as being open.
    ///
    /// Typical workflow with this class should be as follows:
    /// * Create an object with the constructor;
    /// * Subscribe for <see cref="BytesReceived"/> and <see cref="Closed"/> events;
    /// * Call <see cref="Start"/> method;
    /// * Handle incoming data through <see cref="BytesReceived"/> event handler and call <see
    ///   cref="Send"/> to send data;
    /// * Optionally call <see cref="Close"/> method to close the connection.
    /// * Once the <see cref="Closed"/> event is raised, the socket is closed and this object can't
    ///   be reused, but the <see cref="SocketAsyncEventArgs"/> and buffers you passed to it can.
    ///
    /// This class represents a low level building block for the socket. Feel free to use it in your
    /// own designs, but avoid using it directly in the user code.
    /// </summary>
    /// <remarks>
    /// The constructor takes two <see cref="SocketAsyncEventArgs"/> objects which should be already
    /// set up with buffers. This is done because allocating these on the fly is expensive and can be
    /// optimized by using a pool of preallocated objects with buffers.
    ///
    /// Also, because the buffer array set in <see cref="SocketAsyncEventArgs"/> becomes "pinned" i.
    /// e. can't be moved by GC, allocating many small buffers creates memory fragmentation issue, so
    /// this object has to allow for different buffer allocation techniques such as sharing one huge
    /// byte array across many <see cref="SocketAsyncEventArgs"/>.
    /// </remarks>
    public class ChannelWrapperBase : IChannelSettingsAndStats
    {
        // as this object is tied exclusively to one socket, we can store send/receive context data
        // in here instead of passing it through SocketAsyncEventArgs
        private readonly Socket socket;

        private readonly SocketAsyncEventArgs receiver;
        private readonly DirectBytesReceivedEventArgs receivedEventArgs;

        private readonly SocketAsyncEventArgs sender;
        private readonly MemoryStream sendBufferWriter;
        private readonly int outgoingBufferOffset;

        private readonly Queue<byte[]> outgoingQueue = new Queue<byte[]>(10);

        private bool sending;
        private byte[] currentMessage;
        private int currentMessageOffset;

        private int freeSendCounter;
        private int freeReceiveCounter;

        private bool closed;

        // some diagnostics
        private long currentOutgoingQueueSize;

        private int peakOutgoingQueueLength;
        private long sentByteCount;
        private long receivedByteCount;

        private OutgoingQueueOverflowBehavior outgoingQueueOverflowBehavior = OutgoingQueueOverflowBehavior.Ignore;
        private int maxOutgoingQueueLength = Int32.MaxValue;

        /// <summary> Creates the non-reusable socket wrapper. </summary>
        /// <param name="socket"> socket channel to use for communications </param>
        /// <param name="receiver">
        /// <see cref="SocketAsyncEventArgs"/> to be used for receiving, it should have its buffer
        /// already set up
        /// </param>
        /// <param name="receivedEventArgs">
        /// <see cref="DirectBytesReceivedEventArgs"/> object to be used for <see
        /// cref="BytesReceived"/> event notifications (should point to <paramref name="receiver"/>)
        /// </param>
        /// <param name="sender">
        /// <see cref="SocketAsyncEventArgs"/> to be used for sending, it should have its buffer
        /// already set up
        /// </param>
        /// <param name="sendBufferWriter">
        /// <see cref="MemoryStream"/> to be used for writing the data to sending buffer. It should
        /// already point to <paramref name="sender"/>'s buffer
        /// </param>
        /// <remarks>
        /// All these weird arguments allow for preallocation and pooling of <see
        /// cref="SocketAsyncEventArgs"/> and other related resources
        /// </remarks>
        public ChannelWrapperBase(Socket socket, SocketAsyncEventArgs receiver, DirectBytesReceivedEventArgs receivedEventArgs, SocketAsyncEventArgs sender, MemoryStream sendBufferWriter)
        {
            this.socket = socket;

            socket.NoDelay = true;
            socket.Blocking = false;

            this.receiver = receiver;
            receiver.Completed += OnReceiveCompleted;
            this.receivedEventArgs = receivedEventArgs;
            receivedEventArgs.ProceedReceive += OnProceedReceive;

            this.sender = sender;
            sender.Completed += OnSendCompleted;
            this.sendBufferWriter = sendBufferWriter;

            outgoingBufferOffset = sender.Offset;
        }

        /// <summary>
        /// Starts the receive loop. It's not started in the constructor so you can subscribe for
        /// proper events first.
        /// </summary>
        public void Start()
        {
            StartReceive();
        }

        private bool StartReceive()
        {
            bool callbackPending;
            try {
                callbackPending = socket.ReceiveAsync(receiver);
            }
            catch (ObjectDisposedException) {
                // socket is closed
                CloseSocketNormally(isReceiver: true);
                return false;
            }
            catch (Exception ex) {
                CloseSocket(isReceiver: true, exception: ex);
                return false;
            }

            if (!callbackPending) {
                return ProcessReceive();
            }

            return true;
        }

        private void OnReceiveCompleted(object o, SocketAsyncEventArgs e)
        {
            ProcessReceive();
        }

        private bool ProcessReceive()
        {
            try {
                if (receiver.SocketError != SocketError.Success || receiver.BytesTransferred == 0) {
                    if (receiver.SocketError != SocketError.Success) {
                        CloseSocket(isReceiver: true, error: receiver.SocketError);
                    } else {
                        CloseSocketNormally(isReceiver: true);
                    }

                    return false;
                }

                receivedByteCount += receiver.BytesTransferred;

                RaiseBytesReceived();
            }
            catch (Exception ex) {
                CloseSocket(isReceiver: true, exception: ex);
                return false;
            }

            return true;
        }

        private void OnProceedReceive(object o, ProceedReceiveEventArgs e)
        {
            if (!StartReceive()) {
                e.Closed = true;
            }
        }

        /// <summary>
        /// This event is fired when the socket receives some bytes.
        ///
        /// The handler <b> MUST </b> call the <see cref="BytesReceivedEventArgs.Proceed"/> method to
        /// continue receiving data.
        /// </summary>
        public event EventHandler<BytesReceivedEventArgs> BytesReceived;

        protected virtual void RaiseBytesReceived()
        {
            EventHandler<BytesReceivedEventArgs> handler = BytesReceived;
            if (handler != null) {
                handler(this, receivedEventArgs);
            }
        }

        /// <summary>
        /// Sends a pack of bytes through the socket.
        ///
        /// Don't do anything with the byte array after you've passed it inside this method.
        /// </summary>
        public void Send(byte[] msg)
        {
            if (msg == null)
                throw new ArgumentNullException("msg");
            if (msg.Length == 0)
                throw new ArgumentOutOfRangeException("msg", msg.Length, "Message should not be empty (sending empty array is equivalent to not sending anything at all)");

            if (closed) {
                throw new InvalidOperationException("Cannot send data on closed connection");
            }

            bool needStart = false;
            int queueLength;
            lock (outgoingQueue) {
                // check queue size constraints
                if (outgoingQueue.Count >= maxOutgoingQueueLength && outgoingQueueOverflowBehavior != OutgoingQueueOverflowBehavior.Ignore && maxOutgoingQueueLength > 0) {
                    switch (outgoingQueueOverflowBehavior) {
                        case OutgoingQueueOverflowBehavior.DiscardSilently:
                            return;

                        case OutgoingQueueOverflowBehavior.DiscardException:
                            throw new OutgoingQueueOverflowException(maxOutgoingQueueLength);

                        case OutgoingQueueOverflowBehavior.RetireOldMessages:
                            while (outgoingQueue.Count >= maxOutgoingQueueLength) // remove messages from queue's start until it could hold our message
                            {
                                byte[] message = outgoingQueue.Dequeue();
                                currentOutgoingQueueSize -= message.Length;
                            }
                            break;

                        case OutgoingQueueOverflowBehavior.ClearQueue:
                            outgoingQueue.Clear();
                            currentOutgoingQueueSize = 0;
                            break;

                        case OutgoingQueueOverflowBehavior.Close:
                            Close();
                            return;
                    }
                }

                outgoingQueue.Enqueue(msg);

                // update diagnostics
                currentOutgoingQueueSize += msg.Length;
                queueLength = outgoingQueue.Count;

                if (!sending) // check whether noone is sending data already, becuse then they'll pick up the message otherwise.
                {
                    sending = true;
                    needStart = true;
                }
            }

            if (peakOutgoingQueueLength < queueLength) {
                peakOutgoingQueueLength = queueLength;
            }

            if (needStart) {
                StartSend();
            }
        }

        private void StartSend()
        {
            const int preallocatedArrayLength = 10;

            List<byte[]> messages;
            int bytesToSend;
            bool needQueue = true;

            sendBufferWriter.Seek(0, SeekOrigin.Begin);

            // handle the remaining message from the previous sending
            if (currentMessage != null) {
                if (currentMessage.Length - currentMessageOffset > sendBufferWriter.Length) // check if the message is too long for sending buffer
                {
                    messages = new List<byte[]>();

                    bytesToSend = (int)sendBufferWriter.Length;
                    sendBufferWriter.Write(currentMessage, currentMessageOffset, bytesToSend);

                    currentMessageOffset += bytesToSend;

                    needQueue = false; // previous message fills all of the sending buffer, no need to go to queue for additional data
                } else {
                    messages = new List<byte[]>(preallocatedArrayLength);

                    bytesToSend = currentMessage.Length - currentMessageOffset;
                    sendBufferWriter.Write(currentMessage, currentMessageOffset, bytesToSend);

                    currentMessage = null;
                }
            } else {
                messages = new List<byte[]>(preallocatedArrayLength);
                bytesToSend = 0;
            }

            if (needQueue) {
                lock (outgoingQueue) {
                    if (bytesToSend == 0 && outgoingQueue.Count == 0) {
                        sending = false;
                        return;
                    }

                    // take some messages from queue to fill the sending buffer we don't write them
                    // to send buffer here to reduce time spent inside lock
                    while (outgoingQueue.Count > 0) {
                        byte[] message = outgoingQueue.Dequeue();
                        messages.Add(message);

                        currentOutgoingQueueSize -= message.Length;

                        if (bytesToSend + message.Length > sendBufferWriter.Length) {
                            // message doesn't fit into buffer
                            currentMessage = message;
                            currentMessageOffset = (int)(sendBufferWriter.Length - bytesToSend);

                            break;
                        }

                        // message fits into the remaining buffer space
                        bytesToSend += message.Length;
                    }
                }
            }

            // fill in the send buffer
            foreach (byte[] message in messages) {
                if (message == currentMessage) {
                    // message doesn't fit into buffer ReSharper disable AssignNullToNotNullAttribute
                    sendBufferWriter.Write(currentMessage, 0, currentMessageOffset);
                    // ReSharper restore AssignNullToNotNullAttribute

                    break;
                }

                // message fits into the remaining buffer space
                sendBufferWriter.Write(message, 0, message.Length);
            }

            // set the right buffer boundary
            sender.SetBuffer(outgoingBufferOffset, (int)sendBufferWriter.Position);

            StartAsyncSend();
        }

        private void StartAsyncSend()
        {
            bool callbackPending;
            try {
                callbackPending = socket.SendAsync(sender);
            }
            catch (Exception ex) {
                CloseSocket(isReceiver: false, exception: ex);
                return;
            }

            if (!callbackPending) {
                ProcessSent();
            }
        }

        private void OnSendCompleted(object o, SocketAsyncEventArgs e)
        {
            ProcessSent();
        }

        private void ProcessSent()
        {
            try {
                if (sender.SocketError != SocketError.Success) {
                    CloseSocket(isReceiver: false, error: sender.SocketError);
                    return;
                }

                sentByteCount += sender.BytesTransferred;

                StartSend();
            }
            catch (Exception ex) {
                CloseSocket(isReceiver: false, exception: ex);
            }
        }

        private void CloseInternal()
        {
            if (closed) {
                return;
            }

            closed = true;

            try {
                // do a shutdown before you close the socket
                try {
                    socket.Shutdown(SocketShutdown.Both);
                }
                catch (SocketException) { }
                catch (ObjectDisposedException) { }

                // close socket
                socket.Close();
            }
            catch (NullReferenceException) { }
        }

        private void CloseSocketNormally(bool isReceiver)
        {
            CloseSocket(isReceiver, null, null);
        }

        private void CloseSocket(bool isReceiver, SocketError error)
        {
            CloseSocket(isReceiver, error, null);
        }

        private void CloseSocket(bool isReceiver, Exception exception)
        {
            CloseSocket(isReceiver, null, exception);
        }

        private void CloseSocket(bool isReceiver, SocketError? error, Exception exception)
        {
            bool freeSend = false;
            if (!isReceiver) {
                freeSend = Interlocked.Increment(ref freeSendCounter) == 1;
            } else {
                if (!sending) {
                    freeSend = Interlocked.Increment(ref freeSendCounter) == 1;
                }
            }

            bool freeReceive = isReceiver && (Interlocked.Increment(ref freeReceiveCounter) == 1);

            RaiseDuplexChannelClosed(isReceiver ? DuplexSide.Receive : DuplexSide.Send, error, exception);

            if (!freeSend && !freeReceive) {
                //return; // nothing to do
            }

            CloseInternal();

            if (freeSend) {
                sender.Completed -= OnSendCompleted;
            }

            if (freeReceive) {
                receiver.Completed -= OnReceiveCompleted;
                receivedEventArgs.ProceedReceive -= OnProceedReceive;
                RaiseClosed();
            }
        }

        /// <summary> Event is fired when the socket is fully closed. </summary>
        public event EventHandler Closed;

        protected virtual void RaiseClosed()
        {
            EventHandler handler = Closed;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Event is fired when one of the duplex channels closes. This event provides the detailed
        /// information about the reason the channel was closed.
        ///
        /// We can't provide this info in the <see cref="Closed"/> event, because full closure is
        /// always signaled by the closure of the receive channel, but the true cause of the channel
        /// closing may be the failure at the send channel.
        /// </summary>
        public event EventHandler<ChannelCloseEventArgs> DuplexChannelClosed;

        public void RaiseDuplexChannelClosed(DuplexSide duplexSide, SocketError? socketError, Exception exception)
        {
            EventHandler<ChannelCloseEventArgs> handler = DuplexChannelClosed;
            if (handler != null) {
                handler(this, new ChannelCloseEventArgs(duplexSide, socketError, exception));
            }
        }

        public void Close()
        {
            // close the socket itself, both receiver and sender will quickly return errors and free
            // themselves shortly after that
            CloseInternal();
        }

        /// <summary>
        /// Gets or sets the behavior for the channel when the size of outgoing queue in items
        /// exceeds <see cref="MaxOutgoingQueueLength"/>.
        /// </summary>
        public OutgoingQueueOverflowBehavior OutgoingQueueOverflowBehavior
        {
            get { return outgoingQueueOverflowBehavior; }
            set { outgoingQueueOverflowBehavior = value; }
        }

        /// <summary>
        /// Gets or sets a length of ougoing queue to trigger <see
        /// cref="OutgoingQueueOverflowBehavior"/>. Must be greater than zero to take effect.
        ///
        /// For the optimal performance, this should be set to <see cref="int.MaxValue"/> to save two
        /// comparison operations, if you want to turn off the check;
        /// </summary>
        public int MaxOutgoingQueueLength
        {
            get { return maxOutgoingQueueLength; }
            set { maxOutgoingQueueLength = value; }
        }

        /// <summary> Gets the immediate send queue item count. </summary>
        public int CurrentOutgoingQueueLength
        {
            get { return outgoingQueue.Count; }
        }

        /// <summary> Gets the immediate send queue cumulative size in bytes. </summary>
        public long CurrentOutgoingQueueSize
        {
            get { return currentOutgoingQueueSize; }
        }

        /// <summary>
        /// Gets the maximum send queue length in items throughout the life of a channel wrapper.
        /// </summary>
        public int PeakOutgoingQueueLength
        {
            get { return peakOutgoingQueueLength; }
        }

        /// <summary> Gets the number of bytes that were sent through the socket. </summary>
        public long SentByteCount
        {
            get { return sentByteCount; }
        }

        /// <summary> Gets the number of bytes that were sent through the socket. </summary>
        public long ReceivedByteCount
        {
            get { return receivedByteCount; }
        }
    }
}