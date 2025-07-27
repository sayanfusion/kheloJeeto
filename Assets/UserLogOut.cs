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
        // string a = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJhdWQiOiIzIiwianRpIjoiMjE2ZmM0MDg5NDYxOGJiODdiOWY2ZTM2NzQ0MjMwYWRmOTNkM2U3N2FlNWQ3MTljMDU2NjI0MTA3MzlhZjgzMTFmZDc5NWJkMDBhZTUxZjQiLCJpYXQiOjE3NTM1MDY2NTQuNzQ1NDQ4MTEyNDg3NzkyOTY4NzUsIm5iZiI6MTc1MzUwNjY1NC43NDU0NTE5MjcxODUwNTg1OTM3NSwiZXhwIjoxNzg1MDQyNjU0LjczMjU2NjExODI0MDM1NjQ0NTMxMjUsInN1YiI6IjQwMCIsInNjb3BlcyI6W119.GcaC6ZqhMtcuNBR7e9zJ4ZHc53gK8HIAr5HgZ6vMft2_-E0N6BQ1K_ATweDwDqbbeuX9DGtTPVe3lBcbC3Ikyzm9TwXphkm1f2w4XdsZ6vrsNbgY4JdwuRvSol7Tx_Zd8SjKh76DfI5ui7EGDM3IxegFs7ao4xVzPRlGjb5epzQfEwrzYNhj6AYObrRiI93dNODZFcHa0CKjI_l-FfbHT53bFS514yqSkWKMryAlMmNWNebYRJ5SWphUOZNU0I37fetiQI38wDCifXSPanKm7YMBiJKiLGE48xqaMVN6oKHHy0dlGFMOLyT_jOzpHXtmSdunsPFoFrlzRP_Me-5wmqsXaTYIftCVtVYJ2QaXWuHqzyAKDpCZcKW_UionGnldoZvXkVByUm-zDRZGtX8X67e2BaavvdXeXFDrd-xu0oMR1lGPSnlnuiIbDdExls2mdmv6R7MLUzSxqyrUOjl-Kzr3QN427xzq78UAaHofu2iAN87fM5NSP66qBihLNOQQJftcLkqVe3k-nKmUCTjwjOK29VrnE6UH2uar81uk1V7coXMs6BZnpPfbAMVIhtKmNp9JS4IIi6Unt8GTHz1-Vd4lJyxcNaI_-hvBPYJdaH6JDHhjVsl-Lzi3Wvpaq9jQPT6bwqrc-FkR3JtVnUTfNNjOtQwki3xRFp3HNAciBI0";

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
               // if (loginInfo["message"] == "Logged Out")
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
