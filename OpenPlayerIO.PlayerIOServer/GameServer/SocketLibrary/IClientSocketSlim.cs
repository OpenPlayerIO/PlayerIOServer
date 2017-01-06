namespace SocketSlim
{
    public interface IClientSocketSlim : ISocketSlim<ChannelState>
    {
        /// <summary>
        /// Gets or sets the hostname to which the socket will be connecting.
        ///
        /// After calling <see cref="ISocketSlim{T}.Start"/>, changing this property becomes irrelevant.
        /// </summary>
        string Host { get; set; }

        /// <summary>
        /// Gets or sets the port to which the socket will be connecting.
        ///
        /// After calling <see cref="ISocketSlim{T}.Start"/>, changing this property becomes irrelevant.
        /// </summary>
        int Port { get; set; }
    }
}