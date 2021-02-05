using System;

namespace Service
{
    public class SMBService : Service
    {
        private string _version;

        public SMBService(string ip, int port, string version) : base(ip, port)
        {
            _version = version;
        }

        public override bool IsOnline()
        {
            throw new NotImplementedException("TODO: Ping SMB Server");
        }
    }
}