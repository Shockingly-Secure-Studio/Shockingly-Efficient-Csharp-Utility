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
        
        Rectangle page = document.PageSize;
        PdfPTable head = new PdfPTable(1);
        head.TotalWidth = page.Width;
        Phrase phrase = new Phrase(
            DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + " GMT",
            new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.COURIER, 8)
        );    
        PdfPCell c = new PdfPCell(phrase);
        c.Border = Rectangle.NO_BORDER;
        c.VerticalAlignment = Element.ALIGN_TOP;
        c.HorizontalAlignment = Element.ALIGN_CENTER;
        head.AddCell(c);
        head.WriteSelectedRows(
            0, -1,
            0,
            page.Height - document.TopMargin + head.TotalHeight + 20,
            writer.DirectContent 
        );

        var font = iTextSharp.text.Font.FontFamily.TIMES_ROMAN;
        phrase = new Phrase("Rapport de l'analyse",new iTextSharp.text.Font(font,30));
        var title = new Paragraph(phrase);
        title.Alignment = Element.ALIGN_CENTER;
        document.Add(title);

        iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(@"./img.png");
        img.ScaleToFit(140f, 120f);
        img.SpacingBefore = 10f;
        img.SpacingAfter = 1f;
        img.Alignment = Element.ALIGN_CENTER;
        document.Add(img);
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
                        info += "\tFlaw "+i+": "+accessPoints[i].Type.ToString()+".\n";
                    }
                }
            }
            document.Add(new Paragraph(info,new iTextSharp.text.Font(font,12)));
        }
        
        document.Close(); 
        writer.Close();
        fs.Close();

    }
   
    
}
