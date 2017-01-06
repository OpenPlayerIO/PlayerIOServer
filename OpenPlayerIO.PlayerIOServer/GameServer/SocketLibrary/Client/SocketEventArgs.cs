using System;
using System.Net.Sockets;

namespace SocketSlim.Client
{
    public class SocketEventArgs : EventArgs
    {
        private readonly Socket socket;

        public SocketEventArgs(Socket socket)
        {
            this.socket = socket;
        }

        public Socket Socket
        {
            get { return socket; }
        }
    }
}