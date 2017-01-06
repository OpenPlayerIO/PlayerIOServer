namespace OpenPlayerIO.PlayerIOServer.GameServer.Interfaces
{
    using Helpers;

    public interface IConnection
    {
        void Send(Message message);
    }
}