using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GWebUtility;
using SimpleJSON;

public class Login : UIPage, IConnectivityEvent
{
    public InputField iUserName;
    public InputField iPassword;

    public InputField iUserNameForForgotPass;

    public Text tErrorText;
    public Text tErrorTextForForgotPass;

    public GameObject forgotPassPanel;

    public GameObject gLoadingPanel;
    public GameObject noInternetPanel;

    private bool internetConnectionStatus = true;

    private void Start()
    {
        UIControllerTri.instance.Addpage(this, false);
        ConnectivityUtils.Instance.SubscribeToEvents(this, true);
    }

    private void OnDestroy()
    {
        ConnectivityUtils.Instance.SubscribeToEvents(this, false);
    }

    public override void OnEnter()
    {
        base.OnEnter();
        internetConnectionStatus = true;
        tErrorText.text = "";
#if !UNITY_ANDROID
        iUserName.ActivateInputField();
#endif
        iUserName.text = PlayerPrefs.GetString(Constant.LoginUserName);
        iPassword.text = PlayerPrefs.GetString(Constant.LoginPassword);
    }

    public override void Update()
    {
        base.Update();
        if((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) && UIControllerTri.instance.currentPage == this)
        {
            LoginClicked();
        }

        if(Input.GetKeyDown(KeyCode.Tab) && UIControllerTri.instance.currentPage == this)
        {
            iPassword.ActivateInputField();
        }
    }

    public void LoginClicked()
    {
        PlayerPrefs.SetString(Constant.LoginUserName, iUserName.text.Trim());
        PlayerPrefs.SetString(Constant.LoginPassword, iPassword.text.Trim());
       // UIControllerTri.instance.TransitionTo(PageType.LOBBY);
        if (iUserName.text.Trim().Length > 0)
        {
            if (iPassword.text.Trim().Length > 0)
            {
                tErrorText.text = "";
                gLoadingPanel.SetActive(true);

                Debug.Log(Constant.CurrentDeviceType);
                Web.Create()
                   .SetUrl(Constant.LoginURL, Web.RequestType.POST, Web.ResponseType.TEXT)
                   .AddField(Constant.Username, iUserName.text.Trim())
                   .AddField(Constant.Password, iPassword.text.Trim())
                   .AddField(Constant.Login_Device, Constant.CurrentDeviceType)
                   .SetOnSuccessDelegate((Web _web, Response _response) =>
                   {
                      gLoadingPanel.SetActive(false);
                       Debug.Log("Success " + _response.GetText());
                       JSONNode _jsonNode = JSON.Parse(_response.GetText());
                       if (_jsonNode["status"].Value == "1")
                       {
                           Constant.Name = iUserName.text.Trim();   //_jsonNode["result"]["firstname"].Value + _jsonNode["result"]["lastname"].Value;
                           Constant.PointBalance = double.Parse(_jsonNode["result"]["currentblance"].Value);
                           Constant.CurrentGameSession = _jsonNode["result"]["game_session"].Value;
                           Constant.CurrentAccessToken = _jsonNode["result"]["access-token"].Value;
                           UIControllerTri.instance.TransitionTo(PageType.LOBBY);

                           Debug.Log("current session : " + Constant.CurrentGameSession);
                       }
                        else
                        {
                               tErrorText.text = _jsonNode["message"].Value;
                        }
                    
                       _web.Close();
                   })
                   .SetOnFailureDelegate((Web _web, Response _response) =>
                   {
                       gLoadingPanel.SetActive(false);
                       Debug.Log("Found Error " + _response.GetError());
							if(_response.GetError() == Constant.NoInternet || _response.GetError() == Constant.ResolveDestinationHost || _response.GetError() == Constant.ConnectDestinationHost)
                        {
                           InternetStatus(false);
                           //UIControllerTri.instance.NoInterNetPopUp();
                        }
                        else
                        {
                            tErrorText.text = _response.GetError();
                        }
					 _web.Close();
                       
                      


                   })
                   .Connect();


            }
            else
            {
                tErrorText.text = "Enter Passowd".ToUpper();
            }
        }
        else{
            tErrorText.text = "Enter UserName".ToUpper();
        }
      
    }

    public override void YesClicked()
    {
        base.YesClicked();
        Quit();
    }

    public void ForgotPasswordClicked()
    {
        forgotPassPanel.SetActive(true);
    }

    public void CloseForgotPassword()
    {
        forgotPassPanel.SetActive(false);
    }

    public void ForgotPasswordSubmitClicked()
    {
        if(iUserNameForForgotPass.text.Length > 0)
        {
            tErrorTextForForgotPass.text = "";
            gLoadingPanel.SetActive(true);
            Web.Create()
               .SetUrl(Constant.ForgotPasswordURL, Web.RequestType.POST, Web.ResponseType.TEXT)
               .AddField(Constant.Username, iUserNameForForgotPass.text.Trim())
               .SetOnSuccessDelegate((Web _web, Response _response) =>
               {
                   gLoadingPanel.SetActive(false);
                   Debug.Log("Success " + _response.GetText());
                   JSONNode _jsonNode = JSON.Parse(_response.GetText());
                   if (_jsonNode["status"].Value == "1")
                   {
                       Debug.Log("current session : " + Constant.CurrentGameSession);
                       tErrorText.text = "An email sent to your Email Id";
                       forgotPassPanel.SetActive(false);
                   }
                   else
                   {
                       tErrorTextForForgotPass.text = _jsonNode["message"].Value;
                   }

                   _web.Close();
               })
               .SetOnFailureDelegate((Web _web, Response _response) =>
               {
                   gLoadingPanel.SetActive(false);
                   Debug.Log("Found Error " + _response.GetError());
                   if (_response.GetError() == Constant.NoInternet || _response.GetError() == Constant.ResolveDestinationHost || _response.GetError() == Constant.ConnectDestinationHost)
                   {
                       InternetStatus(false);
                       //UIControllerTri.instance.NoInterNetPopUp();
                   }
                   else
                   {
                       tErrorTextForForgotPass.text = _response.GetError();
                   }
                   _web.Close();
               })
               .Connect();
        }
        else
        {
            tErrorTextForForgotPass.text = "Enter UserName";
        }
    }

    public void InternetStatus(bool a_Status)
    {
        if (internetConnectionStatus != a_Status)
            internetConnectionStatus = a_Status;
        else
            return;

        if (!a_Status)
        {
            noInternetPanel.SetActive(true);
        }
        else
        {
            noInternetPanel.SetActive(false);
        }
    }

    public void ApplicationFocus(bool a_HasFocus) { }

    public void ApplicationPause(bool a_PauseStatus)
    {
        if (a_PauseStatus)
        {
            if (!noInternetPanel.activeSelf)
                noInternetPanel.SetActive(true);
        }
        else
        {
            if (noInternetPanel.activeSelf)
                noInternetPanel.SetActive(false);
        }
    }

    public void OnClickNoInternet()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
