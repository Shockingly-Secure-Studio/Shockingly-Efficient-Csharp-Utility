using System;
using System.Threading.Tasks;
using Service;

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

        public string PreCommand(string command)
        {
            string preCommand = "";
            string postCommand = "";
            if (_shellType == ShellType.Linux || _shellType == ShellType.Windows)
            {
                preCommand = "echo <SECUStudio> && ";
                postCommand = " && echo </SECUStudio>";
            }

            if (_isUpgraded)
            {
                preCommand = "passthru(" + preCommand;
                postCommand = postCommand + ");";
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
            cmd = PreCommand(cmd);
            return await _entry.Submit(cmd);
        }
    }
}