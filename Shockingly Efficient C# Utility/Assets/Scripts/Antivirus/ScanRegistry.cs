using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Microsoft.Win32;
using Microsoft.VisualBasic;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Net.NetworkInformation;
using System.IO;

public class ScanRegistry : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        checkIP("163.5.73.7");
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public string checkIP (string ip){
        string url = "https://api.fraudguard.io/v2/ip/";
        url += ip;
        HttpWebRequest requestObj = (HttpWebRequest)WebRequest.Create(url);
        requestObj.Method = "GET";
        requestObj.PreAuthenticate = true;

        HttpMessageHandler handler = new HttpClientHandler(){}; 
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(url),
            Timeout = new TimeSpan(0, 2, 0)
        };
        httpClient.DefaultRequestHeaders.Add("ContentType", "application/json");
        
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes("CkyJOcTkBVswQKZg:6gJIDebITSgCJN6z");
        string val = System.Convert.ToBase64String(plainTextBytes);
        httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + val);
        HttpResponseMessage response = httpClient.GetAsync(url).Result;
        string content = string.Empty;
        
        UnityEngine.Debug.Log(response.Content);
        return response.Content.ToString();
    }
    public void ExternalConnexion(){
        IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
        TcpConnectionInformation[] tcpConnections = ipProperties.GetActiveTcpConnections();

        List<string> UnknowConnexion = new List<string>();

        foreach (TcpConnectionInformation tcpInfo in tcpConnections){
            UnityEngine.Debug.Log("1 connexion from " + tcpInfo.LocalEndPoint.ToString() + " to "+ tcpInfo.RemoteEndPoint.ToString());
            if(tcpInfo.State == TcpState.Established) // Si la connexion est établie
            {
                string ip = tcpInfo.LocalEndPoint.Address.ToString();
                string ipinfo = checkIP(ip);
                if(1 == 2) // Test to know if it's a dangerous IP
                {
                    UnknowConnexion.Add(tcpInfo.LocalEndPoint.Address.ToString()); 
                }
            }
        }
    }


    /* Bug The type name 'RegistryKey' could not be found in the namespace 'Microsoft.Win32'. 
    public void Registery(){
        string[] endpoint = new string[]{"HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Run","HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\RunOnce","HKEY_LOCAL_MACHINE\\Software\\Microsoft\\Windows\\CurrentVersion\\Run","HKEY_LOCAL_MACHINE\\Software\\Microsoft\\Windows\\CurrentVersion\\RunOnce"};
        
        RegistryKey branch = Registry.CurrentUser;
        foreach (string path in endpoint){
            branch = branch.OpenSubKey(path,Microsoft.Win32.RegistryKeyPermissionCheck.ReadSubTree, System.Security.AccessControl.RegistryRights.ReadKey);
            var subKeyNames =branch.GetSubKeyNames();
            var RegistryKeysList = new List<string>();
            var ListProgram = new List<string>();
            foreach (string subkey_name in branch.GetSubKeyNames()){
                using (RegistryKey subkey = branch.OpenSubKey(subkey_name)){
                    RegistryKeysList.Add((string)subkey.GetValue("DisplayName"));
                    ListProgram.Add( subkey.GetValue( "" )?.ToString());
                    UnityEngine.Debug.Log("Path: "+subkey.GetValue( "" )?.ToString());
                    UnityEngine.Debug.Log("Name: "+subkey.GetValue( "DisplayName" )?.ToString());
                }
            }
        }

    }
    */


}
