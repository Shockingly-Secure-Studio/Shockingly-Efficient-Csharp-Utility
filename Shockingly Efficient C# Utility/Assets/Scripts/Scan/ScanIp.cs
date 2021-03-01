using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Ping = System.Net.NetworkInformation.Ping;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using Scan;
using UnityEditor.Experimental.GraphView;
using Debug = UnityEngine.Debug;

public class ScanIp
{
    public List<IPAddress> ipList=new List<IPAddress>();//tableau [ip,[port]] (list couple Ip, port)
    public List<(IPAddress, List<int>)> results;
    
    public ScanIp()
    {
    }

    public static string GETLocalIp()
    {
        IPHostEntry ipLocal = Dns.GetHostEntry("");//recherche la liste d'adrese ip associer a notre machine
        foreach (IPAddress ip in ipLocal.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                Debug.Log("My ip:" + ip);
                //return ip.ToString();
                return "127.0.0.1";//TODO effacer
                //TODO vérifier l'interface de l'adresse
            }
        }
        return ""; 
    }
    public static (string,string) ReturnIpRange()
    {
        string ip = GETLocalIp();
        uint firstOctet = uint.Parse(ip.Split('.')[0]);
        switch (firstOctet)
        {
            case 10:
                return ("10.0.0.0","10.255.255.255");//classe A, 10.0.0.0 à 10.255.255.255
            case 172:
                return ("172.16.0.0","172.31.255.255");//classe B, 	172.16.0.0 à 172.31.255.255
            case 192:
                return ("192.168.1.0","192.168.255.255");//classe C,  192.168.1.0 à 192.168.255.255
            default:
                return ("127.0.0.1","127.0.0.1"); 
        }
    }
    public async void makePing()
    {
        List<IPAddress> ipList=new List<IPAddress>();
        (string,string) ipRange = ReturnIpRange();
        int[] ipStart = ipRange.Item1.Split('.').Select(int.Parse).ToArray();
        int[] ipEnd = ipRange.Item2.Split('.').Select(int.Parse).ToArray();
        IPAddress ip;
        var pingTaskList = new List<Task>();
        Debug.Log("start:"+ipRange.Item1+"end:"+ipRange.Item2);
        for (var i = ipStart[3]; i <= ipEnd[3]; i++)
        {
            for (var j = ipStart[2]; j <= ipEnd[2]; j++)
            {
                for (var n = ipStart[1]; n <= ipEnd[1]; n++)
                {
                    ip = IPAddress.Parse(ipStart[0] + "." + n + "." + j + "." + i);
                    pingTaskList.Add(PingAsync(ip));
                }
            }
        }
        while (pingTaskList.Count > 0)
        {
            Task<IPAddress> taskResult = await Task.WhenAny(pingTaskList) as Task<IPAddress>;
            IPAddress newIp = await taskResult;
            pingTaskList.Remove(taskResult);
            if (newIp != null)
            {
                UnityEngine.Debug.Log("New ip found"+newIp);
                Debug.Log("Host name:"+Dns.GetHostEntry(newIp).HostName);
                ipList.Add(newIp);//on peut aussi récuperer les adresse mac et nom NetBios
            }
        }

        Debug.Log("FIN DU SCAN IP");
        results = await makePortScan(ipList);
        new SaveScan().NewJson(results);
    }
    private  static async Task<IPAddress> PingAsync(IPAddress ip)
    {
        Ping pingSender = new Ping ();
        int timeout = 120;
        try
        {
            PingReply reply = await pingSender.SendPingAsync(ip, timeout);
            if (reply !=null && reply.Status == IPStatus.Success)
            {
                return ip;
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    //test pour le scan de port
    public async static Task<List<(IPAddress, List<int>)>> makePortScan (List<IPAddress> ipList)
    {
        var portScanRange = (1, 10000);
        var portScanTaskList = new List<Task>();
        var data = new List<(IPAddress, List<int>)>();
        Debug.Log("port start:");
        foreach (var ip in ipList)
        {
            portScanTaskList.Add(ScanPort.scanTask(ip,(portScanRange.Item1,portScanRange.Item2)));
        }
        while (portScanTaskList.Count > 0)
        {
            Task<(IPAddress,List<int>)> taskResult = await Task.WhenAny(portScanTaskList) as Task<(IPAddress,List<int>)>;
            (IPAddress ip,List<int> portList) = await taskResult;
            portScanTaskList.Remove(taskResult);
            if (portList.Count !=0)
            {
                UnityEngine.Debug.Log("ip:"+ip);
                data.Add((ip,portList));
                foreach (var p in portList)
                {
                    Debug.Log("New port found:"+p);
                }
                
            }
        }
        Debug.Log("FIN DU SCAN Port");
        return data;
    }
    


}
