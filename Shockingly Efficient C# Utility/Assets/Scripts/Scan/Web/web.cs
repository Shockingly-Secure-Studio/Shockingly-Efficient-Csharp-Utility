using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneTemplate;
using Web;

public class web : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public static List<string> map(List<(string, int)> list, List<string> url)
    {
        List<string> nlist = new List<string>();
        if (list.Count == 0)
        {
            Request request = new Request("", -1, null, null);
            foreach (var VARIABLE in url)
            {
                string domain = request.GetDomainName(VARIABLE);
                List<string> nnlist= moche(domain, VARIABLE, 10);
                foreach (var items in nnlist)
                {
                    bool find = false;
                    foreach (var it in nlist)
                    {
                        if (it == items)
                        {
                            find = true;
                        }
                    }
                    if (!find)
                    {
                        nlist.Add(items);
                    }
                }
            }
        }
        else
        {
            foreach (var e in list)
            {
                Request request = new Request(e.Item1, e.Item2, null, null);
                nlist.Add($"http://{e.Item1}:{e.Item2}");
                string domain = request.GetDomainName($"http://{e.Item1}:{e.Item2}");
                List<string> nnlist= moche(domain, $"http://{e.Item1}:{e.Item2}", 10);
                foreach (var items in nnlist)
                {
                    bool find = false;
                    foreach (var it in nlist)
                    {
                        if (it == items)
                        {
                            find = true;
                        }
                    }

                    if (!find)
                    {
                        nlist.Add(items);
                    }
                }
            }  
        }
        return nlist;
    }

    public static (List<string>,List<string>) getlinks(string url, string domain,List<string> patterns)
    {
        List<string> nlist = new List<string>();

        string src = SourceCode(url);
            
        string pattern = "(href=\")+([%-z])+"; // href="fqsdfsqdfsdqfsdqf/QSFDsqdfsqdf/sfdsqdfsqdfsdd
            
        string pattern2 = "("+domain+")"; // https://domain/truc/tuturu

        string pattern3 = "([%-z])+((html)|(php))"; // /truc.html
        string pattern4 = "([.][/]([&-z]+))"; // ./?truc    
        Regex regex = new Regex(pattern);
            
        Regex rgx = new Regex(pattern2);

        Regex rgx2 = new Regex(pattern3);

        Regex rgx3 = new Regex(pattern4);
        
        foreach (Match m in regex.Matches(src))
        {
            string s = m.ToString();
            bool find = false;
            foreach (var VARIABLE in patterns)
            {
                if (s == VARIABLE)
                {
                    find = true;
                }
            }
            if (rgx.IsMatch(s) && !find)
            {
                string ns = "";
                for (int i = 6; i < s.Length; i++)
                {
                    ns += s[i];
                }
                patterns.Add(s);
                nlist.Add(ns);
            }
            else if (rgx2.IsMatch(s) && !find)
            {
                string ns = "";
                ns += $"http://{domain}/";
                for (int i = 6; i < s.Length; i++)
                {
                    ns += s[i];
                }
                patterns.Add(s);
                nlist.Add(ns);
            }
            else if (rgx3.IsMatch(s) && !find)
            {
                string ns = "";
                if (!find)
                {
                    ns += url;
                    for (int i = 8; i < s.Length; i++)
                    {
                        ns += s[i];
                    }
                    patterns.Add(s);
                    nlist.Add(ns); 
                }
            }
        }
        return (nlist,patterns);
    }
    
    public static List<string> moche(string domain, string url, int depth)
    {
        List<string> recurent = new List<string>();
        (List<string>,List<string>) acc = getlinks(url, domain,recurent);
        (List<string>,List<string>) visited = getlinks(url, domain, recurent);
        string url2;
        while (acc.Item1.Count != 0 && depth > 0)
        {
            url2 = acc.Item1[0];
            acc.Item1.Remove(acc.Item1[0]);
            (List<string>,List<string>) acc2 = getlinks(url2, domain,visited.Item2);
            foreach (var VARIABLE in acc2.Item1)
            {
                bool find = false;
                foreach (var it in visited.Item1)
                {

                    if (VARIABLE == it)
                    {
                        find = true;
                    }
                    
                }
                if (!find)
                {
                    acc.Item1.Add(VARIABLE);
                    visited.Item1.Add(VARIABLE);
                }
                
            }
            foreach (var VARIABLE in acc2.Item2)
            {
                bool find = false;
                foreach (var it in visited.Item2)
                {

                    if (VARIABLE == it)
                    {
                        find = true;
                    }
                    
                }
                if (!find)
                {
                    visited.Item2.Add(VARIABLE);
                }
                
            }
            depth--;
        }
        return visited.Item1;
    }

    public static List<string> GetInUrl(List<string> list)
    {
        List<string> nlist = new List<string>();
        string urlPattern = "([?])([a-z]|[A-Z])+(=)+";
        Regex rgx = new Regex(urlPattern);
        foreach (var item in list)
        {
            if (rgx.IsMatch(item))
            {
                nlist.Add(item);
            }
        }

        return nlist;
    }

    public static List<string> GetText(List<string> list)
    {
        string pattern = "(<input)+";
        Regex regex = new Regex(pattern);
        List<string> nlist = new List<string>();
        foreach (var item in list)
        {
            if (regex.IsMatch(item))
            {
                nlist.Add(item);
            }
        }

        return nlist;
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
                foreach (Cookie cook in response.Cookies){
                    CookieList.Add(cook);
                }
        }
        return CookieList;
    }

    
    public static void JwtToken(string url)
    {
        List<Cookie> cookies = GetCookies(url);
        List<Cookie> Exploited_cookies = new List<Cookie>();

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
            if (Part[0] != null)
                header = Encoding.UTF8.GetString(Convert.FromBase64String(Part[0]));
            if (Part[1] != null){
                try //Il peut y avoir une exeception chiante sur la longueur 
                {
                    payloads = Encoding.UTF8.GetString(Convert.FromBase64String(Part[1]));
                }
                catch (System.Exception)
                {
                    payloads = Encoding.UTF8.GetString(Convert.FromBase64String(Part[1]+"=")); //ça regle le problème
                }
            }
            if (payloads.Contains("username"))
                payloads = "{\"username\" : \" admin\"}" ;


            //Manque des tests + l'encodage
            string argus = header+"."+payloads+"."+signature; //Reformer le payload
            
        }
    }


    
    // Update is called once per frame
    void Update()
    {
        
    }
}
