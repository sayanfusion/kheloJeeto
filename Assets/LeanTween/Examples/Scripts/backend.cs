using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using SimpleJSON;
using System;
using System.Runtime.InteropServices;
using TMPro;

public class backend : MonoBehaviour
{
    public InputField user;
    public InputField password2;
    public static string userId;
    private string jsonString;
    public Text popupText;
    public TextMeshProUGUI errorText;
    public Toggle _rememberMeToggle;
    public GameObject panel;
    [SerializeField] private GameObject _circleLoading;
    // ;
    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey(Constants.Token))
        {
            PlayerPrefs.SetString(Constants.Token, "");
        }
        if (!PlayerPrefs.HasKey(Constants.LoginStatus))
        {
            PlayerPrefs.SetInt(Constants.LoginStatus, 0);
        }
        if (!PlayerPrefs.HasKey(Constants.UID))
        {
            PlayerPrefs.SetInt(Constants.UID, 0);
        }
        if (!PlayerPrefs.HasKey(Constant.Token))
        {
            PlayerPrefs.SetString(Constant.Token, "");
        }
        if (!PlayerPrefs.HasKey(Constant.UID))
        {
            PlayerPrefs.SetInt(Constant.UID, 0);
        }
        if (!PlayerPrefs.HasKey(Constant.User))
        {
            PlayerPrefs.SetString(Constant.User, "");
        }
        if (!PlayerPrefs.HasKey(Constant.password))
        {
            PlayerPrefs.SetString(Constant.password, "");
        }
        if (!PlayerPrefs.HasKey(Constant.STOKIESID))
        {
            PlayerPrefs.SetInt(Constant.STOKIESID, 0);
        }
        Debug.LogError("PlayerPrefs.GetString(Constant.User) " + PlayerPrefs.GetString(Constant.User));
        user.text = PlayerPrefs.GetString(Constant.User);
        password2.text = PlayerPrefs.GetString(Constant.password);
        _rememberMeToggle.isOn = PlayerPrefs.GetInt("remember_me") == 1 ? true : false;
        _circleLoading.SetActive(false);
    }

    public void OnToggleValueChange(Toggle toggle)
    {
        if (toggle.isOn)
        {
            PlayerPrefs.SetString(Constant.User, user.text);
            PlayerPrefs.SetString(Constant.password, password2.text);
            PlayerPrefs.SetInt("remember_me", toggle.isOn ? 1 : 0);
        }

        else
        {
            PlayerPrefs.DeleteKey(Constant.User);
            PlayerPrefs.DeleteKey(Constant.password);
        }
    }
    public void login()
    {
        _circleLoading.SetActive(true);
        StartCoroutine(TripleChancePostRequest(Constant.KIBaseURL + "login"));
    }

    IEnumerator postRequest(string url)
    {
        string deviceId = Application.platform.ToString();
#if UNITY_EDITOR
        deviceId = "PC";
#endif
        WWWForm form = new WWWForm();
        form.AddField("user_name", user.text);
        form.AddField("password", password2.text);
        form.AddField("login_device", deviceId);
        //   Debug.LogError("mechine_id "+ deviceId);
        UnityWebRequest uwr = UnityWebRequest.Post(url, form);
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
            popupText.text = "Network Error Try Later";
        }
        else
        {
            // Or retrieve results as binary data
            // byte[] results = uwr.downloadHandler.data;
            // print(results);
            jsonString = System.Text.Encoding.UTF8.GetString(uwr.downloadHandler.data, 3, uwr.downloadHandler.data.Length - 3);
            JSONNode loginInfo = JSON.Parse(uwr.downloadHandler.text);
            //Debug.Log("Login data "+uwr.downloadHandler.text);
            popupText.text = loginInfo["message"];
            panel.SetActive(true);

            Invoke("panelDisabler", 5);
            Debug.LogError(loginInfo["message"]);
            string msg = loginInfo["message"];
            if (msg.Equals("Successfully User Logedin"))
            {
                userId = loginInfo["user_id"];
                PlayerPrefs.SetInt(Constants.UID, Convert.ToInt16(loginInfo["user_id"]));
                PlayerPrefs.SetInt(Constant.UID, Convert.ToInt16(loginInfo["user_id"]));
                PlayerPrefs.SetString(Constants.Token, loginInfo["token"]);
                PlayerPrefs.SetString(Constant.Token, loginInfo["token"]);
                PlayerPrefs.SetInt(Constants.LoginStatus, 1);
                PlayerPrefs.SetInt(Constant.STOKIESID, Convert.ToInt16(loginInfo["userDetails"]["stockiest_id"]));
                Debug.LogError("STOKIESID " + PlayerPrefs.GetInt(Constant.STOKIESID));
                print("yes");
                SceneManager.LoadScene("DashBoard");
            }
            else
            {
                print("no");
            }
        }

    }
    IEnumerator TripleChancePostRequest(string url)
    {
        string deviceId = Application.platform.ToString();
#if UNITY_EDITOR
       deviceId = "PC";
#endif
        WWWForm form = new WWWForm();
        form.AddField("user_name", user.text);
        form.AddField("password", password2.text);
        form.AddField("login_device", deviceId);
        form.AddField("mechine_id", SystemInfo.deviceUniqueIdentifier);

        UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();
        Debug.Log(www.downloadHandler.text);
        _circleLoading.SetActive(false);
        // Parse JSON response into loginSuccessData object
        LoginSuccessData loginSuccessData = JsonUtility.FromJson<LoginSuccessData>(www.downloadHandler.text);
        if (loginSuccessData.status == 200)
        {
            userId = loginSuccessData.user_id.ToString();
            PlayerPrefs.SetInt(Constants.UID, loginSuccessData.user_id);
            PlayerPrefs.SetString(Constants.Token, loginSuccessData.token);
            PlayerPrefs.SetInt(Constants.LoginStatus, 1);
            if(!string.IsNullOrEmpty(loginSuccessData.userDetails.stockiest_id))
            PlayerPrefs.SetInt(Constant.STOKIESID, int.Parse(loginSuccessData.userDetails.stockiest_id));
            PlayerPrefs.SetString(Constant.User, loginSuccessData.userDetails.user_name);
            PlayerPrefs.SetString(Constants.BALANCE, loginSuccessData.walletBlance.ToString());
            Constant.Username = loginSuccessData.userDetails.user_name;
            Constant.PointBalance = Convert.ToDouble(loginSuccessData.walletBlance);
            if (!_rememberMeToggle.isOn)
            {
                PlayerPrefs.DeleteKey(Constant.User);
                PlayerPrefs.DeleteKey(Constant.password);
                PlayerPrefs.DeleteKey("remember_me");
                PlayerPrefs.Save();
            }
            else if (_rememberMeToggle.isOn)
            {
                PlayerPrefs.SetString(Constant.User, user.text);
                PlayerPrefs.SetString(Constant.password, password2.text);
                PlayerPrefs.SetInt("remember_me", _rememberMeToggle.isOn ? 1 : 0);
            }

            print("Triple Chance Login Success");
            // StartCoroutine(ApiCallsAfterLogin());
            SceneManager.LoadScene("DashBoard");
        }
        else if (loginSuccessData.status == 401)
        {
            errorText.text = loginSuccessData.message;
        }
        else
        {
            LoginFailedData loginFailedData = JsonUtility.FromJson<LoginFailedData>(www.downloadHandler.text);
            errorText.text = "Invalid Username or Password";
        }
    }

    public void panelDisabler()
    {
        panel.SetActive(false);
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
}
[System.Serializable]
public class LoginSuccessData
{
    public int status;
    public string message;
    public string token;
    public int user_id;
    public int walletBlance;
    public UserDetails userDetails;
}
public class LoginFailedData
{
    public int status;
    public string message;
}
[System.Serializable]
public class UserDetails
{
    public int id;
    public object name;
    public string user_name;
    public object email;
    public string user_type;
    public int is_login_approve;
    public int device_login_status;
    public string login_device;
    public object gender;
    public object date_of_birth;
    public object email_verified_at;
    public object game_login_status;
    public object recent_game_name;
    public string original_password;
    public int is_block;
    public int is_deleted;
    public string super_stokiest_id;
    public object agent_id;
    public string stockiest_id;
    public string percentage;
    public string player_permission;
    public object otp;
    public string allowed_devices;
    public string last_login_time;
    public int otp_verified;
    public DateTime created_at;
    public DateTime updated_at;
}