using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using DevCommon;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Collections;
using UnityEngine.Networking;
using SimpleJSON;

public class RoomManager : MonoBehaviour
{
    public static void ChangeScene(int SceneID)
    {
        ResultManager.totalBet = 0;
        SceneManager.LoadSceneAsync(SceneID);
    }

    public void GoToSceneAfterLogin(int SceneID)
    {
        if (PlayerPrefs.GetInt(Constants.LoginStatus)==0)
        {
            ResultManager.totalBet = 0;
            SceneManager.LoadSceneAsync(SceneID);
        }
        else
        {
            SceneManager.LoadSceneAsync("DashBoard");
        }
        
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GoToScene(4);
        }
    }
    public void GoToScene(int SceneID)
    {
        Dictionary<string, object> dd = new Dictionary<string, object>();
        dd.Add("playerId", PlayerPrefs.GetInt(Constant.UID).ToString());
        dd.Add("name", PlayerPrefs.GetString(Constant.User));
        dd.Add("roomId", PlayerPrefs.GetString(Constants.ROOMID));
        Debug.LogError(JsonConvert.SerializeObject(dd));

        SocketManager.Core.Socket.Emit("leaveRoom", dd);
        ResultManager.totalBet = 0;

        //SceneManager.LoadScene(SceneID);
        StartCoroutine(GameLoginStatus(Constant.KIBaseURL + "game-login-status"));
    }

    IEnumerator GameLoginStatus(string url)
    {
        Debug.LogError(PlayerPrefs.GetString(Constants.Token));
        WWWForm form = new WWWForm();
        form.AddField("game_login_status", "0");
        form.AddField("recent_game_name", "roulette");

        UnityWebRequest uwr = UnityWebRequest.Post(url, form);
        uwr.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString(Constants.Token));
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);

        }
        else
        {
            Debug.LogError(uwr.result);
            string jsonString = System.Text.Encoding.UTF8.GetString(uwr.downloadHandler.data, 3, uwr.downloadHandler.data.Length - 3);
            JSONNode loginInfo = JSON.Parse(uwr.downloadHandler.text);
            Debug.Log($"User Details : {uwr.downloadHandler.text}");
            string msg = loginInfo["message"];
            if (msg.Equals("Success.") || msg.Equals("Success"))
            {
                print("yes");
                SceneManager.LoadScene("Dashboard");
            }
            else
            {
                print("no");
            }
        }
    }
}
