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
        InputWebService inputWebService = new InputWebService(
            "http://localhost", 
            "127.0.0.1",
            8181, "",
            Utils.WebMethod.GET,
            "name"
            );
        
        inputWebService.SQLInjection();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
