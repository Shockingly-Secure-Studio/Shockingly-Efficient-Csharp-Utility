using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using Scan;
using UnityEngine;

public class SessionSave
{
    private static string _sessionResultDir = "Results";
    private static string _sessionSaveDir = "Saves";
    public static void SaveSession(string saveName)
    {
        if (!Directory.Exists(_sessionSaveDir))
        {
            Directory.CreateDirectory(_sessionSaveDir);
        }
        string savePath = Path.Combine(_sessionSaveDir,saveName);
        DirectoryCopy( _sessionResultDir,savePath);
    }
    public static void EndSession(bool save,string saveName="")
    {
        if(save)
            SaveSession(saveName);
        Directory.Delete(_sessionResultDir);
    }

    public static void LoadSession(string saveName)
    {
        var loadPath = Path.Combine(_sessionSaveDir, saveName);
        if (!Directory.Exists(loadPath))
        {
           Debug.Log("Directory does not exist or could not be found:"+loadPath);
           return;
        }
        DirectoryCopy(loadPath,_sessionResultDir);
        (string info,List<IPAddress> ipList) =SaveScan.LoadIpScan("ipList");
        string[] infos = info.Split(','); 
        List<SaveScan.Device> devices = SaveScan.LoadJson("scanPort");
        List<IPAddress> remainingPortScan = new List<IPAddress>();
        foreach (var d in devices)
        {
            if (!(ipList.Contains(IPAddress.Parse(d.IP))||d.scanStatus == "Underway"))
                remainingPortScan.Add(IPAddress.Parse(d.IP));
        }
        ScanPort.MakePortScan(remainingPortScan,infos[0]);
        if (infos[1] != "completed")
        {
            ScanIp o = new ScanIp(); 
            Thread newScan = new Thread(new ThreadStart( () => o.MakePing((infos[2], infos[3]),infos[0])));
            newScan.Start();
        }
        
        
    }
    private static void DirectoryCopy(string sourceDirName, string destDirName)
    {
        DirectoryInfo dir = new DirectoryInfo(sourceDirName);
        if (!dir.Exists)
        {
            Debug.Log("Source directory does not exist or could not be found:"+sourceDirName);
            return;
        }
        DirectoryInfo[] dirs = dir.GetDirectories();
        Directory.CreateDirectory(destDirName);
        FileInfo[] files = dir.GetFiles();
        foreach (FileInfo file in files)
        {
            string tempPath = Path.Combine(destDirName, file.Name);
            file.CopyTo(tempPath, false);
        }
        foreach (DirectoryInfo subdir in dirs)
        {
            string tempPath = Path.Combine(destDirName, subdir.Name);
            DirectoryCopy(subdir.FullName, tempPath);
        }
    }
}