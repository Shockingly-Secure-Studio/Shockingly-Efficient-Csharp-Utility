using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Ping = System.Net.NetworkInformation.Ping;

public class Scan
{
    private (string, string) ipRange = ("", "");
    private List<IPAddress> ipList=new List<IPAddress>();//tableau [ip,[port]] 
    private readonly object ipListLock = new object();
    public Scan()//TODO maske de sous réseau
    {
    }
    public async void makePing((string, string) ipRange)
    {
        IPAddress ip= IPAddress.Parse("127.0.0.1");//regarder le msque sous reseau trouver la plage d'aresse,
        Task<IPAddress> pingTask = PingAsync(ip);
        IPAddress newIp = await pingTask;
        Debug.Log(newIp);
        if (newIp != null)
        {
            ipList.Add(newIp);
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
