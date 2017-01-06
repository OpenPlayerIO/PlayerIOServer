using System;

namespace SocketSlim.ChannelWrapper
{
    /// <summary>
    /// Abstract class used as interface for the EventArgs object for receiving bytes in <see cref="ChannelWrapperBase"/>.
    /// </summary>
    public abstract class BytesReceivedEventArgs : EventArgs
    {
        /// <summary> Buffer in which recevied sata is stored at the <see cref="Offset"/>. </summary>
        public abstract byte[] Buffer { get; }

        /// <summary> Offset at which received data is stored in the <see cref="Buffer"/>. </summary>
        public abstract int Offset { get; }

        /// <summary> Size of the received data in bytes; </summary>
        public abstract int Size { get; }

        /// <summary> Starts the next asynchronous receive iteration. </summary>
        /// <returns>
        /// false, if while starting the receive operation, the socket was closed and so there is no
        /// need for processing received data. Otherwise true.
        /// </returns>
        public abstract bool Proceed();
    }
}