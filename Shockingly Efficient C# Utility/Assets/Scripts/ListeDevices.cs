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
    
    public GameObject contentFiche;
    public GameObject Local_Fiche;
    public GameObject Device_Fiche;
    public GameObject Device_Prefab;
    
    void Start()
    {
        if (content != null)
        {
            Vector3 temp = new Vector3(-10f,-10f,0);
            content.transform.position = temp;
        }
        SetDevices();
        
    }
    /*public void Spawn_( bool spawn)
    {   
        UnityEngine.Debug.Log("spawn");
        Device_HTB.SetActive(spawn) ;  

    }
    */
    public void SetDevices() //Pour display les carrés rouges
    {
        List<SaveScan.Device> devicesList = SaveScan.LoadJson("scanPort");
        int nb_devices = devicesList.Count;
        UnityEngine.Debug.Log("Nb of device:" + nb_devices.ToString());
        Vector3 acc = content.transform.position +new Vector3(3,0,0);
        
        for (int i = 0; i < nb_devices; i++)
        {
            GameObject DeviceScan = null;
            GameObject Fiche = null;
            if(devicesList[i].hostName == "127.0.0.1"){
                DeviceScan = Instantiate(Device_Prefab, new Vector3(acc[0], -13, 0), Quaternion.identity,content.transform) as GameObject;
                Fiche = Instantiate(Local_Fiche, new Vector3(acc[0], -13, 0), Quaternion.identity,contentFiche.transform) as GameObject;
                Fiche.SetActive(false);
            }
                
            else{
                DeviceScan = Instantiate(Device_Prefab, new Vector3(acc[0], -13, 0), Quaternion.identity,content.transform) as GameObject;
                Fiche = Instantiate(Device_Fiche, new Vector3(acc[0], -13, 0), Quaternion.identity,contentFiche.transform) as GameObject;
                for(int k = 0; k < Fiche.transform.childCount; k++){
                    Text tmp = Fiche.transform.GetChild(k).gameObject.GetComponent<Text>(); //risk,asn_organization,localisation,country,threat,ip 
                    try
                    {
                        if(tmp.name == "Ports")
                            tmp.text = "Ports: "+devicesList[i].Port.Count.ToString() + " ports ouverts";
                        if(tmp.name == "ip")
                            tmp.text = "IP: "+devicesList[i].IP;
                        if(tmp.name == "Score")
                            tmp.text = "score: " + devicesList[i].severityLevel;
                    }
                    catch (System.Exception)
                    {
                        
                        break;
                    }
                    
                }
                Fiche.SetActive(false);
            }
                
           
            
            for (int j = 0; j < DeviceScan.transform.childCount ; j++)
            {
                Text tmp = DeviceScan.transform.GetChild(j).gameObject.GetComponent<Text>();
                try{
                        if(tmp.name == "Name")
                            tmp.text = devicesList[i].hostName;
                        if(tmp.name == "IP")
                            tmp.text = "IP: " + devicesList[i].IP;
                        if(tmp.name == "Score")
                            tmp.text = "score: " + devicesList[i].severityLevel;
                        if(tmp.name == "Ports")
                            tmp.text = devicesList[i].Port.Count.ToString() + " ports ouverts";
                    }
                catch (System.Exception)
                    {
                        
                        break;
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
