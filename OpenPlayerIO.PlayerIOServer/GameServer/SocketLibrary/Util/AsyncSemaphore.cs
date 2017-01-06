using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SocketSlim.Util
{
    public class AsyncSemaphore
    {
        private readonly static Task Completed = GetTaskWithResult(true);

        private readonly Queue<TaskCompletionSource<bool>> waiters = new Queue<TaskCompletionSource<bool>>();
        private readonly int maxCount;
        private int currentCount;

        public AsyncSemaphore(int initialCount)
            : this(initialCount, initialCount)
        { }

        public AsyncSemaphore(int initialCount, int maxCount)
        {
            if (maxCount < initialCount)
                throw new ArgumentException("maxCount should be greater than initialCount", "maxCount");
            if (maxCount < 1)
                throw new ArgumentOutOfRangeException("maxCount", "Value should be greater than zero.");
            if (initialCount < 0)
                throw new ArgumentOutOfRangeException("initialCount", "Value should be greater than or equal to zero.");

            this.maxCount = maxCount;
            currentCount = initialCount;
        }

        public Task WaitAsync()
        {
            lock (waiters) {
                if (currentCount > 0) {
                    --currentCount;
                    return Completed;
                }

                TaskCompletionSource<bool> waiter = new TaskCompletionSource<bool>();
                waiters.Enqueue(waiter);
                return waiter.Task;
            }
        }

        public void Release()
        {
            TaskCompletionSource<bool> toRelease;
            lock (waiters) {
                if (waiters.Count == 0) {
                    if (currentCount >= maxCount) {
                        throw new SemaphoreFullException();
                    }

                    ++currentCount;
                    return;
                }

                toRelease = waiters.Dequeue();
            }

            toRelease.SetResult(true);
        }

        private static Task<TResult> GetTaskWithResult<TResult>(TResult result)
        {
            TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>();
            tcs.SetResult(result);
            return tcs.Task;
        }
    }
}