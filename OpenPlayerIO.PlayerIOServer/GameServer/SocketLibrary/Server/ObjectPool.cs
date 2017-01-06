using System.Collections.Concurrent;

namespace SocketSlim.Server
{
    public class ObjectPool<TObject>
    {
        // Pool of reusable SocketAsyncEventArgs objects.
        private readonly ConcurrentQueue<TObject> pool; // memory leaks of that queue are not of a great concern for us here

        // initializes the object pool to the specified size.
        public ObjectPool()
        {
            pool = new ConcurrentQueue<TObject>();
        }

        // The number of SocketAsyncEventArgs instances in the pool.
        public int Count
        {
            get { return pool.Count; }
        }

        // Removes a SocketAsyncEventArgs instance from the pool. returns SocketAsyncEventArgs
        // removed from the pool.
        public bool TryTake(out TObject result)
        {
            return pool.TryDequeue(out result);
        }

        // Add a SocketAsyncEventArg instance to the pool. "item" = SocketAsyncEventArgs instance to
        // add to the pool.
        public void PutObject(TObject item)
        {
            pool.Enqueue(item);
        }
    }
}