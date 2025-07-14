﻿using UnityEngine;
using System.Drawing;

using System.IO;
using System;
using System.Text;
using iTextSharp.text.pdf;
using OnBarcode.Barcode;
using SpinToWin;
using UnityEngine.UI;

using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using iTextSharp.text;
using System.Threading.Tasks;
//using UnityEngine.UI;
using khelojeetonew;
using System.Collections;
using System.Diagnostics;

public class Printer_TCP : MonoBehaviour
{
    public static Printer_TCP Instance;
    public Button betbtn;

    private void Awake()
    {
        Instance = this;
    }
     private void Start() {
            Application.runInBackground = true;
         UnityEngine.Debug.Log("application running in background");
        }
      


   public  void PrintPdf()
    {
        betbtn.interactable =false;
        //  Stopwatch stopwatch = Stopwatch.StartNew();
        //   betButton.interactable = false;
        string printFolderPath = Path.Combine(Application.dataPath, "print");
        if (Directory.Exists(printFolderPath))
        {
            string[] files = Directory.GetFiles(printFolderPath);
            foreach (string file in files)
            {
                File.Delete(file);
             UnityEngine.Debug.Log("file deleted ");
            }
            string[] subfolders = Directory.GetDirectories(printFolderPath);
            foreach (string subfolder in subfolders)
            {
                Directory.Delete(subfolder, true);
              UnityEngine.Debug.Log("sub folder deleted");
            }
        }
        else
        {

            Directory.CreateDirectory(printFolderPath);
        }
        string pdfFilePath = Path.Combine(Application.dataPath, "print", GamePlay.instance.ticketId + ".pdf");




        string ticketID = GamePlay.instance.ticketId;
        
        PDFManager_TCP pdfMgr =  PDFManager_TCP.CreatePDFBuilderWithPathAndTicketID(pdfFilePath, ticketID);

        string pdfString = GetPdfString(pdfMgr);


       // pdfMgr.CreateParagraph(pdfString, Element.ALIGN_LEFT, 8f, iTextSharp.text.Font.NORMAL);

        pdfMgr.CloseDocument();
       //   stopwatch.Stop();
     //   UnityEngine.Debug.Log("Preparing Data Elapsed time: " + stopwatch.ElapsedMilliseconds + "ms");

        // pdfMgr.print();
        pdfMgr.startprintticket(); 
StartCoroutine(Reactivatebet(5f));
    }
private IEnumerator Reactivatebet(float timeduration)
{
 // Re-enable bet button only after PDF printing is complete
    yield return new WaitForSeconds(timeduration);
    betbtn.interactable = true;
  UnityEngine.Debug.Log("time duration to activate:"+timeduration);
}



    public static string drawTime;
    private int duration = 80;
    public static void OnGameRestart()
    {
        // Reset drawTime to null so that it's recalculated after the game restart
        drawTime = null;
        UnityEngine.Debug.Log("Draw time reset after game restart.");
    }


    private string GetPdfString(PDFManager_TCP pdfManager)
{
    JObject jsonData;
    StringBuilder finalCardValueSet = new StringBuilder(); // Declare it here

    try
    {
        jsonData = JObject.Parse(GamePlay.instance.jsonString_TCP);
    }
    catch (Newtonsoft.Json.JsonReaderException)
    {
        JArray jsonArray;
        try
        {
            jsonArray = JArray.Parse(GamePlay.instance.jsonString_TCP);
        }
        catch (Newtonsoft.Json.JsonReaderException)
        {
            return "Invalid JSON data";
        }
                if (string.IsNullOrEmpty(drawTime))
            {
                DateTime currentDatebetTime = DateTime.Now;
                DateTime calculatedDrawTime = currentDatebetTime.AddSeconds(duration); // Subtract the duration from the current time
                drawTime = calculatedDrawTime.ToString("MM/dd/yyyy hh:mm:ss tt");

            }

            DateTime currentDateTime = DateTime.Now;
        string formattedDateTime = currentDateTime.ToString("MM/dd/yyyy hh:mm:ss tt");

        pdfManager.CreateParagraph("For Amusement only", Element.ALIGN_CENTER, 12f, iTextSharp.text.Font.NORMAL);
        pdfManager.CreateParagraph("S m a r t W i n", Element.ALIGN_CENTER, 30f, iTextSharp.text.Font.BOLD);

        // Populate the final card value set without duplicating it.
        finalCardValueSet.AppendLine("GameName: TripleChance");
        finalCardValueSet.AppendLine("Ticket Id: " + GamePlay.instance.ticketId);
        finalCardValueSet.AppendLine("Ticket Time: " + formattedDateTime);
        finalCardValueSet.AppendLine("Draw Time: " + drawTime);
            finalCardValueSet.AppendLine("Total Point: " + GamePlay.instance.TotalBet);
        pdfManager.CreateParagraph(finalCardValueSet.ToString(), Element.ALIGN_LEFT, 12.5f, iTextSharp.text.Font.NORMAL);

        // Build card data
        StringBuilder CardData = new StringBuilder();
        CardData.AppendLine("Item  Point   Item  Point   Item  Point   Item  Point");
        int count = 0;
        foreach (var card in jsonArray)
        {
            string cardName = (string)card["card"];
            int cardValue = (int)card["value"];
            CardData.AppendFormat("  {0,-8} {1,-8}", cardName, cardValue);

            count++;
            if (count % 4 == 0)
            {
                CardData.AppendLine();
            }
        }

        // Add card data to PDF
        pdfManager.CreateParagraph(CardData.ToString(), Element.ALIGN_LEFT, 12f, iTextSharp.text.Font.NORMAL);
    }

    // Add the barcode and bottom text only once
    pdfManager.AddBarcodeToPdf("           ");
    pdfManager.CreateParagraph("**Ticket not for sale**", Element.ALIGN_CENTER, 12f, iTextSharp.text.Font.NORMAL);

    return finalCardValueSet.ToString();
}
}