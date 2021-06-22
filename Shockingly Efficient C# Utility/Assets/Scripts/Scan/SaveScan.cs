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
            if (File.Exists(path))
                return;
            File.WriteAllText(path, jsonSerializedObj);
        }
        public static void UpdatePortJson((IPAddress ip,List<int> port) scanResult,string fileName, string scanStatus)
        {
            Debug.Log("newSave");
            List<Device> devicesList = LoadJson(fileName);
            var (ip, port) = scanResult;
            var isNew=(true,0);
            if (devicesList == null)
            {
                devicesList = new List<Device>();
            }
            else
            {
                isNew = IsNewDevice(devicesList, ip);
            }
            if (!isNew.Item1)
            {
                List<int> portList= devicesList[isNew.Item2].Port;
                devicesList[isNew.Item2].scanStatus = scanStatus;
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
                else
                {
                    ++i;
                }
            }
            
            return (newDevice,i);
        }

        public static List<Device> LoadJson(string fileName)
        {
            List<Device> devicesList = new List<Device>();
            if (File.Exists("Results/"+fileName+".json"))
            {
                UnityEngine.Debug.Log("file Exist");
                string json = File.ReadAllText("Results/"+fileName+".json");
                devicesList = JsonConvert.DeserializeObject<List<Device>>(json);
            } 
            return devicesList;
        }

        public static void SaveIpScan(string fileName,List<IPAddress> ipList,string scanType)
        {
            if(!File.Exists(Path.Combine("Results", fileName+".json")))
                NewJson(fileName);
            var o = new IPSave();
            List<string> ipList2=new List<string>();
            ipList.ForEach(a => ipList2.Add(a.ToString()));
            o.ipList = ipList2;
            o.scanType = scanType;
            string jsonSerializedObj = JsonConvert.SerializeObject(o, Formatting.Indented);
            Directory.CreateDirectory(@".\Results");
            string path = Path.Combine("Results", fileName+".json");
            File.WriteAllText(path, jsonSerializedObj);
        }
        public class IPSave
        {
            public string scanType { get; set; }
            public List<string> ipList { get; set; }
        }
        public static (string,List<IPAddress>) LoadIpScan(string fileName)
        {
            List<IPAddress> ipList = new List<IPAddress>();
            string scanType="";
            string path = Path.Combine("Results", fileName+".json");
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                var o = JsonConvert.DeserializeObject<IPSave>(json);
                List<string> ipList2=o.ipList;
                ipList2.ForEach(a => ipList.Add(IPAddress.Parse(a)));
                scanType = o.scanType;
            } 
            return (scanType,ipList);
        }
        public static void SaveMap(string path,List<string> map)
        {
            NewJson(path);
            string jsonSerializedObj = JsonConvert.SerializeObject(map, Formatting.Indented);
            path = Path.Combine("Results", path+".json");
            File.WriteAllText(path, jsonSerializedObj);
        }
        public static List<string> LoadMap(string path)
        {
            List<string> map = new List<string>();
            path = Path.Combine("Results", path+".json");
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                map = JsonConvert.DeserializeObject<List<string>>(json);
            }
            else
            {
                Debug.Log($"LoadMap: Result\\{path} does not exist or could not be found");
            }
            return map;
        }
        public class Device
        {
            public string hostName { get; set; }
            public string IP { get; set; }
            public List<int> Port { get; set; }

            public string nbOfSFlaw { get; set; } = "0";
            //sevérité CVE
            public string severityLevel { get; set; } = "0";
            public string scanStatus { get; set; }
            
        }
    }
}