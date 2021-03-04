using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Renci.SshNet;
using Service;
using UnityEngine;
using Machine = Machine.Machine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        WebService ws = new WebService(new global::Machine.Machine("127.0.0.1"), "localhost", "127.0.0.1", 80);
        Thread t = new Thread(ws.Exploit);
        t.Start();
        /*
        Machine.Machine machine = new Machine.Machine("127.0.0.1");
        
        InputWebService inputWebService = new InputWebService(machine, "localhost", "127.0.0.1", 8181, "/", Utils.WebMethod.GET, "name");
        await inputWebService.Exploit(true);
        
        Debug.Log("============== PART 2 ===============");
        
        inputWebService = new InputWebService(machine, "localhost", "127.0.0.1", 8181, "command.php", Utils.WebMethod.GET, "ip");
        await inputWebService.Exploit(false);
        */
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
