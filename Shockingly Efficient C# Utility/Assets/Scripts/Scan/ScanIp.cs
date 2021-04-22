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
using System.Threading;
using Scan;
using Debug = UnityEngine.Debug;

namespace Scan
{
    public class ScanIp
{
    //si ip vide tt tout seul sinon scan a l'aide de l'ip
    public List<(IPAddress, List<int>)> Results;
    
    public static string GETLocalIp()
    {
        IPHostEntry ipLocal = Dns.GetHostEntry("");//recherche la liste d'adrese ip associer a notre machine
        foreach (IPAddress ip in ipLocal.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                Debug.Log("My ip:" + ip);
                return ip.ToString();
                //return "127.0.0.1";
                //TODO vérifier l'interface de l'adresse
            }
        }
        return ""; 
    }
    public static (string,string) ReturnIpRange(string ip)
    {
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
    
    public async void MakePing((string,string) ipRange,string scanType)
    {
        List<IPAddress> ipList=new List<IPAddress>();
        int[] ipStart = ipRange.Item1.Split('.').Select(int.Parse).ToArray();
        int[] ipEnd = ipRange.Item2.Split('.').Select(int.Parse).ToArray();
        IPAddress ip;
        var pingTaskList = new List<Task>();
        Debug.Log("start:"+ipRange.Item1+"end:"+ipRange.Item2);
        
        /*if (ipRange.Item1 == ipRange.Item2)//scan only on Device
        {
            ipList.Add(IPAddress.Parse(ipRange.Item1));
            ScanPort.MakePortScan(ipList,scanType);
            return;
        }*/
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
            Task<(IPAddress,bool)> taskResult = await Task.WhenAny(pingTaskList) as Task<(IPAddress,bool)>;
            (IPAddress,bool) newIp=taskResult.Result;
            pingTaskList.Remove(taskResult);
            if (newIp.Item2)
            {
                UnityEngine.Debug.Log("New ip found"+newIp.Item1);
                Debug.Log("Host name:"+Dns.GetHostEntry(newIp.Item1).HostName);
                ipList.Add(newIp.Item1);
                SaveScan.SaveIpScan("ipScan",ipList,$"{scanType},Underway,{newIp.Item1},{ipRange.Item2}");
            }
        }
        Debug.Log("FIN DU SCAN IP");
        SaveScan.SaveIpScan("ipScan",ipList,$"{scanType},completed");
        ScanPort.MakePortScan(ipList,scanType);
    }
    private  static async Task<(IPAddress,bool)> PingAsync(IPAddress ip)
    {
        Ping pingSender = new Ping ();
        int timeout = 128;
        try
        {
            PingReply reply = await pingSender.SendPingAsync(ip, timeout);
            if (reply !=null && reply.Status == IPStatus.Success)
            {
                return (ip,true);
            }
            return (ip, false);
        }
        catch
        {
            return (ip,false);
        }
    }
    public static string GETHostName(IPAddress ip)
    {
        return Dns.GetHostEntry(ip).HostName;
    }
}

}