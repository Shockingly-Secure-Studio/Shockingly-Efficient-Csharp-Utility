using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tst : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        List<(string, int)> list = new List<(string, int)>();
        list.Add(("216.58.198.206",80));
        await web.SiteMap(list);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}