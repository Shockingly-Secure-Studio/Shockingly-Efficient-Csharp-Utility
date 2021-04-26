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

    public void NewDocument()
    {
        System.IO.FileStream fs = new FileStream(@".\report.pdf", FileMode.Create);
        Document document = new Document(PageSize.A4, 25, 25, 30, 30);  
        PdfWriter writer = PdfWriter.GetInstance(document, fs);  
        document.Open();
        Header(document,writer);
        WriteInfo(document,writer);
        document.Close(); 
        writer.Close();
        fs.Close();
    }

    private void Header(Document document,PdfWriter writer)
    {
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
    }

    private void NewTitle(Document document,string text,iTextSharp.text.Font.FontFamily font,int size)
    {
        var phrase = new Phrase(text,new iTextSharp.text.Font(font,size));
        var title = new Paragraph(phrase);
        title.Alignment = Element.ALIGN_CENTER;
        document.Add(title);
    }

    private void NewImage(Document document,string path)
    {
        iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(path);
        img.ScaleToFit(140f, 120f);
        img.SpacingBefore = 10f;
        img.SpacingAfter = 1f;
        img.Alignment = Element.ALIGN_CENTER;
        document.Add(img);
    }

    private PdfPTable MakeTable(Document document, int nbcol,string colInfo, string colW)
    {
        string[] col =new string[nbcol];
        var n = 0;
        foreach (var colName in colInfo.Split(','))
        {
            col[n] = colName;
            n++;
        }
        
        PdfPTable table = new PdfPTable(nbcol);
        table.WidthPercentage = 100;
        var widths = new Single[nbcol];
        n = 0;
        foreach (var width in colW.Split(','))
        {
            widths[n] = int.Parse(width);
            n++;
        }
        table.SetWidths(widths);
        table.SpacingBefore = 10;
        
        foreach (var t in col)
        {
            PdfPCell cell = new PdfPCell(new Phrase(t));
            cell.BackgroundColor = new BaseColor(204, 204, 204);
            table.AddCell(cell);
        }
        return table;
    }

    private void AddLine(ref PdfPTable table,string info)
    {
        string[] infos = info.Split(',');
        int nbC = table.NumberOfColumns;
        for (var i = 0; i < nbC; i++)
        {
            table.AddCell(infos[i%nbC]);
        }
    }
    private void WriteInfo(Document document,PdfWriter writer)
    {
        var font = iTextSharp.text.Font.FontFamily.TIMES_ROMAN;
        NewTitle(document,"Rapport de l'analyse",font,30);
        NewImage(document,@"./img.png");
        var newTable=MakeTable(document,6,"IP,Severity level,Number of flaw,RCE,SQLi,XSS","7,3,5,3,3,3");
        
        foreach (var device in devicesList)
        {
            string info = $"{device.IP},{device.nbOfSFlaw},{device.severityLevel}";
            DirectoryInfo deviceDirectoryInfo = new DirectoryInfo(@".\Results\"+device.IP+@"\");
            var dirList=deviceDirectoryInfo.EnumerateDirectories();
            foreach (var dir in dirList)
            {
                string path = Path.Combine(dir.ToString(), "output.json");
                if(File.Exists(path))
                {
                    string json = File.ReadAllText(path);
                    List<AccessPoint> accessPoints = JsonConvert.DeserializeObject<ServiceResult>(json).AccessPoints;
                    int[] nbF = new int[3];
                    for(var i=0;i<accessPoints.Count;i++)
                    {
                        nbF[(int) accessPoints[i].Type] += 1;
                    }
                    info += $",{nbF[0]},{nbF[1]},{nbF[2]}";
                }
            }
            AddLine(ref newTable,info);
        }
        document.Add(newTable);

    }
   
    
}
