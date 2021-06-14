using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Machine;
using Service.Exploit;
using UnityEngine;

namespace Service
{
    public class FTPService : TcpService
    {
        private Machine.Machine _machine;
        private Uri _serverUri;

        private (string, string)[] _defaultCredentials;
        private List<(string, string)> _validatedCredentials;

        public FTPService(Machine.Machine machine, int port) : base(machine, port)
        {
            _machine = machine;
            _serverUri = new Uri($"ftp://{machine.IPAdress}:{port}/");
            
            _defaultCredentials = new [] {
                ("anonymous", "anonymous"),
                ("anonymous", ""),
                ("test", "notpossible")
            };
            _validatedCredentials = new List<(string, string)>();
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

        public void DumpFiles(NetworkCredential credentials, string url)
        { 
            Thread.Sleep(100);
            Debug.Log(_serverUri + url);
            
            string localPath = Path.Combine(WorkingDirectory, "FTP_dump");
            if (!Directory.Exists(localPath))
            {
                Directory.CreateDirectory(localPath);
            }
            FtpWebRequest listRequest = (FtpWebRequest)WebRequest.Create(_serverUri + url);
            listRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            listRequest.Credentials = credentials;

            List<string> lines = new List<string>();
            try
            {
                using (FtpWebResponse listResponse = (FtpWebResponse) listRequest.GetResponse())
                using (Stream listStream = listResponse.GetResponseStream())
                using (StreamReader listReader = new StreamReader(listStream))
                {
                    while (!listReader.EndOfStream)
                    {
                        lines.Add(listReader.ReadLine());
                    }
                }
            }
            catch (WebException e)
            {
                //Debug.LogException(e);
                if (e.Message.Contains("Login or password incorrect"))
                    return;
                Thread.Sleep(1000);
                DumpFiles(credentials, url);
                return;
            }

            foreach (string line in lines)
            {
                string[] tokens =
                    line.Split(new[] { ' ' }, 9, StringSplitOptions.RemoveEmptyEntries);
                string name = tokens[8];
                string permissions = tokens[0];

                string localFilePath = Path.Combine(localPath, name);
                string fileUrl = url + name;

                if (permissions[0] == 'd')
                {
                    Debug.Log(fileUrl);
                    
                    DumpFiles(credentials, fileUrl + "/");
                }
                else
                {
                    FtpWebRequest downloadRequest = (FtpWebRequest) WebRequest.Create(_serverUri + fileUrl);
                    downloadRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                    downloadRequest.Credentials = credentials;

                    FtpWebResponse downloadResponse;
                    try
                    {
                        downloadResponse =
                            (FtpWebResponse) downloadRequest.GetResponse();
                    }
                    catch (WebException e)
                    {
                        //Debug.LogException(e);
                        Debug.Log("BOUh");
                        Thread.Sleep(1000);
                        downloadResponse = (FtpWebResponse) downloadRequest.GetResponse();
                    }

                    using (Stream sourceStream = downloadResponse.GetResponseStream())
                    using (Stream targetStream = File.Create(localFilePath))
                    {
                        byte[] buffer = new byte[10240];
                        int read;
                        while ((read = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            targetStream.Write(buffer, 0, read);
                        }
                    }
                    
                    downloadResponse.Close();
                }
            }
        }

        public void DumpFiles()
        {
            foreach ((string, string) cred in _validatedCredentials)
            {
                DumpFiles(new NetworkCredential(cred.Item1, cred.Item2), "");
            }
        }

        public override Task<bool> IsOnline()
        {
            throw new NotImplementedException("FTPService.IsOnline: TODO: Find a way to check async a server");
        }

        public bool Ping(string username, string password)
        {
            NetworkCredential cred = new NetworkCredential(username, password);
            try
            {
                ListFiles(cred);
            }
            catch (WebException e)
            {
                if (e.Message.Contains("Login or password incorrect") || e.Message.Contains("Login incorrect"))
                    return false;
                throw;
            }

            return true;
        }

        public void ValidateCredentials()
        {
            foreach ((string, string) credential in _defaultCredentials)
            {
                string username = credential.Item1;
                string password = credential.Item2;
                if (Ping(username, password))
                    _validatedCredentials.Add((username, password));
                
            }
        }

        public void Exploit()
        {
            ValidateCredentials();
            DumpFiles();

            bool hasWorked = _validatedCredentials.Count > 0;
            if (!hasWorked) return;
            string creds = "";
            foreach ((string, string) cred in _validatedCredentials)
            {
                creds += $"{cred.Item1}:{cred.Item2}\n";
            }
            string username = _validatedCredentials[0].Item1;
            string password = _validatedCredentials[0].Item2;
            AccessPoint vuln = new AccessPoint(
                $"{username}:{password}@{_serverUri}",
                $"{username}:{password}@{_serverUri}",
                AccessPointType.Insecure_Authentication,
                4,
                creds
            );
            Log(vuln);
            //return Directory.GetDirectories(Path.Combine(WorkingDirectory, "FTP_dump")).Length > 0;
        }
    }
}