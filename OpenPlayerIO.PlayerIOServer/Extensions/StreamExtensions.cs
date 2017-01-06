using System.IO;

namespace OpenPlayerIO.PlayerIOServer.Extensions
{
    internal static class StreamExtensions
    {
        internal static void WriteBytes(this Stream stream, byte[] bytes) => stream.Write(bytes, 0, bytes.Length);
    }
}