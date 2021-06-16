using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Scan;
using Service.Exploit;

namespace Service
{
    public abstract class Service
    {
        private readonly IPAddress _ip;
        private readonly int _port;
        protected readonly string WorkingDirectory;
        protected readonly Machine.Machine Host;
        
        public Service(Machine.Machine machine, int port)
        {
            Host = machine;
            _ip = IPAddress.Parse(machine.IPAdress);
            _port = port;
                
            machine.AddService(this);

            WorkingDirectory = Path.Combine("Results", machine.IPAdress, port.ToString());

            Directory.CreateDirectory(WorkingDirectory);
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


        public IPAddress GetIP()
        {
            return _ip;
        }
        
        public int GetPort()
        {
            return _port;
        }

        public Machine.Machine GetHost()
        {
            return Host;
        }
        
        

        public abstract Task<bool> IsOnline();

        public static ServiceResult GetServiceResult(string ip, int port)
        {
            return GetServiceResult(ip, port.ToString());
        }

        public static ServiceResult GetServiceResult(string ip, string port)
        {
            string path = Path.Combine("Results",ip, port, "output.json");
            if (!File.Exists(path))
                throw new FileNotFoundException(
                    $"GetServiceResult: {path} does not exists."
                    );
            
            StreamReader sr = new StreamReader(path);
            ServiceResult result = JsonConvert.DeserializeObject<ServiceResult>(sr.ReadToEnd());
            sr.Close();

            return result;
        }
        
        public void Log(AccessPoint accessPoint)
        {
            string path = Path.Combine(WorkingDirectory, "output.json");
            bool exists = File.Exists(path);
            ServiceResult result;
            result = exists ? GetServiceResult(_ip.ToString(), _port) : new ServiceResult(_ip.ToString(), _port);
            
            result.AccessPoints.Add(accessPoint);
            
            string jsonSerializedObj = JsonConvert.SerializeObject(result, Formatting.Indented);
            byte[] toWrite = new UTF8Encoding(true).GetBytes(jsonSerializedObj);
            
            FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
            fs.Write(toWrite, 0, toWrite.Length);
            fs.Close();
            
            Host.UpdateFlaws();
        }
    }
}