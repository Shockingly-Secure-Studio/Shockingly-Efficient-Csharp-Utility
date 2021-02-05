using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Service
{
    public abstract class Service
    {
        private readonly IPAddress _ip;
        private readonly int _port;
        
        public Service(string ip, int port)
        {
            _ip = IPAddress.Parse(ip);
            _port = port;
        }
        

        public string SocketSendReceive(string message)
        {
            byte[] bytesSent = Encoding.ASCII.GetBytes(message);
            byte[] bytesReceived = new byte[256];
            string result = "";

            using (Socket s = Utils.ConnectSocket(_ip, _port))
            {
                if (s == null)
                    return ("Connection failed");

                // Send request to the server.
                s.Send(bytesSent, bytesSent.Length, 0);

                int bytes = 0;
                
                // The following will block until the page is transmitted.
                do {
                    bytes = s.Receive(bytesReceived, bytesReceived.Length, 0);
                    result += Encoding.ASCII.GetString(bytesReceived, 0, bytes);
                }
                while (bytes > 0);
            }

            return result;
        }
        
         

        public int GetPort()
        {
            return _port;
        }
        
        

        public abstract bool IsOnline();
    }
}