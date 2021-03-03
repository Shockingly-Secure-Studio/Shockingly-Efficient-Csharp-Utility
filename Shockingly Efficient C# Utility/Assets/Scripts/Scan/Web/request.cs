using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
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
            List<string> map = web.map(list,url);
        }
        public string GetDomainName(string url)
        {
            HttpWebRequest r = (HttpWebRequest)WebRequest.Create(url);
            r.Method = "GET";
            WebResponse response = r.GetResponse();
            Uri s = response.ResponseUri;
            string ns = s.ToString();
            ns = ns.Remove(0, 7);
            ns = ns.Remove(ns.Length - 1, 1);
            return ns;
        }
        
    }
}