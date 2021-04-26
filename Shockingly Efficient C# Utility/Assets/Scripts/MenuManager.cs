using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Scan;
using Machine;
using Service.Exploit;
using System.Globalization;
using System;
using System.IO;


public class MenuManager : MonoBehaviour
{
    public GameObject menu; 
    public GameObject alertbox2; // L'alert box pour le check IP
    public Transform WedgeParent; // Les cercles pour le graphique
    public Image PieChartPrefab;// Le prefab du graphique
    public Color[] colors; // Les couleurs du graphique

    public Text[] vulns; // Les textes au dessus du graphique

    public Text IP;
    public Toggle aggresif;
    public Toggle[] ListOption;

    public GameObject[] listVulns; // Les lignes pour les vulns
    // Start is called before the first frame update
    public bool isResultScan;
    void Update()
    {
        UnityEngine.Debug.Log("MenuManager running");
        if (SceneManager.GetActiveScene().name=="ResultScan"||isResultScan)
        {
            SetVulns();
            Chart();
            TextSet();
        }
    }
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
        if (!Directory.Exists("Results"))
            Directory.CreateDirectory("Results");
        Scan.Scan();
    }

    public void SetVulns()
    {
        Machine.Machine mach = new Machine.Machine("127.0.0.1");//TODO généraliser
        List<Service.Exploit.Vulnerability> Vulns = mach.GetVulnerabilities(); // Il me faudrait une liste Vulns d'une classe Vulns
        int nbVulns = Vulns.Count;
        
        for (int i = 0; i < nbVulns; i++)
        {
            for (int j = 0; j < listVulns[i].transform.childCount; j++)
            {
                
                GameObject tmp = listVulns[i] as GameObject;
                try
                {
                    Text Nametxt = tmp.transform.Find("Name").GetComponent<Text>() as Text;
                    Nametxt.text = Vulns[i].Name;

                }
                catch (Exception)
                {
                    UnityEngine.Debug.Log(" [+] NAME PROBLEM");
                }
                try
                {
                    Text Nametxt = tmp.transform.Find("Access point").GetComponent<Text>() as Text;
                    Nametxt.text = Vulns[i].AccessPoint;
                }
                catch (Exception)
                {
                    UnityEngine.Debug.Log(" [+] Access PROBLEM ");
                }
                try
                {
                    Text Nametxt = tmp.transform.Find("IP").GetComponent<Text>() as Text;
                    Nametxt.text = Vulns[i].IP;
                }
                catch (Exception)
                {
                    UnityEngine.Debug.Log(" [+] IP PROBLEM");
                }
            }
        }
        

    }


    public void Chart() //Permet de générer un Chart
    {
        Machine.Machine mach = new Machine.Machine("127.0.0.1");
        List<Service.Exploit.Vulnerability> Vulns = mach.UpdateVulnerabilities(); // Il me faudrait une liste Vulns d'une classe Vulns

        
        float total = 0f; 
        float nbHard = 0f; 
        float nbMedium = 0f; 
        float nbEasy = 0f; 
        foreach (var vulnerability in Vulns)
        {
            if (vulnerability.Severity > 7){
                total += 1;
                nbHard += 1;
            }
                
            if (vulnerability.Severity < 8 && vulnerability.Severity > 4){
                total += 1;
                nbMedium += 1;
            }
                
            if (vulnerability.Severity < 5){
                total += 1;
                nbEasy += 1;
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
        Machine.Machine mach = new Machine.Machine("127.0.0.1");
        List<Service.Exploit.Vulnerability> Vulns = mach.UpdateVulnerabilities(); // Il me faudrait une liste Vulns d'une classe Vulns
        int low = 0;
        int med = 0;
        int crit = 0;

        foreach (var vulnerability in Vulns)
        {
            if (vulnerability.Severity > 7)
                crit++;
            if (vulnerability.Severity < 8 && vulnerability.Severity > 4)
                med++;
            if (vulnerability.Severity < 5)
                low++;
        }
        int[] nbvulns = new int[] { low, med, crit };
        string[] puissance = new string[] { "faibles", "moyennes", "critiques" };
        for (int i = 0; i < vulns.Length  ; i++)
        {
            vulns[i].text = nbvulns[i].ToString() + " vulnérabilités " + puissance[i];
        }
    }

    
}
