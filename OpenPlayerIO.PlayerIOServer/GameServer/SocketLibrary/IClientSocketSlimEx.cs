namespace SocketSlim
{
    public interface IClientSocketSlimEx : IClientSocketSlim
    {
        /// <summary>
        /// Sends specified <see cref="bytes"/> into the socket. This method is thread safe and uses
        /// FIFO queue.
        /// </summary>
        void Send(byte[] bytes);

        /// <summary> Raised when socket receives some bytes. </summary>
        event ChannelMessageHandler<ClientSocketSlim> BytesReceived;
    }
}