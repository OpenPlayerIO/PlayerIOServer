using System;

namespace SocketSlim.Client
{
    public class ExceptionEventArgs : EventArgs
    {
        private readonly Exception exception;

        public ExceptionEventArgs(Exception exception)
        {
            this.exception = exception;
        }

        public Exception Exception
        {
            get { return exception; }
        }
    }
}