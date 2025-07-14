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

namespace khelojeetonew
{
    public class PDFManager :MonoBehaviour
    {
        private string path = "";
        private Document document = null;
        private PdfWriter pdfWriter = null;
        bool Current = false;
        string outputPath;
         

        public  PDFManager(string path)
        {
            this.path = path + "Report" + ".pdf";
        }

        private PDFManager(string path, string ticketID)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            this.path = path + "TicketId-" + ticketID + ".pdf";
        }

       public static PDFManager CreatePDFBuilderWithPathAndTicketID(string path, string ticketID)
        {
            return new PDFManager(path,ticketID).createPDFFile();
        }
        public static PDFManager CreatePDFBuilderWithPath(string path)
        {
            return new PDFManager(path).createPDFFile();
        }
        public PDFManager PdfToImage()
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
          //  Thread thread = new Thread(print);
           Thread thread = new Thread(new ParameterizedThreadStart(print));
            thread.Start();
            return this;
        }

        public  void print()
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

      private PDFManager createPDFFile()
        {
            UnityEngine.Debug.LogError(path);
            

           FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            document = new Document(new Rectangle(3.5f * 72, 15 * 72), 4f, 4f, 0f, 0f);
           // document = new Document(new Rectangle(3.7f * 72, 15f * 72), 3f, 3f, 0f, 0f);
            pdfWriter = PdfWriter.GetInstance(document, fileStream);
            document.Open();
            document.NewPage();
            // document.Close();
            return this;
        }

   public PDFManager CreateParagraph(string content, int alignment, float fontSize, int fontType)
    {
        if (document != null && document.IsOpen())
        {
            iTextSharp.text.Font font = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, fontSize, fontType);
            Paragraph paragraph = new Paragraph(content, font);
            paragraph.Alignment = alignment;
            document.Add(paragraph);
        }
        return this;
    }   
         public PDFManager AddBarcodeToPdf(string code)
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
        Border = PdfPCell.NO_BORDER, 
        HorizontalAlignment = Element.ALIGN_CENTER, 
        VerticalAlignment = Element.ALIGN_MIDDLE 
    };
        table.AddCell(cell);
        document.Add(table);
            return this;
        }
      public PDFManager CloseDocument()
        {
            document.Close();
            pdfWriter.Close();
            UnityEngine.Debug.Log("close document");
            return this;
        }   
       public void StartPrintTicketDirect()
{
    Task.Run(() => PrintTicketDirect());
}

public PDFManager PrintTicketDirect()
{
    try
    {
        UnityEngine.Debug.LogError("Executed.1.........................");

        // Setup process start info for printing the PDF
        System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo(path.ToString());
        info.Verb = "print";
        info.CreateNoWindow = true;
        info.WindowStyle = ProcessWindowStyle.Normal;

        // Start the process asynchronously
        Process process = new Process();
        process.StartInfo = info;
        process.Start();
        // Optionally wait for the process to exit, if needed.
        // process.WaitForExit();
    }
    catch (Exception ex)
    {
        UnityEngine.Debug.LogError("Error while printing PDF: " + ex.Message);
    }

    return this;
}

   public void PrintTicketInBackground(string pdfFilePath)
{
    // Ensure the file exists before trying to print
    if (!File.Exists(pdfFilePath))
    {
        UnityEngine.Debug.LogError("PDF file not found: " + pdfFilePath);
        return;
    }

    UnityEngine.Debug.Log("Printing PDF: " + pdfFilePath);

    // Run the printing process asynchronously to avoid blocking the main game thread
    Task.Run(() =>
    {
        try
        {
            UnityEngine.Debug.LogError("ExeCuted.1.........................");
          System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo(path.ToString());
            info.Verb = "print";
            info.CreateNoWindow = true;
            info.WindowStyle = ProcessWindowStyle.Normal;
            Process process = new Process();
            process.StartInfo = info;
            process.Start();
         //  process.WaitForExit();
        //   return this;

            /* Process.Start(path, "/p");
             return this;*/
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError("Error printing PDF: " + ex.Message);
        }
    });
}

    // Method to print the PDF without blocking the main thread
    /*public PDFManager PrintTicketDirect()
    {
        UnityEngine.Debug.LogError("Executed.1.........................");

        // Run the printing logic asynchronously
        Task.Run(() =>
        {
            System.Diagnostics.ProcessStartInfo info = new ProcessStartInfo(path.ToString());
            info.Verb = "print";
            info.CreateNoWindow = true;
            info.WindowStyle = ProcessWindowStyle.Normal;

            using (Process process = new Process())
            {
                process.StartInfo = info;
                process.Start();*/

                // Wait for the printing process to finish without blocking the main thread
             //   process.WaitForExit();  // This only blocks the Task, not the main thread
          /*  }
        });

        return this;
    }*/


       public PDFManager PrintImage()
        {
            Process.Start("mspaint.exe", "/pt " + path);
            return this;
        }

    }
}

