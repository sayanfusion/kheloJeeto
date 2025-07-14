using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using System;
using SimpleJSON;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using TMPro;

[Serializable]
public class UserInfo
{
    public string name;
    public string balance;
    public string game_permission;
}
public class DashBoardData : MonoBehaviour
{
    private string jsonString;
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Balance;
    [SerializeField] private Image fillImage;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GamesPermissionManager gamesPermissionManager;
    public static Action UserLoggedinWithDetails;
    //Refernce for new activate and deactivate of panel accroding to games....
    public GameObject tablegamePanel;
    public GameObject drawgamePanel;
    public GameObject roulletegamePanel;

    private void Start()
    {
        drawgamePanel.SetActive(true);
        //loading.SetActive(true);
        // SetUserDetails();
        if (UserInfoPersist.Instance.userInfo != null && UserInfoPersist.Instance.userInfo.name != "")
        {
            SetDetails(UserInfoPersist.Instance.userInfo);
        }
        else
        {
            GetDetails();
        }
    }
    #region Minimize Game 
    [DllImportAttribute("user32.dll")]
    public extern static bool ShowWindow(IntPtr hwnd, int nCmdShow);
    //public static boolean{} ShowWindow(IntPtr hwnd, int nCmdShow);
    [DllImportAttribute("user32.dll")]
    public extern static IntPtr GetForegroundWindow();
    [DllImportAttribute("user32.dll")]
    public extern static IntPtr GetActiveWindow();

    public void Minimize()
    {
        //Minimize the window
        ShowWindow(GetActiveWindow(), 2);
    }

    #endregion

    public void Quit()
    {
        Application.Quit();
    }

    public void GetDetails()
    {
        StartCoroutine(get_Request(Constant.KIBaseURL + "user-details"));
    }

    IEnumerator get_Request(string url)
    {
        Debug.LogError(PlayerPrefs.GetString(Constant.Token));
        UnityWebRequest uwr = UnityWebRequest.Get(url);
        uwr.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString(Constant.Token));
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);

        }
        else
        {
            Debug.LogError("User Details : "+uwr.downloadHandler.text);
            jsonString = System.Text.Encoding.UTF8.GetString(uwr.downloadHandler.data, 3, uwr.downloadHandler.data.Length - 3);
            JSONNode loginInfo = JSON.Parse(uwr.downloadHandler.text);
            string message = loginInfo["message"];

            if (message.Equals("Success"))
            {
                print("yes");
                string name = loginInfo["userDetails"]["user_name"];
                string permissions = loginInfo["userDetails"]["game_permission"];
                UserInfoPersist.Instance.userInfo = new UserInfo
                {
                    name = name,
                    game_permission = permissions
                };
                Name.text = name;
                string balance = loginInfo["walletBlance"];
                UserInfoPersist.Instance.userInfo.balance = balance;
                double balanceDouble = Double.Parse(balance);
                Balance.text = balanceDouble.ToString("#0.00");
                PlayerPrefs.SetInt(Constant.STOKIESID, loginInfo["userDetails"]["id"].AsInt);
                Debug.LogError(loginInfo["userDetails"]["id"]);
                gamesPermissionManager.ShowGames(UserInfoPersist.Instance.userInfo.game_permission);
                UserLoggedinWithDetails?.Invoke();

                //loading.SetActive(false);
            }
            else
            {
                print("no");
            }
        }

    }
    public void SceneLoad(string name)
    {
        StartCoroutine(LoadScene(name));
    }

    private IEnumerator LoadScene(string name)
    {
        loadingScreen.SetActive(true);
        float duration = 3f; // Duration in seconds
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime; // Increment elapsed time by the time passed since the last frame
            float progress = Mathf.Clamp01(elapsedTime / duration);

            fillImage.fillAmount = progress;
            yield return null; // Wait for the next frame
        }

        // Additional delay of 2 seconds
        yield return new WaitForSeconds(2f);
        // Now the loading and the additional delay are completed, so switch to the new scene
        SceneManager.LoadScene(name);
    }


    public void SceneLoadTripleChance(string name)
    {
        GameSelector.SelectedGame = "TripleChance";
      //  SceneLoad(name);
    }
//functions call for tableGame..
public void OnclickTablegameBtn()
{
tablegamePanel.SetActive(true);
drawgamePanel.SetActive(false);
roulletegamePanel.SetActive(false);
}
//fucntion call for drawgame..
public void OnclickDrawGameBtn()
{
tablegamePanel.SetActive(false);
drawgamePanel.SetActive(true);
roulletegamePanel.SetActive(false);
}
//fucntion call for roulletegame..
public void OnclickroulleteGame()
{
tablegamePanel.SetActive(false);
drawgamePanel.SetActive(false);
roulletegamePanel.SetActive(true);
}



    public void SetDetails(UserInfo userInfo)
    {
        Name.text = userInfo.name;
        string balance = userInfo.balance;
        double balanceDouble = Double.Parse(balance);
        Balance.text = balanceDouble.ToString("#0.00");
        gamesPermissionManager.ShowGames(userInfo.game_permission);
        GetDetails();
    }

    public void SceneLoadTripleChancePro(string name)
    {
        GameSelector.SelectedGame = "TripleChancePro";
        SceneManager.LoadSceneAsync(name);
    }

}
