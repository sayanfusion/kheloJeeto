using khelojeetonew;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using JeetoJoker;
using UI.Dates;

public class UIHistory : MonoBehaviour
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

    private string _fromDate = ""; // Default "From Date"
    private string _toDate = "";   // Default "To Date"


    private void OnEnable()
    {
         System.DateTime today = System.DateTime.Today;
         _toDate = today.ToString("yyyy-MM-dd");
         _fromDate=today.ToString("yyyy-MM-dd");
        StartCoroutine(DownloadViewResult(Constant.KIBaseURL + "jeetojoker-history", _fromDate, _toDate));
        // StartCoroutine(GetHistoryData(Constant.KIBaseURL + "jeetojoker-history"));
    }

    private void Start()
    {
        System.DateTime today = System.DateTime.Today;

        // Set the date in the date picker

        // Optional: Set selected date too
        fromDatePicker.SelectedDate = today;
        toDatePicker.SelectedDate = today;

        previous.onClick.AddListener(PreviousDatahistory);
        next.onClick.AddListener(Nextpageurlbtncall);

        // Bind DatePicker events
        fromDatePicker.Config.Events.OnDaySelected.AddListener(OnFromDateSelected);
        toDatePicker.Config.Events.OnDaySelected.AddListener(OnToDateSelected);
    }

    private void OnFromDateSelected(DateTime date)
    {
        _fromDate =date.ToString("yyyy-MM-dd");
        Debug.Log($"From Date selected: {_fromDate}");
    }

    private void OnToDateSelected(DateTime date)
    {
        _toDate = date.ToString("yyyy-MM-dd");
        Debug.Log($"To Date selected: {_toDate}");
    }

    private IEnumerator GetHistoryData(string url)
    {
        // Deactivate all existing history UI elements
        foreach (var ui in allHistoryUI)
        {
            ui.gameObject.SetActive(false);
        }

        loadingPanel.SetActive(true);
        // yield return new WaitForSeconds(0.2f);

        UnityWebRequest unityWebRequest = UnityWebRequest.Get(url);
        unityWebRequest.SetRequestHeader(Constants.authorization, "Bearer " + PlayerPrefs.GetString(Constants.token));
        yield return unityWebRequest.SendWebRequest();

        loadingPanel.SetActive(false);

        if (unityWebRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(unityWebRequest.error);
            JeetoJokerManager.instance.ShowMessage(unityWebRequest.error);
        }
        else
        {
            try
            {
                Debug.Log("Response Initial: " + unityWebRequest.downloadHandler.text);
                GameHistoryJeeto historyDataStatus = JsonConvert.DeserializeObject<GameHistoryJeeto>(unityWebRequest.downloadHandler.text);

                if (historyDataStatus.status == 200)
                {
                    int length = historyDataStatus.list.data.Count;

                    for (int i = 0; i < length; i++)
                    {
                        UIHistoryContent uIHistoryContent = Instantiate(historyContentPrefab, contentHolder);
                        allHistoryUI.Add(uIHistoryContent);
                        allHistoryUI[i].gameObject.SetActive(true);
                        allHistoryUI[i].Bind(i, historyDataStatus.list.data[i]);
                    }

                    _previousPageUrl = historyDataStatus.list.prev_page_url;
                    _nextPageUrl = historyDataStatus.list.next_page_url;
                    Debug.Log($"previous url: {_previousPageUrl}, next url: {_nextPageUrl}");
                }
                else
                {
                    JeetoJokerManager.instance.ShowMessage($"History data error status: {historyDataStatus.status}");
                }
            }
            catch (Exception e)
            {
                JeetoJokerManager.instance.ShowMessage(e.ToString());
                Debug.LogError($"Exception caught: {e}");
            }
        }
    }


    /// <summary>
    /// Gets game history data within a range of _fromDate to _toDate
    /// </summary>
    public void GetHistoryInRange()
    {
        StartCoroutine(DownloadViewResult(Constant.KIBaseURL + "jeetojoker-history", _fromDate, _toDate));
    }

    public IEnumerator DownloadViewResult(string baseUrl, string fromDate, string toDate)
    {
        if (string.IsNullOrEmpty(fromDate) || string.IsNullOrEmpty(toDate))
        {
            Debug.LogError("Both From Date and To Date must be provided!");
            JeetoJokerManager.instance.ShowMessage("Please select valid dates.");
            yield break;
        }

        string url = $"{baseUrl}?from_date={fromDate}&to_date={toDate}";



        foreach (var ui in allHistoryUI)
        {
            ui.gameObject.SetActive(false);
        }

        loadingPanel.SetActive(true);
        // yield return new WaitForSeconds(2);

        UnityWebRequest unityWebRequest = UnityWebRequest.Get(url);
        unityWebRequest.SetRequestHeader(Constants.authorization, "Bearer " + PlayerPrefs.GetString(Constants.token));
        yield return unityWebRequest.SendWebRequest();

        loadingPanel.SetActive(false);

        if (unityWebRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(unityWebRequest.error);
            JeetoJokerManager.instance.ShowMessage(unityWebRequest.error);
        }
        else
        {
            try
            {
                Debug.Log($"Response: {unityWebRequest.downloadHandler.text}");
                GameHistoryJeeto historyDataStatus = JsonConvert.DeserializeObject<GameHistoryJeeto>(unityWebRequest.downloadHandler.text);
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

                    Debug.Log($"previous url: {_previousPageUrl}, next url: {_nextPageUrl}");
                }
                else
                {
                    JeetoJokerManager.instance.ShowMessage($"History data error status: {historyDataStatus.status}");
                }
            }
            catch (Exception e)
            {
                JeetoJokerManager.instance.ShowMessage(e.ToString());
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
            JeetoJokerManager.instance.ShowMessage(unityWebRequest.error);
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
                    JeetoJokerManager.instance.ShowMessage($"Error: {historyDataStatus.status}");
                }
            }
            catch (Exception e)
            {
                JeetoJokerManager.instance.ShowMessage(e.ToString());
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
}
