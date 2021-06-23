using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Machine;
using Newtonsoft.Json;
using UnityEngine;

namespace DefaultNamespace
{
    public class WebShellInterface
    {
        public WebShell WebShell;

        public static WebShellInterface AttachWebShell(string ip, string port)
        {
            string serialized = File.ReadAllText(Path.Combine("Results", ip, port, "rce.json"));
            WebShell webShell = JsonConvert.DeserializeObject<WebShell>(serialized);
            WebShellInterface @interface = new WebShellInterface
            {
                WebShell = webShell
            };
            return @interface;
        }

        public async Task<string> SendCommand(string command)
        {
            return await WebShell.SendCommand(command);
        }
    }
}