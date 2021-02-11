using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Service;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        WebService service = new WebService("https://secu.studio", "104.21.43.220", 80);
        Task<string> result = service.Get("/");
        Debug.Log(await result);
        Debug.Log(await service.IsOnline());
        
        SSHService sshService = new SSHService("127.0.0.1", 22);
        sshService.ConnectPassword("alois", "Hello world!");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}