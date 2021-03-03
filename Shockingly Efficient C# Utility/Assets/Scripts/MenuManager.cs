using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Scan;
using System.Globalization;
using System;



public class MenuManager : MonoBehaviour
{
    public GameObject menu;
    public GameObject alertbox2;
    public Transform WedgeParent;
    public Image PieChartPrefab;
    public Color[] colors;

    public Text[] vulns;

    public Text IP;
    public Toggle aggresif;
    public Toggle[] ListOption;
    // Start is called before the first frame update
    void ExitAPP()
    {
        Debug.Log("Exit App");
        Application.Quit();
    }
    public void ChangeScene_(string sceanename)
    {

        SceneManager.LoadScene(sceanename);

    }
     public void ScanStart(string sceanename)
    {
        string ipText = IP.text;
        if ( ipText == null || ipText == "" )
        {
            alertbox2.SetActive(true);
        }
        else
        {
            SceneManager.LoadScene(sceanename);
        }
        string agg = "fast";
        if (aggresif.isOn)
            agg = "all";
        if (!ipText.Contains("/"))
        {
            ipText += '/';
        }
        ScanControl Scan = new ScanControl(ipText,agg);
        Scan.Scan();

    }


    public void Chart() //Permet de générer un Chart
    {
        List<SaveScan.Device> devicesList = SaveScan.LoadJson("scan1");
        int nb_devices = devicesList.Count;
        
        float total = 0f; // Mettre le nombre de failles découvertes ici
        float nbHard = 0f; // Mettre le nombre de failles "hard" découvertes ici
        float nbMedium = 0f; // Mettre le nombre de failles "medium" découvertes ici
        float nbEasy = 0f; // Mettre le nombre de failles "easy" découvertes ici

        foreach (var device in devicesList)
        {
            if(Int32.Parse(device.severityLevel) <= 4){
                total += 1f;
                nbEasy += 1f;
            }
            if(Int32.Parse(device.severityLevel) <= 8 && Int32.Parse(device.severityLevel) > 4){
                total += 1f;
                nbMedium += 1f;
            }
            if(Int32.Parse(device.severityLevel) > 8){
                total += 1f;
                nbHard += 1f;
            }
                
        } 

        float[] nbfailles = new float[] {nbEasy,nbMedium,nbHard};
        float zRot = 0f;

        for (int i = 0; i < 3; i++)
        {
            Image newWedge = Instantiate(PieChartPrefab) as Image;
            newWedge.transform.SetParent (WedgeParent,false);
            newWedge.color = colors[i];
            newWedge.fillAmount = nbfailles[i] / total;
            newWedge.transform.rotation = Quaternion.Euler (new Vector3(0f,0f,zRot));
            zRot -= newWedge.fillAmount * 360f; 
        }

    }
    public void TextSet()
    {
        int[] nbvulns = new int[] { 2, 4, 6 };
        string[] puissance = new string[] { "faibles", "moyennes", "critiques" };
        for (int i = 0; i < vulns.Length  ; i++)
        {
            vulns[i].text = nbvulns[i].ToString() + " vulnérabilité " + puissance[i];
        }
    }

    


    // Update is called once per frame
    void Awake()
    {
        Chart();
        TextSet();
    }
}
