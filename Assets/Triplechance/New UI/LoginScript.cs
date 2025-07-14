using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System;
using UnityEngine.UI;
using System.Collections;

public class LoginScript : MonoBehaviour
{
    [SerializeField] private InputField userNameInput;
    [SerializeField] private InputField passwordInput;
    [SerializeField] private GameObject LoginuserPopup;
    private string loginDevice = "android";
    private string baseUrl = "https://fusionclient.live/FTL8817102/ipl_games/api/user-logins";

    public void OnLoginButtonClicked()
    {
        StartCoroutine(Login());
    }

    private IEnumerator Login()
    {
        string userName = userNameInput.text;
        string password = passwordInput.text;
        string deviceId = SystemInfo.deviceUniqueIdentifier;
        Debug.Log("Sending request with deviceId: " + deviceId);
#if UNITY_EDITOR
        deviceId = "saikat_pc";
#endif

        WWWForm form = new WWWForm();
        form.AddField("user_name", userName);
        form.AddField("password", password);
        form.AddField("machine_id", deviceId);
        form.AddField("login_device", loginDevice);

        using (UnityWebRequest www = UnityWebRequest.Post(baseUrl, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Network error: " + www.error);
            }
            else
            {
                Debug.Log("Response: " + www.downloadHandler.text);
            }
        }
    }
}