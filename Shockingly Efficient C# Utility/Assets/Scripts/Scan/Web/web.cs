using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Web;

public class web : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public static List<string> SiteMap(List<(string,int)> list)
    {
        List<string> map = new List<string>();
        foreach (var e in list)
        {
            Request request = new Request(e.Item1, e.Item2, null, null);
            string hostEntry = request.GetDomainName($"http://{e.Item1}:{e.Item2}");

            static List<string> Map_rec(string url, string domain, int depth, List<string> map)
            {
                int total = map.Count;
                if (depth == 0)
                {
                   return map; 
                }
                else
                {
                    Request request = new Request("",-1 , null, null);
                    string src = SourceCode($"http://");
                    string pattern = "(href=\")+([%-z])+";
                    string pattern2 = "("+domain+")";
                    Regex regex = new Regex(pattern);
                    Regex rgx = new Regex(pattern2);
                    foreach (Match m in regex.Matches(src))
                    {
                        Debug.Log(m);
                        string s = m.ToString();
                        if (rgx.IsMatch(s))
                        {
                            string ns = "";
                            for (int i = 6; i < s.Length; i++)
                            {
                                ns += s[i];
                            }

                            bool find = false;
                            foreach (var VARIABLE in map)
                            {
                                if (ns == VARIABLE)
                                {
                                    find = true;
                                    break;
                                }
                            }
                            if (!find)
                            {
                               map.Add(ns); 
                            }
                        }
                    }
                    if (total == map.Count)
                    {
                        
                    }
                    else
                    {
                        
                    }
                }

                return map;
            }
            
        }
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
                    CookieList.Add(cook);
                }
        }
        return CookieList;
    }

    
    // Update is called once per frame
    void Update()
    {
        
    }
}
