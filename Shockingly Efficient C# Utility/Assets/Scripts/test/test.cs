using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Web;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //gameObject.AddComponent<Scan>().makePing(("127.0.0.1","127.0.0.1"));
        //Scan test = new Scan();
        Request test = new Request("haha",70,null,"fqsdf");
        test.test();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
