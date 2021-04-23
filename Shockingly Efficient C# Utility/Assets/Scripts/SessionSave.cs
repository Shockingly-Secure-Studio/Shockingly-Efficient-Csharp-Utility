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
    public static bool SaveSession(string saveName,bool overwrite=false)
    {
        string savePath = Path.Combine(_sessionSaveDir,saveName);
        if (!Directory.Exists(_sessionSaveDir))
        {
            Directory.CreateDirectory(_sessionSaveDir);
        }
        if(Directory.Exists(savePath))
        {
            Debug.Log($"A save have already the same name, overwrite:{overwrite}");
            if (overwrite)
            {
                Directory.Delete(savePath,true);
            }
            else
                return false;
        }
        DirectoryCopy( _sessionResultDir,savePath);
        return true;
    }
    public static void EndSession(bool save,string saveName="")
    {
        if (Directory.Exists(_sessionResultDir))
        {
            if(save)
                SaveSession(saveName,true);
            Directory.Delete(_sessionResultDir,true);
        }
        else
        {
            Debug.Log("No results directory, nothing to save");
        }
    }

    public static bool LoadSession(string saveName)
    {
        var loadPath = Path.Combine(_sessionSaveDir, saveName);
        if (!Directory.Exists(loadPath))
        {
           Debug.Log("Directory does not exist or could not be found:"+loadPath);
           return false;
        }
        Directory.Delete(_sessionResultDir,true);
        DirectoryCopy(loadPath,_sessionResultDir);
        (string info,List<IPAddress> ipList) =SaveScan.LoadIpScan("ipScan");
        if (info == null)
        {
            Debug.Log("Error the session ipList is empty");
            return false;
        }
        string[] infos = info.Split(','); 
        List<SaveScan.Device> devices = SaveScan.LoadJson("scanPort");
        List<IPAddress> remainingPortScan = new List<IPAddress>();
        if (devices != null)
        {
            foreach (var d in devices)
            {
                if (!(ipList.Contains(IPAddress.Parse(d.IP))||d.scanStatus == "Underway"))
                    remainingPortScan.Add(IPAddress.Parse(d.IP));
            }
            ScanPort.MakePortScan(remainingPortScan,infos[0]);
        }
        if (infos[1] != "completed")
        {
            ScanIp o = new ScanIp(); 
            Thread newScan = new Thread(new ThreadStart( () => o.MakePing((infos[2], infos[3]),infos[0])));
            newScan.Start();
        }
        return true;
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