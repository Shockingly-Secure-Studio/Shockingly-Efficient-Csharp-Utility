using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using JetBrains.Annotations;
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

        private static void ScanTask (IPAddress ip,(int,int) portRange, string scanType,string fileName,Machine.Machine mach)
        {
            Debug.Log("newScanTask :" + ip.ToString() + " type=" + scanType);
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
                //Debug.Log("Test port:" + ip + ":" + port);
                var tcpClient = new TcpClient();
                IAsyncResult asyncResult = tcpClient.BeginConnect(ip, port,ConnectCallback, tcpClient);
                if (asyncResult.AsyncWaitHandle.WaitOne(300, false) && tcpClient.Connected){
                    //changer le timeout
                    if(port == 80|| port==443 || port==8080)
                    {
                        Debug.Log("WEB start exploit");
                        WebService newWebService = new WebService(mach, port, ip.ToString());
                        Thread tr = new Thread((async () => await newWebService.Exploit()));
                        tr.Start();
                    }

                    if (port == 21)
                    {
                        Debug.Log("FTP start exploit");
                        FTPService ftpService = new FTPService(mach, port);
                        Thread tr = new Thread(ftpService.Exploit);
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
                        if (asyncResult.AsyncWaitHandle.WaitOne(300, false) && tcpClient.Connected)
                            portList.Add(port);
                        tcpClient.Close();
                    }
                }
                SaveScan.UpdatePortJson((ip,portList),fileName,"Completed");
            }
            
        }
        private static void SendCallback(IAsyncResult asyncResult)
        {
            UdpClient u = (UdpClient) asyncResult.AsyncState;

            try
            {
                u.EndSend(asyncResult);
            }
            catch (Exception e)
            {
            }

        }

        private static (int,bool) UDPscan(IPAddress ip, int port)
        {
            
            UdpClient u = new UdpClient();
            try
            {
                u.Connect(ip,port);
                byte[] sendBytes = Encoding.ASCII.GetBytes("test");
                var asyncResult=u.BeginSend(sendBytes, sendBytes.Length, new AsyncCallback(SendCallback), u);
                if (asyncResult.AsyncWaitHandle.WaitOne(300, false))//icmp error port ureachable=fermé, pas de réponse ouvert
                {
                    u.Close();
                    return (port,true);
                }
                u.Close();
                return (port,true);
            }
            catch
            {
                return (port,false);
            }
        }
        public static void MakePortScan (List<IPAddress> ipList,string scanType)
        {
            var portScanRange = (1, 65536);
            var portScanTaskList = new List<Task>();
            var data = new List<(IPAddress, List<int>)>();
            Debug.Log("port start:");
            string fileName = "scanPort";
            SaveScan.NewJson(fileName);
            foreach (var ip in ipList)
            {
                Machine.Machine mach = new Machine.Machine(ip.ToString());
                Thread scanPortIPThread = new Thread(() => ScanTask(ip,portScanRange,scanType,fileName,mach));
                scanPortIPThread.Start();
                scanPortIPThread.Join();
                //if(scanPortIPThread.ThreadState != ThreadState.Running);//.join attendre la fin
            }
        }
        
    }
    
}