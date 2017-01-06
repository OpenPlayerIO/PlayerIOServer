namespace SocketSlim
{
    public class ChannelStateChangedEventArgs : StateChangedEventArgs<ChannelState>
    {
        public ChannelStateChangedEventArgs(ChannelState oldState, ChannelState newState)
            : base(oldState, newState)
        {
        }
    }
}