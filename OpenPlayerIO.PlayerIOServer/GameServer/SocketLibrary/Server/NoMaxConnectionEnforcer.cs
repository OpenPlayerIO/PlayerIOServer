using System.Threading.Tasks;

namespace SocketSlim.Server
{
    public class NoMaxConnectionEnforcer : IMaxConnectionsEnforcer
    {
        public Task TakeOne()
        {
            return null;
        }

        public void ReleaseOne()
        {
        }
    }
}