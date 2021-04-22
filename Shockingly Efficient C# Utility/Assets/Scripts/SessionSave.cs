using System.IO;
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
    public static void EndSession()
    {
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
        //resume all scan
        
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