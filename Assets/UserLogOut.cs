using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using SimpleJSON;

public class UserLogOut : MonoBehaviour
{
    private string jsonString;
    public static UserLogOut instance;
    //public GameObject ExitPanel;

    private void Awake()
    {
        instance=this;
    }

    public void OnApplicationQuit()
    {
        logOut();
    }

    public void logOut()
    {
        StartCoroutine(getRequest(Constant.KIBaseURL + "logout"));
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
           // ExitPanel.SetActive(true);
        }
    }
    public void ExitYes()
    {
        logOut();
        Application.Quit();
    }
    public void ExitNo()
    {
       // ExitPanel.SetActive(false);
    }
    IEnumerator getRequest(string url)
    {
        Debug.LogError(PlayerPrefs.GetString(Constants.Token));
        UnityWebRequest uwr = UnityWebRequest.Get(url);
        uwr.SetRequestHeader("Authorization", "Bearer "+PlayerPrefs.GetString(Constants.Token));
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
            
        }
        else if(uwr.result == UnityWebRequest.Result.Success)
        {
            Debug.LogError(uwr.result);
            jsonString = System.Text.Encoding.UTF8.GetString(uwr.downloadHandler.data, 3, uwr.downloadHandler.data.Length - 3);
            JSONNode loginInfo = JSON.Parse(uwr.downloadHandler.text);
            string msg = loginInfo["message"];
            if (msg.Equals("Logged Out"))
            {
                print("yes");
                PlayerPrefs.SetInt(Constants.LoginStatus, 0);
                SceneManager.LoadSceneAsync("login");
            }
            else
            {
                print("no");
            }
        }

    }
}
