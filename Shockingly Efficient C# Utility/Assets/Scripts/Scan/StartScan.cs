using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class StartScan : MonoBehaviour
{
    public Text inputIP;
    public GameObject InvalidIPBox;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void SpawnBox(bool active){
        InvalidIPBox.SetActive(active);
    }
    public void IpRecup(){
        string ip = inputIP.text;
        string regex = "^(?:[0-9]{1,3}\\.){3}[0-9]{1,3}$";
        Regex rgx = new Regex(regex);
        if(! rgx.IsMatch(ip)){
            SpawnBox(true);
            return;
        }
        // Mettre ici pour recup l'ip
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
