namespace SocketSlim
{
    public delegate void ChannelMessageHandler<in TChannel>(TChannel socket, byte[] message);
}