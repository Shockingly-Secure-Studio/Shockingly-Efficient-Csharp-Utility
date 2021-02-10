using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Renci.SshNet;
using TMPro;
using UnityEngine;

namespace Service
{
    public class SSHService : Service
    {
        private string _version;

        private bool IsConnected = false;
        private SshClient _sshClient = null;
        private PrivateKeyFile KeyFile;
        
        public SSHService(string ip, int port, string version = "") : base(ip, port)
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

        public bool ConnectKey(string username, params PrivateKeyFile[] key)
        {
            _sshClient = new SshClient(GetIP().ToString(), GetPort(), username, key);
            IsConnected = _sshClient.IsConnected;
            return _sshClient.IsConnected;
        }

        public static PrivateKeyFile GenerateKey()
        {
            Utils.Exec("ssh-keygen -f ssh_key -t rsa -P");
            PrivateKeyFile keyFile = new PrivateKeyFile(".ssh_key");
            return keyFile;
        }
        
        
    }
}