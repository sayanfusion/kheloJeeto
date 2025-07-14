using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class UserInfoPersist : MonoBehaviour
{
    public static UserInfoPersist Instance;
    public UserInfo userInfo;
    public LiveResultData liveResultData;
     private Dictionary<string, LiveResultData> liveResultDataDict = new Dictionary<string, LiveResultData>();

    private void Awake()
    {
        // Ensure only one instance of UserInfoPersist exists
        if (Instance == null)
        {
            Instance = this;
            // Keep this object alive between scene loads
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // If an instance already exists, destroy this one
            Destroy(gameObject);
        }
        userInfo = null;
        liveResultData = null;
        DashBoardData.UserLoggedinWithDetails += UserLoggedinFetchHistory;
    }
    private void OnDestroy()
    {
        DashBoardData.UserLoggedinWithDetails -= UserLoggedinFetchHistory;
    }

    private void UserLoggedinFetchHistory()
    {
        
        StartCoroutine(getResultRequest(Constant.KIBaseURL + "live-data-history", "TripleChance"));
        StartCoroutine(getResultRequest(Constant.KIBaseURL + "live-data-history", "TripleChancePro"));
        StartCoroutine(getResultRequest(Constant.KIBaseURL + "live-data-history", "Roullete"));     
    }


    
   

    public IEnumerator getResultRequest(string url, string gamename)
    {
        Debug.Log("live result data with triplechancetimer");
        WWWForm form4 = new WWWForm();
        form4.AddField("player_id", PlayerPrefs.GetInt(Constant.UID));
        form4.AddField("game_name", gamename);
        UnityWebRequest uwr = UnityWebRequest.Post(url, form4);

        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
              Debug.Log($"Response for {gamename}: {uwr.downloadHandler.text}");

          //  Debug.Log(uwr.downloadHandler.text);
            // Parse JSON response into loginSuccessData object
            liveResultData = JsonUtility.FromJson<LiveResultData>(uwr.downloadHandler.text);
            if (liveResultData.status == 200)
            {
                if (liveResultData.list.Count > 0)
                {
                    Debug.LogError("live result data"+liveResultData.list.Count);
                      UserInfoPersist.Instance.SetLiveResultData(gamename, liveResultData);
                }

            }
        }
        uwr.Dispose();
    }


    //hghgf
     public void SetLiveResultData(string gameName, LiveResultData data)
    {
        if (liveResultDataDict.ContainsKey(gameName))
        {
            liveResultDataDict[gameName] = data;
        }
        else
        {
            liveResultDataDict.Add(gameName, data);
        }
    }
      public LiveResultData GetLiveResultData(string gameName)
    {
        if (liveResultDataDict.ContainsKey(gameName))
        {
            return liveResultDataDict[gameName];
        }
        return null;
    }
}
