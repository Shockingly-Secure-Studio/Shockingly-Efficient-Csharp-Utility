using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Service;
using Service.Exploit;
using UnityEngine;
using Web;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        string ip = "192.168.56.102";
        Machine.Machine machine = new Machine.Machine(ip);
        WebService ws = new WebService(
            machine, 80, ip
        );
        await ws.Exploit();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
