using System;
using System.Collections.Generic;

namespace DefaultNamespace
{
    public class Machine
    {
        public string IPAdress;
        public List<Service> open_services = new List<Service>();
    }
}