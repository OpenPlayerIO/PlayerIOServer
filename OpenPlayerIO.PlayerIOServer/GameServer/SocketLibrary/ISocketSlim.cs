using System;
using SocketSlim.ChannelWrapper;
using SocketSlim.Client;

namespace SocketSlim
{
    /// <summary> Describes similar parts of both client and server socket functionalities. </summary>
    public interface ISocketSlim<TState>
    {
        /// <summary> Gets the current state of the socket. </summary>
        TState State { get; }

        /// <summary>
        /// Starts the socket.
        ///
        /// It's an asynchronous operation, you should be subscribed to <see cref="Connected"/> event
        /// to get the object through which you can send and receive data.
        ///
        /// If anything fails while starting, you will receive <see cref="StateChanged"/> event with
        /// the appropriate value.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the socket.
        ///
        /// It's an asynchronous operation, you should start the socket again only after you receive
        /// <see cref="StateChanged"/> event with the state that signifies a full stop.
        ///
        /// This won't close any channels you got from <see cref="Connected"/> event. Use <see
        /// cref="ISocketChannel.Close"/> method to close them individually.
        /// </summary>
        void Stop();

        /// <summary> Raised when socket changes state. </summary>
        event EventHandler<StateChangedEventArgs<TState>> StateChanged;

        /// <summary>
        /// Raised when any error occurs in the socket. Channel errors should be handled separately.
        /// </summary>
        event EventHandler<ExceptionEventArgs> Error;

        /// <summary> Raised when the socket produces new communication channel. </summary>
        event EventHandler<ChannelEventArgs> Connected;
    }
}