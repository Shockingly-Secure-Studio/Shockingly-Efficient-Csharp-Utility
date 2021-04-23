using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using Scan;

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
        List<SaveScan.Device> devicesList = SaveScan.LoadJson("scanPort");
        int nb_devices = devicesList.Count;
        UnityEngine.Debug.Log("Nb of device:" + nb_devices.ToString());
        Vector3 acc = content.transform.position +new Vector3(3,0,0);
        
        for (int i = 0; i < nb_devices; i++)
        {
            GameObject DeviceScan = Instantiate(Device_Prefab, new Vector3(acc[0], -13, 0), Quaternion.identity,content.transform) as GameObject;
            for (int j = 0; j < DeviceScan.transform.childCount; j++)
            {
                GameObject tmp = DeviceScan.transform.GetChild(j).gameObject;
                try
                {
                    Text Nametxt = GameObject.Find("Name").GetComponent<Text>();
                    Nametxt.text = devicesList[j].hostName;
                }
                catch (Exception)
                {
                    throw;
                }
                try
                {
                    Text IPtxt = GameObject.Find("IP").GetComponent<Text>();
                    IPtxt.text = "IP: " + devicesList[j].IP;
                }
                catch (Exception)
                {
                    throw;
                }
                try
                {
                    Text Scoretxt = GameObject.Find("Score").GetComponent<Text>();
                    Scoretxt.text = "score: " + devicesList[j].severityLevel;
                }
                catch (Exception)
                {
                    throw;
                }
                try
                {
                    Text Scoretxt = GameObject.Find("Ports").GetComponent<Text>();
                    Scoretxt.text = devicesList[j].Port.Count.ToString() + " ports ouverts";
                }
                catch (Exception)
                {
                    throw;
                }

            }

            acc += new Vector3(5.5f,0,0); //Pour espacer les différents éléments
        }

    }
 
    // Update is called once per frame
    void Update()
    {
        
    }
}
