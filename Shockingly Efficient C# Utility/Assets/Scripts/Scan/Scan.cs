using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
//using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Ping = System.Net.NetworkInformation.Ping;
using System.Diagnostics;
using System.Linq;
using Debug = UnityEngine.Debug;

public class Scan: MonoBehaviour
{
    private List<IPAddress> ipList=new List<IPAddress>();//tableau [ip,[port]] (list couple Ip, port)
    //private readonly object ipListLock = new object();
    public Scan()//TODO maske de sous réseau
    {
    }

    public static string GETLocalIp()
    {
        /*string ip = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].ToString();*/
        IPAddress[] ipList = Dns.GetHostAddresses(Dns.GetHostName());//recherche la liste d'adrese ip associer a notre machine
        foreach (IPAddress ip in ipList)
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                Debug.Log("My ip:"+ip);
                return ip.ToString();
            }

        return null;
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
                return ("172.16.0.0","172.31.255.255");//classe B 	172.16.0.0 à 172.31.255.255
            case 192:
                return ("192.168.1.0","192.168.255.255");//classe C  192.168.1.0 à 192.168.255.255
            default:
                return ("0.0.0.0","0.0.0.0"); 
        }
    }
    public async void makePing()
    {
        (string,string) ipRange = ReturnIpRange();
        int[] ipStart = ipRange.Item1.Split('.').Select(int.Parse).ToArray();
        int[] ipEnd = ipRange.Item2.Split('.').Select(int.Parse).ToArray();
        IPAddress ip;
        Task<IPAddress> pingTask;
        var pingTaskList = new List<Task>();
        for (var i = ipStart[3]; i < ipEnd[3]; i++)
        {
            for (var j = ipStart[2]; j < ipEnd[2]; j++)
            {
                for (var n = ipStart[1]; n < ipEnd[1]; n++)
                {
                    ip = IPAddress.Parse(ipStart[0] + "." + n + "." + j + "." + i);
                    //pingTask = PingAsync(ip);
                    pingTaskList.Add(PingAsync(ip));
                }
            }
        }
        //waite for all
        while (pingTaskList.Count > 0)
        {
            //Task finishedTask = await Task.WhenAny(pingTaskList);
            //pingTaskList.Remove(finishedTask);
            Task<IPAddress> taskResult = await Task.WhenAny(pingTaskList) as Task<IPAddress>;
            IPAddress newIp = await taskResult;
            pingTaskList.Remove(taskResult);
            //IPAddress newIp = await pingTask;
            if (newIp != null)
            {
                UnityEngine.Debug.Log("New ip found"+newIp);
                ipList.Add(newIp);
            }
        }
        
    }
    
    //ping
    //asynic task
    private  static async Task<IPAddress> PingAsync(IPAddress ip)
    {
        Ping pingSender = new Ping ();
        int timeout = 120;//TODO
        PingReply reply = await pingSender.SendPingAsync(ip, timeout);
        if (reply !=null && reply.Status == IPStatus.Success)
        {
            return ip;
        }
        return null;
    }
    


}
