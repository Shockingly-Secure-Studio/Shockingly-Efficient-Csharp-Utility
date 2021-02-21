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
            "localhost", 
            "127.0.0.1",
            8181, 
            "command.php",
            Utils.WebMethod.GET,
            "ip"
            );
        
        inputWebService.Exploit();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
