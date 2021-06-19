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
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using Web;
using Task = System.Threading.Tasks.Task;
using ThreadPriority = System.Threading.ThreadPriority;

public class web : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public static async Task<List<string>> map(List<(string, int)> list, List<string> url = null)
    {
        List<string> nlist = new List<string>();
        foreach (var e in list)
        {
            
                Request request = new Request(e.Item1, e.Item2, null, null);
                string domain = $"{e.Item1}:{e.Item2}";
                List<string> nnlist = new List<string>();
                nlist.Add($"http://{e.Item1}:{e.Item2}");
                //domain = request.GetDomainName($"http://{e.Item1}");
                nnlist = await WebDiscover(domain, $"http://{e.Item1}:{e.Item2}", 10);
                Debug.Log("after WebDiscover");

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
        
        return nlist;
    }

    public static async Task<List<string>> getlinks(string url, string domain)
    {
        List<string> res = new List<string>();

        string src = Utils.MakeRequest(url);
        if (src == "")
        {
            return res;
        }
        
        string pattern = "(href=\")+([%-z])+"; // href="fqsdfsqdfsdqfsdqf/QSFDsqdfsqdf/sfdsqdfsqdfsdd
            
        string pattern2 = "[(http|https)]+:[/][/]+["+domain+"]+([/]+([A-Z|a-z]+))+"; // https://domain/truc/tuturu

        string pattern3 = "([%-z])+((html)|(php)|(phtml))"; // /truc.html

        string pattern4 = "([.][/]([&-z]+))"; // ./?truc  

        Regex regex = new Regex(pattern);
        
        Regex rgx = new Regex(pattern2);
        

        Regex rgx2 = new Regex(pattern3);

        Regex rgx3 = new Regex(pattern4);
        
        
        foreach (Match m in regex.Matches(src))
        {
            string s = m.ToString();
            bool find = false;
            foreach (var VARIABLE in res)
            {
                //prétraitement
                string nmatch = "";
                int j = 6;
                Debug.Log($"tested with : {s}\nWorks with rgx : {rgx.IsMatch(s)}\nWorks with rgx : {rgx2.IsMatch(s)}\nWorks with rgx : {rgx3.IsMatch(s)}");
                for (; j < s.Length; j++)
                {
                    nmatch += s[j];
                }
                string[] urlsplit = VARIABLE.Split('/');
                if (urlsplit.Contains(nmatch))
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
                res.Add(ns);
            }
            else if (rgx2.IsMatch(s) && !find)
            {
                string ns = "";
                ns += $"http://{domain}";
                if (s[6] != '/' )
                {
                    ns+= '/';
                }
                for (int i = 6; i < s.Length; i++)
                {
                    ns += s[i];
                }
                res.Add(ns);
            }
            else if (rgx3.IsMatch(s) && !find)
            {
                string ns = "";
                ns += url + "/";
                    for (int i = 8; i < s.Length; i++)
                    {
                        ns += s[i];
                    }

                    res.Add(ns);
            }
        }
        return res;
    }
    //public static List<string> resultatGo = new List<string>();
    public static HashSet<string> hashSet = new HashSet<string>();
    public static async  Task<List<string>> WebDiscover(string domain, string url, int depth) // will detect all the pages from a website
    {
        List<string> acc = await getlinks(url, domain);
        List<string> visited = await getlinks(url, domain);
        string url2;
        while (acc.Count != 0 && depth > 0)
        {
            url2 = acc[0];
            acc.Remove(acc[0]);
            //if (url2 == null)
            //{
            //}
            //else
            //{
                List<string> acc2 = await getlinks(url2, domain);
                foreach (var VARIABLE in acc2)
                {
                    bool find = false;
                    foreach (var it in visited)
                    {

                        if (VARIABLE == it)
                        {
                            find = true;
                        }

                        if (VARIABLE.Split('?').Length >2)
                        {
                            find = true;
                        }
                    
                    }
                    if (!find)
                    {
                        acc.Add(VARIABLE);
                        visited.Add(VARIABLE);
                    }
                
                }

                depth--;
                //}
        }

        hashSet.Clear();
        
        foreach (var VARIABLE in visited)
        {
            hashSet.Add(VARIABLE);
        }

        return await Gobuster(url);
    }

    public static async Task<List<string>> Gobuster(string url)
    {
        Debug.Log("gobuster DEBUG");
        StreamReader sr = new StreamReader(Path.Combine("Binaries", "wordlistsout.txt"));
        string s;
        var requestTaskList = new List<Task>();
        while ((s = sr.ReadLine()) != null)
        {
            if (url[url.Length-1] !='/' && s[0] != '/')
            {
                requestTaskList.Add(Request.Ping(url +'/'+s));
            }
            
            else if (url[url.Length-1] =='/' && s[0] == '/')
            {
                string nurl = "";
                for (int i = 0; i < url.Length-1; i++)
                {
                    nurl += url[i];
                }
                requestTaskList.Add(Request.Ping(nurl+s));
            }
            else
            {
                requestTaskList.Add(Request.Ping(url +s));
            }
            
        }
        while (requestTaskList.Count > 0)
        {
            Task<(HttpStatusCode,string)> taskres = await Task.WhenAny(requestTaskList) as Task<(HttpStatusCode,string)>;
            (HttpStatusCode,string) res = taskres.Result;
            requestTaskList.Remove(taskres);
            if (res.Item1 == HttpStatusCode.OK)
            {
                hashSet.Add(res.Item2);
            }
        }

        List<string> resultatGo = new List<string>();
        
        foreach (var VARIABLE in hashSet)
        {
            resultatGo.Add(VARIABLE);
        }
        Debug.Log("FIN GOBUSTER");
        return resultatGo;
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

    public static async Task<List<string>> GetText(List<string> list)
    {
        string pattern = "(<input)";
        Regex regex = new Regex(pattern);
        List<string> nlist = new List<string>();
        foreach (var item in list)
        {
            string s= Utils.MakeRequest(item);
            if (regex.IsMatch(s) && s != "")
            {
                nlist.Add(item);
            }
        }

        return nlist;
    }
    public static async Task<string> SourceCode(string url) //Retourne le code source du site à l'url
    {
        HttpWebRequest r = (HttpWebRequest)WebRequest.Create(url);
        r.Method = "GET";
        //try
        //{
        //    WebResponse Response = r.GetResponse();
        //    StreamReader sr = new StreamReader(Response.GetResponseStream(), System.Text.Encoding.UTF8);
        //    string res = sr.ReadToEnd();
        //    sr.Close();
        //    Response.Close();
        //    return res;
        //}
        //catch (Exception e)
        //{
        //    Console.WriteLine(e);
        //    return "";
        //}

        var ping = await Request.Ping(url);
        if (ping.Item1  == HttpStatusCode.OK || ping.Item1 == HttpStatusCode.Redirect)
        { 
            WebResponse Response = await r.GetResponseAsync();
            StreamReader sr = new StreamReader(Response.GetResponseStream(), System.Text.Encoding.UTF8);
            string res = sr.ReadToEnd();
            sr.Close();
            Response.Close();
            return res;
        }
        else
        {
            return "";
        }
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
            if ( c.Name == "jwt") //& //rgx.IsMatch(c.Value) )
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
    // will detect all the */.git files
    public async Task<List<string>> Git(List<string> urls)
    {
        List<Task> tasks = new List<Task>();
        List<string> result = new List<string>();
        foreach (var url in urls)
        {
            tasks.Add(Request.Ping(url+ "/.git"));
        }

        while (tasks.Count !=0)
        {
            Task<(HttpStatusCode,string)> task = await Task.WhenAny(tasks) as Task<(HttpStatusCode,string)>;
            (HttpStatusCode,string) res = task.Result;
            tasks.Remove(task);
            if (res.Item1 is HttpStatusCode.OK)
            {
                result.Add(res.Item2);
            }
        }

        return result;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}