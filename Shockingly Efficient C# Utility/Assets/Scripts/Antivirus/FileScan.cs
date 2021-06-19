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



public class FileScan : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ScanAFile(string path){
        Debug.Log("New file scan : " + path);
        string url = "https://www.virustotal.com/vtapi/v2/file/scan";
        string file = "file=@"+path;
        byte[] apikey = Encoding.ASCII.GetBytes("x-apikey=5b68b8d063fe1421b39ac7e8bfab8baee2b893200e1078b425045cfa09b2ae58 & ");
        FileStream fstream = new FileStream(path,FileMode.Open);
        Debug.Log("Taille = "+ fstream.Length);
        HttpWebRequest requestObj = (HttpWebRequest)WebRequest.Create(url);
        requestObj.Method = "POST";
        requestObj.ContentLength = fstream.Length;
        requestObj.PreAuthenticate = true;
        requestObj.AllowWriteStreamBuffering = true;
        Stream request_stream = requestObj.GetRequestStream();

        // Send apikey // 
        request_stream.Write(apikey, 0, apikey.Length);

        // Send File //
        byte[] indata = new byte[fstream.Length];
        int bytes_read = fstream.Read(indata, apikey.Length+1, indata.Length);
        while (bytes_read > 0)
        {
            request_stream.Write(indata, 0, indata.Length);
            bytes_read = fstream.Read(indata, 0, indata.Length);
        }

        
        Debug.Log(request_stream);
        fstream.Close();
        request_stream.Close();
        requestObj.GetResponse();
        

    }
}
