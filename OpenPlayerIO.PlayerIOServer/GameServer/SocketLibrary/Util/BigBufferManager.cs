using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace SocketSlim.Util
{
    /// <summary>
    /// This class creates a single large buffer which can be divided up and assigned to
    /// SocketAsyncEventArgs objects for use with each socket I/O operation. This enables buffers to
    /// be easily reused and guards against fragmenting heap memory.
    ///
    /// This buffer is a byte array which the Windows TCP buffer can copy its data to.
    /// </summary>
    public class BigBufferManager : IBufferManager
    {
        // the total number of bytes controlled by the buffer pool
        private int totalBytesInBufferBlock;

        private int bufferBytesAllocatedForEachSocket;

        // Byte array maintained by the Buffer Manager.
        private byte[] bufferBlock;

        private int currentIndex;
        private readonly Stack<int> freeIndexPool = new Stack<int>();

        public int TotalBytesInBufferBlock
        {
            get { return totalBytesInBufferBlock; }
            set {
                totalBytesInBufferBlock = value;

                TryInit();
            }
        }

        public int BufferBytesAllocatedForEachSocket
        {
            get { return bufferBytesAllocatedForEachSocket; }
            set {
                bufferBytesAllocatedForEachSocket = value;

                TryInit();
            }
        }

        private void TryInit()
        {
            if (totalBytesInBufferBlock != 0 && bufferBytesAllocatedForEachSocket != 0) {
                InitBuffer(totalBytesInBufferBlock, bufferBytesAllocatedForEachSocket);
            }
        }

        /// <summary>
        /// Allocates buffer space used by the buffer pool.
        ///
        /// Can be called many times to change settings.
        /// </summary>
        public void InitBuffer(int totalBytes, int totalBufferBytesInEachSocket)
        {
            totalBytesInBufferBlock = totalBytes; // remember current settings
            bufferBytesAllocatedForEachSocket = totalBufferBytesInEachSocket;

            currentIndex = 0; // reset some stuff
            freeIndexPool.Clear();

            // Create one large buffer block.
            if (bufferBlock == null || bufferBlock.Length < totalBytesInBufferBlock) // only if we need larger chunk
            {
                bufferBlock = new byte[totalBytesInBufferBlock];
            }
        }

        /// <summary>
        /// Divide that one large buffer block out to each SocketAsyncEventArg object. Assign a
        /// buffer space from the buffer block to the specified SocketAsyncEventArgs object.
        /// </summary>
        /// <param name="args"></param>
        /// <returns> true if the buffer was successfully set, else false </returns>
        public bool SetBuffer(SocketAsyncEventArgs args)
        {
            if (bufferBlock == null) {
                throw new InvalidOperationException("You should initialize buffer before assigning buffers to args");
            }

            if (freeIndexPool.Count > 0) {
                //This if-statement is only true if you have called the FreeBuffer
                //method previously, which would put an offset for a buffer space
                //back into this stack.
                args.SetBuffer(bufferBlock, freeIndexPool.Pop(), bufferBytesAllocatedForEachSocket);
            } else {
                //Inside this else-statement is the code that is used to set the
                //buffer for each SAEA object when the pool of SAEA objects is built
                //in the Init method.
                if ((totalBytesInBufferBlock - bufferBytesAllocatedForEachSocket) < currentIndex) {
                    return false;
                }
                args.SetBuffer(bufferBlock, currentIndex, bufferBytesAllocatedForEachSocket);
                currentIndex += bufferBytesAllocatedForEachSocket;
            }
            return true;
        }

        /// <summary>
        /// Removes the buffer from a SocketAsyncEventArg object. This frees the buffer back to the
        /// buffer pool. Try NOT to use the FreeBuffer method, unless you need to destroy the SAEA
        /// object, or maybe in the case of some exception handling. Instead, on the server keep the
        /// same buffer space assigned to one SAEA object for the duration of this app's running.
        /// </summary>
        /// <param name="args"></param>
        public void FreeBuffer(SocketAsyncEventArgs args)
        {
            freeIndexPool.Push(args.Offset);
            args.SetBuffer(null, 0, 0);
        }

        /// <summary> Resets buffer block, so it can be garbage collected. </summary>
        public void DeinitBuffer()
        {
            bufferBlock = null;
        }
    }
}