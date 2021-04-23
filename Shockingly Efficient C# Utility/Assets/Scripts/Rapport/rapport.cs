using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using Scan;
using Service.Exploit;
using UnityEngine;

public class rapport: MonoBehaviour
{
    List<SaveScan.Device> devicesList = SaveScan.LoadJson("scanPort");
    public void test()
    {

        System.IO.FileStream fs = new FileStream(@".\report.pdf", FileMode.Create);
        Document document = new Document(PageSize.A4, 25, 25, 30, 30);  
        PdfWriter writer = PdfWriter.GetInstance(document, fs);  
        document.Open();
        foreach (var device in devicesList)
        {
            string info = "The machine " + device.hostName + "(" + device.IP + ")" + "has been scaned and we found " +
                          device.nbOfSFlaw
                          + " flaws, the security level is " + device.severityLevel+".\n";
            DirectoryInfo deviceDirectoryInfo = new DirectoryInfo(@".\Results\"+device.IP+@"\");
            var dirList=deviceDirectoryInfo.EnumerateDirectories();
            foreach (var dir in dirList)
            {
                string path = Path.Combine(dir.ToString(), "output.json");
                if(File.Exists(path))
                {
                    string json = File.ReadAllText(path);
                    List<AccessPoint> accessPoints = JsonConvert.DeserializeObject<ServiceResult>(json).AccessPoints;
                    for(var i=0;i<accessPoints.Count;i++)
                    {
                        info += "Flaw "+i+": "+accessPoints[i].Type.ToString()+".\n";
                    }
                }
            }
            document.Add(new Paragraph(info));
        }
        document.Close(); 
        writer.Close();
        fs.Close();

    }
   
    
}
