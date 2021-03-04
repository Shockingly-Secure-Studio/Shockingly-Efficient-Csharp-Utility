using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Web
{
    public class Request 
    {
        private string _ip;

        private int _port;

        private object _data;

        private string _url;

        public Request(string ip, int port, object data, string urlData)
        {
            this._data = data;
            
            if (urlData == null)
            {
                this._url = $"http://{ip}:{port}";
            }
            else
            {
                this._url = $"http://{ip}:{port}/{urlData}";
            }

            if (ip == "" && port == -1)
            {
                this._port = 0;
                this._ip = "";
            }
            else
            {
                this._ip = ip;
                this._port = port;
            }
        }

        public async Task<HttpStatusCode> Ping()
        {
            using var client = new HttpClient();

            var result = await client.GetAsync(this._url);
            return result.StatusCode;
            
        }

        public void test()
        {
            List<(string, int)> list = new List<(string, int)>();
            List<string> url = new List<string>();
            url.Add("http://secu.studio");
            url.Add("http://challenge01.root-me.org/web-serveur/ch19/");
            List<string> map = web.map(list,url);
            List<string> listurl = web.GetInUrl(map);
            List<string> input = web.GetText(map);
        }
        public string GetDomainName(string url)
        {
            if (url.Contains("localhost"))
                return "localhost";
            HttpWebRequest r = (HttpWebRequest)WebRequest.Create(url);
            r.Method = "GET";
            WebResponse response = r.GetResponse();
            Uri s = response.ResponseUri;
            string ns = s.ToString();
            string pattern = "([a-zA-Z0-9]+[.]+[-a-zA-Z]*[.]*[a-zA-Z]+)";
            Regex regex = new Regex(pattern);
            string nns = regex.Match(ns).ToString();
            return nns;
        }
        
    }
}