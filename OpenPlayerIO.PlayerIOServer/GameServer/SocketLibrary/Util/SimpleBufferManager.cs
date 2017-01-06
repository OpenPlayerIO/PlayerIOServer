using System.Net.Sockets;

namespace SocketSlim.Util
{
    public class SimpleBufferManager : IBufferManager
    {
        private int bufferBytesAllocatedForEachSocket;

        public int BufferBytesAllocatedForEachSocket
        {
            get { return bufferBytesAllocatedForEachSocket; }
            set {
                bufferBytesAllocatedForEachSocket = value;

                if (bufferBytesAllocatedForEachSocket != 0) {
                    InitBuffer(bufferBytesAllocatedForEachSocket);
                }
            }
        }

        public void InitBuffer(int totalBufferBytesInEachSocket)
        {
            bufferBytesAllocatedForEachSocket = totalBufferBytesInEachSocket;
        }

        public bool SetBuffer(SocketAsyncEventArgs args)
        {
            byte[] bytes = new byte[bufferBytesAllocatedForEachSocket];

            args.SetBuffer(bytes, 0, bytes.Length);

            return true;
        }

        public void FreeBuffer(SocketAsyncEventArgs args)
        {
            args.SetBuffer(null, 0, 0);
        }

        public void DeinitBuffer()
        {
            // do nothing
        }
    }
}