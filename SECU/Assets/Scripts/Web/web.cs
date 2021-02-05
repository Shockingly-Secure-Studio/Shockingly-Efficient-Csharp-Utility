using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using VSCodeEditor;
using Web;

public class web : MonoBehaviour
{
    
    // Start is called before the first frame update
    async void Start()
    {
        List<(string, int)> list = new List<(string, int)>();
        list.Add(("127.0.0.1",80));
        await SiteMap(list);
    }
    public static async Task<List<string>> SiteMap(List<(string,int)> list)
    {
        List<string> map = new List<string>();
        StreamReader sr = new StreamReader("WordList.txt");
        foreach (var e in list)
        {
            while (sr.ReadLine() != null)
            {
                string nUrl = sr.ReadLine();
                Request request = new Request(e.Item1, e.Item2, null, nUrl);
                
                var trc = await request.ping();
                Debug.Log(trc);
                if (trc == HttpStatusCode.OK)
                {
                    map.Add($"{e.Item1}:{e.Item2}/${nUrl}");
                }
            }
        }

        return map;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
