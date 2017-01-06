using System;
using System.Net;
using SocketSlim.Client;

namespace SocketSlim.ChannelWrapper
{
    /// <summary> Describes single opened duplex channel. Once it closes, it can't be reused. </summary>
    public interface ISocketChannel
    {
        /// <summary>
        /// Call this after you've subscribed to all event so that you don't miss first events.
        /// </summary>
        void Start();

        /// <summary> Gets the channel's remote endpoint. </summary>
        EndPoint RemoteEndPoint { get; }

        /// <summary> Gets the channel's local endpoint. </summary>
        EndPoint LocalEndPoint { get; }

        /// <summary>
        /// Sends specified <paramref name="bytes"/> cref="bytes"/&gt; through socket.
        ///
        /// Never reuse the byte array after passing it to this method.
        /// </summary>
        void Send(byte[] bytes);

        /// <summary> Raised when channel receives new data. </summary>
        event ChannelMessageHandler<ISocketChannel> BytesReceived;

        /// <summary>
        /// Raised when the channel is closed. Once the channel is closed, you can't reuse it and
        /// need to get another one from wherever you've got this one.
        ///
        /// Which is either <see cref="ClientSocketSlim"/> or todo server socket class
        /// </summary>
        event EventHandler Closed;

        /// <summary>
        /// Raised when channel encounters an error.
        ///
        /// Channel closure is always signaled through <see cref="Closed"/>, so don't do anything here.
        /// </summary>
        event EventHandler<ExceptionEventArgs> Error;

        /// <summary> Closes this channel. </summary>
        void Close();

        /// <summary> This property allows you to attach additional data to this channel. </summary>
        object Tag { get; set; }

        /// <summary> Gets additional settings and stats for the channel. </summary>
        IChannelSettingsAndStats SettingsAndStats { get; }
    }
}