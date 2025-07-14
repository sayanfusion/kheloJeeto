using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using SimpleJSON;

public class InfoPanelGetData : MonoBehaviour
{
    [SerializeField] private GameObject infodata;
    [SerializeField] private Transform infodataParent;


    private string jsonString;

    private void OnEnable()
    {
        ShowInfo();
    }

    public void ShowInfo()
    {
        if (infodataParent.childCount > 0)
        {
            for (int i = 0; i < infodataParent.childCount; i++)
            {
                Destroy(infodataParent.GetChild(i).gameObject);
            }
        }

        StartCoroutine(getRequest(Constant.KIBaseURL + "roulette-history"));
    }

    IEnumerator getRequest(string url)
    {
        Debug.LogError(PlayerPrefs.GetString(Constants.Token));
        UnityWebRequest uwr = UnityWebRequest.Get(url);
        uwr.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString(Constants.Token));
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);

        }
        else
        {
            Debug.LogError(uwr.result);
            jsonString = System.Text.Encoding.UTF8.GetString(uwr.downloadHandler.data, 3, uwr.downloadHandler.data.Length - 3);
            JSONNode loginInfo = JSON.Parse(uwr.downloadHandler.text);
            string msg = loginInfo["status"];
            Debug.LogError(msg);
            if (msg.Equals("200"))
            {
                print("yes");
                for (int i = 0; i < loginInfo["list"].Count; i++)
                {
                    GameObject game = Instantiate(infodata, infodataParent);
                    game.SetActive(true);
                    game.GetComponent<HistoryData>().InitData(loginInfo["list"][i]["game_id"].ToString(),
                        loginInfo["list"][i]["win_number"].ToString(),
                        loginInfo["list"][i]["bet_ammount"].ToString(),
                        loginInfo["list"][i]["win_ammount"].ToString());
                }
            }
            else
            {
                print("no");
            }
        }

    }

    private void OnDisable()
    {
        for (int i = 0; i < infodataParent.childCount; i++)
        {
            Destroy(infodataParent.GetChild(i).gameObject);
        }
    }
}
