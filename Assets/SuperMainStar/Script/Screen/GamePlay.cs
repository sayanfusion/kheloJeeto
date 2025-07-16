using DG.Tweening;
using Elendow.SpritedowAnimator;
using GWebUtility;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Block;
using Newtonsoft.Json;
using TMPro;
using DevCommon;
using DevCommon.Utils;
using BestHTTP.JSON;
using tripplechance;

public class GamePlay : UIPage
{

    public GameObject MessagePanel;
    public Text MessageTxt;
    private HashSet<string> usedTicketIds = new HashSet<string>();
    public class GameData
    {
        public string gameId;
        public string userId;
        public List<CardData> cardValueSet;
    }
    public string getCardData;
    public static GamePlay instance;
    public GameObject Logo;
    public GameObject ResultP;
    public TextMeshProUGUI resulttext;
    public RandomClickParent leftRandomClick;
    public RandomClickParent rightRandomClick;
    public RectTransform[] clickPopUp;

    //public GameObject gWinPanel;
    public GameObject gWinData;
    public GameObject gJewel;
    public string KeyBet;
    public string ValueBet;
    public string TotalBet;
    public string jsonString_TCP;
    public string ticketId;
    public GameObject gNumberBlock;
    public TextMeshProUGUI tWinAmount;
    public Text tWinAmountInWinPanel;
    // public GameObject noInternetPanel;
    public GameObject Winpopup;
    public Text winpopupText;
    public AudioClip winAudioClip;
    //public int multiplier = 1;

    public ResultBox resultBox;
    private List<BlockData> lstAllBlockData;
    public List<BlockData> lstBlockDataForRepeat = new List<BlockData>();

    public List<Block> lstResultBlock;
    public List<Chiptri> lstAllChip;
    public Chiptri lastSelectedChip;

    public int currentSelectedChip;
    public TextMeshProUGUI tPointBalance;
    public TMP_Text tPlayValue;
    public TextMeshProUGUI tUserName;

    public double previousPlayPoint;
    public Transform tripleButtonParent;
    public List<Tab> lstAllTab;

    public Button bInfo;
    public Button bClear;
    public Button bDoubleUp;
    public Button bRepeat;
    //public Button bRemove;

    private bool isRemoveClicked;

    public GameObject gInfoPanel;
    [SerializeField] private string Multiplier;
    public string WinningType;//prabir
    public Text messageBoxText;
    public Animation aMessageBoxOverlay;
    public SpriteAnimator spriteAnimator;

    private int iSelectedTabIndex;
    private double iPointBalance;
    public double roundStartBalance;


    public Tab currentSelectedTab;
    public RandomPickBlock currentSelectedDoubleRandomBlock;
    public RandomPickBlock currentSelectedTripleRandomBlock;
    // public Animator innerMostWheelSpinAnimator;
    [System.Serializable]
    public class Data_TCP
    {
        public string ticketId;
        public string gameId;
        public string userId;
        public string gamename;
        public List<CardData> cardValueSet;
    }
    public class CardData
    {
        public string card;
        public int value;
    }
    public string testCardData;

    public TabParent tabParent;
    public double playValue;
    public long totalbetamountget = 0;


    public double IPointBalance
    {
        get
        {
            return iPointBalance;
        }

        set
        {
            iPointBalance = value;
            if (UserInfoPersist.Instance != null && UserInfoPersist.Instance.userInfo != null)
            {
                UserInfoPersist.Instance.userInfo.balance = value.ToString("#0.00");
                roundStartBalance = value;
            }
            tPointBalance.text = value.ToString("#0.00");
            playValue = firstPointBalance - IPointBalance;
            Debug.Log("play value:" + playValue);
            tPlayValue.text = playValue.ToString();
            Debug.Log("playe value11:" + tPlayValue.text);

            // Debug.Log("play value : " + playValue);
        }
    }




    public bool IsRemoveClicked
    {
        get
        {
            return isRemoveClicked;
        }

        set
        {
            isRemoveClicked = value;
            if (!value && lastSelectedChip != null)
            {
                lastSelectedChip.EnableDisable(true);
            }
        }
    }

    private string sResultNumber = "000";
    private string singleWinAmount;
    private string doubleWinAmount;
    private string tripleWinAmount;

    public List<SpinWheel> lstAllWheel;
    public List<SpinWheelSystem> listAllNewWheels;
    public Text tResultNumber;
    public Text multiplierText;
    public ParticleSystem bumperEffect;
    public GameObject winAnimationGameObject;
    public ParticleSystem coinEffectForLessWin;

    public Text singleWinValueText;
    public Text doubleWinValueText;
    public Text tripleWinValueText;

    public delegate void OnClearClicked();
    public static OnClearClicked onClearClicked;
    string numbers;
    string winLoss;
    public AudioClip wheelSound;
    private double firstPointBalance;

    #region Android Version
    public Sprite sNormalBGForPlay;
    public Sprite sSelectedBGForPlay;
    public GameObject gNotEnoughCoin;
    #endregion

    #region All Delegates

    public delegate void OnResultSuccess(string number);
    public static OnResultSuccess onResultSuccess;



    #endregion
    public string gameId;
    public TextMeshProUGUI GameID;
    private float delayBetweenResultCallback;
    public Timer timerScript;
    public GameObject middleAnimation, wintextparent, winobject;
    public GameObject resultdata;
    public Transform reultdataParent;
    [SerializeField] private List<GameObject> resultdataList;
    public double currectpPointvalue;
    public double winningvalue;
    Dictionary<string, int> singleDigit = new Dictionary<string, int>();
    Dictionary<string, int> doubleDigit = new Dictionary<string, int>();
    Dictionary<string, int> tripleDigit = new Dictionary<string, int>();

    public AllDatas allDatas = new AllDatas();

    public bool isListen = false;
    bool getResultHistory = false;
    int midvalue = 3;
    int midHighValue = 1;
    public int myPlayerID;
    public GameObject alertPanel;
    public Text singlePlayText, doublePlayText, TriplePlayText;
    public Text singleWinText, doubleWinText, TripleWinText;
    private int singlePlayValue, doublePlayValue, triplePlayValue;
    //added by shivamfusion07 to check bet button click
    public static bool isbetclicked = false;
    private void Awake()
    {
        getResultHistory = false;
        Application.runInBackground = true;
        instance = this;
        timerScript = GetComponent<Timer>();
        if (!PlayerPrefs.HasKey(Constant.Token))
        {
            PlayerPrefs.SetString(Constant.Token, "");
        }
        if (!PlayerPrefs.HasKey(Constant.LoginStatus))
        {
            PlayerPrefs.SetInt(Constant.LoginStatus, 0);
        }
        if (!PlayerPrefs.HasKey(Constant.UID))
        {
            PlayerPrefs.SetInt(Constant.UID, 0);
        }
        if (!PlayerPrefs.HasKey(Constant.ISFIRST))
        {
            PlayerPrefs.SetInt(Constant.ISFIRST, 0);
        }
        if (!PlayerPrefs.HasKey(Constant.WINNINGTIME))
        {
            PlayerPrefs.SetInt(Constant.WINNINGTIME, 0);
        }
        if (!PlayerPrefs.HasKey(Constant.STOKIESID))
        {
            PlayerPrefs.SetInt(Constant.STOKIESID, 0);
        }
        bClear.interactable = false;
        bRepeat.interactable = false;
        bDoubleUp.interactable = false;
        //gWinPanel.SetActive(false);

        //if (PlayerPrefs.GetInt(Constant.LoginStatus) == 0)
        //{
        //    login();
        //}
        if (UserInfoPersist.Instance != null && UserInfoPersist.Instance.userInfo != null && UserInfoPersist.Instance.userInfo.name != null)
        {
            SetUserDetails(UserInfoPersist.Instance.userInfo);
        }
        else
        {
            GetDetails();
        }

    }
    private void Start()
    {

        //isListen = true;
        playValue = 0;
        // innerMostWheelSpinAnimator.StopPlayback();
        allDatas.ClearAllData();
        gameId = DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString()
            + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
        GameID.text = gameId;
        //float timeValue = 90;
        //UpdateTimer(timeValue);
        //ShowMessage("Place your chip");
        //UIControllerTri.instance.Addpage(this, false);
        //spriteAnimator.gameObject.SetActive(false);
        //spriteAnimator.onFinish.AddListener(OnSpriteAnimationFinished);
        //CheckStockies();
        // StartCoroutine(SetBlockUser());
        for (int i = 0; i < 6; i++)
        {
            GameObject go = Instantiate(resultdata, reultdataParent);
            go.SetActive(true);
            //go.transform.SetSiblingIndex(0);
            resultdataList.Add(go);
        }
        if (UserInfoPersist.Instance && UserInfoPersist.Instance.liveResultData.list.Count > 0)
        {
            SetAllHistoryResults();
        }
        else
        {
            GetAllResults(true);
        }
    }
    public void Betdata()
    {
        isbetclicked = true;
        List<string> keys = new List<string>();
        List<long> values = new List<long>();

        foreach (var item in allDatas.singleDatas)
        {
            if (item.value > 0)
            {
                keys.Add(item.card);
                values.Add(item.value);
            }
        }
        foreach (var item in allDatas.doubleDatas)
        {
            if (item.value > 0)
            {
                keys.Add(item.card);
                values.Add(item.value);
            }
        }
        foreach (var item in allDatas.tripleDatas)
        {
            if (item.value > 0)
            {
                keys.Add(item.card);
                values.Add(item.value);
            }
        }

        long totalBetAmount = values.Sum();
        KeyBet = string.Join(",", keys);
        ValueBet = string.Join(",", values);
        TotalBet = totalBetAmount.ToString();

        // Prepare cardValueSet as a list of dictionaries
        List<CardData> cardValueSet = keys.Select((key, index) => new CardData
        {
            card = key,
            value = (int)values[index]
        }).ToList();

        // Generate unique ticket ID
        ticketId = GenerateUniqueTicketId();

        // Create Data_TCP object
        Data_TCP dataTCP = new Data_TCP
        {
            ticketId = ticketId,
            gameId = gameId,
            userId = PlayerPrefs.GetInt(Constant.STOKIESID).ToString(),
            gamename = "tripleChance",
            cardValueSet = cardValueSet
        };

        // Serialize the object to JSON
        string jsonData = JsonConvert.SerializeObject(dataTCP, Formatting.Indented);

        Debug.Log("SendDataToAPI " + jsonData);
        StartCoroutine(sendbetdataAPI(jsonData));
    }

    IEnumerator sendbetdataAPI(string jsonData)
    {
        WWWForm form = new WWWForm();
        form.AddField("data", jsonData);

        Debug.Log("Sending Data to API: " + jsonData);

        UnityWebRequest request = UnityWebRequest.Post(testCardData, form);
        request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("TCP Request failed: " + request.error);
            StartCoroutine(DisplayBetNOTAcceptedMessage());
        }
        else
        {
            Debug.Log("TCP Request successful: " + request.downloadHandler.text);
            Printer_TCP.Instance.PrintPdf();
        }
    }

    public void ClearClickedforBET()
    {
        Debug.Log("clear clicked for bet works");

        // Reset each block's game amount
        for (int i = 0; i < lstAllBlockData.Count; i++)
        {
            lstAllBlockData[i].game_amount = 0;
            AddToBalance(lstAllBlockData[i].game_amount);
            lstAllBlockData[i].block.OnDeselectSuccess(0);
        }

        // Reset the play value and update the UI
        playValue = 0;
        tPlayValue.text = playValue.ToString(); // Ensure UI reflects the reset
        Debug.Log("tplay value reset: " + tPlayValue.text);

        // Reset firstPointBalance and IPointBalance here
        firstPointBalance = IPointBalance;  // Ensures new balance starts correctly
        Debug.Log("firstPointBalance reset: " + firstPointBalance);

        // Clear data and update UI
        allDatas.ClearAllData();
        lstAllBlockData.Clear();

        // Disable buttons and additional actions
        bClear.interactable = false;
        bDoubleUp.interactable = false;
        bRepeat.interactable |= (lstBlockDataForRepeat != null && lstBlockDataForRepeat.Count > 0);

        ResetPlayValue();
    }

    private IEnumerator DisplayBetNOTAcceptedMessage()
    {
        MessagePanel.SetActive(true);
        MessageTxt.text = "Your Bet Declined! Try again";

        yield return new WaitForSeconds(2);
        MessagePanel.SetActive(false);

    }
    public void GETbetData()
    {
        // ClearClicked();
        // ClearClickedfrprinttriplechance();
        StartCoroutine(GetBetDataAPI());
    }
    IEnumerator GetBetDataAPI()
    {
        print("SendGameIdToAPI " + gameId + " - " + PlayerPrefs.GetInt(Constant.STOKIESID).ToString());


        Dictionary<string, string> formData = new Dictionary<string, string>();
        formData.Add("GameId", gameId.ToString());
        formData.Add("player_id", PlayerPrefs.GetInt(Constant.STOKIESID).ToString());

        WWWForm form = new WWWForm();
        foreach (var entry in formData)
        {
            form.AddField(entry.Key, entry.Value);
        }

        UnityWebRequest request = UnityWebRequest.Post(getCardData, form);
        request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error sending request: " + request.error);
        }
        else
        {
            Debug.Log("Response Get Card data: " + request.downloadHandler.text);
            string jsonResponse = request.downloadHandler.text;
            GameData gameData = JsonConvert.DeserializeObject<GameData>(jsonResponse);
            totalbetamountget = 0;
            for (int i = 0; i < gameData.cardValueSet.Count; i++)
            {
                CardData cardData = gameData.cardValueSet[i];
                // SendData.CardValueSet datanew = new SendData.CardValueSet();
                string key = cardData.card;
                //  datanew.card=key;
                int value = cardData.value;
                //  datanew.value=value;
                // playValue += value;
                totalbetamountget += value;
                BlockType blockType = GetBlockType(key);
                OnBetData(blockType, value, key);
            }
            // tPlayValue.text = playValue.ToString();
            // Debug.Log("tplay value33:"+tPlayValue.text);
        }
    }
    public BlockType GetBlockType(string key)
    {
        // Determine the block type based on the length of the key
        if (key.Length == 1)
        {
            return BlockType.SINGLE;
        }
        else if (key.Length == 2)
        {
            return BlockType.DOUBLE;
        }
        else if (key.Length == 3)
        {
            return BlockType.TRIPLE;
        }
        else
        {
            // Handle other cases or raise an exception if needed
            return BlockType.SINGLE; // Default to SINGLE if the length is not 1, 2, or 3
        }
    }

    private string GenerateUniqueTicketId()
    {
        const int maxAttempts = 100;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            int randomTicketId = UnityEngine.Random.Range(1000000, 10000000);

            string ticketId = randomTicketId.ToString();

            if (!usedTicketIds.Contains(ticketId))
            {
                usedTicketIds.Add(ticketId);
                return ticketId;
            }
        }

        return GenerateFallbackTicketId();
    }

    private string GenerateFallbackTicketId()
    {

        int randomTicketId = UnityEngine.Random.Range(1000000, 10000000);
        return randomTicketId.ToString();
    }


    private void SetAllHistoryResults()
    {
        if (UserInfoPersist.Instance.liveResultData.list.Count > 0)
        {

            //SetWheel to first Data
            int startNumber = int.Parse(UserInfoPersist.Instance.liveResultData.list[0].win_card);
            for (int i = listAllNewWheels.Count - 1; i >= 0; i--)
            {
                int n = startNumber % 10;
                if (n <= 9)
                {
                    listAllNewWheels[i].spinWheel.SetDestinationDirectly(n);
                    startNumber /= 10;
                }
            }
            Debug.LogError(UserInfoPersist.Instance.liveResultData.list.Count);
            for (int i = 0; i < 6; i++)
            {
                string number = UserInfoPersist.Instance.liveResultData.list[i].win_card.ToString().Trim('"');
                resultdataList[i].GetComponent<ResultDate>().SetResultdata(number.Substring(2), number.Substring(1), number.ToString());
            }
        }
        // GetAllResults(true);
    }

    public void StartGame(float time)
    {
        Debug.Log("Start Game Called");
        float timeValue = 90;
        UpdateTimer(time);
        spriteAnimator.gameObject.SetActive(false);
        spriteAnimator.onFinish.AddListener(OnSpriteAnimationFinished);
        CheckStockies();

    }


    public void SetDatas()
    {
        for (int i = 0; i < 10; i++)
        {
            allDatas.singleDatas.Add(new DataClass() { card = i.ToString(), value = 0 });
        }
        for (int i = 0; i < 100; i++)
        {
            allDatas.doubleDatas.Add(new DataClass() { card = i.ToString("00"), value = 0 });
        }
        for (int i = 0; i < 1000; i++)
        {
            allDatas.tripleDatas.Add(new DataClass() { card = i.ToString("000"), value = 0 });
        }
    }

    public void OnBetData(BlockType blockType, int value, string key)
    {
        if (blockType == BlockType.SINGLE)
        {
            allDatas.singleDatas.Find(x => x.card.Equals(key)).value = value;
        }
        if (blockType == BlockType.DOUBLE)
        {

            allDatas.doubleDatas.Find(x => x.card.Equals(key)).value = value;
        }
        if (blockType == BlockType.TRIPLE)
        {

            allDatas.tripleDatas.Find(x => x.card.Equals(key)).value = value;
        }


    }
    public void AllPlayAmountSet(BlockType blockType, int value)
    {
#if UNITY_ANDROID
        if (blockType == BlockType.SINGLE)
        {
            singlePlayValue += value;
            singlePlayText.text = singlePlayValue.ToString();
        }
        if (blockType == BlockType.DOUBLE)
        {
            doublePlayValue += value;
            doublePlayText.text = doublePlayValue.ToString();
        }
        if (blockType == BlockType.TRIPLE)
        {
            triplePlayValue += value;
            TriplePlayText.text = triplePlayValue.ToString();
        }
#endif
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
            Debug.LogError(uwr.result);
            jsonString = System.Text.Encoding.UTF8.GetString(uwr.downloadHandler.data, 3, uwr.downloadHandler.data.Length - 3);
            JSONNode loginInfo = JSON.Parse(uwr.downloadHandler.text);
            Debug.Log("Login Info : Player ID = " + loginInfo["userDetails"]["id"]);
            if (loginInfo["userDetails"]["id"] != null)
            {
                myPlayerID = int.Parse(loginInfo["userDetails"]["id"]);
            }
            string message = loginInfo["message"];

            if (message.Equals("Success"))
            {
                print("yes");
                string balance = loginInfo["walletBlance"];
                Debug.LogError(Convert.ToDouble(balance));
                Constant.PointBalance = Convert.ToDouble(balance);
                IPointBalance = firstPointBalance = Constant.PointBalance;
                Debug.LogError(loginInfo["userDetails"]["id"]);
                tUserName.text = loginInfo["userDetails"]["user_name"];
            }
            else
            {
                print("no");
            }
        }

    }
    public void SetUserDetails(UserInfo info)
    {
        tUserName.text = info.name;
        string balance = info.balance;
        Debug.LogError(Convert.ToDouble(balance));
        Constant.PointBalance = Convert.ToDouble(balance);
        IPointBalance = firstPointBalance = Constant.PointBalance;
        GetDetails();
    }
    private void PlayBlastAnimation()
    {
        spriteAnimator.gameObject.SetActive(true);
        spriteAnimator.Play(false);
    }

    private void OnSpriteAnimationFinished()
    {
        spriteAnimator.gameObject.SetActive(false);
    }

    private void OnCompleteWheelMovement(int number, GameObject gameObject)
    {
        if (gameObject != null)
        {
            gameObject.SetActive(true);
            gameObject.transform.GetComponentInChildren<TextMeshProUGUI>().text = number.ToString();
        }
    }

    private void OnCompleteAllWheelMovement(string sin, string dou, string tri)
    {
        //SoundController.instance.StartAndStopWheelSpin(false);
        //Audio_Manager.instance.StopAudio();
        //SetResults(sin, dou, tri);
        SetResultsData(tri + dou + sin);
        gJewel.SetActive(true);
        gJewel.GetComponent<Animator>().SetBool("blink", true);
        middleAnimation.SetActive(false);
        if (wintextparent)
        {
            wintextparent.SetActive(true);
        }
        if (winobject)
        {
            winobject.transform.localScale = Vector3.zero;
            winobject.transform.DOScale(1, 0.5f);
        }
        if (multiplierText)
        {
            multiplierText.text = Multiplier;
        }

        // double totalWinAmount = (int.Parse(tri) * 900) + (int.Parse(dou) * 90) + (int.Parse(sin) * 9);
        // Debug.Log("Total win AMount : " + totalWinAmount);
        // IPointBalance += totalWinAmount;
        if (SceneManager.GetActiveScene().name.Equals("TripleChanceProGameplay"))
        {
            tResultNumber.text = "";
            tResultNumber.text = allDatas.tripleData.card;

        }
        Debug.Log("show result");
        SetRedult();
        // StartCoroutine(RestartGame());

        ShowResultBlock();//closed by sayam
    }

    public void SetRedult()
    {
        StartCoroutine(setResut());
    }

    IEnumerator setResut()
    {
        if (ResultP)
        {
            ResultP.SetActive(true);
        }
        if (resulttext)
        {
            resulttext.text = Constant.ResultNumber;
        }



        yield return new WaitForSeconds(10f);
        //ResultP.SetActive(false);
        //Logo.SetActive(true);
    }

    void SetResultsData(string result)
    {

        if (SceneManager.GetActiveScene().name == "TripleChanceProGameplay")
        {

            StartCoroutine(setResultRequest(Constant.KIBaseURL + "live-data-history", result, "tripleChancePro"));

        }
        else
        {
            GetAllResults();
            //StartCoroutine(setResultRequest(Constant.KIBaseURL + "live-datas", result, "TripleChance"));//close by prabir
        }
    }
    IEnumerator setResultRequest(string url, string result, string gamename)
    {
        Debug.Log(gamename + "  :Milan");
        WWWForm form4 = new WWWForm();
        form4.AddField("win_number", result);
        form4.AddField("game_name", gamename);
        form4.AddField("user_id", PlayerPrefs.GetInt(Constant.UID));
        UnityWebRequest uwr = UnityWebRequest.Post(url, form4);

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
            Debug.LogError("getResultRequest " + uwr.downloadHandler.text);
            if (msg.Equals("200"))
            {
                print("yes");
                GetAllResults();
            }
            else
            {
                print("no");
            }
        }

        uwr.Dispose();
    }
    void GetAllResults(bool firstTime = false)
    {
        if (SceneManager.GetActiveScene().name == "TripleChanceProGameplay")
        {
            StartCoroutine(getResultRequest(Constant.KIBaseURL + "live-data-history", "TripleChancePro", firstTime));
        }
        else
        {
            StartCoroutine(getResultRequest(Constant.KIBaseURL + "live-data-history", "TripleChance", firstTime));
        }
    }
    IEnumerator getResultRequest(string url, string gamename, bool firstTime)
    {
        WWWForm form4 = new WWWForm();
        form4.AddField("player_id", PlayerPrefs.GetInt(Constant.UID));
        form4.AddField("game_name", gamename);
        UnityWebRequest uwr = UnityWebRequest.Post(url, form4);

        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            Debug.Log(uwr.downloadHandler.text);
            // Parse JSON response into loginSuccessData object
            LiveResultData resultdata = JsonUtility.FromJson<LiveResultData>(uwr.downloadHandler.text);
            if (UserInfoPersist.Instance)
                UserInfoPersist.Instance.liveResultData = resultdata;
            if (resultdata.status == 200)
            {
                if (resultdata.list.Count > 0)
                {
                    if (firstTime)
                    {
                        //SetWheel to first Data
                        int startNumber = int.Parse(resultdata.list[0].win_card);
                        for (int i = listAllNewWheels.Count - 1; i >= 0; i--)
                        {
                            int n = startNumber % 10;
                            if (n <= 9)
                            {
                                listAllNewWheels[i].spinWheel.SetDestinationDirectly(n);
                                startNumber /= 10;
                            }
                            //listAllNewWheels[i].spinWheel.SetDestination(0);
                        }
                    }
                    Debug.LogError(resultdata.list.Count);
                    for (int i = 0; i < 6; i++)
                    {
                        string number = resultdata.list[i].win_card.ToString().Trim('"');
                        resultdataList[i].GetComponent<ResultDate>().SetResultdata(number.Substring(2), number.Substring(1), number.ToString());
                    }
                }

            }
        }
        //if (uwr.isNetworkError)//prabir
        //{
        //    Debug.Log("Error While Sending: " + uwr.error);

        //}
        //else
        //{
        //    Debug.LogError(uwr.result);
        //    jsonString = System.Text.Encoding.UTF8.GetString(uwr.downloadHandler.data, 3, uwr.downloadHandler.data.Length - 3);
        //    JSONNode loginInfo = JSON.Parse(uwr.downloadHandler.text);
        //    string msg = loginInfo["status"];
        //    Debug.LogError("getResultRequest " + uwr.downloadHandler.text);
        //    if (msg.Equals("200"))
        //    {
        //        print("yes");
        //        for (int i = 0; i < loginInfo["list"].Count; i++)
        //        {
        //            string number = loginInfo["list"][i]["win_number"].ToString().Trim('"');
        //            resultdataList[i].GetComponent<ResultDate>().SetResultdate(number.Substring(2), number.Substring(1), number.ToString());
        //        }
        //    }
        //    else
        //    {
        //        print("no");
        //    }
        //}

        uwr.Dispose();
    }
    public void betreset()
    {
        totalbetamountget = 0;
        Debug.Log("restart bet value:" + totalbetamountget);
    }
    public void RestartGame()
    {
        //Change According to Timer Sleep from Server
        // yield return new WaitForSeconds(14f);
        gameId = DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString()
            + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
        GameID.text = gameId;
        roundStartBalance = iPointBalance;
        Debug.Log("Reset All Data called");
        ResetAllData();

        CheckStockies();
        GetDetails();
    }
    private void OnStartAllWheel()
    {
        for (int i = 0; i < listAllNewWheels.Count; i++)
        {
            listAllNewWheels[i].gNumberBlock.SetActive(false);
        }
        gJewel.SetActive(false);
    }

    private void OnDestroy()
    {
        //ConnectivityUtils.Instance.SubscribeToEvents(this, false);
        //BalanceRecharge.Instance.RemoveListener(this);
    }

    private void OnEnable()
    {
        lstAllBlockData = new List<BlockData>();
        lstAllBlockData.Clear();
        bClear.interactable = false;
        bDoubleUp.interactable = false;
        bRepeat.interactable = false;

        Block.onSelectBlock += OnSelectBlock;
        Block.onDeSelectBlock += OnDeSelectBlock;
        Block.onAddBlock += OnSelectFromRowColoum;


        RandomPickBlock.onSelectRandomPickBlock += RemovePreviousSelectedBlock;

        //if (UIControllerTri.instance.sAllResult.Count > 0)
        //{
        //    sResultNumber = int.Parse(UIControllerTri.instance.sAllResult[0].number).ToString("000");

        //    // commented by somnath
        //    //for (int i = 0; i < lstAllWheel.Count; i++)
        //    //{
        //    //    lstAllWheel[i].FindNumberInArrayAndSetAngle(int.Parse(sResultNumber[i].ToString()));
        //    //}
        //    Debug.Log("Called");
        //    for (int i = 0; i < listAllNewWheels.Count; i++)
        //    {
        //        listAllNewWheels[i].spinWheel.SetDestinationDirectly(int.Parse(sResultNumber[i].ToString()));
        //    }
        //    Debug.Log("Bet numbers Previous: " + int.Parse(sResultNumber[0].ToString()) + int.Parse(sResultNumber[1].ToString()) + int.Parse(sResultNumber[2].ToString()));
        //}
    }

    private void OnDisable()
    {
        Block.onSelectBlock -= OnSelectBlock;
        Block.onDeSelectBlock -= OnDeSelectBlock;
        Block.onAddBlock -= OnSelectFromRowColoum;

        RandomPickBlock.onSelectRandomPickBlock -= RemovePreviousSelectedBlock;
        //SoundController.instance.PlayAndStopWinSound(false);
    }
    public override void OnEnter()
    {
        //base.OnEnter();
        //float timeValue = 90 - Constant.TimerForGame;
        //UpdateTimer(timeValue);
        //ShowMessage( "Place your chip");
        if (tUserName != null)
            tUserName.text = Constant.Name.ToUpper();
        gWinData.SetActive(false);


        if (singleWinValueText != null)
        {
            singleWinValueText.text = singleWinAmount = "0";
            doubleWinValueText.text = doubleWinAmount = "0";
            tripleWinValueText.text = tripleWinAmount = "0";
        }

        ResetPlayValue();
    }

    public void UpdateTimer(float timeValue)
    {
        Debug.LogError("Updating Timer :" + timeValue);
        if (timeValue <= 5)
        {
            CloseInfo();
            //
            Debug.Log("Disabling all buttons");
            bInfo.interactable = false;
            bClear.interactable = false;
            bDoubleUp.interactable = false;
            bRepeat.interactable = false;
        }
        if (timeValue <= 0)
        {
            Debug.Log("Timer complete");
            timerScript.StopTimer();
            timerScript.BlockAllButton();
            if (Constant.GameStatus == ErrorCode.RUNNING)
            {
                StartSpinning();
            }
            else
            {
                UIControllerTri.instance.MessagePopUp();
                //StopAllWheel();
            }
        }
        else
        {
            timerScript.StartTimer(timeValue);
            // bInfo.interactable = true;
            // bClear.interactable = true;
            // bDoubleUp.interactable = true;
            // bRepeat.interactable = true;
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }


    public void ResetAllData()
    {
        if (UIControllerTri.instance.messagePopUp != null)
        {
            Destroy(UIControllerTri.instance.messagePopUp);
        }
        // commented out by somnath
        //lstBlockDataForRepeat = new List<BlockData>();
        //lstBlockDataForRepeat.Clear();
        //lstBlockDataForRepeat.Clear();
        Debug.LogError("lstAllBlockData.Count " + lstAllBlockData.Count);
        if (lstAllBlockData.Count > 0)
        {
            lstBlockDataForRepeat.Clear();
        }
        for (int i = 0; i < lstAllBlockData.Count; i++)
        {
            BlockData _blockData = new BlockData(lstAllBlockData[i].block, lstAllBlockData[i].game_number, lstAllBlockData[i].game_amount);
            lstBlockDataForRepeat.Add(_blockData);
        }
        if (leftRandomClick != null)
        {
            leftRandomClick.DeselectAllButton();
        }
        if (rightRandomClick != null)
        {
            rightRandomClick.DeselectAllButton();
        }
        //#if UNITY_ANDROID
        //        if (lstBlockDataForRepeat.Count > 0)
        //        {
        //            bRepeat.gameObject.SetActive((lstBlockDataForRepeat.Count > 0));
        //            bDoubleUp.gameObject.SetActive(!(lstBlockDataForRepeat.Count > 0));
        //        }
        //       bRemove.interactable = false;
        //#endif
        Debug.Log("Reset All data started");
        ClearAllBlock();
        for (int i = 0; i < lstResultBlock.Count; i++)
        {
            lstResultBlock[i].OnDeselectSuccess(0);
        }
        bRepeat.interactable = ((lstBlockDataForRepeat.Count > 0));
        /*  if(lstBlockDataForRepeat.Count > 0)
          else
               bRepeat.interactable = false;*/
        gWinData.SetActive(false);
        tPlayValue.text = 0.ToString();
        // totalbetamountget= Convert.ToDouble(tPlayValue.text);;
        // Debug.Log("tplay valu5566:"+ totalbetamountget);
        //OnStartAllWheel();
        // ShowMessage("Place your chip");// close by prabir

        //added by shivamfusion07
        ResetPlayValue();
        Debug.Log("play value rest from here shivammmmmm");
        if (singleWinValueText != null)
        {
            singleWinValueText.text = singleWinAmount = "0";
            doubleWinValueText.text = doubleWinAmount = "0";
            tripleWinValueText.text = tripleWinAmount = "0";
        }

        // DisableWinPanel();
        gJewel.GetComponent<Animator>().SetBool("blink", false);
        tabParent.ResetAllTabs();
        Debug.Log("Reset All Data finished");
        playValue = 0;
        // timerScript.StartTimer(90);// close by prabir
        // OnStartAllWheel();
    }


    void ResetPlayValue()
    {
        for (int i = 0; i < allBlockDetails.Count; i++)
        {
            allBlockDetails[i].tPlayValue.text = "PLAY : 0";
            Debug.Log("play value:" + allBlockDetails[i].tPlayValue.text);
            allBlockDetails[i].tWinValue.text = "WIN : 0";

            allBlockDetails[i].tPlayBG.sprite = sNormalBGForPlay;
            allBlockDetails[i].tWinBG.sprite = sNormalBGForPlay;
        }
        Debug.Log("Reset All Data Reset Play Value");
#if UNITY_ANDROID
        singlePlayValue = 0;
        singlePlayText.text = singlePlayValue.ToString();
        doublePlayValue = 0;
        doublePlayText.text = doublePlayValue.ToString();
        triplePlayValue = 0;
        TriplePlayText.text = triplePlayValue.ToString();
#endif
    }
    void OnSelectBlock(Transform _trans)
    {
        if (timerScript.timeLeft > 6f)
        {
            if (IsRemoveClicked)
            {
                OnDeSelectBlock(_trans.GetComponent<Block>());
                if (lstAllBlockData.Count == 0)
                    IsRemoveClicked = false;
            }
            else
            {
                //SoundController.instance.PlayAudio(SoundController.ClipType.BLOCK);
                Block _block = _trans.GetComponent<Block>();
                int _amount = Constant.GetLimitedBetAmount(_block.blockType, currentSelectedChip);
                Debug.Log("Bet Amount : " + (_block.iBetAmount + currentSelectedChip));

                if (currentSelectedChip > _amount || (_block.iBetAmount + currentSelectedChip) > _block.limit)
                {

                    return;
                }
                if (CheckBalance(_amount))
                {

                    if (Constant.GetMaximumBetAmount(_block.blockType) >= (_block.iBetAmount + _amount))
                    {
                        IPointBalance -= _amount;
                        AddDataToList(_trans, _amount);
                        EnablePopupForBlock(_trans);

                    }
                    else
                    {
                        EnablePopUpForInsufficient(_trans, _trans.GetComponent<Block>().blockType, PopUp.ERROR.Maximum_Bet);
                        ShowMessage("Maximum Bet Limit Exceed");
                        ShowAlertPanel();
                    }
                }
                else
                {
                    EnablePopUpForInsufficient(_trans, _trans.GetComponent<Block>().blockType, PopUp.ERROR.Insufficient_Balance);
                    ShowMessage("Not enough points,Top up your account");
                    ShowAlertPanel();
                }
            }

        }

    }
    public void ShowAlertPanel()
    {
#if UNITY_ANDROID

        alertPanel?.SetActive(true);
#endif
    }


    void OnDeSelectBlock(Block _block)
    {
        //SoundController.instance.PlayAudio(SoundController.ClipType.BUTTON);
#if !UNITY_ANDROID
        clickPopUp[0].gameObject.SetActive(false);
#endif
        BlockData _blockData = lstAllBlockData.Find((BlockData obj) => (obj.game_number == _block.normalStateNum.text));
        if (_blockData != null)
        {

            AddToBalance(_blockData.allGameAmount.Last());
            bool _isLast = _blockData.RemoveLastAmount();
            _block.OnDeselectSuccess(_blockData.game_amount);
            if (_isLast)
            {
                RemoveBlockData(_blockData, _block.blockType, false);
            }
            else
            {
                EnablePopupForBlock(_block.transform);
#if UNITY_ANDROID
                int playValue = GetPlayValueFromBlock(_block.blockType);
               // Debug.Log("After Remove : " + playValue);
                allBlockDetails[(int)_block.blockType].tPlayValue.text = "PLAY : " + playValue;
                if(playValue == 0)
                    allBlockDetails[(int)_block.blockType].tPlayBG.sprite = sNormalBGForPlay;
                else
                    allBlockDetails[(int)_block.blockType].tPlayBG.sprite = sSelectedBGForPlay;
#endif
            }

            // RemoveBlockData(_blockData);
        }

        if (lstAllBlockData.Count == 0)
            IsRemoveClicked = false;

    }

    public void OnSelectFromRowColoum(Transform _trans)
    {
        Block _block = _trans.GetComponent<Block>();
        int _amount = Constant.GetLimitedBetAmount(_block.blockType, currentSelectedChip);

        if (CheckBalance(_amount) && Constant.GetMaximumBetAmount(_block.blockType) >= (_block.iBetAmount + _amount))
        {
            IPointBalance -= _amount;
            AddDataToList(_trans, _amount);

        }
        else
        {

        }
    }


    public void AddDataToList(Transform _trans, int _iExtraAmount)
    {
        Block _block = _trans.GetComponent<Block>();
        BlockData _blockData = lstAllBlockData.Find((BlockData obj) => (obj.game_number == _block.normalStateNum.text));
        if (_blockData != null)
        {
            _blockData.game_amount += _iExtraAmount;
            _blockData.allGameAmount.Add(_iExtraAmount);
            AllPlayAmountSet(_block.blockType, _iExtraAmount);
        }
        else
        {
            _blockData = new BlockData(_block, _block.normalStateNum.text, _iExtraAmount);
            lstAllBlockData.Add(_blockData);
            bClear.interactable = true;
            Debug.Log("clear buton still activated");
            bDoubleUp.interactable = true;
#if UNITY_ANDROID
            //bRemove.interactable = true;
            AllPlayAmountSet(_block.blockType, _iExtraAmount);
#endif
        }

        clickPopUp[0].GetComponent<PopUp>().SetBlockData(_block.blockType, _block.normalStateNum.text, _blockData.game_amount);
        //Block.BlockType type = _trans.GetComponent<Block>().blockType;
        //allBlockDetails[(int)type].tPlayValue.text = "PLAY : " + GetPlayValueFromBlock(type);
        //allBlockDetails[(int)_block.blockType].tPlayBG.sprite = sSelectedBGForPlay;

        Debug.Log("custom: " + _blockData.game_amount);
        _trans.GetComponent<Block>().OnSelectSuccess(_blockData.game_amount);
        bRepeat.interactable = false;
        //#if UNITY_ANDROID
        //        bRepeat.gameObject.SetActive(false);
        //#endif
        //bDoubleUp.gameObject.SetActive(true);
    }

    public void AddDataToListForDoubleUp(BlockData _blockData)
    {
        int _iExtraAmount = _blockData.game_amount;
        _blockData.game_amount += _iExtraAmount;
        _blockData.allGameAmount.Add(_iExtraAmount);
        AllPlayAmountSet(_blockData.block.blockType, _iExtraAmount);
        if ((currentSelectedTab.name[0] == _blockData.game_number[0] && _blockData.block.blockType == Block.BlockType.TRIPLE)
            || (_blockData.block.blockType == Block.BlockType.DOUBLE)
             || (_blockData.block.blockType == Block.BlockType.SINGLE))

            _blockData.block.OnSelectSuccess(_blockData.game_amount);


        //#if UNITY_ANDROID
        //        Block.BlockType type = _blockData.block.blockType;
        //        allBlockDetails[(int)type].tPlayValue.text = "PLAY : " + GetPlayValueFromBlock(type);
        //        allBlockDetails[(int)_blockData.block.blockType].tPlayBG.sprite = sSelectedBGForPlay;
        //#endif

    }

    public void AddDataToListForRepeat(BlockData _perviousBlockData, int _iExtraAmount)
    {
        BlockData _blockData = lstAllBlockData.Find((BlockData obj) => (obj == _perviousBlockData));
        if (_blockData != null)
        {
            _blockData.game_amount += _iExtraAmount;
            _blockData.allGameAmount.Add(_iExtraAmount);
        }
        else
        {
            _blockData = new BlockData(_perviousBlockData.block, _perviousBlockData.game_number, _iExtraAmount);
            lstAllBlockData.Add(_blockData);
            bClear.interactable = true;
            bDoubleUp.interactable = true;
#if UNITY_ANDROID
            //bRemove.interactable = true;
#endif
        }
        if ((currentSelectedTab.name[0] == _blockData.game_number[0] && _blockData.block.blockType == Block.BlockType.TRIPLE)
            || (_blockData.block.blockType == Block.BlockType.DOUBLE)
             || (_blockData.block.blockType == Block.BlockType.SINGLE))
            _blockData.block.OnSelectSuccess(_blockData.game_amount);

        if (_blockData.block.blockType == Block.BlockType.TRIPLE)
        {
            int _index = int.Parse(_blockData.game_number[0].ToString());
            lstAllTab[_index].AddData(_blockData);
            lstAllTab[_index].Select();
        }

        //#if UNITY_ANDROID
        //        Block.BlockType type = _blockData.block.blockType;
        //        allBlockDetails[(int)type].tPlayValue.text = "PLAY : " + GetPlayValueFromBlock(type);
        //        allBlockDetails[(int)_blockData.block.blockType].tPlayBG.sprite = sSelectedBGForPlay;
        //#endif
        bRepeat.interactable = false;

    }

    public List<BlockData> GetAllBlockDataForTab(string name)
    {
        List<BlockData> _allBlockData = new List<BlockData>();
        _allBlockData = lstAllBlockData.FindAll((BlockData obj) => (obj.game_number[0] == name[0] && obj.block.blockType == Block.BlockType.TRIPLE));
        return _allBlockData;
    }
    void RemovePreviousSelectedBlock(Block.BlockType _blockType)
    {
        List<BlockData> _blockData = lstAllBlockData.FindAll((BlockData obj) => (obj.block.blockType == _blockType));
        for (int i = 0; i < _blockData.Count; i++)
        {
            _blockData[i].block.OnDeselectSuccess(0);
            RemoveBlockData(_blockData[i], _blockType);
        }
    }

    public void RemovePreviousSelectedTripleBlock(string _sTabNumber)
    {
        List<BlockData> _blockData = lstAllBlockData.FindAll((BlockData obj) => ((obj.block.blockType == Block.BlockType.TRIPLE) && obj.game_number[0] == _sTabNumber[0]));

        for (int i = 0; i < _blockData.Count; i++)
        {
            _blockData[i].block.OnDeselectSuccess(0);
            RemoveBlockData(_blockData[i], Block.BlockType.TRIPLE);
        }
    }

    int GetPlayValueFromBlock(Block.BlockType _blockType)
    {
        int _iPlayValue = 0;
        List<BlockData> _blockData = lstAllBlockData.FindAll((BlockData obj) => (obj.block.blockType == _blockType));
        for (int i = 0; i < _blockData.Count; i++)
        {
            _iPlayValue += _blockData[i].game_amount;
        }

        return _iPlayValue;
    }


    void RemoveBlockData(BlockData _blockData, Block.BlockType _blockType, bool _deductbalance = true)
    {
        if (_deductbalance)
            AddToBalance(_blockData.game_amount);
        lstAllBlockData.Remove(_blockData);
        if (lstAllBlockData.Count == 0)
        {
            bClear.interactable = false;
            bDoubleUp.interactable = false;
            bRepeat.interactable |= lstBlockDataForRepeat != null && lstBlockDataForRepeat.Count > 0;

            //#if UNITY_ANDROID
            //            if(bRepeat.interactable)
            //            {
            //                bRepeat.gameObject.SetActive(true);
            //                bDoubleUp.gameObject.SetActive(false);
            //            }

            //#endif
        }

        //#if UNITY_ANDROID
        //        int playValue = GetPlayValueFromBlock(_blockType);
        //        allBlockDetails[(int)_blockType].tPlayValue.text = "PLAY : " + playValue;

        //        if (playValue == 0)
        //            allBlockDetails[(int)_blockType].tPlayBG.sprite = sNormalBGForPlay;
        //        else
        //            allBlockDetails[(int)_blockType].tPlayBG.sprite = sSelectedBGForPlay;
        //#endif
    }


    int SetPositionOfPopUp(Transform _trans, bool onWin = false)
    {
        int index = 0;

        if (onWin)
        {
            foreach (var item in clickPopUp)
            {
                if (!item.gameObject.activeSelf)
                {
                    break;
                }
                index++;
            }
        }

        Debug.Log(index);
        clickPopUp[index].transform.SetParent(_trans);
        // clickPopUp.transform.localScale = new Vector3(0.7f, 0.7f,0.7f);
        clickPopUp[index].anchoredPosition = Vector3.one;
        clickPopUp[index].transform.SetParent(this.transform.GetChild(0));

        clickPopUp[index].gameObject.SetActive(true);
        return index;

    }

    public void EnablePopupForBlock(Transform _trans,bool win=false)
    {

#if !UNITY_ANDROID
        int popup=SetPositionOfPopUp(_trans,win);

        Block _block = _trans.GetComponent<Block>();
        clickPopUp[popup].GetComponent<PopUp>().SetBlockData(_block.blockType, _block.normalStateNum.text, _block.iBetAmount);
#endif
    }


    public void EnablePopUpForRowColoum(Transform _trans, string _betAmount)
    {

#if !UNITY_ANDROID
        int popup=SetPositionOfPopUp(_trans);
        clickPopUp[popup].GetComponent<PopUp>().SetBlockDataForRow(_trans.GetComponent<RowColumPickBlock>().blockType, _betAmount);
#endif
    }


    public void EnablePopUpForInsufficient(Transform _trans, Block.BlockType _blockType, PopUp.ERROR _error)
    {
#if !UNITY_ANDROID
        int popup=SetPositionOfPopUp(_trans);
        clickPopUp[popup].GetComponent<PopUp>().ShowInsufficientPopUp(_blockType, _error);
        // alertPanel.SetActive(true);
#else
        //// Show Insufficient popup
        //gNotEnoughCoin.SetActive(true);
        //string _msg = "";
        //if(_error == PopUp.ERROR.Insufficient_Balance)
        //{
        //    _msg = "Not enough points please top up your account or contact adminstrator";
        //}
        //else
        //{
        //    _msg = "Maximum bet limit exceed";
        //}

        //gNotEnoughCoin.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = _msg;

#endif
    }

    public void DisablePopUp()
    {
#if !UNITY_ANDROID
        foreach (var item in clickPopUp)
        {
            item.gameObject.SetActive(false);
        }
        // clickPopUp.gameObject.SetActive(false);
#endif
    }

    public void DeSelectAllChip()
    {
        for (int i = 0; i < lstAllChip.Count; i++)
        {
            lstAllChip[i].EnableDisable(false);
        }
    }

    public void SelectChip(Chiptri chip, string chipName)
    {
        lastSelectedChip = chip;
        currentSelectedChip = int.Parse(chipName);
        Debug.LogError("currentSelectedChip " + currentSelectedChip);
        IsRemoveClicked = false;

#if UNITY_ANDROID
        if (lstAllBlockData.Count > 0)
        {
           //bRemove.interactable = true;
        }
#endif

    }

    public void DeSelectAllTab()
    {
        for (int i = 0; i < lstAllTab.Count; i++)
        {
            lstAllTab[i].DeSelect();
        }
    }


    public bool CheckBalance(float _iAmount)
    {
        if (IPointBalance - (_iAmount) >= 0)
        {
            return true;
        }
        return false;

    }



    public void AddToBalance(int _iBalance)
    {
        IPointBalance += _iBalance;
    }

    public float startSpinningTime = 0;

    public void StartSpinning()
    {
        Debug.Log("Start Spinning");
        startSpinningTime = Time.time;
        tResultNumber.text = "";
        multiplierText.text = "";
        ResultP?.SetActive(false);
        if (Logo)
        {
            Logo?.SetActive(false);
        }
        if (middleAnimation)
        {
            middleAnimation?.SetActive(true);//prabir
        }
        if (Audio_Manager.instance && wheelSound != null)
        {
            Audio_Manager.instance?.PlayAudio(wheelSound);
        }
        //SoundController.instance.StartAndStopWheelSpin (true);

        //Commented out by somnath
        //for (int i = 0; i < lstAllWheel.Count; i++)
        //{
        //    // lstAllWheel[i].FindNumberInArray( int.Parse(sResultNumber[i].ToString()));
        //    lstAllWheel[i].RotateWheelInfinitely();
        //}
        OnStartAllWheel();
        for (int i = 0; i < listAllNewWheels.Count; i++)
        {
            listAllNewWheels[i].spinWheel.SpinTheWheel();
        }
        firstWheelStop = secondWhelStop = false;
        CloseInfo();
        SetresultData();//prabir

    }


    public void ShowMessage(string _sMessage)
    {
        messageBoxText.text = _sMessage;

        aMessageBoxOverlay.Play();
    }



    public void ShowResultBlock()
    {
        //  if (!isbetclicked) 
        //     return;  // Exit if the bet button was not clicked
        Debug.LogError("Win Amount : " + Constant.WinAmount);
        gWinData.SetActive(true);
        tWinAmountInWinPanel.text = Constant.WinAmount;
        if (Constant.WinAmount.Length > 0 && double.Parse(Constant.WinAmount) > 0)
        {
            tWinAmount.text = Constant.WinAmount;
            // IPointBalance += double.Parse(Constant.WinAmount);
            iPointBalance += double.Parse(Constant.WinAmount);
            tPointBalance.text = iPointBalance.ToString("#0.00");
            if (UserInfoPersist.Instance != null && UserInfoPersist.Instance.userInfo != null)
            {
                UserInfoPersist.Instance.userInfo.balance = iPointBalance.ToString("#0.00");
            }
            // tWinAmountInWinPanel.text = Constant.WinAmount;
            Audio_Manager.instance.PlayAudio(winAudioClip);
            //EnableWinPanel();

            //ResetWinningCount();//prabir
        }
        if (playValue != 0)
        {
            //GameHistoryElement element = Instantiate(gameHistoryElement, gameHistory);//by sayam
            //element.SetParameter(Constant.CurrentGameSession, (firstPointBalance - IPointBalance), double.Parse(Constant.WinAmount));//by sayam

            totalPlayValue += playValue;
            totalWinValue += double.Parse(Constant.WinAmount);

            txtTotalPlay.text = totalPlayValue.ToString();
            txtTotalWin.text = totalWinValue.ToString();
        }



        // resultBox.UpdateResult();//by sayam
        tWinAmount.text = Constant.WinAmount;
        // Debug.Log("Result : " + Constant.ResultNumber);
        int _iIndex = int.Parse(Constant.ResultNumber.Substring(0, 1));
        tabParent.EnableATab(_iIndex);
        tResultNumber.text = allDatas.tripleData.card;
        //PlayBlastAnimation();
        //closed by sayam
        /*if (UIControllerTri.instance.sAllResult[0].multiplier != "1")
        {
            //Sprite Animation
            PlayBlastAnimation();
            if (UIControllerTri.instance.sAllResult[0].multiplier == "10")
            {
                multiplierText.text = "B";
                if (gWinData.activeInHierarchy)
                    bumperEffect.Play();
            }
            else
                multiplierText.text = UIControllerTri.instance.sAllResult[0].multiplier + "X";
        }*/

        if (double.Parse(Constant.WinAmount) >= 900)// && (UIControllerTri.instance.sAllResult[0].multiplier != "10"))
        {
            Debug.Log("Win amount Greater than 900" + Constant.WinAmount);
            ShowMessage("You Win");
            StartCoroutine(winEffect(5f));
        }

        //  coinEffectForLessWin.Play();
        if (onResultSuccess != null)
        {
            onResultSuccess(Constant.ResultNumber);
        }
        //viewBalance();
        // Invoke("ResetAllData", 10f);
        isbetclicked = false;
    }

    public void testWinEffect(float fadeOutDuration)
    {
        StartCoroutine(winEffect(fadeOutDuration));
    }

    private IEnumerator winEffect(float fadeOutDuration)
    {
        winAnimationGameObject.SetActive(true);
        // Animator animator;
        // float waitDuration = 0.0f;
        // animator = winAnimationGameObject.GetComponent<Animator>();
        // if (animator != null)
        //{
        //  AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        //     if (clipInfo.Length > 0)
        //     {

        //         waitDuration = (clipInfo[0].clip.length / animator.GetFloat("AnimationSpeed")) - fadeOutDuration;
        //         Debug.LogWarning("FadeOutDuration" + waitDuration);
        //     }
        // }
        // if (waitDuration > 0.0f)
        //     yield return new WaitForSeconds(waitDuration);

        // Image image = winAnimationGameObject.GetComponent<Image>();
        // if (image != null)
        //{
        //  image.DOFade(0, fadeOutDuration).OnComplete(() =>
        // {
        yield return new WaitForSeconds(7f);
        winAnimationGameObject.gameObject.SetActive(false);
        // Color color = image.color;
        // color.a = 1.0f;
        //  image.color = color;
        //         });
        //     }
        //     else
        //     {
        //         Debug.LogError("Image Component not found on Win Animation");
        //     }
        // }
    }

    void EnableWinPanel()
    {
        //gWinPanel.SetActive(true);
        //SoundController.instance.PlayAndStopWinSound(true);
        Invoke("DisableWinPanel", 8f);
    }
    void DisableWinPanel()
    {
        bumperEffect.gameObject.SetActive(false);
        //gWinPanel.SetActive(false);
        //Constant.PointBalance = winningvalue + currectpPointvalue;
        //firstPointBalance = Constant.PointBalance;
        //IPointBalance = Constant.PointBalance;
    }
    #region Block Content For Android

    public float speed = 100;

    public bool isOpen = false;
    private bool isMoving = false;
    private int previousIndex;
    private int afterIndex;
    private int currentIndex;

    public List<BlockDetails> allBlockDetails;
    public Sprite sOpenArrow;
    public Sprite sCloseArrow;
    float fAdditionalAmount;

    public void BlockPanelClicked(int index)
    {
        if (!isMoving)
        {
            afterIndex = -1;
            if (isOpen && index != previousIndex)
            {
                afterIndex = index;
                Move(previousIndex);
            }
            else
            {
                previousIndex = index;
                Move(index);
            }


        }
    }

    private void Move(int i)
    {
        currentIndex = i;
        float endPosX = 0;
        RectTransform _rectTransform = null;

        _rectTransform = allBlockDetails[i].rectTransform;
        if (!isOpen)
        {
            endPosX = allBlockDetails[i].fOpenPos;
            allBlockDetails[i].arrow.sprite = sOpenArrow;
        }
        else
        {
            endPosX = allBlockDetails[i].fClosePos;
            allBlockDetails[i].arrow.sprite = sCloseArrow;
        }
        isOpen = !isOpen;

        fAdditionalAmount = allBlockDetails[i].fAdditionValueForPanel;
        StartCoroutine(MovePanel(_rectTransform, endPosX, allBlockDetails[i].fAdditionValueForPanel));
    }

    IEnumerator MovePanel(RectTransform _rectTransform, float endPosX, float fAdditionalAmount)
    {
        isMoving = true;
        Vector2 pos = allBlockDetails[currentIndex].rBlockPanel.anchoredPosition;
        if (isOpen)
        {
            pos.x += fAdditionalAmount;
        }

        allBlockDetails[currentIndex].rBlockPanel.anchoredPosition = pos;

        Vector3 endPos = _rectTransform.anchoredPosition;
        endPos.x = endPosX;

        yield return null;
        _rectTransform.DOAnchorPosX(endPosX, 0.35f).SetEase(Ease.OutQuad).OnComplete(() => { OnCompleteMove(); });
    }

    void OnCompleteMove()
    {
        Vector3 pos = allBlockDetails[currentIndex].rBlockPanel.anchoredPosition;
        if (!isOpen)
            pos.x -= fAdditionalAmount;
        allBlockDetails[currentIndex].rBlockPanel.anchoredPosition = pos;

        isMoving = false;

        if (afterIndex != -1)
        {
            previousIndex = afterIndex;
            Move(afterIndex);
            afterIndex = -1;
        }
    }

    public void ShowWinAmount(Block.BlockType _blockType, string _number, int _iAmount)
    {
        string winValue = "";
        if (_blockType == Block.BlockType.SINGLE)
        {
            winValue = singleWinAmount;
            if (singleWinValueText != null)
                singleWinValueText.text = singleWinAmount;
        }
        else if (_blockType == Block.BlockType.DOUBLE)
        {
            winValue = doubleWinAmount;
            if (doubleWinValueText != null)
                doubleWinValueText.text = doubleWinAmount;
        }
        else if (_blockType == Block.BlockType.TRIPLE)
        {
            winValue = tripleWinAmount;
            if (tripleWinValueText != null)
                tripleWinValueText.text = tripleWinAmount;
        }
        allBlockDetails[(int)_blockType].tWinValue.text = "WIN :" + winValue; // Constant.GetWinValue(_blockType, _number, _iAmount)
        allBlockDetails[(int)_blockType].tWinBG.sprite = sSelectedBGForPlay;
    }
    #endregion
    #region UI Button Function

    public void Close()
    {
        UIControllerTri.instance.TransitionTo(PageType.LOBBY);
    }

    public void ClearClicked()
    {
        //SoundController.instance.PlayAudio (SoundController.ClipType.CHIP);
        for (int i = 0; i < lstAllBlockData.Count; i++)
        {
            AddToBalance(lstAllBlockData[i].game_amount);
            lstAllBlockData[i].block.OnDeselectSuccess(0);
        }
        allDatas.ClearAllData();
        foreach (var tab in lstAllTab)
        {
            tab.TurnSelectedTabGreen(false);
            tab.ResetTab();
        }
        lstAllBlockData.Clear();
        bClear.interactable = false;
        bDoubleUp.interactable = false;
        //#if UNITY_ANDROID
        //        //bRemove.interactable = false;
        //        if (lstBlockDataForRepeat!= null && lstBlockDataForRepeat.Count > 0)
        //        {
        //            bRepeat.gameObject.SetActive(true);
        //            bDoubleUp.gameObject.SetActive(false);
        //        }
        //#endif

        if (onClearClicked != null)
            onClearClicked();

        bRepeat.interactable |= (lstBlockDataForRepeat != null && lstBlockDataForRepeat.Count > 0);

        ResetPlayValue();
        playValue = 0;
    }
    public void ClearClickedfrprinttriplechance()
    {
        Debug.Log("clear clicked for print works");
        //SoundController.instance.PlayAudio (SoundController.ClipType.CHIP);
        for (int i = 0; i < lstAllBlockData.Count; i++)
        {
            //  AddToBalance(lstAllBlockData[i].game_amount);
            lstAllBlockData[i].block.OnDeselectSuccess(0);
        }
        Debug.Log("bet clicked true by bet button click play value reset...");
        bRepeat.interactable |= (lstBlockDataForRepeat != null && lstBlockDataForRepeat.Count > 0);
        ResetPlayValue();
        playValue = 0;
        tPlayValue.text = playValue.ToString();
        Debug.Log("tplay valuee77:" + tPlayValue.text);
        allDatas.ClearAllData();
        /* foreach (var tab in lstAllTab)
         {
             tab.TurnSelectedTabGreen(false);
             tab.ResetTab();
         }*/
        lstAllBlockData.Clear();
        bClear.interactable = false;
        bDoubleUp.interactable = false;
        //#if UNITY_ANDROID
        //        //bRemove.interactable = false;
        //        if (lstBlockDataForRepeat!= null && lstBlockDataForRepeat.Count > 0)
        //        {
        //            bRepeat.gameObject.SetActive(true);
        //            bDoubleUp.gameObject.SetActive(false);
        //        }
        //#endif

        /*  if (onClearClicked != null)
              onClearClicked();

          bRepeat.interactable |= (lstBlockDataForRepeat != null && lstBlockDataForRepeat.Count > 0);

          ResetPlayValue();
          playValue = 0;
           tPlayValue.text = playValue.ToString();
           Debug.Log("tplay valuee77:"+tPlayValue.text);*/
    }



    void ClearAllBlock()
    {
        for (int i = 0; i < lstAllBlockData.Count; i++)
        {
            lstAllBlockData[i].block.OnDeselectSuccess(0);
            lstAllBlockData[i].block.ResetWinEffect();
        }

        Debug.Log("Reset All Data Clear All Block");

        lstAllBlockData.Clear();
        bClear.interactable = false;
        bDoubleUp.interactable = false;

        if (onClearClicked != null)
            onClearClicked();

        bRepeat.interactable |= (lstBlockDataForRepeat != null && lstBlockDataForRepeat.Count > 0);
    }

    public void DoubleUpClicked()
    {
        //SoundController.instance.PlayAudio (SoundController.ClipType.CHIP);
        for (int i = 0; i < lstAllBlockData.Count; i++)
        {
            if (Constant.IsEligibleForDoubleUp(lstAllBlockData[i].block.blockType, lstAllBlockData[i].game_amount))
            {
                if (CheckBalance(lstAllBlockData[i].game_amount))
                {
                    IPointBalance -= (lstAllBlockData[i].game_amount);

                    AddDataToListForDoubleUp(lstAllBlockData[i]);
                }
                else
                {
                    ShowMessage("Not enough points,Top up your Account");
                }
            }
            else
            {
                ShowMessage("Maximum Bet Limit Reached");
            }
        }
    }


    public void RepeatClicked()
    {
        Debug.Log("firstPointBalance" + firstPointBalance + " IPointBalance " + IPointBalance);
        //SoundController.instance.PlayAudio (SoundController.ClipType.CHIP);
        int lastBlockTab = -1;
        for (int i = 0; i < lstBlockDataForRepeat.Count; i++)
        {
            Block block = lstBlockDataForRepeat[i].block;
            int _iAmount = lstBlockDataForRepeat[i].game_amount;
            if (block.blockType == BlockType.TRIPLE)
            {
                Debug.Log("Repeat Block : " + lstBlockDataForRepeat[i].game_number.ToString());
                for (int j = 0; j < lstAllTab.Count; j++)
                {
                    if (lstBlockDataForRepeat[i].game_number[0] == lstAllTab[j].gameObject.name[0])
                    {
                        Debug.Log("Block Normal State text 0 : " + block.normalStateNum.text[0] + ",Game Object Name 0 : " + lstAllTab[j].gameObject.name[0]);
                        lstAllTab[j].TurnSelectedTabGreenRepeat(true);
                        //Select The last Tab
                        if (i == lstBlockDataForRepeat.Count - 1)
                        {
                            lastBlockTab = j;
                        }
                    }
                }
            }
            Transform _trans = lstBlockDataForRepeat[i].block.transform;
            if (IPointBalance - (_iAmount) >= 0)
            {
                IPointBalance -= _iAmount;
                OnBetData(lstBlockDataForRepeat[i].block.blockType, _iAmount, lstBlockDataForRepeat[i].game_number);
                AddDataToListForRepeat(lstBlockDataForRepeat[i], _iAmount);
            }
            else
            {
                ShowMessage("Not enough points and top up your account");
            }
        }
        if (lastBlockTab >= 0)
        {
            Debug.Log("Activating Last Tab" + lstAllTab[lastBlockTab].gameObject.name);
            lstAllTab[lastBlockTab].OnClick(false);
            lstAllTab[lastBlockTab].gameObject.transform.GetChild(0).SetActive(true);
        }
#if UNITY_ANDROID
        //bRepeat.gameObject.SetActive(false);
        //bDoubleUp.gameObject.SetActive(true);
#else
        bRepeat.interactable = false;
#endif
    }

    public void InfoClicked()
    {
        if (clickPopUp != null)
            clickPopUp[0].gameObject.SetActive(false);
        //SoundController.instance.PlayAudio (SoundController.ClipType.CHIP);
        gInfoPanel.SetActive(true);
    }

    public void CloseInfo()
    {
        // SoundController.instance.PlayAudio(SoundController.ClipType.BUTTON);
        gInfoPanel.SetActive(false);


    }

    public void RemoveClicked()
    {
        //SoundController.instance.PlayAudio (SoundController.ClipType.CHIP);
        IsRemoveClicked = true;
        // bRemove.interactable = false;

        DeSelectAllChip();
    }

    public override void YesClicked()
    {
        base.YesClicked();
        StopAllCoroutines();
        UIControllerTri.instance.TransitionTo(PageType.LOBBY);
    }
    #endregion

    #region Info Section

    public List<Image> selectedTabImage;
    public List<GameObject> allTabContent;

    public double totalPlayValue;
    public double totalWinValue;

    public Text txtTotalPlay;
    public Text txtTotalWin;

    public Transform gameHistory;
    public GameHistoryElement gameHistoryElement;

    public void TabInInfoClicked(int index)
    {
        //SoundController.instance.PlayAudio(SoundController.ClipType.BUTTON);
        for (int i = 0; i < 3; i++)
        {
            selectedTabImage[i].enabled = false;
            allTabContent[i].SetActive(false);
        }

        selectedTabImage[index].enabled = true;
        allTabContent[index].SetActive(true);
    }

    #endregion

    #region All API

    public void SendGameData()
    {
        delayBetweenResultCallback = Time.time;
        //  Debug.Log(timerScript.timeLeft + " send ");
        if (clickPopUp != null)
            clickPopUp[0].gameObject.SetActive(false);
#if UNITY_ANDROID
        // To close Panel if it is opened

        if (isOpen)
        {
            if (isMoving)
            {
               // Debug.Log("is moving true");
                isMoving = false;
            }
            BlockPanelClicked(previousIndex);
        }

        gNotEnoughCoin.SetActive(false);
#endif
        Debug.LogError(" Data submit time " + Time.time + " :::: " + HrtzzJsonUtilityHelper.ToJson(lstAllBlockData.ToArray(), Constant.CurrentGameSession, true));
        //File.WriteAllText("C:/Users/Redapple083/Desktop/data.text", HrtzzJsonUtilityHelper.ToJson(lstAllBlockData.ToArray(), Constant.CurrentGameSession, Constant.CurrentDeviceType).ToString());
        GetComponent<AudioSource>().Play();
        ShowMessage("No More Play");
        Web.Create()
           .SetUrl(Constant.AddGameDataURL, Web.RequestType.POST, Web.ResponseType.TEXT)
             .AddPostData(HrtzzJsonUtilityHelper.ToJson(lstAllBlockData.ToArray(), Constant.CurrentGameSession, true))
             .AddHeader("Content-Type", "application/json")
             .AddHeader(Constant.AccessToken, Constant.CurrentAccessToken)
                    .SetOnSuccessDelegate((Web _web, Response _response) =>
                    {
                        Debug.LogError("Success " + _response.GetText());
                        Debug.Log("Data submit success " + Time.time);
                        JSONNode _jsonNode = JSON.Parse(_response.GetText());
                        if (_jsonNode["status"].Value == "1")
                        {
                            if (lstBlockDataForRepeat.Count > 0 && lstAllBlockData.Count > 0)
                            {
                                Debug.Log("Clearing Data" + lstBlockDataForRepeat.Count + "   " + lstAllBlockData.Count);
                                lstBlockDataForRepeat.Clear();
                            }
                        }
                        else
                        {
                            InternetStatus(false);
                            //UIControllerTri.instance.NoInterNetPopUp();
                        }

                        _web.Close();
                    })
                    .SetOnFailureDelegate((Web _web, Response _response) =>
                    {
                        Debug.Log("Found Error " + _response.GetError());
                        InternetStatus(false);
                        //UIControllerTri.instance.NoInterNetPopUp();
                        _web.Close();
                    })
                    .Connect();


    }


    public string result;
    public void SetresultData()
    {

        long singlewin = 0;
        long doublewin = 0;
        long triplewin = 0;
        int singleNo = 0;
        int doubleNO = 0;
        int tripleNo = 0;

        //if (playValue > 0)//prabir
        //{
        //    if (WinningType.Equals("High"))
        //    {
        //        Debug.LogError("High");
        //        allDatas.FindLargestPossible();
        //        singleNo = Convert.ToInt32(allDatas.tripleData.key[2]);
        //        doubleNO = Convert.ToInt32(allDatas.tripleData.key[1]);
        //        tripleNo = Convert.ToInt32(allDatas.tripleData.key[0]);
        //        numbers = allDatas.tripleData.key;
        //        singleWinValueText.text = allDatas.tripleData.key.Substring(2);
        //        doubleWinValueText.text = allDatas.tripleData.key.Substring(1);
        //        tripleWinValueText.text = allDatas.tripleData.key;
        //    }
        //    else if (WinningType.Equals("Low"))
        //    {
        //        Debug.LogError("Low");
        //        allDatas.FindLowestPossible();
        //        singleNo = Convert.ToInt32(allDatas.tripleData.key[2]);
        //        doubleNO = Convert.ToInt32(allDatas.tripleData.key[1]);
        //        tripleNo = Convert.ToInt32(allDatas.tripleData.key[0]);
        //        numbers = allDatas.tripleData.key;
        //        singleWinValueText.text = allDatas.tripleData.key.Substring(2);
        //        doubleWinValueText.text = allDatas.tripleData.key.Substring(1);
        //        tripleWinValueText.text = allDatas.tripleData.key;
        //    }
        //    else if (WinningType.Equals("Medium"))
        //    {
        //        Debug.LogError("Medium");
        //        allDatas.FindMediumPossible();
        //        singleNo = Convert.ToInt32(allDatas.tripleData.key[2]);
        //        doubleNO = Convert.ToInt32(allDatas.tripleData.key[1]);
        //        tripleNo = Convert.ToInt32(allDatas.tripleData.key[0]);
        //        numbers = allDatas.tripleData.key;
        //        singleWinValueText.text = allDatas.tripleData.key.Substring(2);
        //        doubleWinValueText.text = allDatas.tripleData.key.Substring(1);
        //        tripleWinValueText.text = allDatas.tripleData.key;
        //    }
        //    else if (WinningType.Equals("HighMedium"))
        //    {
        //        Debug.LogError("HighMedium");
        //        allDatas.FindLargestMediumPossible();
        //        singleNo = Convert.ToInt32(allDatas.tripleData.key[2]);
        //        doubleNO = Convert.ToInt32(allDatas.tripleData.key[1]);
        //        tripleNo = Convert.ToInt32(allDatas.tripleData.key[0]);
        //        numbers = allDatas.tripleData.key;
        //        singleWinValueText.text = allDatas.tripleData.key.Substring(2);
        //        doubleWinValueText.text = allDatas.tripleData.key.Substring(1);
        //        tripleWinValueText.text = allDatas.tripleData.key;
        //    }
        //}
        //else
        //{
        //    singleNo = UnityEngine.Random.Range(0, 10);
        //    doubleNO = UnityEngine.Random.Range(0, 10);
        //    tripleNo = UnityEngine.Random.Range(0, 10);
        //    numbers = tripleNo.ToString() + doubleNO.ToString() + singleNo.ToString();
        //    singleWinValueText.text = singleNo.ToString();
        //    doubleWinValueText.text = doubleNO.ToString();
        //    tripleWinValueText.text = tripleNo.ToString();
        //}
        string a = Constant.ResultNumber;
        numbers = a;//prabir

        //UIControllerTri.instance.GetResultForGame(_response.GetText());
        for (int i = 0; i < lstAllBlockData.Count; i++)
        {
            if (lstAllBlockData[i].game_number.Count() == 1)
            {
                if (singleDigit.ContainsKey(lstAllBlockData[i].game_number))
                {
                    singleDigit[lstAllBlockData[i].game_number] = lstAllBlockData[i].game_amount;
                }
                else
                {
                    singleDigit.Add(lstAllBlockData[i].game_number, lstAllBlockData[i].game_amount);
                }
            }
            else if (lstAllBlockData[i].game_number.Count() == 2)
            {
                if (doubleDigit.ContainsKey(lstAllBlockData[i].game_number))
                {
                    doubleDigit[lstAllBlockData[i].game_number] = lstAllBlockData[i].game_amount;
                }
                else
                {
                    doubleDigit.Add(lstAllBlockData[i].game_number, lstAllBlockData[i].game_amount);
                }
            }
            else if (lstAllBlockData[i].game_number.Count() == 3)
            {
                if (tripleDigit.ContainsKey(lstAllBlockData[i].game_number))
                {
                    tripleDigit[lstAllBlockData[i].game_number] = lstAllBlockData[i].game_amount;
                }
                else
                {
                    tripleDigit.Add(lstAllBlockData[i].game_number, lstAllBlockData[i].game_amount);
                }
            }
        }
        //for (int i = 0; i < lstAllBlockData.Count; i++)
        //{
        //    if (lstAllBlockData[i].game_number.Count() == 2)
        //    {
        //        doubleDigit.Add(lstAllBlockData[i].game_number, lstAllBlockData[i].game_amount);
        //    }
        //}
        //for (int i = 0; i < lstAllBlockData.Count; i++)
        //{
        //    if (lstAllBlockData[i].game_number.Count() == 3)
        //    {
        //        tripleDigit.Add(lstAllBlockData[i].game_number, lstAllBlockData[i].game_amount);
        //    }
        //}
        Debug.LogError(" allDatas.doubleData.value " + allDatas.tripleData.value);
        Debug.LogError(" allDatas.doubleData.value " + allDatas.doubleData.value);
        Debug.LogError(" allDatas.singleData.value " + allDatas.singleData.value);
        //if (GameSelector.SelectedGame == "TripleChancePro")//prabir
        //{

        //    singlewin = allDatas.singleData.value * 10;
        //    doublewin = allDatas.doubleData.value * 100;
        //    triplewin = allDatas.tripleData.value * 1000;
        //}
        //else
        //{
        //    singlewin = allDatas.singleData.value * 9;
        //    doublewin = allDatas.doubleData.value * 90;
        //    triplewin = allDatas.tripleData.value * 900;
        //}
        currectpPointvalue = Constant.PointBalance;
        //int value = 0;
        //// value = singlewin + doublewin + triplewin;
        //Debug.LogError(Constant.WinAmount);
        //value =int.Parse(Constant.WinAmount);


        //if (GameSelector.SelectedGame == "TripleChance")//close by prabir
        //{
        //    Debug.LogError("Multiplier ... " + Multiplier);
        //    switch (Multiplier)
        //    {
        //        case "2x":
        //            value = value * 2;
        //            Multiplier = "2x";
        //            break;
        //        case "3x":
        //            value = value * 3;
        //            Multiplier = "3x";
        //            break;
        //        case "4x":
        //            value = value * 4;
        //            Multiplier = "4x";
        //            break;
        //        case "5x":
        //            value = value * 5;
        //            Multiplier = "5x";
        //            break;
        //        case "6x":
        //            value = value * 6;
        //            Multiplier = "6x";
        //            break;
        //        case "7x":
        //            value = value * 7;
        //            Multiplier = "7x";
        //            break;
        //        case "8x":
        //            value = value * 8;
        //            Multiplier = "8x";
        //            break;
        //        case "9x":
        //            value = value * 9;
        //            Multiplier = "9x";
        //            break;
        //        case "10x":
        //            value = value * 10;
        //            Multiplier = "10x";
        //            break;
        //        default:
        //            value = value * 1;
        //            Multiplier = "1x";
        //            break;
        //    }
        //}
        // Constant.WinAmount = value.ToString();
        //winningvalue = value;
        //if (playValue > 0)
        //{
        //    if (value > 0)
        //    {
        //        winLoss = "win";
        //    }
        //    else
        //    {
        //        winLoss = "lose";
        //    }
        //   // getuserdetails();//close by prabir
        //}
        if (numbers != null)
        {
            Debug.LogError(numbers);
            singleWinText.text = numbers[2].ToString();           // 3rd digit (units)
            doubleWinText.text = numbers.Substring(1, 2);         // last 2 digits
            TripleWinText.text = numbers;                         // full 3 digits
            StartCoroutine(StopAllWheel(numbers));
        }
        singleDigit.Clear();
        doubleDigit.Clear();
        tripleDigit.Clear();
        numbers = null;
    }
    public void SetWinValue()
    {
        int value = 0;
        Debug.LogError(Constant.WinAmount);
        value = int.Parse(Constant.WinAmount);
        winningvalue = value;
        if (playValue > 0)
        {
            if (value > 0)
            {
                winLoss = "win";
                ShowPopupWin(value.ToString());
                // Invoke("ShowPopupWin(value.ToString())", 3f);
            }
            else
            {
                winLoss = "lose";
            }
        }
    }

    public void ShowPopupWin(string message)
    {
        StartCoroutine(ShowPopupCoroutine(message));
        //#if UNITY_ANDROID
        //        StartCoroutine(ShowPopupCoroutine(message));
        //#endif
    }

    private IEnumerator ShowPopupCoroutine(string message)
    {

        if (Winpopup)
        {
            Winpopup.SetActive(true);
        }

        string textToShow = message.ToString();

        // Set win popup main text
        if (winpopupText != null)
            winpopupText.text = textToShow;

        yield return new WaitForSeconds(3f);

        if (Winpopup)
            Winpopup.SetActive(false);

    }



    public void getuserdetails()
    {
        StartCoroutine(getuserdetailsrequest(Constant.KIBaseURL + "game_data_inserts"));
    }

    IEnumerator getuserdetailsrequest(string url)
    {
        WWWForm form4 = new WWWForm();
        form4.AddField("player_id", PlayerPrefs.GetInt(Constant.UID).ToString());
        form4.AddField("win_number", numbers);
        form4.AddField("win_ammount", Constant.WinAmount);
        form4.AddField("game_name", GameSelector.SelectedGame == "TripleChance" ? "tripleChance" : "tripleChancePro");
        form4.AddField("bet_ammount", tPlayValue.text);
        form4.AddField("win_loose", winLoss);
        form4.AddField("game_id", gameId);
        form4.AddField("start_point", (int)Constant.PointBalance);

        UnityWebRequest uwr = UnityWebRequest.Post(url, form4);
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {

            Debug.Log("uuuuuuuuuuusssssssserrrrrrrrrrrrdddddd" + uwr.downloadHandler.text);


        }
        uwr.Dispose();
    }

    public void CheckStockies()
    {
        allDatas.ClearAllData();
        StartCoroutine(TripleChancePostRequest(Constant.KIBaseURL + "winning-hotlist"));
    }

    IEnumerator TripleChancePostRequest(string url)
    {

        UnityWebRequest uwr = UnityWebRequest.Get(url);
        Debug.LogError(PlayerPrefs.GetString(Constant.Token));
        uwr.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString(Constant.Token));
        yield return uwr.SendWebRequest();
        if (uwr.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error While Sending: " + uwr.error);
        }
        else
        {
            Debug.LogError("Winning Hotlist" + uwr.downloadHandler.text);
            tripplechance.ResponseData.WinningHotlist winType = JsonUtility.FromJson<tripplechance.ResponseData.WinningHotlist>(uwr.downloadHandler.text);
            if (winType.status == 200)
            {
                Debug.Log(winType.list.Count);
                for (int i = 0; i < winType.list.Count; i++)
                {
                    if (winType.list[i].player_id == myPlayerID)
                    {
                        tripplechance.ResponseData.List myWinningList = winType.list[i];
                        WinningType = winType.list[i].win_type;
                        if (myWinningList != null && myWinningList.win_price != null)
                        {

                            // multiplier = int.Parse(myWinningList.win_price.Remove(myWinningList.win_price.Length - 1));
                            // Debug.Log("Multiplier Set from API : " + multiplier);
                        }
                        else
                        {
                            // multiplier = 1;
                        }
                    }
                }
            }
        }
    }

    IEnumerator getRequest(string url)
    {
        UnityWebRequest uwr = UnityWebRequest.Get(url);
        Debug.LogError(PlayerPrefs.GetString(Constant.Token));
        uwr.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString(Constant.Token));
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);

        }
        else
        {
            jsonString = System.Text.Encoding.UTF8.GetString(uwr.downloadHandler.data, 3, uwr.downloadHandler.data.Length - 3);
            JSONNode loginInfo = JSON.Parse(uwr.downloadHandler.text);
            Debug.LogError($"Winner Hot List {uwr.downloadHandler.text}");
            string status = loginInfo["status"];
            Debug.LogError(status);
            if (status.Equals("200"))
            {
                if (loginInfo["message"].ToString().Trim('"') == "no")
                {
                    Multiplier = "1x";

                    if (midvalue > 0)
                    {
                        WinningType = "Medium";
                        midvalue--;
                        if (midHighValue == 0)
                        {
                            midHighValue = 1;
                        }
                    }
                    else
                    {
                        if (midHighValue > 0)
                        {
                            WinningType = "HighMedium";
                            midHighValue--;
                            if (midHighValue == 0)
                            {
                                midvalue = 3;
                            }
                        }
                    }
                    Debug.LogError($" Winning Type : {WinningType} :: midvalue: {midvalue} :: {midHighValue}");
                }
                else
                {
                    int stokiesID = Convert.ToInt32(loginInfo["list"]["player_id"]);
                    if (PlayerPrefs.GetInt(Constant.UID) == stokiesID)
                    {
                        Multiplier = loginInfo["list"]["win_price"].ToString().Trim('"');
                        WinningType = loginInfo["list"]["win_type"].ToString().Trim('"');
                        if (loginInfo["list"]["is_x_excuted"].ToString().Trim('"') == "1")
                        {
                            Multiplier = "1x";
                        }
                    }
                    else
                    {
                        Multiplier = "1x";
                        WinningType = "Medium";
                        Debug.LogError("Enter to Else part");

                        if (midvalue > 0)
                        {
                            WinningType = "Medium";
                            midvalue--;
                            if (midHighValue == 0)
                            {
                                midHighValue = 1;
                            }
                        }
                        else
                        {
                            if (midHighValue > 0)
                            {
                                WinningType = "HighMedium";
                                midHighValue--;
                                if (midHighValue == 0)
                                {
                                    midvalue = 3;
                                }
                            }
                        }
                    }
                }

            }
            else
            {
                print("no");
            }
        }

    }
    public void ResetWinningCount()
    {
        StartCoroutine(getWinningCount(Constant.KIBaseURL + "winner-hotlist-execution"));
    }

    IEnumerator getWinningCount(string url)
    {
        WWWForm form4 = new WWWForm();
        Debug.LogError($"Stockiest ID : {PlayerPrefs.GetInt(Constant.STOKIESID).ToString()}");
        Debug.Log("Player ID : " + PlayerPrefs.GetInt(Constant.STOKIESID).ToString());
        form4.AddField("player_id", PlayerPrefs.GetInt(Constant.STOKIESID).ToString());

        UnityWebRequest uwr = UnityWebRequest.Post(url, form4);
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {

            Debug.Log("uuuuuuuuuuusssssssserrrrrrrrrrrrdddddd" + uwr.downloadHandler.text);
            jsonString = System.Text.Encoding.UTF8.GetString(uwr.downloadHandler.data, 3, uwr.downloadHandler.data.Length - 3);
            JSONNode loginInfo = JSON.Parse(uwr.downloadHandler.text);
            string status = loginInfo["status"];
            Debug.LogError(status);
            if (status.Equals("200"))
            {
                // multiplier = 1;
                PlayerPrefs.SetInt(Constant.ISFIRST, 0);
            }
            else
            {
                Debug.Log("Winning Hotlist execution error : " + uwr.error);
            }
            //CheckStockies();

        }
        uwr.Dispose();
    }
    public void GetGameResult()
    {
        Debug.Log("GetGameResult");
        float fetchingData = Time.time;
        Debug.Log("Fetching Data: " + Time.time);
        Debug.LogError("AccessToken : " + Constant.CurrentAccessToken + " GameSession : " + Constant.CurrentGameSession);
        Web.Create()
           .SetUrl(Constant.GameResultURL, Web.RequestType.POST, Web.ResponseType.TEXT)
           .AddField(Constant.GameSession, Constant.CurrentGameSession)
           .AddHeader(Constant.AccessToken, Constant.CurrentAccessToken)
                  .SetOnSuccessDelegate((Web _web, Response _response) =>
                  {
                      Debug.Log("Time diferrence: " + (Time.time - fetchingData));
                      Debug.LogError("Success " + _response.GetText());
                      Debug.Log("Get result : " + Time.time);
                      JSONNode _jsonNode = JSON.Parse(_response.GetText());
                      if (_jsonNode["status"].Value == "1")
                      {
                          if (_jsonNode["result"]["wining_number"].Value != null)
                              sResultNumber = Constant.ResultNumber = int.Parse(_jsonNode["result"]["wining_number"].Value).ToString("000");
                          Constant.WinAmount = _jsonNode["result"]["win_amount"].Value;

                          singleWinAmount = _jsonNode["result"]["result_win_singles_return_amount"].Value;
                          doubleWinAmount = _jsonNode["result"]["result_win_doubles_return_amount"].Value;
                          tripleWinAmount = _jsonNode["result"]["result_win_triples_return_amount"].Value;


                          singleWinValueText.text = singleWinAmount;
                          doubleWinValueText.text = doubleWinAmount;
                          tripleWinValueText.text = tripleWinAmount;


                          UIControllerTri.instance.GetResultForGame(_response.GetText());
                          Constant.PointBalance = double.Parse(_jsonNode["result"]["remaining_balance"].Value);

                          //commented by somnath
                          //for (int i = 0; i < lstAllWheel.Count; i++)
                          //{
                          //    lstAllWheel[i].number = ( int.Parse(sResultNumber[i].ToString()));
                          //    Debug.Log("Bet numbers: " + lstAllWheel[i].number);
                          //}
                          //New Wheel Update
                          //listAllNewWheels.First().spinWheel.SetDestination(int.Parse(sResultNumber.First().ToString()))
                          //.OnComplete((number) =>
                          //{
                          //    OnCompleteWheelMovement(int.Parse(sResultNumber.First().ToString()), listAllNewWheels.First().gNumberBlock);
                          //    listAllNewWheels[1].spinWheel.SetDestination(int.Parse(sResultNumber[1].ToString()))
                          //    .OnComplete((number) =>
                          //    {
                          //        OnCompleteWheelMovement(int.Parse(sResultNumber[1].ToString()), listAllNewWheels[1].gNumberBlock);
                          //        listAllNewWheels[2].spinWheel.SetDestination(int.Parse(sResultNumber[2].ToString()))
                          //        .OnComplete((number) =>
                          //        {
                          //            OnCompleteWheelMovement(int.Parse(sResultNumber[2].ToString()), listAllNewWheels[2].gNumberBlock);
                          //            OnCompleteAllWheelMovement();
                          //        });
                          //    });
                          //});
                          StartCoroutine(StopAllWheel(sResultNumber));

                          Debug.Log("Bet numbers: " + int.Parse(sResultNumber[0].ToString()) + int.Parse(sResultNumber[1].ToString()) + int.Parse(sResultNumber[2].ToString()));

                          Debug.Log("Result CallbackTime:  " + (Time.time - delayBetweenResultCallback));
                          //StartCoroutine(viewBalance());
                          //commented by somnath
                          //if ((Time.time - delayBetweenResultCallback) > 6.5f)
                          //{
                          //    MoveToSpecificNumberInWheel();
                          //}
                          //else
                          //{
                          //    float delay = 6.5f - (Time.time - delayBetweenResultCallback);
                          //    Invoke("MoveToSpecificNumberInWheel", delay);
                          //}

                      }
                      else
                      {
                          InternetStatus(false);
                          //UIControllerTri.instance.NoInterNetPopUp();
                      }

                      _web.Close();
                  })
                  .SetOnFailureDelegate((Web _web, Response _response) =>
                  {
                      Debug.LogError("Found Error " + _response.GetError());
                      InternetStatus(false);
                      //UIControllerTri.instance.NoInterNetPopUp();
                      _web.Close();
                  })
                  .Connect();


    }

    void viewBalance()
    {
        Debug.LogError("Ie");
        //yield return new WaitForSeconds(19f);
        Web.Create()
           .SetUrl(Constant.GameViewProfile, Web.RequestType.POST, Web.ResponseType.TEXT)
             .AddHeader("Content-Type", "application/json")
             .AddHeader(Constant.AccessToken, Constant.CurrentAccessToken)
                    .SetOnSuccessDelegate((Web _web, Response _response) =>
                    {
                        Debug.LogError("View balance");
                        Debug.LogError("Success " + _response.GetText());
                        JSONNode _jsonNode = JSON.Parse(_response.GetText());
                        if (_jsonNode["status"].Value == "1")
                        {
                            Debug.LogError("Banalce " + _jsonNode["result"]["currentblance"].Value);
                            Constant.PointBalance = double.Parse(_jsonNode["result"]["currentblance"].Value);
                            tPointBalance.text = Constant.PointBalance.ToString();
                        }
                        else
                        {

                        }

                        _web.Close();
                    })
                    .SetOnFailureDelegate((Web _web, Response _response) =>
                    {
                        Debug.Log("Found Error " + _response.GetError());
                        //InternetStatus(false);
                        _web.Close();
                    })
                    .Connect();
    }

    #endregion
    private bool firstWheelStop = false, secondWhelStop = false;
    private string jsonString;

    private IEnumerator StopAllWheel(string resultNumber)
    {
        Debug.Log(resultNumber[0]);
        yield return new WaitForSeconds(1.10f);

        listAllNewWheels[0].spinWheel.SetDestination(int.Parse(resultNumber[0].ToString()))
                          .OnComplete((number) =>
                          {
                              OnCompleteWheelMovement(int.Parse(resultNumber[0].ToString()), listAllNewWheels[0].gNumberBlock);
                              firstWheelStop = true;
                              listAllNewWheels[0].spinWheel.finishSound.Play();
                              listAllNewWheels[1].spinWheel.SetDestination(int.Parse(resultNumber[1].ToString()))
                              .OnComplete((number) =>
                              {
                                  OnCompleteWheelMovement(int.Parse(resultNumber[1].ToString()), listAllNewWheels[1].gNumberBlock);
                                  secondWhelStop = true;
                                  listAllNewWheels[1].spinWheel.finishSound.Play();
                                  listAllNewWheels[2].spinWheel.SetDestination(int.Parse(resultNumber[2].ToString()))
                                  .OnComplete((number) =>
                                  {
                                      Audio_Manager.instance.StopAudio();
                                      listAllNewWheels[2].spinWheel.finishSound.Play();
                                      OnCompleteWheelMovement(int.Parse(resultNumber[2].ToString()), listAllNewWheels[2].gNumberBlock);
                                      OnCompleteAllWheelMovement(resultNumber[2].ToString(), resultNumber[1].ToString(), resultNumber[0].ToString());
                                      //   SetResults(numbers.Substring(2), numbers.Substring(1), numbers);
                                  });
                              });
                          });
    }

    void MoveToSpecificNumberInWheel()
    {
        lstAllWheel[0].MoveToSpecificNumber(lstAllWheel[0].number);
    }

    public void StopAllWheel()
    {
        for (int i = 0; i < 3; i++)
        {
            lstAllWheel[i].StopWheel(sResultNumber[i].ToString());
        }
        // SoundController.instance.StopWheelSound();
    }

    public void InternetStatus(bool a_Status)
    {
        if (!a_Status)
        {
            //if (!noInternetPanel.activeSelf)
            //    noInternetPanel.SetActive(true);
            //SoundController.instance.StopWheelSound();
        }
    }

    public void ApplicationFocus(bool a_HasFocus) { }

    public void ApplicationPause(bool a_PauseStatus)
    {
        if (a_PauseStatus)
        {
            //if (!noInternetPanel.activeSelf)
            //    noInternetPanel.SetActive(true);
        }
    }

    public void OnClickNoInternet()
    {
#if UNITY_ANDROID
            Constant.CurrentDeviceType = "mobile";
            SceneManager.LoadScene("Game_16X9");
#else
        Constant.CurrentDeviceType = "pc";
        SceneManager.LoadScene("Game_" + Constant.ScreenRatio);
#endif
    }

    public void UpdateBalance(float updatedBalance)
    {
        Constant.PointBalance += updatedBalance;
        //tPointBalance.text = Constant.PointBalance.ToString();
        firstPointBalance += updatedBalance;
        IPointBalance += updatedBalance;
    }


    public void BackToDashBoard()
    {
        tripplechance.TripleChanceSocketManager.tripleChanceSocketManager.EmmiteLeave();
        SceneManager.LoadScene("DashBoard");
    }

    void SetResults(string sin, string dou, string tri)
    {
        GameObject go = Instantiate(resultdata, reultdataParent);
        go.SetActive(true);
        go.GetComponent<ResultDate>().SetResultdata(sin, dou, tri);
        resultdataList.Add(go);
        if (reultdataParent.childCount > 6)
        {
            Destroy(reultdataParent.GetChild(0).gameObject);
        }
    }

    IEnumerator SetBlockUser()
    {
        while (true)
        {
            StartCoroutine(getBlockUserRequest(Constant.KIBaseURL + "user-details" + "/" + PlayerPrefs.GetInt(Constant.UID).ToString()));
            yield return new WaitForSeconds(1f);
        }
    }
    IEnumerator getBlockUserRequest(string url)
    {
        WWWForm form4 = new WWWForm();
        UnityWebRequest uwr = UnityWebRequest.Get(url);

        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);

        }
        else
        {
            try
            {
                //Debug.LogError(uwr.result);
                jsonString = System.Text.Encoding.UTF8.GetString(uwr.downloadHandler.data, 3, uwr.downloadHandler.data.Length - 3);
                JSONNode loginInfo = JSON.Parse(uwr.downloadHandler.text);
                string msg = loginInfo["status"];
                //Debug.LogError("User Block "+uwr.downloadHandler.text);
                if (msg.Equals("200"))
                {
                    if (loginInfo["list"]["is_block"].ToString().Trim() == "1")
                    {
                        Debug.LogError("Data...... " + loginInfo["list"]["is_block"]);
                        logOut();
                    }
                }
                else
                {
                    print("no");
                }
            }
            catch (Exception e) { }
        }

        uwr.Dispose();
    }

    public void logOut()
    {
        StartCoroutine(getlogOutRequest(Constant.KIBaseURL + "user-log-outs"));
    }

    IEnumerator getlogOutRequest(string url)
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
            string msg = loginInfo["message"];
            if (msg.Equals("Logged Out"))
            // if (loginInfo["message"] == "Logged Out")
            {
                print("yes");
                PlayerPrefs.SetInt(Constants.LoginStatus, 0);
                SceneManager.LoadSceneAsync(4);
            }
            else
            {
                print("no");
            }
        }

    }

}

[System.Serializable]
public class BlockData
{
    [System.NonSerialized] public Block block;  //[System.NonSerialized] 
    public string game_number;
    public int game_amount;
    [System.NonSerialized] public List<int> allGameAmount;

    public BlockData(Block block, string number, int game_amount)
    {
        this.block = block;
        this.game_number = number;
        this.game_amount = game_amount;
        allGameAmount = new List<int>();
        allGameAmount.Add(game_amount);
    }

    public bool RemoveLastAmount()
    {
        bool isLast = false;

        this.game_amount -= allGameAmount[allGameAmount.Count - 1];
        allGameAmount.RemoveAt(allGameAmount.Count - 1);

        if (!(allGameAmount.Count > 0))
        {
            isLast = true;
        }
        return isLast;
    }

}

[System.Serializable]
public class BlockDetails
{
    public string name;
    public float fOpenPos;
    public float fClosePos;
    public RectTransform rectTransform;
    public RectTransform rBlockPanel;
    public float fAdditionValueForPanel;
    public Image arrow;
    public Text tPlayValue;
    public Text tWinValue;

    public Image tPlayBG;
    public Image tWinBG;
}

[System.Serializable]
public class SpinWheelSystem
{
    public SpinWheelNew spinWheel;
    public GameObject gNumberBlock;
}

[System.Serializable]
public class DataClass
{
    public string card;
    public long value;

}

[System.Serializable]
public class AllDatas
{
    public long totalAmount = 0;
    public DataClass singleData = new DataClass();
    public DataClass doubleData = new DataClass();
    public DataClass tripleData = new DataClass();
    public List<DataClass> singleDatas = new List<DataClass>();
    public List<DataClass> doubleDatas = new List<DataClass>();
    public List<DataClass> tripleDatas = new List<DataClass>();
    public List<DataClass> highList = new List<DataClass>();
    public List<DataClass> lowList = new List<DataClass>();
    public void FindLowestPossible()
    {
        foreach (DataClass d in tripleDatas)
        {
            DataClass dd = doubleDatas.Find(x => x.card.Equals(d.card.Substring(1)));
            DataClass ss = singleDatas.Find(x => x.card.Equals(d.card.Substring(2)));
            if (SceneManager.GetActiveScene().name.Equals("TripleChanceProGameplay"))
            {
                if (((d.value * 1000) + (dd.value * 100) + (ss.value * 10)) == 0)
                {
                    lowList.Add(d);
                }
            }
            else
            {
                if (((d.value * 900) + (dd.value * 90) + (ss.value * 9)) == 0 || ((d.value * 900) + (dd.value * 90) + (ss.value * 9)) < GamePlay.instance.playValue)
                {
                    lowList.Add(d);
                }
            }
        }
        if (lowList.Count == 0)
        {

            foreach (DataClass d in tripleDatas)
            {
                DataClass dd = doubleDatas.Find(x => x.card.Equals(d.card.Substring(1)));
                DataClass ss = singleDatas.Find(x => x.card.Equals(d.card.Substring(2)));
                if (((d.value * 1000) + (dd.value * 100) + (ss.value * 10)) < GamePlay.instance.playValue)
                {
                    lowList.Add(d);
                }
            }
        }
        DataClass res = lowList[UnityEngine.Random.Range(0, lowList.Count)];
        if (res == null)
        {
            res = lowList[UnityEngine.Random.Range(0, tripleDatas.Count)];
        }
        for (int i = 0; i < singleDatas.Count; i++)
        {
            if (res.card.Substring(2).Equals(singleDatas[i].card))
            {
                singleData = singleDatas[i];
                break;
            }
        }
        for (int i = 0; i < doubleDatas.Count; i++)
        {
            if (res.card.Substring(1).Equals(doubleDatas[i].card))
            {
                doubleData = doubleDatas[i];
                break;
            }
        }
        tripleData = res;
    }
    public void FindMediumPossible()
    {
        foreach (DataClass d in tripleDatas)
        {
            DataClass dd = doubleDatas.Find(x => x.card.Equals(d.card.Substring(1)));
            DataClass ss = singleDatas.Find(x => x.card.Equals(d.card.Substring(2)));
            //Debug.LogError("Value data "+GamePlay.instance.playValue * (30 / 100));
            //Debug.LogError("Value data "+ GamePlay.instance.playValue);
            //Debug.LogError($"Triple data Key {d.key} : {d.value}");
            //Debug.LogError($"Double data Key {dd.key} : {dd.value}");
            //Debug.LogError("Single data "+ ss.value);
            if ((((d.value * 1000) + (dd.value * 100) + (ss.value * 10))
                >= (GamePlay.instance.playValue - (GamePlay.instance.playValue * 30 / 100)))
                && (((d.value * 1000) + (dd.value * 100) + (ss.value * 10))
                < GamePlay.instance.playValue))
            {
                Debug.LogError($"Low List Add At Tripple {d.card} : {d.value}");
                lowList.Add(d);
            }
        }
        DataClass res = new DataClass();
        if (lowList.Count > 0)
        {
            Debug.Log("First");
            res = lowList[UnityEngine.Random.Range(0, lowList.Count)];
        }
        else
        {
            foreach (DataClass d in tripleDatas)
            {
                DataClass dd = doubleDatas.Find(x => x.card.Equals(d.card.Substring(1)));
                DataClass ss = singleDatas.Find(x => x.card.Equals(d.card.Substring(2)));
                if ((((d.value * 1000) + (dd.value * 100) + (ss.value * 10)) >= (GamePlay.instance.playValue - ((GamePlay.instance.playValue * 60) / 100))) && ((d.value * 1000) + (dd.value * 100) + (ss.value * 10)) < (GamePlay.instance.playValue - ((GamePlay.instance.playValue * 30) / 100)))
                {
                    lowList.Add(d);
                }
            }
            if (lowList.Count > 0)
            {
                Debug.Log("Second");
                res = lowList[UnityEngine.Random.Range(0, lowList.Count)];
            }
            else
            {
                foreach (DataClass d in tripleDatas)
                {
                    DataClass dd = doubleDatas.Find(x => x.card.Equals(d.card.Substring(1)));
                    DataClass ss = singleDatas.Find(x => x.card.Equals(d.card.Substring(2)));
                    if ((((d.value * 1000) + (dd.value * 100) + (ss.value * 10))
                        >= (GamePlay.instance.playValue - (GamePlay.instance.playValue * (100 / 100))))
                        && ((d.value * 1000) + (dd.value * 100) + (ss.value * 10))
                        < (GamePlay.instance.playValue - ((GamePlay.instance.playValue * 60) / 100)))
                    {
                        lowList.Add(d);
                    }
                }
                if (lowList.Count > 0)
                {
                    Debug.LogError(lowList.Count);
                    Debug.LogError("GamePlay " + (GamePlay.instance.playValue * (60 / 100)));
                    Debug.Log("Third");
                    res = lowList[UnityEngine.Random.Range(0, lowList.Count)];
                }
                else
                {
                    Debug.Log("Fourth");
                    res = tripleDatas[UnityEngine.Random.Range(0, tripleDatas.Count)];
                }
            }
        }

        for (int i = 0; i < singleDatas.Count; i++)
        {
            if (res.card.Substring(2).Equals(singleDatas[i].card))
            {
                singleData = singleDatas[i];
                break;
            }
        }
        for (int i = 0; i < doubleDatas.Count; i++)
        {
            if (res.card.Substring(1).Equals(doubleDatas[i].card))
            {
                doubleData = doubleDatas[i];
                break;
            }
        }
        tripleData = res;
    }
    public void FindLargestPossible()
    {
        long max = 0;

        List<DataClass> triple = new List<DataClass>();

        foreach (DataClass d in tripleDatas)
        {
            DataClass dd = doubleDatas.Find(x => x.card.Equals(d.card.Substring(1)));
            DataClass ss = singleDatas.Find(x => x.card.Equals(d.card.Substring(2)));
            if (SceneManager.GetActiveScene().name.Equals("TripleChanceProGameplay"))
            {
                if (((d.value * 1000) + (dd.value * 100) + (ss.value * 10)) >= GamePlay.instance.playValue)
                {
                    highList.Add(d);
                }
            }
            else
            {
                if (((d.value * 900) + (dd.value * 90) + (ss.value * 9)) >= GamePlay.instance.playValue)
                {
                    highList.Add(d);
                }
            }
        }

        for (int i = 0; i < highList.Count - 1; i++)
        {
            for (int j = 0; j < highList.Count - i - 1; j++)
            {
                if (highList[j].value > highList[j + 1].value)
                {
                    long temp = highList[j].value;
                    string key = highList[j].card;
                    highList[j].value = highList[j + 1].value;
                    highList[j].card = highList[j + 1].card;
                    highList[j + 1].value = temp;
                    highList[j + 1].card = key;
                }
            }
        }
        List<DataClass> totalHighList = new List<DataClass>();
        if (highList.Count > 1)
        {
            for (int i = 0; i < highList.Count; i++)
            {
                if (highList[highList.Count - 1].value == highList[i].value)
                {
                    totalHighList.Add(highList[i]);
                }

            }
        }
        DataClass res = new DataClass();
        if (totalHighList.Count > 1)
        {
            res = totalHighList[UnityEngine.Random.Range(0, totalHighList.Count)];
        }
        else
        {
            res = highList[UnityEngine.Random.Range(0, highList.Count)];
        }
        Debug.LogError("highList " + JsonConvert.SerializeObject(highList));
        // Closed by sayam
        //DataClass res = highList[highList.Count-1];
        if (res == null)
        {
            res = lowList[UnityEngine.Random.Range(0, tripleDatas.Count)];
        }
        for (int i = 0; i < singleDatas.Count; i++)
        {
            if (res.card.Substring(2).Equals(singleDatas[i].card))
            {
                singleData = singleDatas[i];
                break;
            }
        }
        for (int i = 0; i < doubleDatas.Count; i++)
        {
            if (res.card.Substring(1).Equals(doubleDatas[i].card))
            {
                doubleData = doubleDatas[i];
                break;
            }
        }
        tripleData = res;

    }
    public void FindLargestMediumPossible()
    {
        List<DataClass> triple = new List<DataClass>();
        DataClass res = new DataClass();
        foreach (DataClass d in tripleDatas)
        {
            DataClass dd = doubleDatas.Find(x => x.card.Equals(d.card.Substring(1)));
            DataClass ss = singleDatas.Find(x => x.card.Equals(d.card.Substring(2)));
            float totalBetAmount = (d.value * 1000) + (dd.value * 100) + (ss.value * 10);
            float percentMultiplier = 100;
            if (totalBetAmount > 99)
            {
                percentMultiplier = 1000;
            }
            else if (totalBetAmount > 999)
            {
                percentMultiplier = 10000;
            }
            if (((d.value * 1000) + (dd.value * 100) + (ss.value * 10))
                <= (GamePlay.instance.playValue + ((GamePlay.instance.playValue * 30) / percentMultiplier))
                && ((d.value * 1000) + (dd.value * 100) + (ss.value * 10)) >
                (GamePlay.instance.playValue + ((GamePlay.instance.playValue * 10) / percentMultiplier)))
            {
                highList.Add(d);
            }
        }
        if (highList.Count > 0)
        {
            res = highList[UnityEngine.Random.Range(0, highList.Count - 1)];

        }
        else
        {
            foreach (DataClass d in tripleDatas)
            {
                DataClass dd = doubleDatas.Find(x => x.card.Equals(d.card.Substring(1)));
                DataClass ss = singleDatas.Find(x => x.card.Equals(d.card.Substring(2)));
                if (((d.value * 1000) + (dd.value * 100) + (ss.value * 10))
                    <= (GamePlay.instance.playValue + ((GamePlay.instance.playValue * 50) / 100))
                    && ((d.value * 1000) + (dd.value * 100) + (ss.value * 10)) > (GamePlay.instance.playValue + ((GamePlay.instance.playValue * 30) / 100)))
                {
                    highList.Add(d);
                }
            }
            if (highList.Count > 0)
            {
                res = highList[UnityEngine.Random.Range(0, highList.Count - 1)];

            }
            else
            {
                res = tripleDatas[UnityEngine.Random.Range(0, tripleDatas.Count)];
            }
        }


        for (int i = 0; i < singleDatas.Count; i++)
        {
            if (res.card.Substring(2).Equals(singleDatas[i].card))
            {
                singleData = singleDatas[i];
                break;
            }
        }
        for (int i = 0; i < doubleDatas.Count; i++)
        {
            if (res.card.Substring(1).Equals(doubleDatas[i].card))
            {
                doubleData = doubleDatas[i];
                break;
            }
        }
        tripleData = res;
    }

    public void ClearAllData()
    {
        totalAmount = 0;
        singleData = new DataClass();
        doubleData = new DataClass();
        tripleData = new DataClass();
        singleDatas.Clear();
        doubleDatas.Clear();
        tripleDatas.Clear();
        highList.Clear();
        lowList.Clear();
        GamePlay.instance.SetDatas();
    }
}
[System.Serializable]
public class List
{
    public int id;
    public string win_card;
    public string game_name;
    public DateTime created_at;
    public DateTime updated_at;
}
[System.Serializable]
public class LiveResultData
{
    public int status;
    public List<HistoryDataListItem> list;
}