using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using UnityEngine;

namespace Scan
{
    public class SaveScan
    {
        public void NewJson(List<(IPAddress,List<int>)> scanResult)
        {
            Debug.Log("newSave");
            List<Device> devicesList = new List<Device>();
            foreach (var (ip,port) in scanResult)
            {
                devicesList.Add(new Device(){IP=ip.ToString(),Port=port});
            }
            string jsonSerializedObj = JsonConvert.SerializeObject(devicesList, Formatting.Indented);
            Directory.CreateDirectory("Results");
            string path = Path.Combine("Results", "scan.json");
            File.WriteAllText(path, jsonSerializedObj);
        }
        private class Device
        {
            public string IP { get; set; }
            public List<int> Port { get; set; }
        }
    }
}