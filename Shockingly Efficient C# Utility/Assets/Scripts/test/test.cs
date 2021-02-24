using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.AddComponent<ScanIp>().makePing();
        //Scan test = new Scan();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
