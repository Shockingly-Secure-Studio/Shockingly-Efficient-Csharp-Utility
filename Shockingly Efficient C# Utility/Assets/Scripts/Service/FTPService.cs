using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Machine;
using UnityEngine;

namespace Service
{
    public class FTPService : Service
    {
        private Machine.Machine _machine;
        private Uri _serverUri;

        public FTPService(Machine.Machine machine, int port) : base(machine, port)
        {
            _machine = machine;
            _serverUri = new Uri($"ftp://{machine.IPAdress}:{port}");
        }

        /// <summary>
        /// Try to list all files, with specified credentials.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public List<string> ListFiles(NetworkCredential credential)
        {
            // The serverUri parameter should start with the ftp:// scheme.
            if (_serverUri.Scheme != Uri.UriSchemeFtp)
            {
                throw new ArgumentException($"FTPService.ListFiles: Invalid uri scheme \"{_serverUri}\"");
            }

            // Get the object used to communicate with the server.
            FtpWebRequest request = (FtpWebRequest) WebRequest.Create(_serverUri);
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            request.Credentials = credential;

            FtpWebResponse response = (FtpWebResponse) request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            if (responseStream == null)
                throw new Exception("FTPService: Server seems to be offline. responseStream is null");
            
            StreamReader reader = new StreamReader(responseStream);  
            string names = reader.ReadToEnd();
  
            reader.Close();  
            response.Close();
            
            return names.Split(new [] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public List<string> ListFiles()
        {
            HashSet<string> result = new HashSet<string>();
            (string, string)[] credentials = new[]
            {
                ("anonymous", "anonymous"),
                ("anonymous", "")
            };
            foreach ((string, string) cred in credentials)
            {
                List<string> temp = ListFiles(new NetworkCredential(cred.Item1, cred.Item2));
                foreach (string filename in temp)
                {
                    result.Add(filename);
                }
            }
            
            return result.ToList();
        }

        public void DumpFiles(NetworkCredential credential)
        {
            List<string> filenames = ListFiles(credential);
            IEnumerable<string> allFiles = filenames.Union(ListFiles());
            
        }

        public override Task<bool> IsOnline()
        {
            throw new NotImplementedException("FTPService.IsOnline: TODO: Find a way to check async a server");
        }
    }
}