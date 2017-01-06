using System.Net.Sockets;

namespace SocketSlim.Server
{
    public interface ISocketAsyncEventArgsPool
    {
        int Count { get; }

        bool TryTake(out SocketAsyncEventArgs result);

        void Put(SocketAsyncEventArgs item);
    }
}