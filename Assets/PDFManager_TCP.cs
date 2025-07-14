using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System;
using System.Diagnostics;
using Ghostscript.NET;
using System.Threading;
using System.Threading.Tasks;
using PdfiumViewer;

//using NewJeeto;

namespace SpinToWin
{
    public class PDFManager_TCP : MonoBehaviour
    {
           private FileStream fileStream;
        public static PDFManager_TCP instance;
        public string path = "";
        private Document document = null;
        private PdfWriter pdfWriter = null;
        bool Current = false;
        public string outputPath;
        private void Awake()
        {
            instance = this;
        }
          private PDFManager_TCP(string path)
        {
            this.path = path + "Report" + ".pdf";
        }
    /*  private PDFManager_TCP(string path)
{
    // Initialize the Document first
    document = new Document();
    // Initialize the FileStream
    fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);  
    // Initialize the PdfWriter with the Document and FileStream
    pdfWriter = PdfWriter.GetInstance(document, fileStream);  
    // Open the Document
    document.Open();  
    // Adjust the path as needed
    this.path = path + "Report" + ".pdf";
}*/
        private PDFManager_TCP(string path, string ticketID)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            this.path = path + "TicketId-" + ticketID + ".pdf";
        }
    
    
       
    
      /*  public static PDFManager_TCP CreatePDFBuilderWithPathAndTicketID(string path, string ticketID)
        {
            return new PDFManager_TCP(path, ticketID).createPDFFile();
        }

        public static PDFManager_TCP CreatePDFBuilderWithPath(string path)
        {
            return new PDFManager_TCP(path).createPDFFile();
        }*/
  public static PDFManager_TCP CreatePDFBuilderWithPathAndTicketID(string path, string ticketID)
        {
            return new PDFManager_TCP(path, ticketID).createPDFFile();
        }

        public static PDFManager_TCP CreatePDFBuilderWithPath(string path)
        {
            return new PDFManager_TCP(path).createPDFFile();
        }

        public PDFManager_TCP PdfToImage()
        {
            outputPath = path.Replace("pdf", "png");
            GhostscriptPngDevice ghostscriptPngDevice = new GhostscriptPngDevice(GhostscriptPngDeviceType.PngMono);
            ghostscriptPngDevice.GraphicsAlphaBits = GhostscriptImageDeviceAlphaBits.V_4;
            ghostscriptPngDevice.TextAlphaBits = GhostscriptImageDeviceAlphaBits.V_4;
            ghostscriptPngDevice.ResolutionXY = new GhostscriptImageDeviceResolution(210, 297);
            ghostscriptPngDevice.InputFiles.Add(path);
            ghostscriptPngDevice.Pdf.FirstPage = 1;
            ghostscriptPngDevice.Pdf.LastPage = 1;
            ghostscriptPngDevice.PostScript = string.Empty;
            ghostscriptPngDevice.OutputPath = outputPath;
            ghostscriptPngDevice.Process();
            Thread thread = new Thread(new ParameterizedThreadStart(print));
            thread.Start();
            return this;
        }

        public void print()
        {
            string outputPath1 = outputPath;
            var processInfo = new ProcessStartInfo("cmd.exe", $"/c wmic printer get name,default")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                WorkingDirectory = @"C:\Windows\System32\"
            };

            Process p = Process.Start(processInfo);

            p.OutputDataReceived += (sender, args) =>
            {

                if (args.Data.Contains("TRUE"))
                {
                    string printerName = args.Data.Replace("TRUE", "").Trim();
                    UnityEngine.Debug.LogError(printerName);
                    PrintingTool.CmdPrintThreaded(printerName, outputPath1);
                }

            };
            p.BeginOutputReadLine();
            p.WaitForExit();
        }
     private PDFManager_TCP createPDFFile()
{
    UnityEngine.Debug.LogError($"Creating PDF at path: {path}");
    FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
    document = new Document(new Rectangle(3.7f * 72, 15f * 72), 3f, 3f, 0f, 0f);
    pdfWriter = PdfWriter.GetInstance(document, fileStream);
    document.Open();
    document.NewPage();
    
    // Add default content to prevent empty PDF
    document.Add(new Paragraph("Default Content for PDF."));
    UnityEngine.Debug.Log("Added default content to PDF.");
    
    return this;
}

       /* public async Task<PDFManager_TCP> CreatePDFFileAsync()
       {
    return await Task.Run(() =>
    {
        try
        {
            // Open file stream only once
            using (FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                document = new Document(new Rectangle(3.7f * 72, 15f * 72), 3f, 3f, 0f, 0f);
                pdfWriter = PdfWriter.GetInstance(document, fileStream);
                document.Open();
                document.NewPage();
                // Add content here
            }
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError($"Error creating PDF: {ex.Message}");
        }
        return this;
    });
}*/


        public PDFManager_TCP CreateParagraph(string content, int alignment, float fontSize, int fontType)
        {
            if (fontSize == 0)
                fontSize = 10;
            iTextSharp.text.Font font = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, fontSize, fontType);
            Paragraph paragraph = new Paragraph(content, font);
            paragraph.Alignment = alignment;
            document.Add(paragraph);


            return this;

        }

        public PDFManager_TCP AddBarcodeToPdf(string code)
        {
            Barcode39 barcodeCodabar = new Barcode39();
            barcodeCodabar.Code = code;
            barcodeCodabar.CodeType = Barcode.CODABAR;
            PdfContentByte pdfContentByte = new PdfContentByte(pdfWriter);
            Image image = barcodeCodabar.CreateImageWithBarcode(pdfContentByte, null, null);
             PdfPTable table = new PdfPTable(1)
    {
        WidthPercentage = 100 // Set table width to 100% of the page width
    };
         PdfPCell cell = new PdfPCell(image)
    {
        Border = PdfPCell.NO_BORDER, // Optional: Remove cell border
        HorizontalAlignment = Element.ALIGN_CENTER, // Center image horizontally
        VerticalAlignment = Element.ALIGN_MIDDLE // Center image vertically (if cell height is set)
    };
    // Add the cell to the table
 table.AddCell(cell);
    // Add the table to the document
 document.Add(table);
            return this;
        }
public PDFManager_TCP CloseDocument()
        {
           // document.Close();
           // pdfWriter.Close();
          // fileStream.Close();
           if (document != null && document.IsOpen())
    {
        document.Close();
        UnityEngine.Debug.Log("Document closed successfully.");
    }

    if (pdfWriter != null)
    {
        pdfWriter.Close();
        UnityEngine.Debug.Log("Writer closed successfully.");
    }
            UnityEngine.Debug.Log("close document");
            return this;
        }
           public void startprintticket()
        {
             Task.Run(() =>PrintTicketDirect());
        }
     public PDFManager_TCP PrintTicketDirect()
        {
            UnityEngine.Debug.LogError("ExeCuted.1.........................");
            System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo(path.ToString());
            info.Verb = "print";
            info.CreateNoWindow = true;
            info.WindowStyle = ProcessWindowStyle.Normal;
            Process process = new Process();
            process.StartInfo = info;
            process.Start();

            return this;


        }
 /* public void PrintTicketDirect(string pdfFilePath, string printerName)
    {
        if (!File.Exists(pdfFilePath))
        {
            UnityEngine.Debug.LogError("PDF file not found: " + pdfFilePath);
            return;
        }

        UnityEngine.Debug.Log("Starting print process for: " + pdfFilePath);

        try
        {
            using (var document = PdfiumViewer.PdfDocument.Load(pdfFilePath))
            {
                using (var printDocument = document.CreatePrintDocument())
                {
                    printDocument.PrinterSettings.PrinterName = printerName;
                    printDocument.Print();
                }
            }

            UnityEngine.Debug.Log("Print process completed.");
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError("Error printing PDF: " + ex.Message);
        }
    }

        public PDFManager_TCP PrintImage()
        {
            Process.Start("mspaint.exe", "/pt " + path);
            return this;
        }

    }*/
}
}

