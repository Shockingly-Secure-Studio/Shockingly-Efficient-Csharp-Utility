using System.Net;
using System.Net.Sockets;

namespace DefaultNamespace
{
    public abstract class Service
    {
        protected IPAddress Ip;
        protected int Port;
        protected IPEndPoint _Ipe;
        protected bool isTCP;

        protected Socket Socket;

        public Service(string ip, int port, bool isTCP)
        {
            Ip = IPAddress.Parse(ip);
            Port = port;
            _Ipe = new IPEndPoint(Ip, port);
            this.isTCP = isTCP;
            
            Socket = new Socket(_Ipe.AddressFamily, SocketType.Stream, isTCP ? ProtocolType.Tcp : ProtocolType.Udp);
            Socket.Connect(_Ipe);
        }
        
        public string Send
        
         

        public int GetPort()
        {
            return Port;
        }
        
        

        public abstract bool IsOnline();
    }
}