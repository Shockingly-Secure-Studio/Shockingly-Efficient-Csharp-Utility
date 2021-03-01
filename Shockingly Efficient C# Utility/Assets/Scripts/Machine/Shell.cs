using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Service;
using UnityEngine;

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

        private bool _isUpgraded = false;
        
        public WebShell(InputWebService entry, ShellType shellType, bool interactive)
        {
            _entry = entry;
            _shellType = shellType;
            _interactive = interactive;
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
    }
}