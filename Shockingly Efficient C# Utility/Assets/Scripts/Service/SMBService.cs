using System;
using System.Net;

namespace DefaultNamespace
{
    public class SMBService : Service
    {
        protected string Version;

        public SMBService(string ip, int port, string version) : base(ip, port, true)
        {
            Version = version;
        }

        public override bool IsOnline()
        {
            throw new NotImplementedException("TODO: Ping SMB Server");
        }
    }
}