// using UnityEngine;

// using System.IO;
// using System;
// using System.Text;

// using UnityEngine.UI;
// using Newtonsoft.Json.Linq;
// using iTextSharp.text;
// using Newtonsoft.Json;
// using System.Diagnostics;
// using System.Threading.Tasks;
// using System.Collections;

// namespace khelojeetonew
// {
//     public class Printer16card : MonoBehaviour
//     {
//         //  public Image Barcode;

//         public Button betButton16card;
//         private void Start()
//         {
//             Application.runInBackground = true;
//             UnityEngine.Debug.Log("application running in background");
//         }
//         public async void PrintPdf()
//         {
//             betButton16card.interactable = false;
//             UnityEngine.Debug.Log("bet button deactivate print process running..");
//             Stopwatch stopwatch = Stopwatch.StartNew();
//             string printFolderPath = Path.Combine(Application.dataPath, "print");

//             // Clean up old files
//             if (Directory.Exists(printFolderPath))
//             {
//                 foreach (string file in Directory.GetFiles(printFolderPath))
//                 {
//                     File.Delete(file);
//                 }
//                 foreach (string subfolder in Directory.GetDirectories(printFolderPath))
//                 {
//                     Directory.Delete(subfolder, true);
//                 }
//             }
//             else
//             {
//                 Directory.CreateDirectory(printFolderPath);
//             }

//             string pdfFilePath = Path.Combine(printFolderPath, Bet16card.instance.ticketId + ".pdf");
//             string ticketID = Bet16card.instance.ticketId;

//             // Create the PDF asynchronously
//             PDFManager pdfMgr = PDFManager.CreatePDFBuilderWithPathAndTicketID(pdfFilePath, ticketID);

//             // Ensure PDF content is created before closing the document
//             string pdfString = GetPdfString(pdfMgr);
//             // pdfMgr.CreateParagraph(pdfString, Element.ALIGN_LEFT, 12, iTextSharp.text.Font.NORMAL);

//             pdfMgr.CloseDocument();
//             await Task.Run(() => pdfMgr.StartPrintTicketDirect());

//             // Stopwatch log (if needed)
//             stopwatch.Stop();
//             UnityEngine.Debug.Log("Preparing Data Elapsed time: " + stopwatch.ElapsedMilliseconds + "ms");
//             UnityEngine.Debug.Log("bet button activate print process Complete.");
//             StartCoroutine(Reactivatebet(5f));

//         }
//         private IEnumerator Reactivatebet(float timeduration)
//         {
//             // Re-enable bet button only after PDF printing is complete
//             yield return new WaitForSeconds(timeduration);
//             betButton16card.interactable = true;
//             UnityEngine.Debug.Log("time duration to activate:" + timeduration);
//         }
//         //print full code change by shivamfusion07
//         /*public void PrintPdf()
//         {
//             Stopwatch stopwatch = Stopwatch.StartNew();

//             // Ensure correct path construction
//             string printFolderPath = Path.Combine(Application.dataPath, "print");
//             string pdfFilePath = Path.Combine(printFolderPath, BET.instance.ticketId + ".pdf");

//             // Ensure directory exists
//             if (!Directory.Exists(printFolderPath))
//             {
//                 Directory.CreateDirectory(printFolderPath);
//             }

//             // Clean up old files
//             foreach (string file in Directory.GetFiles(printFolderPath))
//             {
//                 File.Delete(file);
//             }

//             try
//             {
//                 // Create the PDF asynchronously
//                 PDFManager pdfMgr = PDFManager.CreatePDFBuilderWithPathAndTicketID(pdfFilePath, BET.instance.ticketId);

//                 // Ensure PDF content is created before closing the document
//                 string pdfString = GetPdfString(pdfMgr);
//                 pdfMgr.CloseDocument();

//                 if (File.Exists(pdfFilePath))
//                 {
//                     UnityEngine.Debug.Log("PDF file successfully created at: " + pdfFilePath);
//                 }
//                 else
//                 {
//                     UnityEngine.Debug.LogError("PDF creation failed: " + pdfFilePath);
//                 }

//             betButton.interactable = true;
//             }
//         }*/





//         //private string GetPdfString(PDFManager pdfManager)
//         //{
//         //    JObject jsonData;
//         //    StringBuilder finalCardValueSet = new StringBuilder(); // Declare it here

//         //    try
//         //    {
//         //        jsonData = JObject.Parse(BET.instance.jsonString);
//         //    }
//         //    catch (Newtonsoft.Json.JsonReaderException)
//         //    {
//         //        // If parsing as an object fails, try parsing as an array
//         //        JArray jsonArray;
//         //        try
//         //        {
//         //            jsonArray = JArray.Parse(BET.instance.jsonString);
//         //        }
//         //        catch (Newtonsoft.Json.JsonReaderException)
//         //        {
//         //            // Handle invalid JSON data or any other errors here
//         //            return "Invalid JSON data";
//         //        }


//         //        DateTime currentDateTime = DateTime.Now;
//         //        string formattedDateTime = currentDateTime.ToString("MM/dd/yyyy hh:mm:ss tt");

//         //        pdfManager.CreateParagraph("For Amusement only", Element.ALIGN_CENTER, 11f, iTextSharp.text.Font.NORMAL);
//         //        pdfManager.CreateParagraph("KheloIndians", Element.ALIGN_CENTER, 25f, iTextSharp.text.Font.BOLD);

//         //        //finalCardValueSet.AppendLine("  For Amusement only   ");
//         //        //finalCardValueSet.AppendLine("     KHELO JEETO     ");
//         //        finalCardValueSet.AppendLine("GAME NAME: JEETO JOKER");
//         //        finalCardValueSet.AppendLine("Terminal: " + BET.instance.usernameTerminal);
//         //        finalCardValueSet.AppendLine("Game Id: " + BET.instance.GameId_);
//         //        finalCardValueSet.AppendLine("Ticket Id: " + BET.instance.ticketId);
//         //        finalCardValueSet.AppendLine("Draw Time: " + TimerControllerNew.inst.DrawTime.ToString("MM/dd/yyyy hh:mm:ss tt"));
//         //        //finalCardValueSet.AppendLine(formattedDateTime);
//         //        finalCardValueSet.AppendLine("Ticket Time: " + formattedDateTime);
//         //        //finalCardValueSet.AppendLine(formattedDateTime);
//         //        finalCardValueSet.AppendLine("Total Point: " + BET.instance.TotalBet);
//         //        //  finalCardValueSet.Append(finalCardValueSet.ToString());
//         //        //finalCardValueSet.AppendLine("   ");
//         //        pdfManager.CreateParagraph(finalCardValueSet.ToString(), Element.ALIGN_LEFT, 16f, iTextSharp.text.Font.NORMAL);

//         //        StringBuilder CardData = new StringBuilder();
//         //        CardData.AppendLine("Item   Point   Item   Point");
//         //        int count = 0;
//         //        foreach (var card in jsonArray)
//         //        {
//         //            string cardName = (string)card["card"];
//         //            int cardValue = (int)card["value"];
//         //            if (count % 2 == 0)
//         //            {
//         //                //CardData.Append($"{cardName,-4}    {cardValue}");
//         //                  CardData.Append($"  {cardName,-3}       {cardValue}");
//         //            }
//         //            else
//         //            {
//         //                //CardData.Append($"     {cardName,-3}     {cardValue}");
//         //                    CardData.Append($"      {cardName,-3}     {cardValue}");
//         //            }

//         //            count++;

//         //            if (count % 2 == 0)
//         //            {
//         //                CardData.AppendLine();
//         //            }
//         //        }
//         //        //iTextSharp.text.Font fontArial = FontFactory.GetFont("Arial");
//         //        //pdfManager.CreateParagraph(finalCardValueSet.ToString(), Element.ALIGN_LEFT, 8f, FontFactory.GetFont("Arial").);
//         //        pdfManager.CreateParagraph(CardData.ToString(), Element.ALIGN_LEFT, 16f, iTextSharp.text.Font.NORMAL);
//         //    }

//         //    //pdfManager.AddBarcodeToPdf("**************");
//         //    pdfManager.AddBarcodeToPdf("           ");

//         //    StringBuilder bottomText = new StringBuilder();
//         //    //bottomText.AppendLine("   ");
//         //    //bottomText.AppendLine("   ");
//         //    bottomText.AppendLine("**Ticket not for sale**");
//         //    pdfManager.CreateParagraph(bottomText.ToString(), Element.ALIGN_CENTER, 11f, iTextSharp.text.Font.NORMAL);

//         //    return finalCardValueSet.ToString();
//         //}


//     public static string drawTime; // Class-level variable to store draw time
// private int Duration = 90;     // Duration in seconds for draw time calculation

// public static void OnGameRestart()
// {
//     // Reset drawTime to "00:00:00" or any placeholder value
//     drawTime = null;
//     UnityEngine.Debug.Log("Draw time reset to default value after game restart: " + drawTime);
// }

// private string GetPdfString(PDFManager pdfManager)
// {
//     JObject jsonData = null;
//     JArray jsonArray = null;
//     StringBuilder finalCardValueSet = new StringBuilder();

//     try
//     {
//         using (var stringReader = new StringReader(Bet16card.instance.jsonString))
//         using (var jsonReader = new JsonTextReader(stringReader))
//         {
//             if (!jsonReader.Read())
//             {
//                 // Unable to read JSON data
//                 return "Invalid JSON data";
//             }

//             if (jsonReader.TokenType == JsonToken.StartObject)
//             {
//                 // JSON is an object, parse it as JObject
//                 jsonData = JObject.Load(jsonReader);
//             }
//             else if (jsonReader.TokenType == JsonToken.StartArray)
//             {
//                 // JSON is an array, parse it as JArray
//                 jsonArray = JArray.Load(jsonReader);
//             }
//             else
//             {
//                 // JSON data is neither object nor array
//                 return "Invalid JSON data";
//             }
//         }
//     }
//     catch (JsonReaderException)
//     {
//         // Handle invalid JSON data or any other errors here
//         return "Invalid JSON data";
//     }

//     if (jsonData != null)
//     {
//         // Process JSON object if needed
//     }
//     else if (jsonArray != null)
//     {
//         DateTime currentBetTime = DateTime.Now; // Get current time for this bet
//         string currenttimeforticket = currentBetTime.ToString("MM/dd/yyyy hh:mm:ss tt");

//         // Recalculate drawTime for every game restart, ensuring a fresh time
//         if (string.IsNullOrEmpty(drawTime))
//         {
//             // Calculate drawTime using the current time and the specified duration
//             TimeSpan durationTimeSpan = TimeSpan.FromSeconds(Duration);
//             DateTime calculatedDrawTime =  TimerControllerNew.inst.STartDateTime.Add(durationTimeSpan);          //APICardHistory.Instance.gamestarttime.Add(durationTimeSpan);
//             drawTime = calculatedDrawTime.ToString("MM/dd/yyyy hh:mm:ss tt");
//             UnityEngine.Debug.Log("draw time in bet:++++++++"+drawTime);
//         }

//         // Use the newly calculated drawTime for the tickets in this gameplay
//         pdfManager.CreateParagraph("For Amusement only", Element.ALIGN_CENTER, 11f, iTextSharp.text.Font.NORMAL);
//         pdfManager.CreateParagraph("SmartWin", Element.ALIGN_CENTER, 25f, iTextSharp.text.Font.BOLD);

//         finalCardValueSet.AppendLine("GAME NAME: 16CARDS");
//         finalCardValueSet.AppendLine("Terminal: " + Bet16card.instance.usernameTerminal);
//         finalCardValueSet.AppendLine("Ticket Id: " + Bet16card.instance.ticketId);
//         finalCardValueSet.AppendLine("Ticket Time: " + currenttimeforticket);
//         finalCardValueSet.AppendLine("Draw Time: " + drawTime); // Use the new draw time for all bets in this gameplay
//         finalCardValueSet.AppendLine("Total Point: " + Bet16card.instance.TotalBet);
//         pdfManager.CreateParagraph(finalCardValueSet.ToString(), Element.ALIGN_LEFT, 13.5f, iTextSharp.text.Font.NORMAL);

//         StringBuilder CardData = new StringBuilder();
//         CardData.AppendLine("Item   Point   Item   Point");
//         int count = 0;
//         foreach (var card in jsonArray)
//         {
//             string cardName = (string)card["card"];
//             int cardValue = (int)card["value"];
//             if (count % 2 == 0)
//             {
//                 CardData.Append($"  {cardName,-3}       {cardValue}");
//             }
//             else
//             {
//                 CardData.Append($"      {cardName,-3}     {cardValue}");
//             }

//             count++;

//             if (count % 2 == 0)
//             {
//                 CardData.AppendLine();
//             }
//         }

//         pdfManager.CreateParagraph(CardData.ToString(), Element.ALIGN_LEFT, 12.5f, iTextSharp.text.Font.NORMAL);
//         pdfManager.AddBarcodeToPdf("           ");

//         StringBuilder bottomText = new StringBuilder();
//         bottomText.AppendLine("**Ticket not for sale**");
//         pdfManager.CreateParagraph(bottomText.ToString(), Element.ALIGN_CENTER, 12.5f, iTextSharp.text.Font.NORMAL);

//         return finalCardValueSet.ToString();
//     }

//     // If no valid data is found, return default message
//     return "No valid data processed.";
// }
//     }
// }