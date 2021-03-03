using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Scan;
using Service.Exploit;

namespace Machine
{
    public class Machine
    {
        public string IPAdress;
        public string WorkingDirectory;
        public List<Service.Service> OpenServices = new List<Service.Service>();

        public Machine(string ip)
        {
            IPAdress = ip;
            WorkingDirectory = Path.Combine("Results", ip);
        }

        private int MaxSeverity(List<AccessPoint> accessPoints)
        {
            int max = accessPoints[0].Severity;
            for (int i = 1; i < accessPoints.Count; ++i)
            {
                if (accessPoints[i].Severity > max)
                    max = accessPoints[i].Severity;
            }

            return max;
        }

        public void UpdateFlaws()
        {
            var flawsByServices =
                Directory.EnumerateFiles(WorkingDirectory, "output.json", SearchOption.AllDirectories);

            int nbFlaws = 0;
            int severity = 0;
            foreach (string outputjson in flawsByServices)
            {
                StreamReader sr = new StreamReader(outputjson);
                ServiceResult result = JsonConvert.DeserializeObject<ServiceResult>(sr.ReadToEnd());
                sr.Close();

                nbFlaws += result.AccessPoints.Count;
                int LocalMax = MaxSeverity(result.AccessPoints);
                if (LocalMax > severity)
                    severity = LocalMax;
            }
            
            SaveScan.UpdateFlawJson(IPAdress,nbFlaws, severity, "scan1");
        }
    }
}