using khelojeetonew;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System;
using JeetoJoker;

public class FakeUserPopup : MonoBehaviour
{
    [SerializeField] private InputField userNameInput;
    [SerializeField] private InputField passwordInput;
    [SerializeField] private GameObject loadingPanel;
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
            JeetoJokerManager.instance.ShowMessage("Enter valid username!");
        }
        else if (string.IsNullOrEmpty(password))
        {
            JeetoJokerManager.instance.ShowMessage("Enter valid password!");
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
        UnityWebRequest unityWebRequest = UnityWebRequest.Post(JeetoJokerManager.instance.apiData.loginApi, form);
        yield return unityWebRequest.SendWebRequest();
        loadingPanel.SetActive(false);
        if (unityWebRequest.result != UnityWebRequest.Result.Success)
        {
            try
            {
                Debug.LogError(unityWebRequest.downloadHandler.text);
              khelojeetonew.LoginData loginData = JsonConvert.DeserializeObject<khelojeetonew.LoginData>(unityWebRequest.downloadHandler.text);
                JeetoJokerManager.instance.ShowMessage(loginData.message);
            }
            catch (Exception e)
            {
                JeetoJokerManager.instance.ShowMessage(unityWebRequest.error);
            }
        }
        else
        {
            try
            {
                Debug.LogError(unityWebRequest.downloadHandler.text);
                khelojeetonew.LoginData loginData = JsonConvert.DeserializeObject<khelojeetonew.LoginData>(unityWebRequest.downloadHandler.text);
                if (loginData.status == 200)
                {
                    PlayerPrefs.SetString(Constants.token, loginData.token);
                    int.TryParse(loginData.walletBlance, out int coins);
                    Debug.Log("join room call from here start login");
                    if (SocketController.Instance.JoinRoom(loginData.userDetails.id.ToString(), loginData.userDetails.user_name, coins))
                    {
                        JeetoJokerManager.instance.Initialize(loginData.userDetails.user_name, coins);
                        gameObject.SetActive(false);
                    }
                    else
                    {
                        JeetoJokerManager.instance.LogoutSilent();
                        JeetoJokerManager.instance.ShowMessage("Socket not connected!");
                    }

                }
                else
                {
                    JeetoJokerManager.instance.ShowMessage(loginData.message);
                }
            }
            catch (Exception e)
            {
                JeetoJokerManager.instance.ShowMessage(e.ToString());
            }
        }
    }

    public IEnumerator AutoLogin(string token)
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(1);
        while (!SocketController.Instance.ipAndPortDownloaded)
        {
            yield return waitForSeconds;
        }
        Debug.LogError(token);
        loadingPanel.SetActive(true);
        yield return new WaitForSeconds(2);
        UnityWebRequest unityWebRequest = UnityWebRequest.Get(Constant.KIBaseURL + "user-details");
        unityWebRequest.SetRequestHeader(Constants.authorization, "Bearer " + token);
        yield return unityWebRequest.SendWebRequest();
        loadingPanel.SetActive(false);
        if (unityWebRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(unityWebRequest.error);
            JeetoJokerManager.instance.ShowMessage(unityWebRequest.error);
            Debug.LogError(unityWebRequest.downloadHandler.text);
        }
        else
        {
            try
            {
                Debug.LogError(unityWebRequest.downloadHandler.text);
                khelojeetonew.LoginData loginData = JsonConvert.DeserializeObject<khelojeetonew.LoginData>(unityWebRequest.downloadHandler.text);
                if (loginData.status == 200)
                {
                    int.TryParse(loginData.walletBlance, out int coins);
                      Debug.Log("join room aUTO login");
                    bool statusRoom = SocketController.Instance.JoinRoom(loginData.userDetails.id.ToString(), loginData.userDetails.user_name, coins);
                    if (statusRoom)
                    {
                        JeetoJokerManager.instance.Initialize(loginData.userDetails.user_name, coins);
                        gameObject.SetActive(false);
                    }
                    else
                    {
                        JeetoJokerManager.instance.LogoutSilent();
                        JeetoJokerManager.instance.ShowMessage("Socket not connected!");
                    }

                }
                else
                {
                    JeetoJokerManager.instance.ShowMessage(loginData.message);
                }
            }
            catch (Exception e)
            {
                JeetoJokerManager.instance.ShowMessage(e.ToString());
            }
        }
    }
}