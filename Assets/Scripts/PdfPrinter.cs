using UnityEngine;
using System.Diagnostics;
using System.IO;

public class PdfPrinter : MonoBehaviour
{
    public string pdfFileName = "Sample.pdf"; // Name of the PDF file in the Assets folder

    public void PrintPDF()
    {
        string pdfFilePath = Path.Combine(Application.dataPath, pdfFileName);

        if (File.Exists(pdfFilePath))
        {
            Process printProcess = new Process();
            printProcess.StartInfo.FileName = pdfFilePath;
            printProcess.StartInfo.Verb = "print";
            printProcess.StartInfo.CreateNoWindow = true;
            printProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            printProcess.Start();
            printProcess.WaitForExit();

            // Optional: To clean up the print job
            printProcess.Close();
            UnityEngine.Debug.Log("Print job started for: " + pdfFilePath);
        }
        else
        {
            UnityEngine.Debug.LogError("PDF file not found: " + pdfFilePath);
        }
    }

    // For testing purposes, you can call this method in Start
    private void Start()
    {
        PrintPDF();
    }
}
