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
        Machine.Machine machine = new Machine.Machine("127.0.0.1");
        WebService ws = new WebService(
            machine, 80, "127.0.0.1"
        );
        await ws.Exploit();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
