using System.Threading;
using System.Threading.Tasks;
using SocketSlim.Util;

namespace SocketSlim.Server
{
    public class MaxConnectionsEnforcer : IMaxConnectionsEnforcer
    {
        private readonly AsyncSemaphore semaphore;

        public MaxConnectionsEnforcer(int maxConnections)
        {
            semaphore = new AsyncSemaphore(maxConnections);
        }

        /// <summary> Blocks until the connection is available. </summary>
        public Task TakeOne()
        {
            return semaphore.WaitAsync();
        }

        public void ReleaseOne()
        {
            try {
                semaphore.Release();
            }
            catch (SemaphoreFullException) { }
        }
    }
}