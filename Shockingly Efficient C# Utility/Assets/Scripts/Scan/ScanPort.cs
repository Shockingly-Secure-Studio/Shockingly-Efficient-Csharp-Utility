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
                        WebService newWebService = new WebService(mach, port, ip.ToString());
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
        //RAW socket ? 
        //Socket socketTest = Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Tcp);//TCP 
        //ip header
        //TCP header
        public void test()
        {
            string source_ip = "192.168.1.101";
            string dest_ip = "192.168.1.1";
            int ip_ihl = 5;//ip header len
            int ip_version = 4;
            int ip_tos = 0;
            int ip_total_lenth = 0;	//# kernel will fill the correct total length 
            int ip_id = 54321;	//#Id of this packet
            int ip_frag_off = 0;
            int ip_ttl = 255;
            ProtocolType ip_proto = ProtocolType.Tcp;
            int ip_checksum = 0;	//# kernel will fill the correct checksum
            IPAddress ip_saddr = IPAddress.Parse(source_ip);	//#Spoof the source ip address if you want to
            IPAddress ip_daddr = IPAddress.Parse( dest_ip );
            //int ip_ihl_ver = (version << 4) + ihl; 
            //ip_header = ('!BBHHHBBH4s4s' ,ip_ihl_ver, ip_tos, ip_tot_len, ip_id, ip_frag_off, ip_ttl, ip_proto, ip_check, ip_saddr, ip_daddr);

            int tcp_source = 1234;	//source port
            int  tcp_dest = 80;	// destination port
            int tcp_seq = 454;
            int tcp_ack_seq = 0;
            int tcp_doff = 5;	//4 bit field, size of tcp header, 5 * 4 = 20 bytes
            //tcp flags
            int tcp_fin = 0;
            int tcp_syn = 1;
            int tcp_rst = 0;
            int tcp_psh = 0;
            int tcp_ack = 0;
            int tcp_urg = 0;
            //tcp_window = socket.htons(5840);	//	maximum allowed window size
            int tcp_check = 0;
            int tcp_urg_ptr = 0;
            int tcp_offset_res = (tcp_doff << 4) + 0;
            int tcp_flags = tcp_fin + (tcp_syn << 1) + (tcp_rst << 2) + (tcp_psh << 3) + (tcp_ack << 4) + (tcp_urg << 5);
            // the ! in the pack format string means network order
            //tcp_header = pack('!HHLLBBHHH' , tcp_source, tcp_dest, tcp_seq, tcp_ack_seq, tcp_offset_res, tcp_flags,  tcp_window, tcp_check, tcp_urg_ptr)
            //https://inc0x0.com/tcp-ip-packets-introduction/tcp-ip-packets-3-manually-create-and-send-raw-tcp-ip-packets/
            /*s = socket.socket(socket.AF_INET, socket.SOCK_RAW, socket.IPPROTO_TCP)
            s.setsockopt(socket.IPPROTO_IP, socket.IP_HDRINCL, 1)

            ip_header  = b'\x45\x00\x00\x28'  # Version, IHL, Type of Service | Total Length
            ip_header += b'\xab\xcd\x00\x00'  # Identification | Flags, Fragment Offset
            ip_header += b'\x40\x06\xa6\xec'  # TTL, Protocol | Header Checksum
            ip_header += b'\x0a\x0a\x0a\x02'  # Source Address
            ip_header += b'\x0a\x0a\x0a\x01'  # Destination Address

            tcp_header  = b'\x30\x39\x00\x50' # Source Port | Destination Port
            tcp_header += b'\x00\x00\x00\x00' # Sequence Number
            tcp_header += b'\x00\x00\x00\x00' # Acknowledgement Number
            tcp_header += b'\x50\x02\x71\x10' # Data Offset, Reserved, Flags | Window Size
            tcp_header += b'\xe6\x32\x00\x00' # Checksum | Urgent Pointer

            packet = ip_header + tcp_header
            s.sendto(packet, ('10.10.10.1', 0))*/
        }
    }
    
}