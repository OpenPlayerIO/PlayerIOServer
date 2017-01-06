using System;
using System.Net.Sockets;

namespace SocketSlim.Server
{
    public class QueueSocketAsyncEventArgsPool : ObjectPool<SocketAsyncEventArgs>, ISocketAsyncEventArgsPool
    {
        public void Put(SocketAsyncEventArgs item)
        {
            if (item == null) {
                throw new ArgumentNullException("item", "Item added to a QueueSocketAsyncEventArgsPool cannot be null");
            }

            PutObject(item);
        }
    }
}