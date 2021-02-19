using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Web;

public class web : MonoBehaviour
{
    
    // Start is called before the first frame update
    async void Start()
    {
    }
    public static async Task<List<string>> SiteMap (List<(string,int)> list)
    {
        List<string> map = new List<string>();
        StreamReader sr = new StreamReader("./Assets/Scripts/Web/WordList.txt");
        foreach (var e in list)
        {
            IPHostEntry hostEntry = Dns.GetHostEntry(e.Item1);
            string src = SourceCode($"http://{e.Item1}:{e.Item2})");
            string pattern = "(href=\")+([%-z])+";
            string pattern2 = "("+hostEntry+")";
            Regex regex = new Regex(pattern);
            Regex rgx = new Regex(pattern2);
            foreach (string s in regex.Matches(src))
            {
                if (rgx.IsMatch(s))
                {
                    string ns = "";
                    for (int i = 5; i < s.Length; i++)
                    {
                        ns += s[i];
                    }
                    map.Add(ns);
                }
            }
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
                    CookieList.Add(cook);     
        }
        return CookieList;
    }

    
    public static void JwtToken(string url)
    {
        List<Cookie> cookies = GetCookies(url);
        //string pattern = "^[A-Za-z0-9-_=]+\.[A-Za-z0-9-_=]+\.?[A-Za-z0-9-_.+/=]*$";
        //Regex rgx = new Regex(pattern2);
        List<Cookie> JWT_list = new List<Cookie>(); // Liste de JWT
        
        // Find JWT token
        foreach (Cookie c in cookies)
        {
            if ( c.Name == "jwt") //rgx.IsMatch(c.Value) ||
                JWT_list.Add(c);
        }

        foreach (Cookie token in JWT_list)
        {
            string header = "";
            string payloads = "";
            string signature = "";
            string Value = token.Value;
            string[] Part = Value.Split('.');
            header = Encoding.UTF8.GetString(Convert.FromBase64String(Part[0]));
            payloads = Encoding.UTF8.GetString(Convert.FromBase64String(Part[0]));
            signature = Encoding.UTF8.GetString(Convert.FromBase64String(Part[0]));
            UnityEngine.Debug.Log(header+  payloads+signature);

        }
    }

    
    // Update is called once per frame
    void Update()
    {
        
    }
}
