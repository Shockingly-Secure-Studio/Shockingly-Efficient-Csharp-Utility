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
        Debug.Log("Bonjour");
        List<(string, int)> list = new List<(string, int)>();
        List<string> url = new List<string>();
        url.Add("http://localhost:8181");
        List<string> map = web.map(list,url);
        List<InputWebService> total = new List<InputWebService>();
        
        map.Add("http://localhost:8181");
        
        foreach (var link in map)
        {
            Debug.Log(link);
            foreach (var inputWebService in await InputWebService.FromLink("127.0.0.1", 8181, link))
            {
                total.Add(inputWebService);
            }
        }

        foreach (InputWebService inputWebService in total)
        {
            await inputWebService.Exploit(true);
        }
        
        /*
        Machine.Machine machine = new Machine.Machine("127.0.0.1");
        
        InputWebService inputWebService = new InputWebService(machine, "localhost", "127.0.0.1", 8181, "/", Utils.WebMethod.GET, "name");
        await inputWebService.Exploit(true);
        
        Debug.Log("============== PART 2 ===============");
        
        inputWebService = new InputWebService(machine, "localhost", "127.0.0.1", 8181, "command.php", Utils.WebMethod.GET, "ip");
        await inputWebService.Exploit(false);
        */
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
