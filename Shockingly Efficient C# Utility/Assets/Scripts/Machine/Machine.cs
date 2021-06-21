using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Renci.SshNet;
using Scan;
using Service;
using Service.Exploit;
using UnityEngine;

namespace Machine
{
    public class Machine
    {
        [JsonProperty("ip")]
        public string IPAdress;
        public string WorkingDirectory;

        public Machine(string ip)
        {
            IPAdress = ip;
            WorkingDirectory = Path.Combine("Results", IPAdress);
            if (!Directory.Exists(WorkingDirectory))
            {
                Directory.CreateDirectory(WorkingDirectory);
            }
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
            
            SaveScan.UpdateFlawJson(IPAdress,nbFlaws, severity, "scanPort");
        }

        public List<Vulnerability> UpdateVulnerabilities()
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
                    Vulnerability vulnerability = new Vulnerability(vuln.Type.ToString(), vuln.Access, vuln.Severity, IPAdress, vuln.POC);
                    result.Add(vulnerability);
                }
            }

            return result;
        }

        public void AddShell(WebShell webShell)
        {
            Debug.Log("In Machine.AddShell");
            
            //JsonSerializerSettings settings = new JsonSerializerSettings { ContractResolver = new Utils.MyContractResolver() };
            
            
            string serialized = JsonConvert.SerializeObject(webShell);
            Debug.Log(serialized);
            string path = Path.Combine(webShell.Entry.WorkingDirectory, "rce.json");
            Debug.Log(path);
            if (File.Exists(path))
                File.Delete(path);
            File.WriteAllText(path, serialized);
        }
    }
}