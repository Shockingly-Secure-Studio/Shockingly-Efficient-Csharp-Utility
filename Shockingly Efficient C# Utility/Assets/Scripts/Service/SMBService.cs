using System;
using System.Threading.Tasks;

namespace Service
{
    public class SMBService : TcpService
    {
        private string _version;

        public SMBService(Machine.Machine machine, int port, string version) : base(machine, port)
        {
            _version = version;
        }

        public override Task<bool> IsOnline()
        {
            throw new NotImplementedException("TODO: Ping SMB Server");
        }
    }
}