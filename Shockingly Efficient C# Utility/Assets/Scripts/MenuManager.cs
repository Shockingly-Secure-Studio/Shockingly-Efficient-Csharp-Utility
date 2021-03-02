using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


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
        if (IP.text == null || IP.text == "" )
        {
            alertbox2.SetActive(true);
        }
        else
        {
            SceneManager.LoadScene(sceanename);
        }

    }


    public void Chart() //Permet de générer un Chart
    {
        float total = 12f; // Mettre le nombre de failles découvertes ici
        float nbHard = 4f; // Mettre le nombre de failles "hard" découvertes ici
        float nbMedium = 3f; // Mettre le nombre de failles "medium" découvertes ici
        float nbEasy = 8f; // Mettre le nombre de failles "easy" découvertes ici
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

    



    public void SetAttScan(){
        string ip = IP.text;
        if (ip.Contains("/"))
        {
            
        }

        string methode = "";
        if (aggresif.isOn)
            methode = "all";
        else
        {
            methode = "fast"; 
        }
        
    }

    // Update is called once per frame
    void Start()
    {
        Chart();
    }
}
