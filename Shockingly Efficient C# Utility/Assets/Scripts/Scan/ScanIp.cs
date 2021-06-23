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
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace Scan
{
    public class ScanIp: MonoBehaviour
{
    //si ip vide tt tout seul sinon scan a l'aide de l'ip
    public List<(IPAddress, List<int>)> Results;
    public static (string, string) GETLocalIp()
    {
        IPHostEntry ipLocal = Dns.GetHostEntry("");//recherche la liste d'adrese ip associer a notre machine

        foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (ni.Name == NetworkInterfaceDropdown.dropdown.options[NetworkInterfaceDropdown.dropdown.value].text)
            {
                Debug.Log(ni.Name);
                foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        Debug.Log(ip.Address.ToString());
                        
                        return (ip.Address.ToString(), ip.IPv4Mask.ToString());
                    }
                }
            }
        }

        foreach (IPAddress ip in ipLocal.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                Debug.Log("My ip:" + ip);
                return (ip.ToString(), "ip");
                //return "127.0.0.1";
            }
        }
        return ("", "IPAddress.Any"); 
    }
    public static (string,string) ReturnIpRange(string ip, string mask)
    {
        string[] parts = ip.Split('.');
        string[] mask_part = mask.Split('.');

        if (mask_part[0] == "0") // mask == 0.0.0.0 
            return ("0.0.0.0", "255.255.255.255");
        if (mask_part[1] == "0") // mask == 255.0.0.0
            return (parts[0] + ".0.0.0", parts[0] + ".255.255.255");
        if (mask_part[2] == "0") // mask == 255.255.0.0
            return ($"{parts[0]}.{parts[1]}.0.0", $"{parts[0]}.{parts[1]}.255.255");
        if (mask_part[3] == "0") // mask == 255.255.255.0
            return ($"{parts[0]}.{parts[1]}.{parts[2]}.0", $"{parts[0]}.{parts[1]}.{parts[2]}.255");
        
        // mask == 255.255.255.255
        
        return (ip, ip);
    }
    
    /// <summary>
    /// Ping all ip addresses in range and if they respond back, start scanning their ports and exploiting them.
    /// The function waits for the exploitation to end.
    /// </summary>
    /// <param name="ipRange">Couple representing the lower and upper bound of the addresses to scan</param>
    /// <param name="scanType">If scanType == "all", perform a more thourough (and slow) process, i.e. scan all ports</param>
    public async Task MakePing((string,string) ipRange,string scanType)
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
        ScanPort.MakePortScan(ipList, scanType);

        MenuManager.IsThreadRunning = false;
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