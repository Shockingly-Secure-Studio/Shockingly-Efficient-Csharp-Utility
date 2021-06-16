using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Renci.SshNet;
using UnityEngine;

namespace Service
{
    public class SSHService : TcpService
    {
        private string _version;

        private bool IsConnected = false;
        private SshClient _sshClient = null;
        private PrivateKeyFile KeyFile;
        
        public SSHService(Machine.Machine machine, int port, string version = "") : base(machine, port)
        {
            _version = version;
            KeyFile = GenerateKey();
        }

        public override Task<bool> IsOnline()
        {
            throw new System.NotImplementedException();
        }

        public bool ConnectPassword(string username, string password)
        {
            _sshClient = new SshClient(GetIP().ToString(), GetPort(), username, password);
            IsConnected = _sshClient.IsConnected;
            return _sshClient.IsConnected;
        }
        //ajouter un clef si on a une reverse shell, brut force le mot de passe 
        public bool ConnectKey(string username, params PrivateKeyFile[] key)
        {
            _sshClient = new SshClient(GetIP().ToString(), GetPort(), username, key);
            IsConnected = _sshClient.IsConnected;
            return _sshClient.IsConnected;
        }

        public static PrivateKeyFile GenerateKey()
        {
            Utils.Exec("ssh-keygen -f ssh_key -t rsa -P \"\"");
            PrivateKeyFile keyFile = new PrivateKeyFile("ssh_key");
            return keyFile;
        }
        
        
    }
}