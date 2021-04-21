using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Service.Exploit;
using UnityEngine;

namespace Service
{
    public class WebService : Service
    {
        private readonly string _vHost;

        private static HttpClientHandler _handler = new HttpClientHandler()
        {
            Proxy = new WebProxy("127.0.0.1:8080", false),
            UseProxy = true
        };
        private readonly HttpClient _httpClient = new HttpClient(_handler);

        public WebService(Machine.Machine machine, int port, string vhost) : base(machine, port)
        {
            _vHost = vhost;
        }

        public override async Task<bool> IsOnline()
        {
            Task<string> result = Get("");
            return (await result) != "";
        }
        
        
        private async Task<string> MakeHttpRequest(string url, HttpMethod method, Dictionary<string, string> cookies = null,
            
            Dictionary<string, string> headers = null, Dictionary<string, string> content = null)
        {
            Uri uri = new Uri($"http://{_vHost}:{GetPort()}/{url}");
            HttpRequestMessage requestMessage = new HttpRequestMessage()
            {
                RequestUri = uri,
                Method = method
            };

            if (cookies != null)
            {
                string cookieString = "";
                foreach (KeyValuePair<string, string> cookie in cookies)
                {
                    cookieString += $"{cookie.Key}={cookie.Value}; ";
                }
                requestMessage.Headers.Add("Cookie", cookieString);
            }

            if (headers != null)
            {
                foreach (KeyValuePair<string, string> header in headers)
                {
                    requestMessage.Headers.Add(header.Key, header.Value);
                }
            }
            
            if (content != null)
                requestMessage.Content = new FormUrlEncodedContent(content);

            // Debug.Log(requestMessage.ToString());
            HttpResponseMessage responseMessage = await _httpClient.SendAsync(requestMessage);
            responseMessage.EnsureSuccessStatusCode();
            string responseContent = await responseMessage.Content.ReadAsStringAsync();
            return responseContent;
        }
        
        public async Task<string> Get(string url, Dictionary<string, string> cookies = null, Dictionary<string, string> headers = null)
        {
           /*
            GET / HTTP/2
            Host: www.google.com
            User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:86.0) Gecko/20100101 Firefox/86.0
            Accept-Language: fr,fr-FR;q=0.8,en-US;q=0.5,en;q=0.3
            Accept-Encoding: gzip, deflate, br
            DNT: 1
            Connection: keep-alive
            Cookie: 1P_JAR=2021-02-03-09; NID=208=fT_u75xbnJuhlEWdK-Sy0RIA2u9A_l6Haq_xotqm-qcIgZ_NEl8zj90Yc6C2irIb_mJyd698vE3DMLu1HgbKAVXN76cp4b6FWT6JOvJkCtm6h1JpPkBnEX5K61yMW5Sdd3d2tCcmXCFmir6PsjFltcq0jqdEw9S9Io6_Nh7sTq6LTZqNocPxO7UiZmsHhdYtW2NOt1sLOSZ_SRNrVQBEq9GRXv3xnnFcBAKCnnan_3NPpGy8XKASGseZzlkPvhS3uEmYbzfm41M_puEtHM0vEHR2ft3Gnh8; CONSENT=YES+FR.fr+20150628-20-0; SID=5wfFxupEIiEB9xzqiZd2CYYMPIhWW9PDKyYPLsCnFTebxLXSO8k3-LSNJ0_kVfEKchEswA.; __Secure-3PSID=5wfFxupEIiEB9xzqiZd2CYYMPIhWW9PDKyYPLsCnFTebxLXSig88CZidJ8fxcN0LSMp4EQ.; HSID=AmVWNyaVvYbFe3bPH; SSID=Aa-nAFkOaUD9Y0Eua; APISID=T-tr6hKG0iAqokWO/AyybAtKxOCURhlc-v; SAPISID=7FdvG7_c95_2mCrC/Aa7TMw4yRyVdnvAd5; __Secure-3PAPISID=7FdvG7_c95_2mCrC/Aa7TMw4yRyVdnvAd5; SIDCC=AJi4QfGyJ0p1JvjvsEI5x1DfNNtfT9JE9sdTaSQ_2N0U0t-8iHAPWi5bocyWzk-I6T1YfHIO65g; __Secure-3PSIDCC=AJi4QfF-CPEGUv9FVjRbeLuG3NLtj6vtOCpgsCGrZ8wGTLKc60nkaGCiUeXKGPyqThdVxd0vugA; OTZ=5798468_52_52_123900_48_436380
            Upgrade-Insecure-Requests: 1
            */

           return await MakeHttpRequest(url, HttpMethod.Get, cookies, headers);
        }

        public async Task<string> Post(string url, Dictionary<string, string> cookies = null,
            Dictionary<string, string> headers = null, Dictionary<string, string> content = null)
        {
            return await MakeHttpRequest(url, HttpMethod.Post, cookies, headers, content);
        }

        public string GetVhost()
        {
            return _vHost;
        }

        public async void Exploit()
        {
            List<(string, int)> list = new List<(string, int)>();
            List<string> url = new List<string>();
            url.Add($"http://{_vHost}:{GetPort()}/");
            List<string> map = web.map(list,url);
            List<InputWebService> total = new List<InputWebService>();
        
            map.Add($"http://{_vHost}:{GetPort()}/");
        
            
            foreach (var link in map)
            {
                Debug.Log(link);
                foreach (var inputWebService in await InputWebService.FromLink(GetIP().ToString(), GetPort(), link))
                {
                    total.Add(inputWebService);
                }
            }

            
            foreach (InputWebService inputWebService in total)
            {
                await inputWebService.Exploit(true);
            }

            
            Host.UpdateVulnerabilities();
        }
    }
}