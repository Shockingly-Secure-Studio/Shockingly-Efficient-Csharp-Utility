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
using Newtonsoft.Json;
using System.Linq;
using UnityEngine.UI;
public class Scans : MonoBehaviour
{

    public GameObject content;
    public GameObject prefabCo;
    // Start is called before the first frame update
    void Start()
    {

        setup(ExternalConnexion());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void setup(List<string[]> co){
        float acc = 0;
        for(int j=0; j<co.Count; j++){
            var c = co[j];
            
            GameObject Connexion = Instantiate(prefabCo, new Vector3(-7.8f , -5f, 0), Quaternion.identity,content.transform) as GameObject;
            Connexion.transform.position -= new Vector3(0,acc,0);
            acc += 0.5f;
            UnityEngine.Debug.Log("1 New COOOOO");
            for(int i =0; i< Connexion.transform.childCount; i++){
                Text tmp = Connexion.transform.GetChild(i).gameObject.GetComponent<Text>(); //risk,asn_organization,localisation,country,threat,ip 
                if(tmp.name == "criticité")
                    tmp.text = c[0];
                if(tmp.name == "organisation")
                    tmp.text = c[1];
                if(tmp.name == "localisation")
                    tmp.text = c[3];
                if(tmp.name == "ip")
                    tmp.text = c[c.Length -1 ];

                
            }
        }
       
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
        string val = System.Convert.ToBase64String(plainTextBytes); //Encode les autorisations
        httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + val); //Ajoute les autorisations
        
        string result = httpClient.GetStringAsync(url).Result;
        string[] infos = result.Split(',');

        string risk = "";
        string asn_organization = "";
        string latitude = "";
        string longitude = "";
        string country = "";
        string threat = "";
        foreach(var e in infos){
            string[] acc = e.Split(',');
            foreach(var tmp in acc){
                if(e.Contains("risk")){     
                    string[] tmp1 = tmp.Split(':');
                    risk = tmp1[tmp1.Length -1].Replace('"',' ');
                    risk = risk.Replace('}',' ');
                }
                if(e.Contains("asn_organization")){
                    string[] tmp1 = tmp.Split(':');
                    asn_organization = tmp1[tmp1.Length -1].Replace('"',' ');
                }
                if(e.Contains("country")){
                    string[] tmp1 = tmp.Split(':');
                    country = tmp1[tmp1.Length -1].Replace('"',' ');
                }
                if(e.Contains("threat")){
                    string[] tmp1 = tmp.Split(':');
                    threat = tmp1[tmp1.Length -1].Replace('"',' ');
                }
                if(e.Contains("latitude")){
                    string[] tmp1 = tmp.Split(':');
                    latitude = tmp1[tmp1.Length -1].Replace('"',' ');
                }
                if(e.Contains("longitude")){
                    string[] tmp1 = tmp.Split(':');
                    longitude = tmp1[tmp1.Length -1].Replace('"',' ');
                }
            } 
        }
    
        string localisation = latitude + ":" + longitude;
        UnityEngine.Debug.Log("Risk:" + risk);
        UnityEngine.Debug.Log("asn_organization:" + asn_organization);
        UnityEngine.Debug.Log("localisation:" + localisation);
        UnityEngine.Debug.Log("country:" + country);
        UnityEngine.Debug.Log("threat:" + threat);
        return risk+","+asn_organization+","+localisation+","+country+","+threat;
    }
    public List<string[]> ExternalConnexion(){
        IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
        TcpConnectionInformation[] tcpConnections = ipProperties.GetActiveTcpConnections();

        List<string[]> UnknowConnexion = new List<string[]>();

        int protection = 0;

        foreach (TcpConnectionInformation tcpInfo in tcpConnections){
            /*if(protection == 4){ // In order to avoid flooding API during test phase
                return UnknowConnexion;
            }*/
            bool FromLocal = false;
            string ipDest = tcpInfo.RemoteEndPoint.Address.ToString().Split(':')[0];
            if(ipDest == "127.0.0.1" || ipDest == "0.0.0.0"){
                FromLocal = true;
            }

            if(tcpInfo.State == TcpState.Established && !FromLocal  ) // If ESTABLISHED and not from local
            {
                UnityEngine.Debug.Log("1 connexion from " + tcpInfo.LocalEndPoint.ToString() + " to "+ tcpInfo.RemoteEndPoint.ToString());
                string ip = tcpInfo.LocalEndPoint.Address.ToString();
                string ipinfo = checkIP(ip); //risk,asn_orga,localisation,country,threat
                
                string[] tmp = ipinfo.Split(',');
                UnityEngine.Debug.Log(tmp);
                int check = 0;
                bool succes = Int32.TryParse(tmp[1],out check);
                UnityEngine.Debug.Log(succes);
                Array.Resize(ref tmp, tmp.Length + 1);
                tmp[tmp.Length -1] = ip;
                UnknowConnexion.Add(tmp); 
                //protection++;
                
            }
        }
        return UnknowConnexion;
    }

    public void FindRSA(){
        var RSAKeys = from file in Directory.GetFiles("C:\\", "*.*", SearchOption.AllDirectories) where (file == "id_rsa" || file == "id_rsa.pub") select file;
        foreach(var key in RSAKeys){
            UnityEngine.Debug.Log(key); 
            // Use RSACtfTools here
        }
    }

    public void ScanAFile(string path){
        string apiKey = "apikey=5b68b8d063fe1421b39ac7e8bfab8baee2b893200e1078b425045cfa09b2ae58";
        string url = "https://www.virustotal.com/vtapi/v2/file/scan";
        string file = "file=@"+path;
        HttpWebRequest requestObj = (HttpWebRequest)WebRequest.Create(url);
        requestObj.Method = "GET";
        requestObj.PreAuthenticate = true;


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
