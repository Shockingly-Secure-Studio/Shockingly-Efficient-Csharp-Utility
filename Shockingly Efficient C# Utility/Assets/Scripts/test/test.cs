using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Scan;
using UnityEngine;
using Web;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        //gameObject.AddComponent<Scan>().makePing(("127.0.0.1","127.0.0.1"));
        //Scan test = new Scan();
        //Request test = new Request("haha",70,null,"fqsdf");
        //test.test();
        //
        //ScanIp o = new ScanIp(); 
        //o.MakePing();
        //Thread t = new Thread(new ThreadStart( o.MakePing));
        //t.Start();
        //Debug.Log("========================== Test Begin =============");
        //Request request = new Request("",-1,null,"");
        //await request.test();
        //Debug.Log("========================== Test ended =============");
        //aréter de laisser vos test SVP, commenter ou effacer
        gameObject.AddComponent<rapport>().NewDocument();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
