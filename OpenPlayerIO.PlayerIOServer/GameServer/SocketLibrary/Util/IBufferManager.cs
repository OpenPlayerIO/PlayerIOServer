using System.Net.Sockets;

namespace SocketSlim.Util
{
    public interface IBufferManager
    {
        /// <summary>
        /// Divide that one large buffer block out to each SocketAsyncEventArg object. Assign a
        /// buffer space from the buffer block to the specified SocketAsyncEventArgs object.
        /// </summary>
        /// <param name="args"></param>
        /// <returns> true if the buffer was successfully set, else false </returns>
        bool SetBuffer(SocketAsyncEventArgs args);

        /// <summary>
        /// Removes the buffer from a SocketAsyncEventArg object. This frees the buffer back to the
        /// buffer pool. Try NOT to use the FreeBuffer method, unless you need to destroy the SAEA
        /// object, or maybe in the case of some exception handling. Instead, on the server keep the
        /// same buffer space assigned to one SAEA object for the duration of this app's running.
        /// </summary>
        /// <param name="args"></param>
        void FreeBuffer(SocketAsyncEventArgs args);

        /// <summary> Resets buffer block, so it can be garbage collected. </summary>
        void DeinitBuffer();
    }
}