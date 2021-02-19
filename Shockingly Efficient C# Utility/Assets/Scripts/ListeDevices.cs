using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListeDevices : MonoBehaviour
{
    public GameObject menu;
    public GameObject content;
    public GameObject display;

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
    public void SetDevices(){
        int nb_devices = 3;
        int acc = 0;
        for (int i = 0; i < nb_devices; i++)
        {
            Instantiate(Device_Prefab, new Vector3(86, -12, 0), Quaternion.identity,content.transform);
            //newChild.transform.SetParent(content.transform);
            UnityEngine.Debug.Log("Ajouté");
            acc += 420;
        }

    }
    public void SetAttribut()
    {
        for (int i = 0; i< display.transform.childCount; i++)
        {
            GameObject tmp = display.transform.GetChild(i).gameObject;
            if (tmp.name == "Name") 
            {
                //Mettre le Nom ici
            }
            if (tmp.name == "Score") 
            {
                //Mettre le Score ici
            }
            if (tmp.name == "IP") 
            {
                //Mettre l'IP ici
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
