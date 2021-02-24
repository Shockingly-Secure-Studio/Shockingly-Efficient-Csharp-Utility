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
using Debug = UnityEngine.Debug;

public class Scan: MonoBehaviour
{
    private (string, string) ipRange = ("", "");
    private List<IPAddress> ipList=new List<IPAddress>();//tableau [ip,[port]] 
    private readonly object ipListLock = new object();
    public Scan()//TODO maske de sous réseau
    {
        //UnityEngine.Debug.Log(ReturnSubnetmask());
    }

    public static string GETLocalIp()
    {
        /*string ip = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].ToString();*/
        IPAddress[] ipList = Dns.GetHostAddresses(Dns.GetHostName());//recherche la liste d'adrese ip associer a notre machine
        foreach (IPAddress ip in ipList)
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                Debug.Log(ip);
                return ip.ToString();
            }

        return null;
    }
    static public string ReturnSubnetmask()
    {
        string ip = GETLocalIp();
        uint firstOctet = uint.Parse(ip.Split('.')[0]);
        if (firstOctet <= 127)//classe A, 1.0.0.1 à 126.255.255.254
            return "255.0.0.0";
        if (firstOctet >= 128 && firstOctet <= 191)//classe B 	128.1.0.1 à 191.255.255.254
            return "255.255.0.0";
        if (firstOctet >= 192 && firstOctet <= 223)//classe C 192.0.1.1 à 223.255.254.254
            return "255.255.255.0";//faire fonction ipv4
        return "0.0.0.0";
    }
    
    public async void makePing((string, string) ipRange)
    {
        IPAddress ip= IPAddress.Parse("127.0.0.1");//regarder le msque sous reseau trouver la plage d'aresse,
        Task<IPAddress> pingTask = PingAsync(ip);
        IPAddress newIp = await pingTask;
        UnityEngine.Debug.Log(newIp);
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
