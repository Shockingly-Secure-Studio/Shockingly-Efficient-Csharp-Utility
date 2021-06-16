using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DnsClient;

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
            var lookup = new LookupClient();
            var result = lookup.QueryAsync("google.com", QueryType.A);

            var record = result.Answers.ARecords().FirstOrDefault();
            var ip = record?.Address;
        }

        
    }
}