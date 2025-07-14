using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class playfabmerger : MonoBehaviour
{
    public Text abc;
    public InputField em;
    public InputField nu;
    public InputField pw;

    public InputField amount;
    public TMP_Text totalamount;

    private int addamount;
    // private int totalamountint;
    // Start is called before the first frame update
    void Start()
    {
        // totalamount.text = "00";

        getmoneydata();

    }

    // Update is called once per frame
    void Update()
    {
        // moneygiver.score2 = totalamountint;
    }


    // void Login()
    // {
    //     var request = new LoginWithCustomIDRequest
    //     {
    //         CustomId = SystemInfo.deviceUniqueIdentifier,
    //         CreateAccount = true
    //     };
    //     PlayFabClientAPI.LoginWithCustomID(request, OnSuccess, OnError);

    // }
    // void OnSuccess(LoginResult result)
    // {
    //     Debug.Log("success");
    // }
    // void OnError(PlayFabError eror)
    // {
    //     Debug.Log("fail");
    // }

    public void RegisterButton()
    {
        var request = new RegisterPlayFabUserRequest {
            Username = em.text,
            // Number = nu.text,
            Password = pw.text,
            RequireBothUsernameAndEmail = false
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnError);

    }
    void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("register success2");
        SceneManager.LoadScene("Forindian Mobile");
    }

    void OnError(PlayFabError error)
    {
        abc.text = "There is some invalid information";
        Debug.Log(error.GenerateErrorReport());
    }
    void OnError2(PlayFabError error)
    {
        abc.text = "Incorrect username or password";
        Debug.Log(error.GenerateErrorReport());
    }
    public void LoginButton() {
        var request = new LoginWithPlayFabRequest {
            Username = em.text,
            Password = pw.text
        };
        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnError2);
    }
    void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("login success3");
        SceneManager.LoadScene("add money");
    }

    public void savemoney()
    {
        var request = new UpdateUserDataRequest{
            Data = new Dictionary<string, string> {
                {"money", totalamount.text}
            }
        };
        PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
    }

    void OnDataSend(UpdateUserDataResult result)
    {
        Debug.Log("data send success");
    }

    public void getmoneydata()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnDataRecieved, OnError);
    }

    void OnDataRecieved(GetUserDataResult result)
    {
        Debug.Log("Recieved data");
        Debug.Log(result.Data["money"].Value);
        string gotdatast = result.Data["money"].Value;
        int gotdata = Int32.Parse(gotdatast);
        totalamount.text = (int)gotdata + "";
        BalanceManager.Balance = gotdata;
    }

    public void add()
    {
        addamount = Int32.Parse(amount.text);
        BalanceManager.Balance += addamount;
        totalamount.text = (int)BalanceManager.Balance + "";

        amount.text = "";
    }
}
