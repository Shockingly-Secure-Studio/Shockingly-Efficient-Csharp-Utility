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
        public List<Service.Service> OpenServices;

        public Machine(string ip)
        {
            IPAdress = ip;
            WorkingDirectory = Path.Combine("Results", ip);
            OpenServices = new List<Service.Service>();
        }

        private static int MaxSeverity(List<AccessPoint> accessPoints)
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
                int localMax = MaxSeverity(result.AccessPoints);
                if (localMax > severity)
                    severity = localMax;
            }
            
            SaveScan.UpdateFlawJson(IPAdress,nbFlaws, severity, "scan1");
        }

        public List<Vulnerability> GetVulnerabilities()
        {
            List<Vulnerability> result = new List<Vulnerability>();
            
            var flawsByServices =
                Directory.EnumerateFiles(WorkingDirectory, "output.json", SearchOption.AllDirectories);

            foreach (string outputjson in flawsByServices)
            {
                StreamReader sr = new StreamReader(outputjson);
                ServiceResult serviceResult = JsonConvert.DeserializeObject<ServiceResult>(sr.ReadToEnd());
                sr.Close();

                foreach (AccessPoint vuln in serviceResult.AccessPoints)
                {
                    Vulnerability vulnerability = new Vulnerability(vuln.Type.ToString(), vuln.Access, vuln.Severity, IPAdress);
                    result.Add(vulnerability);
                }
            }

            return result;
        }

        public void AddService(Service.Service service)
        {
            OpenServices.Add(service);
        }
    }
}