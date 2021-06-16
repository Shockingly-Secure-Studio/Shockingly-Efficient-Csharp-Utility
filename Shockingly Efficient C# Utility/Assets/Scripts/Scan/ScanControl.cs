
using System.Linq;
using System.Net;
using System.Threading;
using UnityEngine;

namespace Scan
{
    public class ScanControl : MonoBehaviour
    {
        private (string,string) _ipRange;
        private string _portScanType;
        public ScanControl(string ipRangeI,string portScanType)
        {
            string[] input = ipRangeI.Split('/');//TODO vérifier avant
            if (input[0] == "")
            {
                (string, string) ip_et_mask = ScanIp.GETLocalIp();
                string ip = ip_et_mask.Item1;
                string mask = ip_et_mask.Item2;
                _ipRange = ScanIp.ReturnIpRange(ip, mask);
                Debug.Log(_ipRange.Item1);
                Debug.Log(_ipRange.Item2);
            }
            else if (input[1] == "")
            {
                _ipRange=(input[0],input[0]);
            }
            else
            {
                _ipRange = (input[0],input[1]);
            }
            _portScanType = portScanType;//all,fast
        }
        
        /// <summary>
        /// Start the scan of the network in a separate thread.
        /// </summary>
        /// <returns>The thread representing the scanning process. When this thread ends, the exploitation is complete.</returns>
        public Thread Scan()
        {
            ScanIp o = new ScanIp();
            Thread newScan = new Thread(() => o.MakePing(_ipRange,_portScanType));
            newScan.Start();
            MenuManager.IsThreadRunning = true;
            // TODO: Bien détecter quand le thread est terminé
            return newScan;
        }
        private static bool MAXCheck(int i,uint[] octet)
        {
            bool status = true;
            for (; i < 3;i++)
            {
                if (octet[i] > 255)
                {
                    status = false;
                }
            }
            return status;
        }
        public static bool CheckIp (string ip)
        {
            if (ip == "127.0.0.1")
                return true;
            uint[] Octet = ip.Split('.').Select(uint.Parse).ToArray();
            bool status = true;
            switch (Octet[0])
            {
                case 10:
                    return MAXCheck(1, Octet);//classe A, 10.0.0.0 à 10.255.255.255
                case 172:
                    if (Octet[1] < 16 || Octet[1] > 31)
                        status = false;
                    return MAXCheck(2,Octet)&&status;//classe B, 	172.16.0.0 à 172.31.255.255
                case 192:
                    if (Octet[1] != 168)
                        status = false;
                    if (Octet[2] < 1)
                        status = false;
                    return MAXCheck(2,Octet)&&status;//classe C,  192.168.1.0 à 192.168.255.255
                default:
                    return false; 
            }
        }
    }
}