using System;
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
            this._port = port;
            this._ip = ip;
            this._url = $"http://{ip}:{port}/{urlData}";
        }

        public async Task<HttpStatusCode> Ping()
        {
            using var client = new HttpClient();

            var result = await client.GetAsync(this._url);
            return result.StatusCode;
            
        }

        public string GetDomainName(string url)
        {
            
        }
        
    }
}