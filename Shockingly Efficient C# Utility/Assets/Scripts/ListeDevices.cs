using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListeDevices : MonoBehaviour
{
    public GameObject menu;
    public GameObject content;
    public GameObject display;

    public GameObject Device_Prefab;
    public GameObject Device_HTB;
    
    void Start()
    {
        if (content != null)
        {
            Vector3 temp = new Vector3(-10f,-10f,0);
            content.transform.position = temp;
        }
        SetDevices();
        
    }
    public void Spawn_( bool spawn)
    {   
        UnityEngine.Debug.Log("spawn");
        Device_HTB.SetActive(spawn) ;  

    }
    public void SetDevices() //Pour display les carrés rouges
    {
        
        int nb_devices = 7; // TODO - prendre depuis le fichier
        Vector3 acc = content.transform.position;
        
        for (int i = 0; i < nb_devices; i++)
        {
            Instantiate(Device_Prefab, new Vector3(acc[0], -13, 0), Quaternion.identity,content.transform);
 
            acc += new Vector3(5.5f,0,0); //Pour espacer les différents éléments
        }

    }
    private void SetAttribut(string name, string ip, string Score) //Pour les carrés rouges
    {
        for (int i = 0; i< display.transform.childCount; i++)
        {
            GameObject tmp = display.transform.GetChild(i).gameObject;
            if (tmp.name == "Name") 
            {
                //tmp.text = name; 
            }
            if (tmp.name == "Score") 
            {
                //tmp.text = Score; 
            }
            if (tmp.name == "IP") 
            {
                //tmp.text = ip;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
