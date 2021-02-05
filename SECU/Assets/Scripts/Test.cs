using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Service;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        WebService service = new WebService("http://google.com", "216.58.201.238", 80);
        string result = await service.Get("/");
        Debug.Log(result);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
