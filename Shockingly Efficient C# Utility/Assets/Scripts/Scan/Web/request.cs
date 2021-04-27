using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Runtime.Serialization;
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

        public static async Task<(HttpStatusCode,string)> Ping(string url)
        {
            using var client = new HttpClient();
            try
            {
                var result = await client.GetAsync(url);
                result.EnsureSuccessStatusCode();
                return (result.StatusCode,url);
            }
            catch
            {
                return (HttpStatusCode.GatewayTimeout, url);
            }
        }
        
        public async Task test()
        {
            List<(string, int)> list = new List<(string, int)>();
            List<string> url = new List<string>();
            list.Add(("127.0.0.1",80));
            List<string> map = await web.map(list,url);
            foreach (var element in map)
            {
                Debug.Log(element);
            }
            List<string> listurl = web.GetInUrl(map);
            List<string> input = await web.GetText(map);
        }
        public string GetDomainName(string url)
        {
            if (url.Contains("localhost") || this._ip != "")
            {
                if (this._ip == "")
                {
                    if (this._port != -1)
                    {
                        return "localhost:" + this._port;
                    }
                    return "localhost";
                }
                else
                {
                    if (this._port != -1)
                    {
                        return "localhost:" + this._port;
                    }
                    return this._ip;
                }   
            }
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