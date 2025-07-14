using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour
{
    [SerializeField] private GameObject infodata;
    [SerializeField] private Transform infodataParent;
    private string jsonString;

    //Report Data
    public Text date, sale, win, comm, ntp;
    //history data 
    public Text totalbet, winAmount;
    public GameObject sideScrollRectGameObject;
    private void OnEnable()
    {
        ShowGameHistory();
    }
    public void ShowGameHistory()
    {
        // if (infodataParent.childCount > 0)
        // {
        //     for (int i = 0; i < infodataParent.childCount; i++)
        //     {
        //         Destroy(infodataParent.GetChild(i).gameObject);
        //     }
        // }

        string urlEndPoint = "game-history";
        string game_name = SceneManager.GetActiveScene().name.Equals("TripleChanceProGameplay") ? "tripleChancePro" : "tripleChance";
        StartCoroutine(getRequest(Constant.KIBaseURL + urlEndPoint,game_name));
    }

      IEnumerator getRequest(string url, string game_name)
    {
        WWWForm form4 = new WWWForm();
        form4.AddField("game_name", game_name);
        UnityWebRequest uwr = UnityWebRequest.Post(url, form4);
        Debug.LogError(PlayerPrefs.GetString(Constant.Token));
        uwr.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString(Constant.Token));
        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            Debug.Log("Info Panel Get : " + uwr.downloadHandler.text);
            GameHistory gamehistory = JsonUtility.FromJson<GameHistory>(uwr.downloadHandler.text);
            bool calculateWin = false;
            if (gamehistory.total_win <= 0)
            {
                calculateWin = true;
            }
            if (gamehistory.status == 200)
            {
                int totalWin = 0;
                if (infodataParent.childCount > 0)
                {
                    for (int i = 0; i < infodataParent.childCount; i++)
                    {
                        Destroy(infodataParent.GetChild(i).gameObject);
                    }
                }
                if (gamehistory.list.Count >= 6 && sideScrollRectGameObject)
                {
                    sideScrollRectGameObject.SetActive(true);
                }
                else if (sideScrollRectGameObject)
                {
                    sideScrollRectGameObject.SetActive(false);
                }
                for (int i = 0; i < gamehistory.list.Count; i++)
                {
                    GameObject game = Instantiate(infodata, infodataParent);
                    game.SetActive(true);
                    game.GetComponent<InitData>().Initialized((i + 1).ToString(), gamehistory.list[i].game_id.ToString(),
                        gamehistory.list[i].bet_ammount.ToString(),
                         gamehistory.list[i].win_ammount.ToString());
                    if (calculateWin)
                        totalWin += int.Parse(gamehistory.list[i].win_ammount);
                }
                if (totalbet)
                    totalbet.text = gamehistory.total_bet.ToString();
                if (calculateWin == false && winAmount)
                    winAmount.text = gamehistory.total_win.ToString();
                else if (winAmount)
                {
                    winAmount.text = totalWin.ToString();
                }
            }
        }
    }


        //if (uwr.isNetworkError)
        //{
        //    Debug.Log("Error While Sending: " + uwr.error);

        //}
        //else
        //{
        //    Debug.LogError(uwr.result);
        //    jsonString = System.Text.Encoding.UTF8.GetString(uwr.downloadHandler.data, 3, uwr.downloadHandler.data.Length - 3);
        //    JSONNode loginInfo = JSON.Parse(uwr.downloadHandler.text);

        //    string status = loginInfo["status"];
        //    Debug.LogError(status);
        //    if (status.Equals("200"))
        //    {
        //        Debug.LogError(uwr.downloadHandler.text);
        //        print("yes");
        //        for (int i = 0; i < loginInfo["list"].Count; i++)
        //        {
        //            GameObject game = Instantiate(infodata, infodataParent);
        //            game.SetActive(true);
        //            game.GetComponent<InitData>().Initialized((i+1).ToString(),loginInfo["list"][i]["game_id"].ToString(),
        //                loginInfo["list"][i]["bet_ammount"].ToString(),
        //                loginInfo["list"][i]["win_ammount"].ToString());
        //        }
        //    }
        //    else
        //    {
        //        print("no");
        //    }
        //}

  //  }
  /* IEnumerator getRequest(string url)
{
    WWWForm form4 = new WWWForm();
   // form4.AddField("game_name", game_name);
    
    // Log request details
   // Debug.LogError("Requesting URL: " + url + " with game_name: " + game_name);
    Debug.LogError("Authorization Token: " + PlayerPrefs.GetString(Constants.Token));
//string fullUrl = url + "?game_name=" + UnityWebRequest.EscapeURL(game_name);
     //   Debug.Log("full url :"+fullUrl);
    UnityWebRequest uwr = UnityWebRequest.Get(url);
    uwr.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString(Constants.Token));
    yield return uwr.SendWebRequest();

    if (uwr.isNetworkError || uwr.isHttpError)
    {
        Debug.LogError("Error While Sending: " + uwr.error + " | Response: " + uwr.downloadHandler.text);
    }
    else
    {
        // Log the full response
        Debug.Log("Full JSON Response: " + uwr.downloadHandler.text);

        // Extract the jsonString from the downloadHandler (removing any unwanted characters)
        jsonString = System.Text.Encoding.UTF8.GetString(uwr.downloadHandler.data, 3, uwr.downloadHandler.data.Length - 3);
        
        // Log the cleaned jsonString
        Debug.Log("Cleaned JSON Data: " + jsonString);

        // Parse the JSON response
        JSONNode loginInfo = JSON.Parse(uwr.downloadHandler.text);

        // Log the parsed JSON structure
        Debug.Log("Parsed JSON Data: " + loginInfo.ToString());

        // Log the specific fields
        string msg = loginInfo["status"];
        Debug.LogError("Status: " + msg);
        Debug.Log("JSON List Count: " + loginInfo["list"].Count);

        if (msg.Equals("200"))
        {
            Debug.Log("Success: Info printed");
             for (int i = 0; i < loginInfo["list"]["data"].Count; i++)
            {
                // Extract relevant data
                string gameId = loginInfo["list"]["data"][i]["game_id"].ToString();
                string winNumber = loginInfo["list"]["data"][i]["win_number"].ToString();
                string betAmount = loginInfo["list"]["data"][i]["bet_ammount"].ToString();
                string winAmount = loginInfo["list"]["data"][i]["win_ammount"].ToString();
                Debug.Log("gameId shivam"+gameId);
 Debug.Log(" winNumber ssss"+winNumber);
  Debug.Log(" betAmountvfddg "+betAmount);
   Debug.Log(" winAmount aaya :"+winAmount);
                // Instantiate the prefab
                GameObject game = Instantiate(infodata, infodataParent);
                game.SetActive(true);

                // Populate the prefab's text components
                game.GetComponent<HistoryData>().InitData(gameId, winNumber, betAmount, winAmount);
            }
    }
}
}*/


    private void OnDisable()
    {
        for (int i = 0; i < infodataParent.childCount; i++)
        {
            Destroy(infodataParent.GetChild(i).gameObject);
        }
    }




    public void ShowGameReport()
    {
        if (infodataParent.childCount > 0)
        {
            for (int i = 0; i < infodataParent.childCount; i++)
            {
                Destroy(infodataParent.GetChild(i).gameObject);
            }
        }
        // string urlEndPoint = GameSelector.SelectedGame == "TripleChance" ? "triple-chance-reports" : "triple-chancepro-reports";
        StartCoroutine(GetRequest(Constant.KIBaseURL + "game-reports"));
    }
    [Serializable]
    public class GetRequestSendData
    {
        public string player_id;
        public string game_name;
    }

    public IEnumerator GetRequest(string url)
    {
        Debug.LogError(PlayerPrefs.GetString(Constant.Token));

        GetRequestSendData getRequestSendData = new GetRequestSendData
        {
            player_id = GamePlay.instance.myPlayerID.ToString(),
            game_name = SceneManager.GetActiveScene().name.Equals("TripleChanceProGameplay") ? "tripleChancePro" : "tripleChance"
        };

        string sendRequestDataJson = JsonUtility.ToJson(getRequestSendData);
        Debug.Log("Report send data: " + sendRequestDataJson);

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(sendRequestDataJson);
        UnityWebRequest uwr = new UnityWebRequest(url, "POST")
        {
            uploadHandler = new UploadHandlerRaw(bodyRaw),
            downloadHandler = new DownloadHandlerBuffer()
        };

        // Set the request headers
        uwr.SetRequestHeader("Content-Type", "application/json");
        uwr.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString(Constant.Token));

        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log("Error While Sending Report: " + uwr.error);
        }
        else
        {
            Debug.LogError("Report result: " + uwr.result);
            string jsonString = System.Text.Encoding.UTF8.GetString(uwr.downloadHandler.data, 3, uwr.downloadHandler.data.Length - 3);
            JSONNode loginInfo = JSON.Parse(uwr.downloadHandler.text);

            string status = loginInfo["status"];
            Debug.LogError(status);
            if (status.Equals("200"))
            {
                Debug.Log("Report: yes");
                // Assuming 'date', 'sale', 'win', 'comm', and 'ntp' are UI elements
                date.text = DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year.ToString();
                sale.text = loginInfo["bet_amount"].ToString();
                win.text = loginInfo["win_amount"].ToString();
                comm.text = loginInfo["com_pt"].ToString();
                ntp.text = loginInfo["NTP"].ToString();
            }
            else
            {
                Debug.Log("Report: no");
            }
        }
    }
}