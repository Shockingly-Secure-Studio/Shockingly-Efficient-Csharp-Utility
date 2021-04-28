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
        Machine.Machine machine = new Machine.Machine("192.168.56.102");
        WebService ws = new WebService(
            machine, 80, "192.168.56.102"
        );
        await ws.Exploit();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
