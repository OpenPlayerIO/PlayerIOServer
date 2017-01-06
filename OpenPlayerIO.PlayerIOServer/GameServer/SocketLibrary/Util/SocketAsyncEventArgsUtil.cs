using System;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;

namespace SocketSlim.Util
{
    public static class SocketAsyncEventArgsUtil
    {
        private static readonly FieldInfo BufferField = typeof(SocketAsyncEventArgs).GetField("m_Buffer", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo PinnedBufferField = typeof(SocketAsyncEventArgs).GetField("m_PinnedSingleBuffer", BindingFlags.Instance | BindingFlags.NonPublic);

        public static void TearDown(this SocketAsyncEventArgs args, bool isSend = true)
        {
            bool success = false;
            int tries = 0;
            do {
                try {
                    args.SetBuffer(null, 0, 0);
                    success = true;
                }
                catch (InvalidOperationException) {
                    ++tries;

                    if (isSend && tries == 2000) {
                        // remove reference from the socket object forcefully.
                        {
                            // m_Buffer
                            if (BufferField != null) {
                                BufferField.SetValue(args, null);
                            }

                            // m_PinnedSingleBuffer
                            if (PinnedBufferField != null) {
                                PinnedBufferField.SetValue(args, null);
                            }
                        }

                        break;
                    }

                    // socket haven't received the close event yet
                    Thread.Sleep(1);
                }
            } while (!success);

            args.Dispose();
        }
    }
}