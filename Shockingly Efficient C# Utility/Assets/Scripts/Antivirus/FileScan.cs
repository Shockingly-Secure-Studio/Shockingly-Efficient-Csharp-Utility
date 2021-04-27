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
        
        string url = "https://www.virustotal.com/vtapi/v2/file/scan";
        string file = "file=@"+path;
        HttpWebRequest requestObj = (HttpWebRequest)WebRequest.Create(url);
        requestObj.Method = "POST";
        requestObj.PreAuthenticate = true;
        requestObj.Headers.Add("x-apikey","5b68b8d063fe1421b39ac7e8bfab8baee2b893200e1078b425045cfa09b2ae58");


    }
}
