namespace SocketSlim
{
    public class ServerStateChangedEventArgs : StateChangedEventArgs<ServerState>
    {
        public ServerStateChangedEventArgs(ServerState oldState, ServerState newState)
            : base(oldState, newState)
        {
        }
    }
}