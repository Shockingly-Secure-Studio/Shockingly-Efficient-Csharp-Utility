using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Service
{
    public class DNSService : UdpService
    {

        /// <summary>
        /// DNS Service is UDP-based
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="port"></param>
        public DNSService(Machine.Machine machine, int port) : base(machine, port)
        {
        }

        public void MakeZoneTransfer()
        {
            
        }
    }
}