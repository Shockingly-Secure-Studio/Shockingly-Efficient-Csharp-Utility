using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Renci.SshNet;
using Service;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        SSHService sshService = new SSHService("127.0.0.1", 22);
        PrivateKeyFile file = SSHService.GenerateKey();
        Debug.Log(file);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
