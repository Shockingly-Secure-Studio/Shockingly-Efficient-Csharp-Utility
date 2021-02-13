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
    public static String SourceCode(string url) //Retourne le code source du site à l'url
    {
        HttpWebRequest r = (HttpWebRequest)WebRequest.Create(url);
        r.Method = "GET";
        WebResponse Response = r.GetResponse();
        StreamReader sr = new StreamReader(Response.GetResponseStream(), System.Text.Encoding.UTF8);
        string result = sr.ReadToEnd();
        sr.Close();
        Response.Close();
        return result;
        
    }
    public static List<String> GetCommentaire(string sourceCode) // retourne une liste avec tout les commentaire pour après check si y'a des trucs intérréssant
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
                    UnityEngine.Debug.Log(1);
                    accS += sourceCode[i];
                    i++;
                }
                if(accS.Length != 0){
                    commentaires.Add(accS + ">"); //On stocke accS dans notre liste de commentaire
                    accS = ""; //Reset de accS
                }
            }
        }
        return commentaires;
        
    }
    public static List<Cookie> GetCookies(string url) //retourne une liste d'objets cookies qui sont les cookies de la page 
    {
        List<Cookie> CookieList = new List<Cookie>();
        HttpWebRequest r = (HttpWebRequest)WebRequest.Create(url);
        r.CookieContainer = new CookieContainer(); //Crée le container de cookies
        r.Method = "GET";
      
        using (var response = (HttpWebResponse) r.GetResponse())
            {
                // Print the properties of each cookie.
                foreach (Cookie cook in response.Cookies)
                {
                    CookieList.Add(cook)
                }
            }
        return CookieList;
    }

    
    // Update is called once per frame
    void Update()
    {
        
    }
}
