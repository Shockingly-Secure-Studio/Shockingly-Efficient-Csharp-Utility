
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Service;
using UnityEngine;

public class XSS
{
    private string path_payload;
    private List<InputWebService> inputList;

    public XSS(List<InputWebService> inputs)
    {
        inputList = inputs;
        path_payload=@"./xss_payloads";
    }
    public async Task<(bool,InputWebService)> TestPayload()
    {
        if (File.Exists(path_payload))
        {
            using var file = new System.IO.StreamReader(path_payload);
            string payload;
            while((payload = await file.ReadLineAsync()) != null)
            {
                foreach (var input in inputList)
                {
                    string result=await input.Submit(payload);
                    if (result.Contains(payload))
                    {
                        return (true,input);
                    }
                }
            }
        }
        Debug.Log("XSS: file \""+path_payload+"\" not find");
        return (false, null);
    }
    
   




}
