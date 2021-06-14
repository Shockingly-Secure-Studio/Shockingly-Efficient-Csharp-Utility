using System.Net;
using System.Net.Sockets;

namespace Service
{
    public class UdpService
    {
        protected UdpClient UdpClient;
        protected Machine.Machine Machine;
        protected string IP;
        protected int Port;
        public UdpService(Machine.Machine machine, int port)
        {
            Machine = machine;
            IP = Machine.IPAdress;
            Port = port;
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(IP), Port);
            UdpClient = new UdpClient(ipEndPoint);
        }
    }
}