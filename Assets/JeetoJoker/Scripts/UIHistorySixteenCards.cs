using khelojeetonew;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UI.Dates;

public class UIHistorySixteenCards : MonoBehaviour
{
    [SerializeField] private UIHistoryContent historyContentPrefab;
    [SerializeField] private Transform contentHolder;
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private Button previous;
    [SerializeField] private Button next;

    [SerializeField] private DatePicker fromDatePicker; // DatePicker for "From Date"
    [SerializeField] private DatePicker toDatePicker;   // DatePicker for "To Date"

    private List<UIHistoryContent> allHistoryUI = new List<UIHistoryContent>();

    private string _previousPageUrl;
    private string _nextPageUrl;

    private string _fromDate = "23-12-2024"; // Default "From Date"
    private string _toDate = "26-12-2024";   // Default "To Date"

    private void OnEnable()
    {
        StartCoroutine(GetHistoryData(Constant.KIBaseURL + "sixteencards-history"));
    }

    private void Start()
    {
        previous.onClick.AddListener(PreviousDatahistory);
        next.onClick.AddListener(Nextpageurlbtncall);

        // Bind DatePicker events
        fromDatePicker.Config.Events.OnDaySelected.AddListener(OnFromDateSelected);
        toDatePicker.Config.Events.OnDaySelected.AddListener(OnToDateSelected);
    }

    private void OnFromDateSelected(DateTime date)
    {
        _fromDate = date.ToString("dd-MM-yyyy");
        Debug.Log($"From Date selected: {_fromDate}");
    }

    private void OnToDateSelected(DateTime date)
    {
        _toDate = date.ToString("dd-MM-yyyy");
        Debug.Log($"To Date selected: {_toDate}");
    }

    private IEnumerator GetHistoryData(string url)
    {
        // Deactivate all existing history UI elements
        int count = allHistoryUI.Count;
        for (int i = 0; i < count; i++)
        {
            allHistoryUI[i].gameObject.SetActive(false);
        }

        loadingPanel.SetActive(true);
        yield return new WaitForSeconds(0.2f);

        // Use GET request
        UnityWebRequest unityWebRequest = UnityWebRequest.Get(url);
        unityWebRequest.SetRequestHeader(Constants.authorization, "Bearer " + PlayerPrefs.GetString(Constants.token));
        yield return unityWebRequest.SendWebRequest();

        loadingPanel.SetActive(false);

        if (unityWebRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(unityWebRequest.error);
            Sixteen_cards.instance.ShowMessage(unityWebRequest.error);
            Debug.LogError("Web request error: " + unityWebRequest.downloadHandler.text);
        }
        else
        {
            try
            {
                Debug.Log("Response: " + unityWebRequest.downloadHandler.text);
                GameHistoryJeeto historyDataStatus = JsonConvert.DeserializeObject<GameHistoryJeeto>(unityWebRequest.downloadHandler.text);
                Debug.Log($"Deserialized status: {historyDataStatus.status}, data count: {historyDataStatus.list.data.Count}");

                // Check if status is successful
                if (historyDataStatus.status == 200)
                {
                    int length = historyDataStatus.list.data.Count;
                    Debug.Log($"History data retrieved: {length} records found.");

                    // Instantiate history UI elements
                    for (int i = 0; i < length; i++)
                    {
                        Debug.Log($"Creating history UI element for record {i}: {JsonConvert.SerializeObject(historyDataStatus.list.data[i])}");
                        UIHistoryContent uIHistoryContent = Instantiate(historyContentPrefab, contentHolder);
                        allHistoryUI.Add(uIHistoryContent);
                        allHistoryUI[i].gameObject.SetActive(true);
                        allHistoryUI[i].Bind(i, historyDataStatus.list.data[i]);
                    }

                    // Update the next and previous page URLs
                    _previousPageUrl = historyDataStatus.list.prev_page_url;
                    _nextPageUrl = historyDataStatus.list.next_page_url;
                }
                else
                {
                    Sixteen_cards.instance.ShowMessage($"History data error status: {historyDataStatus.status}");
                    Debug.LogError($"Unexpected status: {historyDataStatus.status}");
                }
            }
            catch (Exception e)
            {
                Sixteen_cards.instance.ShowMessage(e.ToString());
                Debug.LogError($"Exception caught: {e}");
            }

        }
    }

    /// <summary>
    /// Gets game history data within a range of _fromDate to _toDate
    /// </summary>
    public void GetHistoryInRange()
    {
        Debug.Log("View button clicked.");
        StartCoroutine(DownloadViewResult(Constant.KIBaseURL + "sixteencards-history", _fromDate, _toDate));
    }

    public IEnumerator DownloadViewResult(string baseUrl, string fromDate, string toDate)
    {
        if (string.IsNullOrEmpty(fromDate) || string.IsNullOrEmpty(toDate))
        {
            Debug.LogError("Both From Date and To Date must be provided!");
            Sixteen_cards.instance.ShowMessage("Please select valid dates.");
            yield break;
        }

        string url = $"{baseUrl}?from_date={fromDate}&to_date={toDate}";

        foreach (var ui in allHistoryUI)
        {
            ui.gameObject.SetActive(false);
        }

        loadingPanel.SetActive(true);
        yield return new WaitForSeconds(2);

        UnityWebRequest unityWebRequest = UnityWebRequest.Get(url);
        unityWebRequest.SetRequestHeader(Constants.authorization, "Bearer " + PlayerPrefs.GetString(Constants.token));
        yield return unityWebRequest.SendWebRequest();

        loadingPanel.SetActive(false);

        if (unityWebRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(unityWebRequest.error);
            Sixteen_cards.instance.ShowMessage(unityWebRequest.error);
        }
        else
        {
            try
            {
                Debug.Log($"Response: {unityWebRequest.downloadHandler.text}");
                GameHistoryJeeto historyDataStatus = JsonConvert.DeserializeObject<GameHistoryJeeto>(unityWebRequest.downloadHandler.text);
                // q       Debug.Log($"Total records: {historyDataStatus.list.}")
                if (historyDataStatus.status == 200)
                {
                    foreach (var item in historyDataStatus.list.data)
                    {
                        UIHistoryContent uIHistoryContent = Instantiate(historyContentPrefab, contentHolder);
                        allHistoryUI.Add(uIHistoryContent);
                        uIHistoryContent.gameObject.SetActive(true);
                        uIHistoryContent.Bind(allHistoryUI.Count - 1, item);
                    }
                    _previousPageUrl = historyDataStatus.list.prev_page_url;
                    _nextPageUrl = historyDataStatus.list.next_page_url;
                }
                else
                {
                    Sixteen_cards.instance.ShowMessage($"History data error status: {historyDataStatus.status}");
                }
            }
            catch (Exception e)
            {
                Sixteen_cards.instance.ShowMessage(e.ToString());
                Debug.LogError($"Exception caught: {e}");
            }
        }
    }

    public void PreviousDatahistory()
    {
        if (!string.IsNullOrEmpty(_previousPageUrl))
        {
            StartCoroutine(DownloadHistory(_previousPageUrl));
        }
        else
        {
            Debug.Log("No previous page available.");
        }
    }
    public void Nextpageurlbtncall()
    {
        if (!string.IsNullOrEmpty(_nextPageUrl))
        {
            StartCoroutine(DownloadHistory(_nextPageUrl));
        }
        else
        {
            Debug.Log("No next page available.");
        }
    }

    private IEnumerator DownloadHistory(string baseUrl)
    {
        foreach (var ui in allHistoryUI)
        {
            ui.gameObject.SetActive(false);
        }

        loadingPanel.SetActive(true);

        UnityWebRequest unityWebRequest = UnityWebRequest.Get(baseUrl);
        unityWebRequest.SetRequestHeader(Constants.authorization, "Bearer " + PlayerPrefs.GetString(Constants.token));
        yield return unityWebRequest.SendWebRequest();

        loadingPanel.SetActive(false);

        if (unityWebRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(unityWebRequest.error);
            Sixteen_cards.instance.ShowMessage(unityWebRequest.error);
        }
        else
        {
            try
            {
                Debug.Log($"Response DH: {unityWebRequest.downloadHandler.text}");
                GameHistoryJeeto historyDataStatus = JsonConvert.DeserializeObject<GameHistoryJeeto>(unityWebRequest.downloadHandler.text);

                if (historyDataStatus.status == 200)
                {
                    UpdateHistoryUI(historyDataStatus.list);
                }
                else
                {
                    Sixteen_cards.instance.ShowMessage($"Error: {historyDataStatus.status}");
                }
            }
            catch (Exception e)
            {
                Sixteen_cards.instance.ShowMessage(e.ToString());
                Debug.LogError($"Exception: {e}");
            }
        }
    }

    private void UpdateHistoryUI(HistoryDataList list)
    {
        foreach (var ui in allHistoryUI)
        {
            Destroy(ui.gameObject);
        }
        allHistoryUI.Clear();

        foreach (var item in list.data)
        {
            UIHistoryContent historyUI = Instantiate(historyContentPrefab, contentHolder);
            allHistoryUI.Add(historyUI);
            historyUI.gameObject.SetActive(true);
            historyUI.Bind(allHistoryUI.Count - 1, item);
        }

        _previousPageUrl = list.prev_page_url;
        _nextPageUrl = list.next_page_url;

        previous.interactable = !string.IsNullOrEmpty(_previousPageUrl);
        next.interactable = !string.IsNullOrEmpty(_nextPageUrl);
    }

    // Deserialize the response correctly
    /*GameHistory historyDataStatus = JsonConvert.DeserializeObject<GameHistory>(unityWebRequest.downloadHandler.text);

 // Check if historyDataStatus and the data list are valid
 if (historyDataStatus == null || historyDataStatus.list == null || historyDataStatus.list.data == null)
 {
     Debug.LogError("historyDataStatus, list, or data list is null.");
     yield break;
 }
     if (historyDataStatus.status == 200)
     {
         int length = historyDataStatus.list.data.Count;
            // int index = ((historyDataStatus.list.from - 1) * historyDataStatus.list.last_page) + 1;
         Debug.Log($"History data retrieved: {length} records found.");

         // Loop through the records and extract the data
         for (int i = 0; i < historyDataStatus.list.data.Count; i++)
         {
             var historyItem = historyDataStatus.list.data[i];
             Debug.Log("history item: " + historyItem);

             // Instantiate UI elements and bind the relevant data
             UIHistoryContent uIHistoryContent = Instantiate(historyContentPrefab, contentHolder);
             allHistoryUI.Add(uIHistoryContent);
             allHistoryUI[i].gameObject.SetActive(true);

             //allHistoryUI[i].Bind(i, historyDataStatus.list[i]);

             // Bind data to the UI
             uIHistoryContent.Bind(i, historyItem);
         }

         // Update the next and previous page URLs
         previousPageUrl = historyDataStatus.list.prev_page_url;
         nextPageUrl = historyDataStatus.list.next_page_url;
     }
     else
     {
         JeetoJokerManager.instance.ShowMessage($"History data error status: {historyDataStatus.status}");
         Debug.LogError($"Unexpected status: {historyDataStatus.status}");
     }
 }
 catch (Exception e)
 {
     JeetoJokerManager.instance.ShowMessage(e.ToString());
     Debug.LogError($"Exception caught: {e}");
 }
         }*/

}
/*private IEnumerator Download(string url, string GameName)
   {
       int count = allHistoryUI.Count;
       for (int i = 0; i < count; i++)
       {
           allHistoryUI[i].gameObject.SetActive(false);
       }
       loadingPanel.SetActive(true);
       yield return new WaitForSeconds(2);
       WWWForm wWWForm = new WWWForm();
       wWWForm.AddField("game_name", GameName);
       UnityWebRequest unityWebRequest = UnityWebRequest.Post(url, wWWForm);
       unityWebRequest.SetRequestHeader(Constants.authorization, "Bearer " + PlayerPrefs.GetString(Constants.token));
       yield return unityWebRequest.SendWebRequest();
       loadingPanel.SetActive(false);
       if (unityWebRequest.result != UnityWebRequest.Result.Success)
       {
           Debug.LogError(unityWebRequest.error);
           JeetoJokerManager.instance.ShowMessage(unityWebRequest.error);
           Debug.LogError(unityWebRequest.downloadHandler.text);
       }
       else
       {
           try
           {
               Debug.LogError(unityWebRequest.downloadHandler.text);
               GameHistory historyDataStatus = JsonConvert.DeserializeObject<GameHistory>(unityWebRequest.downloadHandler.text);
               if (historyDataStatus.status == 200)
               {
                   int length = historyDataStatus.list.Count;
                   for (int i = 0; i < length; i++)
                   {
                       UIHistoryContent uIHistoryContent = Instantiate(historyContentPrefab, contentHolder);
                       allHistoryUI.Add(uIHistoryContent);
                       allHistoryUI[i].gameObject.SetActive(true);
                       allHistoryUI[i].Bind(i, historyDataStatus.list[i]);
                   }
               }
               else
               {
                   JeetoJokerManager.instance.ShowMessage("history data error status " + historyDataStatus.status);
               }
           }
           catch (Exception e)
           {
               JeetoJokerManager.instance.ShowMessage(e.ToString());
           }
       }
   }*/
//view button call fro histroy 16 cards
//     string fromDate = "22-12-2024"; // format of date dd-mm-yyyy
// string toDate = "24-12-2024";   // Replace with your desired date
//     public void Viewbtn_call()
// {
//     Debug.Log("view button clicked..");
//   StartCoroutine(DownloadViewResult16card(Constant.KIBaseURL+"sixteencards-history",fromDate,toDate))  ;
// }
// public IEnumerator DownloadViewResult16card(string baseUrl, string fromDate, string toDate)
// {
//     // Validate input dates
//     if (string.IsNullOrEmpty(fromDate) || string.IsNullOrEmpty(toDate))
//     {
//         Debug.LogError("Both from_date and to_date must be provided!");
//         JeetoJokerManager.instance.ShowMessage("Please enter valid From Date and To Date.");
//         yield break;
//     }

//     // Construct the full URL with query parameters
//     string url = $"{baseUrl}?from_date={fromDate}&to_date={toDate}";

//     // Deactivate all existing history UI elements
//     foreach (var ui in allHistoryUI)
//     {
//         ui.gameObject.SetActive(false);
//     }

//     loadingPanel.SetActive(true);
//     yield return new WaitForSeconds(2);

//     // Send GET request
//     Debug.Log("form date url 12cardssssssss:"+url);
//     UnityWebRequest unityWebRequest = UnityWebRequest.Get(url);
//     unityWebRequest.SetRequestHeader(Constants.authorization, "Bearer " + PlayerPrefs.GetString(Constants.token));
//     Debug.Log("authorizatin token for view btn:"+ PlayerPrefs.GetString(Constants.token));
//     yield return unityWebRequest.SendWebRequest();

//     loadingPanel.SetActive(false);

//     if (unityWebRequest.result != UnityWebRequest.Result.Success)
//     {
//         Debug.LogError(unityWebRequest.error);
//         JeetoJokerManager.instance.ShowMessage(unityWebRequest.error);
//         Debug.LogError(unityWebRequest.downloadHandler.text);
//     }
//     else
//     {
//         try
//         {
//             Debug.Log($"Response: {unityWebRequest.downloadHandler.text}");
//            GameHistoryJeeto historyDataStatus = JsonConvert.DeserializeObject<GameHistoryJeeto>(unityWebRequest.downloadHandler.text);

//             if (historyDataStatus.status == 200)
//             {
//                int length = historyDataStatus.list.data.Count;
//         Debug.Log($"History data retrieved: {length} records found.");

//         // Instantiate history UI elements
//         for (int i = 0; i < length; i++)
//         {
//             Debug.Log($"Creating history UI element for record {i}: {JsonConvert.SerializeObject(historyDataStatus.list.data[i])}");
//             UIHistoryContent uIHistoryContent = Instantiate(historyContentPrefab, contentHolder);
//             allHistoryUI.Add(uIHistoryContent);
//             allHistoryUI[i].gameObject.SetActive(true);
//             allHistoryUI[i].Bind(i, historyDataStatus.list.data[i]);
//         }
//             }
//             else
//             {
//                 JeetoJokerManager.instance.ShowMessage($"History data error status: {historyDataStatus.status}");
//             }
//         }
//         catch (Exception e)
//         {
//             JeetoJokerManager.instance.ShowMessage(e.ToString());
//             Debug.LogError($"Exception caught: {e}");
//         }
//     }
// }


