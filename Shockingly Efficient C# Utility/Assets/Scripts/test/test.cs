using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Scan;
using UnityEngine;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        ScanIp o = new ScanIp();
        o.makePing();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
