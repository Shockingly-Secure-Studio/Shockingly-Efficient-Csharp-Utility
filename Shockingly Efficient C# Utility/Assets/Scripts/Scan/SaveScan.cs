using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using UnityEngine;

namespace Scan
{
    public class SaveScan
    {
        public static void NewJson(string fileName)
        {
            string jsonSerializedObj = "";
            Directory.CreateDirectory("Results");
            string path = Path.Combine("Results", fileName+".json");
            File.WriteAllText(path, jsonSerializedObj);
        }
        public static void UpdatePortJson((IPAddress ip,List<int> port) scanResult,string fileName, string scanStatus)
        {
            Debug.Log("newSave");
            List<Device> devicesList = LoadJson(fileName);
            var (ip, port) = scanResult;
            var isNew = IsNewDevice(devicesList, ip);
            if (!isNew.Item1)
            {
                List<int> portList= devicesList[isNew.Item2].Port;
                portList.AddRange(port);
            }
            else
            {
                devicesList.Add(new Device(){IP=ip.ToString(),Port=port, scanStatus = scanStatus,hostName = ScanIp.GETHostName(ip)});
            }
            SaveJson(devicesList, fileName);
        }
        public static void SaveJson(List<Device> devicesList, string fileName)
        {
            string jsonSerializedObj = JsonConvert.SerializeObject(devicesList, Formatting.Indented);
            Directory.CreateDirectory("Results");
            string path = Path.Combine("Results", fileName+".json");
            File.WriteAllText(path, jsonSerializedObj);
        }

        public static void UpdateFlawJson(string ip, int nbOfSFlaw, int severityLevel,string fileName)
        {
            List<Device> devicesList = LoadJson(fileName);
            if (IsNewDevice(devicesList, IPAddress.Parse(ip)).Item1)
                devicesList.Add(new Device(){IP=ip});
            
            int idDevice=FindeDevice(ip, devicesList);
            devicesList[idDevice].severityLevel = severityLevel.ToString();
            devicesList[idDevice].nbOfSFlaw = nbOfSFlaw.ToString();
            SaveJson(devicesList,fileName);
        }

        public static int FindeDevice(string ip,List<Device> devicesList)
        {
            var i = 0;
            while (i<devicesList.Count)
            {
                if (devicesList[i].IP.Equals(ip))
                {
                    return i;
                }

                i++;
            }
            return i;
        }
        
        private static (bool,int) IsNewDevice(List<Device> devicesList, IPAddress ip)
        {
            var i = 0;
            bool newDevice = true;
            while (newDevice && i<devicesList.Count)
            {
                if (IPAddress.Parse(devicesList[i].IP).Equals(ip))
                {
                    newDevice = false;
                }
            }
            return (newDevice,i);
        }

        public static List<Device> LoadJson(string fileName)
        {
            List<Device> devicesList = new List<Device>();
            if (File.Exists("Result/"+fileName+".json"))
            {
                string json = File.ReadAllText("Result/"+fileName+".json");
                devicesList = JsonConvert.DeserializeObject<List<Device>>(json);
            }
            return devicesList;
        }
        public class Device
        {
            public string hostName { get; set; }
            public string IP { get; set; }
            public List<int> Port { get; set; }
            public string nbOfSFlaw { get; set; }
            //sevérité CVE
            public string severityLevel { get; set; }
            public string scanStatus { get; set; }
            
        }
    }
}