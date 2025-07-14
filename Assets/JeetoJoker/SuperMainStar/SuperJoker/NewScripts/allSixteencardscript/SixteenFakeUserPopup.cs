using khelojeetonew;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System;
using JeetoJoker;

public class SixteenFakeUserPopup : MonoBehaviour
{
    public static SixteenFakeUserPopup instance;
    [SerializeField] private InputField userNameInput;
    [SerializeField] private InputField passwordInput;
    [SerializeField] private GameObject loadingPanel;

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        if (PlayerPrefs.HasKey(Constants.userName))
        {
            userNameInput.text = PlayerPrefs.GetString(Constants.userName);
        }
        else
        {
            userNameInput.text = "";
        }
        if (PlayerPrefs.HasKey(Constants.password))
        {
            passwordInput.text = PlayerPrefs.GetString(Constants.password);
        }
        else
        {
            passwordInput.text = "";
        }
        if (PlayerPrefs.HasKey(Constants.token))
        {
            StartCoroutine(AutoLogin(PlayerPrefs.GetString(Constants.token)));
        }
    }

    public void StartGame()
    {

        string userName = userNameInput.text.Trim();
        string password = passwordInput.text;
        if (string.IsNullOrEmpty(userName))
        {
            Sixteen_cards.instance.ShowMessage("Enter valid username!");
        }
        else if (string.IsNullOrEmpty(password))
        {
            Sixteen_cards.instance.ShowMessage("Enter valid password!");
        }
        else
        {
            PlayerPrefs.SetString(Constants.userName, userName);
            PlayerPrefs.SetString(Constants.password, password);
            StartCoroutine(StartLogin(userName, password));
        }
    }

    IEnumerator StartLogin(string userName, string password)
    {
        string deviceId = SystemInfo.deviceUniqueIdentifier;
#if UNITY_EDITOR
        deviceId = "saikat_pc";
#endif
        loadingPanel.SetActive(true);
        WWWForm form = new WWWForm();
        form.AddField(Constants.userName, userName);
        form.AddField(Constants.password, password);
        form.AddField(Constants.machineId, deviceId);
        form.AddField(Constants.login_device, Constants.android);
        UnityWebRequest unityWebRequest = UnityWebRequest.Post(Sixteen_cards.instance.apiData.loginApi, form);
        yield return unityWebRequest.SendWebRequest();
        loadingPanel.SetActive(false);
        if (unityWebRequest.result != UnityWebRequest.Result.Success)
        {
            try
            {
                Debug.LogError(unityWebRequest.downloadHandler.text);
              JeetoJoker.LoginData loginData = JsonConvert.DeserializeObject<JeetoJoker.LoginData>(unityWebRequest.downloadHandler.text);
                Sixteen_cards.instance.ShowMessage(loginData.message);
            }
            catch (Exception e)
            {
                Sixteen_cards.instance.ShowMessage(unityWebRequest.error);
            }
        }
        else
        {
            try
            {
                Debug.LogError(unityWebRequest.downloadHandler.text);
             JeetoJoker.LoginData loginData = JsonConvert.DeserializeObject<JeetoJoker.LoginData>(unityWebRequest.downloadHandler.text);
                if (loginData.status == 200)
                {
                    PlayerPrefs.SetString(Constants.token, loginData.token);
                   // int.TryParse(loginData.walletBlance, out int coins);
                      
                        if (SixteenCardsSocketController.Instance.JoinRoom(loginData.userDetails.id.ToString(), loginData.userDetails.user_name, loginData.walletBlance))
                    {
                    
                            Sixteen_cards.instance.Initialize(loginData.userDetails.user_name, loginData.walletBlance);
                        
                      
                        
                            // Sixteen_cards.instance.Initialize(loginData.userDetails.user_name, loginData.walletBlance);
                            Debug.Log("print username from fake user popup " + loginData.userDetails.user_name);
                            gameObject.SetActive(false);
                      
                       
                    }
                    else
                    {
                        Sixteen_cards.instance.LogoutSilent();
                        Sixteen_cards.instance.ShowMessage("Socket not connected!");
                    }

                }
                else
                {
                    Sixteen_cards.instance.ShowMessage(loginData.message);
                }
            }
            catch (Exception e)
            {
                Sixteen_cards.instance.ShowMessage(e.ToString());
            }
        }
    }

   public IEnumerator AutoLogin(string token)
{
    WaitForSeconds waitForSeconds = new WaitForSeconds(1);
    while (!SixteenCardsSocketController.Instance.ipAndPortDownloaded)
    {
        yield return waitForSeconds;
    }

    Debug.LogError("Token received: " + token);  // Log the token

    loadingPanel.SetActive(true);
    yield return new WaitForSeconds(2);

    // Log the complete URL before making the request
    string userDetailsUrl = Sixteen_cards.instance.apiData.userDetailsApi;
    Debug.LogError("Request URL: " + userDetailsUrl);  // Debug the complete URL

    UnityWebRequest unityWebRequest = UnityWebRequest.Get(userDetailsUrl);
    unityWebRequest.SetRequestHeader(Constants.authorization, "Bearer " + token);

    yield return unityWebRequest.SendWebRequest();

    loadingPanel.SetActive(false);

    if (unityWebRequest.result != UnityWebRequest.Result.Success)
    {
        Debug.LogError("Request failed: " + unityWebRequest.error);  // Log error message
        Debug.LogError("Response body: " + unityWebRequest.downloadHandler.text);  // Log full response body for more details
        Sixteen_cards.instance.ShowMessage(unityWebRequest.error);  // Show error message to user
    }
    else
    {
        try
        {
            Debug.LogError("Response: " + unityWebRequest.downloadHandler.text); // Log the response body for debugging
           JeetoJoker.LoginData loginData = JsonConvert.DeserializeObject<JeetoJoker.LoginData>(unityWebRequest.downloadHandler.text);
            if (loginData.status == 200)
            {
                    // Handle successful login
                    if (SixteenCardsSocketController.Instance.JoinRoom(loginData.userDetails.id.ToString(), loginData.userDetails.user_name,loginData.walletBlance))
                {
                    Sixteen_cards.instance.Initialize(loginData.userDetails.user_name, loginData.walletBlance);
                    gameObject.SetActive(false);
                }
                else
                {
                    Sixteen_cards.instance.LogoutSilent();
                    Sixteen_cards.instance.ShowMessage("Socket not connected!");
                }
            }
            else
            {
                Sixteen_cards.instance.ShowMessage(loginData.message);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Exception: " + e.ToString());
            Sixteen_cards.instance.ShowMessage(e.ToString());
        }
    }
}
}
