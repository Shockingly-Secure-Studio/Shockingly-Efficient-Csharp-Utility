
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Service;
using Service.Exploit;
using TMPro;
using UnityEngine;

public class XSS
{
    private string _pathPayload;
    private InputWebService _input;
    private string _url;

    public XSS(InputWebService inputs,string url)
    {
        _input = inputs;
        _pathPayload=@".\Assets\Scripts\Service\xss_payloads";
        _url = url;
    }
    public async Task TestPayload()
    {
        if (File.Exists(_pathPayload))
        {
            using var file = new System.IO.StreamReader(_pathPayload);
            string payload;
            while((payload = await file.ReadLineAsync()) != null)
            {
                
                string result=await _input.Submit(payload);
                if (result.Contains(payload))
                {
                    AccessPoint accessPoint = new AccessPoint(_url, payload, AccessPointType.XSS, 6);
                    _input.Log(accessPoint);
                    return;
                }
            }
        }
        Debug.Log("XSS: file \""+_pathPayload+"\" not find");
    }
    
}
