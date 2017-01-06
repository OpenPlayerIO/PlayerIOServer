using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SocketSlim.Client
{
    /// <summary>
    /// Extended <see cref="ClientConnector"/> implemetation that allows getting a <see
    /// cref="Socket"/> through <see cref="Task"/>.
    /// </summary>
    public class TaskClientConnector : ClientConnector
    {
        private TaskCompletionSource<Socket> taskCompletionSource;

        public TaskClientConnector(SocketType socketType, ProtocolType protocolType, SocketAsyncEventArgs connector)
            : base(socketType, protocolType, connector)
        {
        }

        /// <summary>
        /// Initiates connection process and returns a task with the resulting connected socket.
        /// </summary>
        public Task<Socket> ConnectAsync()
        {
            if (taskCompletionSource != null) {
                throw new InvalidOperationException("We're already connecting.");
            }

            TaskCompletionSource<Socket> newTaskSource = new TaskCompletionSource<Socket>();
            taskCompletionSource = newTaskSource;

            Connect();

            return newTaskSource.Task;
        }

        public override bool StopConnecting()
        {
            TaskCompletionSource<Socket> tcs = taskCompletionSource;
            if (tcs != null) {
                tcs.TrySetCanceled();
            }

            return base.StopConnecting();
        }

        protected override void RaiseConnected(Socket s)
        {
            base.RaiseConnected(s);

            TaskCompletionSource<Socket> tcs = taskCompletionSource;
            if (tcs != null) {
                taskCompletionSource.TrySetResult(s);
                taskCompletionSource = null;
            }
        }

        protected override void RaiseFailed(Exception e)
        {
            base.RaiseFailed(e);

            TaskCompletionSource<Socket> tcs = taskCompletionSource;
            if (tcs != null) {
                taskCompletionSource.TrySetException(e);
                taskCompletionSource = null;
            }
        }
    }
}