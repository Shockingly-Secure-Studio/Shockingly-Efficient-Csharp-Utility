using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Debug = UnityEngine.Debug;

public static class Utils
{
    // DEBUG mode, for local debugging when needed    
    public static bool DEBUG = true;

        
    // https://docs.microsoft.com/fr-fr/dotnet/api/system.net.sockets.socket?view=net-5.0
    public static Socket ConnectSocket(IPAddress server, int port)
    {
        Socket s = null;

        // Get host related information.
        IPHostEntry hostEntry = Dns.GetHostEntry(server);

        // Loop through the AddressList to obtain the supported AddressFamily. This is to avoid
        // an exception that occurs when the host IP Address is not compatible with the address family
        // (typical in the IPv6 case).
        foreach(IPAddress address in hostEntry.AddressList)
        {
            IPEndPoint ipe = new IPEndPoint(address, port);
            Socket tempSocket =
                new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            tempSocket.Connect(ipe);

            if(tempSocket.Connected)
            {
                s = tempSocket;
                break;
            }
        }
        return s;
    }
    
    public static string Bash(this string cmd)
    {
        var escapedArgs = cmd.Replace("\"", "\\\"");
            
        var process = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"{escapedArgs}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };
        process.Start();
        string result = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        return result;
    }
    
    public static (string, int) Cmd(this string cmd)
    {
        var escapedArgs = cmd.Replace("\"", "\\\"");


        string result = "";
        var process = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "C:\\Windows\\System32\\cmd.exe",
                Arguments = $"/C \"{escapedArgs}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            },
        };

        process.OutputDataReceived += (sender, e) =>
        {
            // Prepend line numbers to each line of the output.
            if (!string.IsNullOrEmpty(e.Data))
            {
                Debug.Log(escapedArgs + " : " + e.Data);
                result += e.Data;
            }
        };
        
        process.Start();
        process.BeginOutputReadLine();
        process.WaitForExit();
        return (result, process.ExitCode);
    }

    public static string Exec(this string cmd)
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? Cmd(cmd).Item1 : Bash(cmd);
    }

    public static bool IsProgrammInstalled(string programm)
    {
        string res;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return Cmd($"where {programm} /Q").Item2 == 0;
        else
            res = Bash($"which {programm}").Split('\n')[0].TrimEnd();

        try
        {
            return new FileInfo(res).Exists;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Make a simple request to a foreign server, it is made to be simple so no POST params or cookies or things like this.
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public static string MakeRequest(string url)
    {
        WebClient wc = new WebClient();
        Stream data = wc.OpenRead(url);
        StreamReader reader = new StreamReader(data);
        string s = reader.ReadToEnd();
        Console.WriteLine(s);
        data.Close();
        reader.Close();

        return s;
    }

    public enum WebMethod
    {
        GET,
        POST
    }
}