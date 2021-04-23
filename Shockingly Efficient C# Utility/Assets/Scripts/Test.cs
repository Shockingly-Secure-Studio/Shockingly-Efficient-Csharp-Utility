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
        /*
        InputWebService service = new InputWebService(
                machine, "127.0.0.1", 80, "command.php",Utils.WebMethod.GET, "ip"); 
        await service.Exploit(true);
        await service.Shell.AsyncLogFingerprint();
        */

        WebService service = new WebService(
            machine, 80, "127.0.0.1"
        );
        await service.Exploit();
        
        await machine.GetShell().AsyncLogFingerprint();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
