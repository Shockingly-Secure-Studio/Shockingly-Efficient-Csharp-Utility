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
        DNSService dnsService = new DNSService();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
