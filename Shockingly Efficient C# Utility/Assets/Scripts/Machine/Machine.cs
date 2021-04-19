using System.Collections.Generic;
using Service;

namespace Machine
{
    public class Machine
    {
        public string IPAdress;
        public List<Service.Service> OpenServices;

        public Machine(string ip)
        {
            IPAdress = ip;
            OpenServices = new List<Service.Service>();
        }

        public void AddService(Service.Service service)
        {
            OpenServices.Add(service);
        }
    }
}