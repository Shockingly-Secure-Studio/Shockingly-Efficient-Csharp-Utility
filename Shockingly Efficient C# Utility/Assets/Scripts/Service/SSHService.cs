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

        private bool _isConnected = false;
        private SshClient _sshClient = null;
        private PrivateKeyFile _keyFile;
        
        public SSHService(string ip, int port, string version = "") : base(ip, port)
        {
            _version = version;
            //Task<PrivateKeyFile> task = GenerateKey();
            //task.Wait(100);
            //_keyFile = task.Result;
        }

        public override async Task<bool> IsOnline()
        {
            return _isConnected;
        }

        public bool ConnectPassword(string username, string password)
        {
            _sshClient = new SshClient(GetIP().ToString(), GetPort(), username, password);
            _sshClient.Connect();
            _isConnected = _sshClient.IsConnected;
            return _sshClient.IsConnected;
        }

        public bool ConnectKey(string username, params PrivateKeyFile[] key)
        {
            _sshClient = new SshClient(GetIP().ToString(), GetPort(), username, key);
            _isConnected = _sshClient.IsConnected;
            return _sshClient.IsConnected;
        }

        public static async Task<PrivateKeyFile> GenerateKey()
        {
            Debug.Log(Utils.Exec("ssh-keygen -f .ssh_key -t rsa -P \"\""));
            PrivateKeyFile keyFile = new PrivateKeyFile(".ssh_key");
            return keyFile;
        }

        public string SendCommand(string cmd)
        {
            string result;
            using (var command = _sshClient.CreateCommand(cmd))
            {
                result = command.Execute();
            }

            return result;
        }
        
    }
}