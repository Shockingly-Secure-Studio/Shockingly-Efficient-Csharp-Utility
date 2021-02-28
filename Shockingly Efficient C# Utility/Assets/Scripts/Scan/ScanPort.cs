using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEditor.Sprites;
using UnityEngine;

namespace Scan
{
    public class ScanPort: ScanIp
    { 
        public static void ConnectCallback(IAsyncResult asyncResult)
        {
            TcpClient tcpClient = (TcpClient)asyncResult.AsyncState;
            tcpClient.EndConnect(asyncResult);
        }
        public static async Task<(IPAddress,List<int>)> scanTask(IPAddress ip,(int,int)portRange)
        {
            List<int> portList = new List<int>();
            var tcpClient = new TcpClient();//try
            for (var port = portRange.Item1; port < portRange.Item2; port++)
            {
                IAsyncResult asyncResult = tcpClient.BeginConnect(ip, port, ConnectCallback, tcpClient);
                if (asyncResult.AsyncWaitHandle.WaitOne(120,false)==false || tcpClient.Connected==false)
                {
                    continue;
                }
                portList.Add(port);
            }
            tcpClient.Close();
            return (ip,portList);
        }
        //RAW socket

    }
    
}