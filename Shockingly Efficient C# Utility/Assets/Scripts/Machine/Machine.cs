using System.Collections.Generic;

namespace Machine
{
    public class Machine
    {
        public string IPAdress;
        public List<Service.Service> OpenServices = new List<Service.Service>();

        public Machine(string ip)
        {
            IPAdress = ip;
        }
    }
}