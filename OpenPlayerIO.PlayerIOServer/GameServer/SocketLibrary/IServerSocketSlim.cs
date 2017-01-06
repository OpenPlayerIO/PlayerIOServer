using System.Net;

namespace SocketSlim
{
    public interface IServerSocketSlim : ISocketSlim<ServerState>
    {
        /// <summary>
        /// Gets or sets local IP address the socket should be listening on. You can use both IPv6
        /// and IPv4 addresses.
        ///
        /// After calling <see cref="ISocketSlim{T}.Start"/>, changing this property becomes irrelevant.
        /// </summary>
        IPAddress ListenAddress { get; set; }

        /// <summary>
        /// Gets or sets local port the socket should be listening on.
        ///
        /// After calling <see cref="ISocketSlim{T}.Start"/>, changing this property becomes irrelevant.
        /// </summary>
        int ListenPort { get; set; }
    }
}