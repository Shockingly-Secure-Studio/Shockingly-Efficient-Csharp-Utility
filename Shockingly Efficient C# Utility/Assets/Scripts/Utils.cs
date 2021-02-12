using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

public static class Utils
{
        
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
    
    public static string Cmd(this string cmd)
    {
        var escapedArgs = cmd.Replace("\"", "\\\"");
            
        var process = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "C:\\Windows\\System32\\cmd.exe",
                Arguments = $"/C \"{escapedArgs}\"",
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

    public static string Exec(this string cmd)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return Cmd(cmd);
        return Bash(cmd);
    }

    public static bool IsProgrammInstalled(string programm)
    {
        string res;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            res = Cmd($"where {programm}").Split('\n')[0];
        else
            res = Bash($"which {programm}").Split('\n')[0];
        
        return new FileInfo(res).Exists;
    }

    public enum WebMethod
    {
        GET,
        POST
    }
}