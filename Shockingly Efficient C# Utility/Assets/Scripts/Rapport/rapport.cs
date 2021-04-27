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
        var fontFamily = iTextSharp.text.Font.FontFamily.TIMES_ROMAN;
        NewTitle(document,"Rapport de l'analyse",fontFamily,30);
        NewImage(document,@"./img.png");
        NewTitle(document,"Tableaux récapitulatif",fontFamily,15);
        var newTable=MakeTable(document,6,"IP,Niveau de vulnérabilité,Nombre total de failles,RCE,SQLi,XSS","7,5,5,3,3,3");
        List<AccessPointType> flaws = new List<AccessPointType>();
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
                    int[] nbF = new int[10];
                    for(var i=0;i<accessPoints.Count;i++)
                    {
                        nbF[(int) accessPoints[i].Type] += 1;
                        if(!flaws.Contains(accessPoints[i].Type))
                            flaws.Add(accessPoints[i].Type);
                    }
                    info += $",{nbF[0]},{nbF[1]},{nbF[2]}";
                }
            }
            AddLine(ref newTable,info);
        }
        document.Add(newTable);
        foreach (var flaw in flaws)
        {
            switch (flaw)
            {
                case AccessPointType.RCE:
                    NewTitle(document,"RCE",fontFamily,13);
                    document.Add(new Paragraph("\tRCE ou remonte commande injection, ce sont des failles qui" +
                                               " permettent à l’utilisateur d’exécuter des commandes, pour éviter " +
                                               "ce type de faille il faut vérifier  les requêtes de l’utilisateur, " +
                                               "par exemple en vérifiant les symboles utilisées. \n", 
                        new iTextSharp.text.Font(fontFamily, 12)));
                    break;
                case AccessPointType.SQLi:
                    NewTitle(document,"SQLi",fontFamily,13);
                    document.Add(new Paragraph(
                        "\tLes SQLi, aussi appelé injection SQL est un type de faille qui a pour but interagir avec une base de " +
                        "données, pour cela on injecte un morceau malveillant de requête SQL dans une requête SQL qui" +
                        " va par exemple vérifier un mot de passe. Ce type de faille peux permettre par exemple de " +
                        "récupérer tous les mot de passe et les nom d’utilisateurs.Voici une ressource pour vous " +
                        "protéger contre les injections sql\nhttps://cheatsheetseries.owasp.org/cheatsheets/SQL_Inject" +
                        "ion_Prevention_Cheat_Sheet.html\n",
                        new iTextSharp.text.Font(fontFamily, 12)));
                    break;
                case AccessPointType.XSS:
                    NewTitle(document,"XSS",fontFamily,13);
                    document.Add(new Paragraph(
                        "\tLes failles XSS sont des failles liées aux différents point d’entrés du site web, per exemple " +
                        "en laissant un commentaire l’utilisateur peux tenter d’injecter du code malveillant, si votre serveur " +
                        "ne vérifie pas ce que rentre l’utilisateur il va renvoyer le code qui va être interpréter par les " +
                        "navigateurs des autres utilisateur qui visiterons la page. Ce type d’attaques peux permettre de " +
                        "voler les cookies, si un personne vole un cookie d’un administrateur il peux élever ses privilèges" +
                        " et accéder à des informations sensible. Il peux aussi par exemple rediriger les utilisateurs sur " +
                        "un page malveillantes. Voici une ressouce pour vous protéger contre les XSS\nhttps://cheatsheet" +
                        "series.owasp.org/cheatsheets/Content_Security_Policy_Cheat_Sheet.html#defense-against-xss\n",
                        new iTextSharp.text.Font(fontFamily, 12)));
                    break;
            }
        }

    }
   
    
}
