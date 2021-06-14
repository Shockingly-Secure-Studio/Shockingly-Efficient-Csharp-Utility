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
using System.Linq;
using DefaultNamespace;
using Newtonsoft.Json;


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

    // Start is called before the first frame update
    public bool isResultScan;
    
    /// Variables for vulns numbers ///
    private int nbFaible = 0;
    private int nbMoy = 0;
    private int nbCrit = 0;

    private int AccText = 0;
    private int AccText2 = 0;
    public Text LoadingText;
    List<Vulnerability> vulnsFound = new List<Vulnerability>();

    public Image loadingbar;
    public bool loadingScene;
    
    public GameObject vulnPrefab;
    public GameObject vulnScrollView;
    
    void Update()
    {
        //UnityEngine.Debug.Log("MenuManager running");
        if (loadingScene)
        {
            //UnityEngine.Debug.Log("LOADING");
            SetLoading();
        }
        
        else if (SceneManager.GetActiveScene().name=="ResultScan"||isResultScan)
        {
            SetVulns();
            Chart();
            TextSet();
        }
    }
    void ExitAPP()
    {
        //Debug.Log("Exit App");
        Application.Quit();
    }

    public void SetLoading(){

        string[] loading = new string[]{"chargement.","chargement..","chargement..."};
        
        if(AccText2 == 10){
            AccText++;
            AccText2 = 0;
        }  
        else if(AccText == 2){
            AccText = 0;
            AccText2++;
        }
        else
            AccText2++;


        LoadingText.text = loading[AccText];
        Image tmp = loadingbar;
        tmp.fillAmount += 0.015f;
        if(tmp.fillAmount == 1f){
            SceneManager.LoadScene("ResultScan");
        }

    }

    public void ChangeScene_(string sceanename)
    {

        SceneManager.LoadScene(sceanename);

    }

    public void ScanStart()
    {
        string ipText;
        if (IP.text == null || IP.text == "")
            ipText = "";
        else
            ipText = IP.text;
        //Debug.Log("ScanStart");
        if ( ipText == null || ipText == "" )
        {
            alertbox2.SetActive(true);
        }
        else {
            ForceScanStart();
        }
        
        if (IP.text == null || IP.text == "")
            ipText = "";
        else
            ipText = IP.text;
        //Debug.Log("Vrai scan");
        string agg = "fast";
        if (aggresif.isOn)
            agg = "all";
        if (!ipText.Contains("/"))
        {
            ipText += '/';
        }
        ScanControl Scan = new ScanControl(ipText,agg);
        if (Directory.Exists("Results")) 
            Directory.Delete("Results", true);
        
        Directory.CreateDirectory("Results");
        Scan.Scan();
        SceneManager.LoadScene("loadingpage");
        
    }

    public void ForceScanStart()
    {
    }

    /////            SETUP VULNS                 //////


    public void GetVulns(){
        // TODO : prends une ip en paramètre et renvoie la liste de ses vulns
        // Si c'est plus ismple autrement pas de soucis
        // Si tu peux faire List<Vulnerability> comme type de retour ça m'arrange encore + ok
        nbCrit = 0;
        nbMoy = 0;
        nbFaible = 0;
        List<Vulnerability> vulnerabilities = new List<Vulnerability>();
        if(!Directory.Exists("Results"));
            Directory.CreateDirectory("Results");
        var flawsByServices =
            Directory.EnumerateFiles("Results", "output.json", SearchOption.AllDirectories);
        vulnsFound = new List<Vulnerability>();
        foreach (string outputjson in flawsByServices)
        {
            StreamReader sr = new StreamReader(outputjson);
            ServiceResult serviceResult = JsonConvert.DeserializeObject<ServiceResult>(sr.ReadToEnd());
            sr.Close();
            foreach (AccessPoint vuln in serviceResult.AccessPoints)
            {
                Vulnerability vulnerability = new Vulnerability(vuln.Type.ToString(), vuln.Access, vuln.Severity, serviceResult.Identifier);
                if (vulnerability.Severity > 7)
                    nbCrit++;           

                if (vulnerability.Severity < 8 && vulnerability.Severity > 4)
                    nbMoy++;
                
                if (vulnerability.Severity < 5)
                    nbFaible ++;

                vulnsFound.Add(vulnerability);
            }
        }
    }

    public void SetVulns()
    {
        GetVulns(); 
        int nbVulns = vulnsFound.Count;
        for (int i = 0; i < nbVulns; i++)
        {
            bool alreadyExists = false;
            // iterate over the child of vulnScrollView
            foreach (Transform child in vulnScrollView.transform)
            {
                if (
                    child.Find("Access point").GetComponent<Text>().text ==
                    vulnsFound[i].AccessPoint &&
                    child.Find("IP").GetComponent<Text>().text == vulnsFound[i].IP
                )
                {
                    alreadyExists = true;
                    break;
                }
            }
            
            if (alreadyExists) continue;

            foreach (Transform child in vulnScrollView.transform)
            {
                child.transform.position += Vector3.down;
                child.Find("Image").GetComponent<Image>().enabled = !child.Find("Image").GetComponent<Image>().enabled;
            }
            GameObject tmp = Instantiate(vulnPrefab, vulnScrollView.transform, false);
            tmp.transform.position += Vector3.right;
            
            try
            {
                Text Nametxt = tmp.transform.Find("Name").GetComponent<Text>() as Text;
                Nametxt.text = vulnsFound[i].Name;
            }
            catch (Exception)
            {
                UnityEngine.Debug.Log(" [+] NAME PROBLEM");
            }
            try
            {
                Text Nametxt = tmp.transform.Find("Access point").GetComponent<Text>() as Text;
                Nametxt.text = vulnsFound[i].AccessPoint;
            }
            catch (Exception)
            {
                UnityEngine.Debug.Log(" [+] Access PROBLEM ");
            }
            try
            {
                Text Nametxt = tmp.transform.Find("IP").GetComponent<Text>() as Text;
                Nametxt.text = vulnsFound[i].IP;
            }
            catch (Exception)
            {
                UnityEngine.Debug.Log(" [+] IP PROBLEM");
            }


        }
            
        
        
    }

    public void AddVuln(string vulnName, string accessPoint, string ip, int severity)
    {
        if (severity > 7)
            nbCrit++;           

        if (severity < 8 && severity > 4)
            nbMoy++;
                
        if (severity < 5)
            nbFaible ++;

        GameObject child = Instantiate(vulnPrefab, new Vector3(0, 0, 0), Quaternion.identity, vulnScrollView.transform);
        
        try
        {
            Text Nametxt = child.transform.Find("Name").GetComponent<Text>() as Text;
            Nametxt.text = vulnName;

        }
        catch (Exception)
        {
            UnityEngine.Debug.Log(" [+] NAME PROBLEM");
        }
        try
        {
            Text Nametxt = child.transform.Find("Access point").GetComponent<Text>() as Text;
            Nametxt.text = accessPoint;
        }
        catch (Exception)
        {
            UnityEngine.Debug.Log(" [+] Access PROBLEM ");
        }
        try
        {
            Text Nametxt = child.transform.Find("IP").GetComponent<Text>() as Text;
            Nametxt.text = ip;
        }
        catch (Exception)
        {
            UnityEngine.Debug.Log(" [+] IP PROBLEM");
        }
    }

   /////            SETUP GRAPHICS                 //////

    public void Chart() //Permet de générer un Chart
    {
        GetVulns(); 

        float nbHard = nbCrit; 
        float nbMedium = nbMoy; 
        float nbEasy = nbFaible; 
        float total = (float) vulnsFound.Count;
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
        
        int low = nbFaible;
        int med = nbMoy;
        int crit = nbCrit;        
        int[] nbvulns = new int[] { low, med, crit };
        string[] puissance = new string[] { "faibles", "moyennes", "critiques" };
        for (int i = 0; i < 3; i++)
        {
            vulns[i].text = nbvulns[i].ToString() + " vulnérabilités " + puissance[i];
        }
    }

}
