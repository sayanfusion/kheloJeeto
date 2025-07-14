using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class PdfiumWrapper : MonoBehaviour
{
    // Import PDFium functions
    [DllImport("pdfium.dll", EntryPoint = "FPDF_InitLibrary")]
    public static extern void InitLibrary();

    [DllImport("pdfium.dll", EntryPoint = "FPDF_DestroyLibrary")]
    public static extern void DestroyLibrary();

    [DllImport("pdfium.dll", EntryPoint = "FPDF_LoadDocument")]
    public static extern IntPtr LoadDocument(string filePath, string password);

    [DllImport("pdfium.dll", EntryPoint = "FPDF_CloseDocument")]
    public static extern void CloseDocument(IntPtr document);

    [DllImport("pdfium.dll", EntryPoint = "FPDF_LoadPage")]
    public static extern IntPtr LoadPage(IntPtr document, int pageIndex);

    [DllImport("pdfium.dll", EntryPoint = "FPDF_ClosePage")]
    public static extern void ClosePage(IntPtr page);

    private void Start()
    {
        InitLibrary();

        // Path to a sample PDF file
        string pdfPath = Application.dataPath + "/Sample.pdf";

        // Load the PDF document
        IntPtr document = LoadDocument(pdfPath, null);
        if (document != IntPtr.Zero)
        {
            Debug.Log("PDF Loaded Successfully");

            // Example: Load and close a page
            IntPtr page = LoadPage(document, 0);
            if (page != IntPtr.Zero)
            {
                Debug.Log("Page Loaded Successfully");
                ClosePage(page);
            }

            CloseDocument(document);
        }

        DestroyLibrary();
    }
}
