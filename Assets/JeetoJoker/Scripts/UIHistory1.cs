using khelojeetonew;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using JeetoJoker;
using UnityEngine.SceneManagement;
using SimpleJSON;

public class UIHistory1 : MonoBehaviour
{
    [SerializeField] private UIHistoryContent1 historyContentPrefab;
    [SerializeField] private Transform contentHolder;
    // [SerializeField] private GameObject loadingPanel;


    private List<UIHistoryContent1> allHistoryUI = new List<UIHistoryContent1>();

    private string previousPageUrl;
    private string nextPageUrl;

    private void OnEnable()
    {
        if (SceneManager.GetActiveScene().name == "jokerScenePC")
        {
            StartCoroutine(Download("https://mplgames.in/admin/api/roulette-history"));
        }
        else
        {
            StartCoroutine(Download("https://mplgames.in/admin/api/roulette-history"));
        }

    }



    private IEnumerator Download(string url)
    {
        int count = allHistoryUI.Count;
        for (int i = 0; i < count; i++)
        {
            allHistoryUI[i].gameObject.SetActive(false);
        }
        //   loadingPanel.SetActive(true);
        yield return new WaitForSeconds(2);
        UnityWebRequest unityWebRequest = UnityWebRequest.Get(url);
        unityWebRequest.SetRequestHeader(Constants.authorization, "Bearer " + PlayerPrefs.GetString(Constants.token));
        yield return unityWebRequest.SendWebRequest();
        // loadingPanel.SetActive(false);
        if (unityWebRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("milano123");
            Debug.LogError(unityWebRequest.error);
            JeetoJokerManager.instance.ShowMessage(unityWebRequest.error);
            Debug.LogError(unityWebRequest.downloadHandler.text);
        }
        else
        {
            try
            {
                Debug.Log(unityWebRequest.downloadHandler.text + "History: ");

                String jsonString = System.Text.Encoding.UTF8.GetString(unityWebRequest.downloadHandler.data, 3, unityWebRequest.downloadHandler.data.Length - 3);
                JSONNode loginInfo = JSON.Parse(unityWebRequest.downloadHandler.text);

                string status = loginInfo["status"];
                Debug.LogError(status);
                loginInfo = loginInfo["list"];
                if (status.Equals("200"))
                {
                    for (int i = 0; i < loginInfo["data"].Count; i++)
                    {
                        if (i < count)
                        {
                            allHistoryUI[i].gameObject.SetActive(true);
                            allHistoryUI[i].Bind(i + 1, loginInfo["data"][i]);
                        }
                        else
                        {
                            UIHistoryContent1 uIHistoryContent = Instantiate(historyContentPrefab, contentHolder);
                            uIHistoryContent.gameObject.SetActive(true);
                            uIHistoryContent.Bind(i + 1, loginInfo["data"][i]);
                            allHistoryUI.Add(uIHistoryContent);
                        }
                    }

                    /*
                    HistoryDataStatus0 historyDataStatus = JsonConvert.DeserializeObject<HistoryDataStatus0>(unityWebRequest.downloadHandler.text);
                    if (historyDataStatus.status == 200)
                    {
                        Debug.LogError(historyDataStatus.status + "History: ");
                        int length = historyDataStatus.list.Count; // Use Count instead of Length

                        // Access other properties from historyDataStatus if needed
                        // int index = ((historyDataStatus.list[0].current_page - 1) * historyDataStatus.list[0].per_page) + 1;

                        for (int i = 0; i < length; i++)
                        {
                            if (i < count)
                            {
                                allHistoryUI[i].gameObject.SetActive(true);
                                allHistoryUI[i].Bind(i + 1, historyDataStatus.list[i]);
                            }
                            else
                            {
                                UIHistoryContent1 uIHistoryContent = Instantiate(historyContentPrefab, contentHolder);
                                uIHistoryContent.gameObject.SetActive(true);
                                uIHistoryContent.Bind(i + 1, historyDataStatus.list[i]);
                                allHistoryUI.Add(uIHistoryContent);
                            }
                        }
                        */
                    // Remove any code related to nextPageUrl and previousPageUrl since they are not present in the response.
                }
                else
                {
                    //  JeetoJokerManager.instance.ShowMessage("history data error status " + historyDataStatus.status);
                }
            }
            catch (Exception e)
            {
                // JeetoJokerManager.instance.ShowMessage(e.ToString());
            }
        }
    }
}

[Serializable]
public class HistoryDataStatus0
{
    public int status { get; set; }
    public List<HistoryData0> list { get; set; }
}

[Serializable]
public class HistoryData0
{
    public string game_id { get; set; }
    public string win_number { get; set; }
    public string bet_ammount { get; set; }
    public string win_ammount { get; set; }
}
