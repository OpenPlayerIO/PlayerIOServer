using System.Threading.Tasks;

namespace SocketSlim.Server
{
    public interface IMaxConnectionsEnforcer
    {
        /// <summary> Returns task which ends when the slot is available. </summary>
        Task TakeOne();

        void ReleaseOne();
    }
}