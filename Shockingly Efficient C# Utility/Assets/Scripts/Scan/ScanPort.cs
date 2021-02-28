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
            try
            {
                TcpClient tcpClient = (TcpClient) asyncResult.AsyncState;
                tcpClient.EndConnect(asyncResult);
            }
            catch
            {
            }
        }
        
        public static async Task<(IPAddress,List<int>)> scanTask(IPAddress ip,(int,int)portRange)
        {
            List<int> portList = new List<int>();
            var tcpClient = new TcpClient();
            for (var port = portRange.Item1; port < portRange.Item2; port++)
            {
                Debug.Log(port);
                IAsyncResult asyncResult = tcpClient.BeginConnect(ip, port, ConnectCallback, tcpClient);
                if (asyncResult.AsyncWaitHandle.WaitOne(120, false) && tcpClient.Connected)
                    portList.Add(port);
            }
            tcpClient.Close();
            return (ip,portList);
        }
        //RAW socket ?

    }
    
}