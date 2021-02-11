using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        new Scan().makePing(("127.0.0.1","127.0.0.1"));
        //Scan test = new Scan();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
