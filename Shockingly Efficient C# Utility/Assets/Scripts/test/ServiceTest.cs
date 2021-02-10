using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Service;
using UnityEngine;

public class ServiceTest : MonoBehaviour
{

    public void Start()
    {
        Thread t1 = new Thread(WebServiceTest) { Name = "WebServiceThread" };
        Thread t2 = new Thread(SSHServiceTest) { Name = "SSHServiceThread" };
        
        t1.Start();
        t2.Start();
    }

    private async void WebServiceTest()
    {
        WebService service = new WebService("https://secu.studio", "104.21.43.220", 80);
        Task<string> result = service.Get("/");
        Debug.Log("GET https://secu.studio :" + await result);
        Debug.Log("https://secu.studio is online ? :" + await service.IsOnline());
    }
    
    private async void SSHServiceTest()
    {
        SSHService sshService = new SSHService("127.0.0.1", 22);
        sshService.ConnectPassword("alois", "Hello world!");
        Debug.Log("SSHService localhost is online ?" + await sshService.IsOnline());
        Debug.Log("uname -a : " + sshService.SendCommand("uname -a"));
    }
}
