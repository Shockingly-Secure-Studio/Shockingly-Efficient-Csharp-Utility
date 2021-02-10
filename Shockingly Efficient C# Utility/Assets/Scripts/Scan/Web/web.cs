using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using VSCodeEditor;
using Web;

public class web : MonoBehaviour
{
    
    // Start is called before the first frame update
    async void Start()
    {
        List<(string, int)> list = new List<(string, int)>();
        list.Add(("64.233.160.30",80));
        await SiteMap(list);
    }
    public static async Task<List<string>> SiteMap (List<(string,int)> list)
    {
        List<string> map = new List<string>();
        StreamReader sr = new StreamReader("./Assets/Scripts/Web/WordList.txt");
        foreach (var e in list)
        {
            while (sr.ReadLine() != null)
            {
                string nUrl = sr.ReadLine();
                Request request = new Request(e.Item1, e.Item2, null, nUrl);
                
                var trc = await request.Ping();
                if (trc == HttpStatusCode.OK)
                {
                    map.Add($"http://{e.Item1}:{e.Item2}/${nUrl}");
                }
            }
        }
        sr.Close();

        return map;
    }
    public static void SourceCode(string url) //Retourne le code source du site à l'url
    {
        HttpWebRequest r = (HttpWebRequest)WebRequest.Create(url);
        r.Method = "GET";
        WebResponse Response = r.GetResponse();
        StreamReader sr = new StreamReader(Response.GetResponseStream(), System.Text.Encoding.UTF8);
        string result = sr.ReadToEnd();
        sr.Close();
        Response.Close();
        GetCommentaire(result);
        
    }
    public static void GetCommentaire(string sourceCode)
    {
        List<String> commentaires = new List<String>();
        string accS = "";
        int leng = sourceCode.Length;
        for(int i = 0; i< leng-1; i++)
        {
            String acc = String.Concat(sourceCode[i] , sourceCode[i + 1]); // en c# comme en C les char sont des int donc on peut pas faire char + char = string
            if (acc == "<!") //Check si c'est le début d'un commentaire
            {
                while(sourceCode[i] != '>') //Jusqu'à la fin du commentaire on enregistre tout ça dans accS
                {
                    accS += sourceCode[i];
                    i++;
                }
                if(accS.Length != 0){
                    commentaires.Add(accS + ">"); //On stocke accS dans notre liste de commentaire
                    accS = ""; //Reset de accS
                }
            }
        }
        foreach(String e in commentaires){
            UnityEngine.Debug.Log(e);
        }
        
    }

    
    // Update is called once per frame
    void Update()
    {
        
    }
}
