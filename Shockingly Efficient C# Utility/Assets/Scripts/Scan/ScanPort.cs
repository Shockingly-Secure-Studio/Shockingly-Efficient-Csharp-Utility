using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using Service;
using Machine;

namespace Scan
{
    public class ScanPort
    {
        public static void ConnectCallback(IAsyncResult asyncResult)//Délégué AsyncCallback qui fait référence à la méthode à appeler quand l’opération de connexion est terminée.
        {
            TcpClient tcpClient = (TcpClient) asyncResult.AsyncState;

            try
            {
                tcpClient.EndConnect(asyncResult);
            }
            catch (Exception e)
            {
            }

        }
        //liste port importatn les écrire rapidement

        private static void ScanTask (IPAddress ip,(int,int) portRange, string scanType,string fileName)//mettre dans autre thread //10 par 10
        {
            Debug.Log("newScanTask");
            List<int> portList = new List<int>();
            SaveScan.UpdatePortJson((ip,portList),fileName,"Underway");
            //on scan liste des port imporant
            int[] tabPorts =
            {
                21, 22, 23, 25, 69, 80, 110, 115, 119, 123, 135, 139, 143, 194, 443, 445, 465, 554, 563, 587, 993, 995,
                2869, 5357, 8080, 10000
            };
            foreach (var port in tabPorts)
            {
                
                var tcpClient = new TcpClient();
                IAsyncResult asyncResult = tcpClient.BeginConnect(ip, port,ConnectCallback, tcpClient);
                if (asyncResult.AsyncWaitHandle.WaitOne(300, false) && tcpClient.Connected){
                    //changer le timeout
                    if(port == 80){
                        Debug.Log("WEB start exploit");
                        Machine.Machine mach = new Machine.Machine(ip.ToString());
                        WebService newWebService = new WebService(mach,"localhost",ip.ToString(),port);
                        Thread tr = new Thread(newWebService.Exploit);
                        tr.Start();
                    }
                    portList.Add(port);
                }
                tcpClient.Close();
            }
            SaveScan.UpdatePortJson((ip,portList),fileName,"MajorPortsScanCompleted");
            Debug.Log("major port have been scanned and saved!");
            if (scanType == "all")
            {
                for (var port = portRange.Item1; port < portRange.Item2; port++)
                {
                    if (!tabPorts.Contains(port))
                    {
                        var tcpClient = new TcpClient();
                        IAsyncResult asyncResult = tcpClient.BeginConnect(ip, port, ConnectCallback, tcpClient);
                        if (asyncResult.AsyncWaitHandle.WaitOne(300, false) && tcpClient.Connected) //changer le timeout
                            portList.Add(port);
                        tcpClient.Close();
                    }
                }
                SaveScan.UpdatePortJson((ip,portList),fileName,"Completed");
            }
            
        }
        
        public static void MakePortScan (List<IPAddress> ipList,string scanType)
        {
            var portScanRange = (1, 65536);
            var portScanTaskList = new List<Task>();
            var data = new List<(IPAddress, List<int>)>();
            Debug.Log("port start:");
            string fileName = "scan1";
            SaveScan.NewJson(fileName);
            foreach (var ip in ipList)
            {
                Thread scanPortIPThread = new Thread(() => ScanTask(ip,portScanRange,scanType,fileName));
                scanPortIPThread.Start();
                if(scanPortIPThread.ThreadState != ThreadState.Running);//.join attendre la fin
            }
        }
        
    }
    
}