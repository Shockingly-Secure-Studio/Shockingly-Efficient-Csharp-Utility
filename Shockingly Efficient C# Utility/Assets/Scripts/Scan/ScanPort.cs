using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEditor.Sprites;
using UnityEngine;

namespace Scan
{
    public class ScanPort
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
        
        public static async Task<(IPAddress, List<int>)> ScanTask(IPAddress ip,int port)
        {
            List<int> portList = new List<int>();
            var tcpClient = new TcpClient();
            IAsyncResult asyncResult = tcpClient.BeginConnect(ip, port,ConnectCallback, tcpClient);
            if (asyncResult.AsyncWaitHandle.WaitOne(300, false) && tcpClient.Connected)//changer le timeout
                portList.Add(port);
            tcpClient.Close();
            //await Task.WhenAny();//je comprend pas
            Debug.Log("fin scan port:"+port);
            return (ip,portList);
        }
        public static async Task<List<(IPAddress, List<int>)>> MakePortScan (List<IPAddress> ipList)
        {
            var portScanRange = (1, 655);
            var portScanTaskList = new List<Task>();
            var data = new List<(IPAddress, List<int>)>();
            Debug.Log("port start:");
            //var portScanRangeI = (portScanRange.Item2 - portScanRange.Item1) / 10;
            //var portScanRangeStart=portScanRange.Item1;
            //var portScanRangeEnd = portScanRangeStart+portScanRangeI;
            foreach (var ip in ipList)
            {
                //await ScanPort.ScanTask(ip,(portScanRangeStart,portScanRangeEnd));
                for (var n = portScanRange.Item1; n < portScanRange.Item2; n++)
                {
                    portScanTaskList.Add(ScanPort.ScanTask(ip,n));
                }
                
            }
            while (portScanTaskList.Count > 0)
            {
                Task<(IPAddress,List<int>)> taskResult = await Task.WhenAny(portScanTaskList) as Task<(IPAddress,List<int>)>;
                (IPAddress ip,List<int> portList) = await taskResult;
                Debug.Log("new result:"+taskResult);
                portScanTaskList.Remove(taskResult);
                if (portList.Count !=0)
                {
                    UnityEngine.Debug.Log("ip:"+ip);
                    bool find = false;
                    var i = 0;
                    while (!find && i<data.Count)
                    {
                        if (data[i].Item1.Equals(ip))
                        {
                            find = true;
                            data[i].Item2.AddRange(portList);
                        }
                        i++;
                    }
                    if (!find)
                    {
                        data.Add((ip,portList));
                    }
                    foreach (var p in portList)
                    {
                        Debug.Log("New port found:"+p);
                    }
                
                }
            }
            
            Debug.Log("FIN DU SCAN Port");
            return data;
        }
        //RAW socket ? 
    }
    
}