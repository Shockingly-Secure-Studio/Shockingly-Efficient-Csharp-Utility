using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Renci.SshNet;
using Service;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        Machine.Machine machine = new Machine.Machine("127.0.0.1");

        FTPService service = new FTPService(machine, 21);
        List<string> files = service.ListFiles();
        foreach (string file in files)
        {
            Debug.Log("[*] ListFiles: " + file);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
