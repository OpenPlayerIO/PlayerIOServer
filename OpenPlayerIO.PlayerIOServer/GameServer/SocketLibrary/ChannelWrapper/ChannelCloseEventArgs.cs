using System;
using System.Net.Sockets;

namespace SocketSlim.ChannelWrapper
{
    public class ChannelCloseEventArgs : EventArgs
    {
        private readonly DuplexSide duplexSide;
        private readonly SocketError? socketError;
        private readonly Exception exception;

        public ChannelCloseEventArgs(DuplexSide duplexSide, SocketError? socketError, Exception exception)
        {
            this.duplexSide = duplexSide;
            this.socketError = socketError;
            this.exception = exception;
        }

        /// <summary> Gets the part of the duplex channel that has closed. </summary>
        public DuplexSide DuplexSide
        {
            get { return duplexSide; }
        }

        /// <summary> Gets the exception which caused channel closure. </summary>
        public Exception Exception
        {
            get { return exception; }
        }

        /// <summary> Gets the <see cref="SocketError"/> which caused channel closure. </summary>
        public SocketError? SocketError
        {
            get { return socketError; }
        }
    }
}