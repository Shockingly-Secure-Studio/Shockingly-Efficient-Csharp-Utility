using System.Threading.Tasks;

namespace Service
{
    public class SSHService : Service
    {
        private string _version;

        private bool IsConnected = false;
        private string _sshService;
        
        
        private SSHService(string ip, int port, string version) : base(ip, port)
        {
            _version = version;
            _sshService = "";
        }

        public override Task<bool> IsOnline()
        {
            throw new System.NotImplementedException();
        }
    }
}