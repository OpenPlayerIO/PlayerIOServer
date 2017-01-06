using System;
using SocketSlim.ChannelWrapper;
using SocketSlim.Client;

namespace SocketSlim
{
    /// <summary> Contains some common methods between client and server flavors of SocketSlim. </summary>
    public abstract class SocketSlimBase<TState>
    {
        public event EventHandler<StateChangedEventArgs<TState>> StateChanged;

        protected void RaiseStateChanged(StateChangedEventArgs<TState> e)
        {
            StateChanged?.Invoke(this, e);
        }

        public event EventHandler<ExceptionEventArgs> Error;

        protected void RaiseError(ExceptionEventArgs e)
        {
            Error?.Invoke(this, e);
        }

        public event EventHandler<ChannelEventArgs> Connected;

        protected virtual void RaiseConnected(ISocketChannel channel)
        {
            Connected?.Invoke(this, new ChannelEventArgs(channel));
        }

        // other members of ISocketSlim{T} are not included here to avoid making them virtual calls
    }
}