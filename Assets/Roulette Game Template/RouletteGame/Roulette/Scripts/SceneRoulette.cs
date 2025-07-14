using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using SimpleJSON;
using UnityEngine.SceneManagement;
using System;
using DevCommon;
using Newtonsoft.Json;
using System.Collections.Generic;
//using UnityEditor.SearchService;

public class SceneRoulette : MonoBehaviour
{
    public static SceneRoulette Instance;
    public static int counter1;
    public static SceneRoulette _Instance;
    public static bool numDisplayed = false;
    public static Chip selected = null;

    bool spinned;
    public static bool ispressed = false;
    public static int uiState = 0;  // popup window shows or not

    public BetPool pool;
    public EuropeanWheel _EuroWheel;    // slot game clase
    public AmericanWheel _AmeWheel;     // slot game clase

    [Space]
    [Header("Text")]
    public TMP_Text textBalance;        // user balance info
    public TMP_Text textBet;            // user bet info
    /////////////////////public TMP_Text resultText;            // result info

    [Space]
    [Header("UI")]
    public Button clearButton;
    public Button undoButton;
    public Button rebetButton;
    public Button rollButton;

    public Slider volumeSlider;
    public Toggle soundToggle;
    public Toggle musicToggle;

    [Space]
    [Header("Extra")]
    public CameraController camCtrl;
    public static float WaitTime;
    public static bool GameStarted = false;
    public static bool MenuOn = false;


    public int starttime;
    public static bool isClearPressed = false;
    private string jsonString;

    void Awake()
    {
        Instance = this;
        Application.targetFrameRate = 60;
        _Instance = this;
        timeCounter.instance.secondleft = starttime;
    }

    private void Start()
    {
        GetDetails();
        rebetButton.interactable = false;
        
        StartCoroutine(GameLoginStatus(Constant.KIBaseURL + "game-login-status"));
    }

    IEnumerator GameLoginStatus(string url)
    {
        Debug.LogError(PlayerPrefs.GetString(Constants.Token));
        WWWForm form = new WWWForm();
        form.AddField("game_login_status", "1");
        form.AddField("recent_game_name", "roulette");

        UnityWebRequest uwr = UnityWebRequest.Post(url, form);
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
            Debug.Log($"User Details : {uwr.downloadHandler.text}");
            string msg = loginInfo["message"];
            if (msg.Equals("Success.") || msg.Equals("Success"))
            {
                print("yes");
                GetDetails();
            }
            else
            {
                print("no");
            }
        }
    }

    public void GetDetails()
    {
        StartCoroutine(getRequest(Constant.KIBaseURL + "user-details-all"));
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
            Debug.Log($"User Details : {uwr.downloadHandler.text}");
            string msg = loginInfo["message"];
            if (msg.Equals("Success."))
            {
                print("yes");
                Debug.Log((float)Convert.ToDouble(loginInfo["walletBlance"]) + "BalanceRoullete");
                PlayerPrefs.SetInt(Constants.BALANCE, int.Parse(loginInfo["walletBlance"]));
                BalanceManager.SetBalance((float)Convert.ToDouble(loginInfo["walletBlance"]));
                Debug.LogError(loginInfo["userDetails"]["id"]);
                if (loginInfo["userDetails"]["is_deleted"].ToString().Trim().Equals("1".Trim()))
                {
                    Dictionary<string, object> dd = new Dictionary<string, object>();
                    dd.Add("playerId", PlayerPrefs.GetInt(Constant.UID).ToString());
                    dd.Add("name", PlayerPrefs.GetString(Constant.User));
                    dd.Add("roomId", PlayerPrefs.GetString(Constants.ROOMID));
                    Debug.LogError(JsonConvert.SerializeObject(dd));
                    SocketManager.Core.Socket.Emit("leaveRoom", dd);
                    ResultManager.totalBet = 0;

                    SceneManager.LoadScene("DashBoard");
                }
            }
            else
            {
                print("no");
            }
        }

    }
    void Update()
    {
     

        if (WinSequence.numberdisplayed == true)
        {
            numDisplayed = true;
            audios.Instance.InvokingPlaceBet();
            Invoke("BettingAgain", 8);
            //////////////////BettingAgain();
            BetSpace.isDoublePressed = false;
            counter1 = BetSpace.counter;
            BetSpace.counter = 1;
          //  Invoke(nameof(SpinRoulette), 52);
            //SpinRoulette();
            undoButton.interactable = true;
            WinSequence.numberdisplayed = false;
            
        }
    }

    public void BettingAgain()
    {
        //timeCounter.instance.secondleft = 60;
        clearButton.interactable = true;
        rollButton.interactable = true;
        rebetButton.interactable = true;
        undoButton.interactable = true;
        audios.Instance.InvokingNoMoreBet();
        ResettingPress();
        
    }



    public void MessageQuitResult(int value)
    {
        if (value == 0)
        {
            Application.Quit();
        }
    }
    
    public void OnButtonClear()
    {
        BetSpace.Instance.CountReset();
        Debug.Log("cccccllllllllleeeeeeeeaaaaaaaarrrrrbbbbbbbbbbbbtttttt");
        BetSpace.counter = 1;
        AudioManager.SoundPlay(3);
        clearButton.interactable = false;
        //rollButton.interactable = false;
        rebetButton.interactable = true;
        pool.Clear();
        BalanceManager.Balance += ResultManager.totalBet;
        SceneRoulette.UpdateLocalPlayerText();
        isClearPressed = true;
        ispressed = false;
        ResultManager.totalBet = 0;
        SceneRoulette._Instance._AmeWheel.ClearAllData();
        Invoke("ClearReset", 0.3f);
        // timeCounter.instance.secondleft = 30;
        // Invoke(nameof(SpinRoulette), 30);
    }

    public void ClearReset()
    {
        isClearPressed = false;
    }

    public void OnButtonUndo()
    {
        // undoButton.interactable = false;
        // AudioManager.SoundPlay(3);
        // pool.Undo();
        // OnButtonRebet();
        // OnButtonRebet();
        StartCoroutine(pool.Debet());
        //StartCoroutine(pool.Rebet());
    }

    public void onDouble()
    {
        if (mousetest.rightclick == true)
        {
            OnButtonClear();
        }
        
        ////////////int selectedValue = ResultManager.totalBet;
        ////////////BetSpace.Instance.DoubleBet(selectedValue);

        //////////////////if (BetSpace.BetsEnabled && selectedValue * 2 > 0 && BalanceManager.Balance - selectedValue  >= 0)
        //////////////////{ 
        //////////////////    counter = counter * 2;
        //////////////////}
    }

    public void counterClear()
    {
        BetSpace.counter = 1;
    }

    public void OnButtonRebet()
    {
        Debug.LogError("ispressed " + ispressed);
        if(ispressed == false)
        { 
            /////////////rebetButton.gameObject.SetActive(false);
            StartCoroutine(pool.Rebet());
            rebetButton.interactable = false;
            //timeCounter.instance.secondleft = 30;
            /////////////////////////timeCounter.instance.RollBall = false;
            ispressed = true;
            //////////////////////////////////Invoke("ResettingPress", 60);
            //Invoke(nameof(SpinRoulette), 30);
        }

    }

    public void ResettingPress()
    {
        ispressed = false;
        rebetButton.interactable = true;
    }

    public void OnButtonRoll()
    {
        undoButton.interactable = false;
        clearButton.interactable = false;
        rollButton.interactable = false;
        ////////////////////////resultText.text = "";
        timeCounter.instance.secondleft = 1;
        timeCounter.instance.RollBall = true;
        SpinRoulette();
        
    }

    public void SpinRoulette()
    {
        
        if(_EuroWheel != null)
            _EuroWheel.Spin();
        else if(_AmeWheel != null)
            _AmeWheel.Spin();

        ChangeUI();
        ////////////////////////////AudioManager.SoundPlay(2);
        
    }

    public void ChangeUI()
    {
        
        if (camCtrl != null)
            camCtrl.GoToTarget();
        ToolTipManager.Deselect();
        clearButton.interactable = false;
        undoButton.interactable = false;
        /////////////////////////rebetButton.gameObject.SetActive(false);
        ////////////////////////rebetButton.interactable = false;
        rollButton.interactable = false;
        ChipManager.EnableChips(false);
    }

    public void BlockBets()
    {
        MenuOn = true;
        BetSpace.EnableBets(false);
    }

    public void ReleaseBets()
    {
        MenuOn = false;
        BetSpace.EnableBets(!GameStarted);
    }

    public static void UpdateLocalPlayerText()
    {
        _Instance.textBet.text =  (int)ResultManager.totalBet + "";
        _Instance.textBalance.text = (int)BalanceManager.Balance + "";
        // _Instance.textBalance.text = BalanceManager.Balance.ToString("F2");
    }



}

