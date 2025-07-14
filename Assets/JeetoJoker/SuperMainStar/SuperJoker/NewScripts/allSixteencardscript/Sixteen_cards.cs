using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using khelojeetonew;
using Newtonsoft.Json;
using UnityEngine.Networking;
using System;
using JeetoJoker;
using SoundControllerJeeto = JeetoJoker.SoundController;
using UnityEngine.SceneManagement;
using SimpleJSON;
using UnityEngine.Animations;
using BestHTTP.Extensions;
using System.Threading.Tasks;



#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Runtime.InteropServices;

namespace khelojeetonew
{
    public class Sixteen_cards : MonoBehaviour
    {           /*// Sixteen_cards is copy of jeeto Joker Manager*/
        public enum GameType
        {
            JeetoJoker,
            Cards16,
            Cards12_IPL
        }

        [SerializeField] private GameType _gameType;
        public ParticleSystem xisexecuted;
        public AudioSource Winsound;
        [SerializeField] private Sprite cardSelected;
        [SerializeField] private Sprite cardDiselect;

        [SerializeField] private List<BetButtonss> betButtonss;
        public string userbetdata;

        [SerializeField] private long UserCoins = 10000;

        [SerializeField] public Text userCoinsText;

        [SerializeField] public Text currentTotalBetText;
        [SerializeField] private Text totalWinText;

        [SerializeField] public Text userNameText;

       // [SerializeField] private GameObject removeButtonObj;
       // [SerializeField] private GameObject repeatButtonObj;
       [SerializeField] private Button repeatButton;
        [SerializeField] private SpriteRenderer cardImage;
        [SerializeField] private SpriteRenderer suiteImage;
        [SerializeField] private Text multiplierText;
        [SerializeField] public GameObject multiplierObject;


        [SerializeField] private Text messagePopupText;
        [SerializeField] private GameObject messagePopup;

        [HideInInspector] public List<ButtonHandlerss> ButtonHandlerss = new List<ButtonHandlerss>();
        public List<List<IRemoveHandlers>> removeHandlerss = new List<List<IRemoveHandlers>>();
        [HideInInspector] public List<IWinHandlers> winHandlerss = new List<IWinHandlers>();
          [HideInInspector] public List<IWinHandler> winHandlers = new List<IWinHandler>();
 //[HideInInspector] public List<IWinHandlers> winHandlerss = new List<IWinHandlers>();
        [HideInInspector] public bool canBet = false;

        [SerializeField] private GameObject loadingPanel;
        [SerializeField] private GameObject fakeUserPopup;

        [SerializeField] private APIData _apiData;

        [SerializeField] private sixteencard_cardhistory cardHistoryDeck;



        private long currentWinAmount;
        private int currentWinNumber;

        private List<int> bettingTime = new List<int>();
        private List<int> bettingPosition = new List<int>();
        private List<long> bettingAmount = new List<long>();
        private List<long> winAmount = new List<long>();

        public APIData apiData => _apiData;

        public GameType gameType => _gameType;

        public int removeCount = 0;

        private int winNumber;
        private long totalBetAmount = 0;

        private BetButtonss selectedBetbuttons;
        public long totalBet = 0;
        private long totalUserCoins = 0;

        public Sprite CardSelected { get => cardSelected; }
        public Sprite CardDiselect { get => cardDiselect; }
        public BetButtonss SelectedBetbuttons { get => selectedBetbuttons; }

        public static Sixteen_cards instance;
        private bool isSpining = false;
        public SixteenCars_Foo WheelSpinFoo;
        int cardIdLive;
        int suitIDLive;
        private bool wheelIsSpinning = false;
        // public HistoryCard centerWheelCardAndSuite;
        public GameObject infoPanel;
        public GameObject cardFlashAnimation;
        public GameObject wheelLights;
        public GameObject Xanimation;
        public GameObject WithoutXanimation;

        private string _token;

        private int noBetSoundIndex;
        public int placeBetSoundIndex { get; private set; }
        public bool networkpopup = true;
        public GameObject WinPopUp;
        public Text Winamount;
        public GameObject CardHistory;
         public Text dateTimeText;
         [SerializeField]
        private ParticleSystem _heavyCoinsWinEffect;
        [SerializeField]
        private GameObject _heavyWinAnimation;
      //  public Text drawTime16;
         [SerializeField] private Text userCoinsText_COPY;
        private void Awake()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }


        }
        private void Start()
        {
            Reset();
            noBetSoundIndex = (int)SoundControllerJeeto.SoundType.NoBet;
            placeBetSoundIndex = (int)SoundControllerJeeto.SoundType.PlaceBet;
            //StartCoroutine(GameLoginStatus(Constant.KIBaseURL + "game-login-status", "1"));
        }
        private void Update()
        {
        DateTime currentDateTime = DateTime.Now;
        string formattedDateTime = currentDateTime.ToString("dd-MM-yyyy HH:mm:ss tt");
        // Update the Text component with the formatted date and time
        dateTimeText.text = formattedDateTime;
         userCoinsText_COPY.text = userCoinsText.text;
       // int balance = int.Parse(userCoinsText.text);

// Format it to show two decimal places
//userCoinsText_COPY.text = balance.ToString("#0.00");
        //  DateTime drawDateTime;
      // if (DateTime.TryParse(Printer16card.drawTime, out drawDateTime))
      // {
       //drawTime16.text = drawDateTime.ToString("hh:mm tt");
      // }
           // StartCoroutine(CheckInternet());
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Logout();
            }
        }

        private IEnumerator CheckInternet()
        {
            using (UnityWebRequest www = UnityWebRequest.Head("http://www.google.com"))
            {
                yield return www.SendWebRequest();
                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                {
                    SetPopupActive();
                }
                else
                {
                    if (networkpopup == true)
                    {

                        yield return StartCoroutine(SetPopupDeActive());
                    }
                }
            }
            yield return new WaitForSeconds(5f);
        }

        public void SetPopupActive()
        {
            networkpopup = true;
            messagePopup.SetActive(true);
            messagePopupText.text = "No Internet Connection found. Check Your Connection";
        }
        private IEnumerator SetPopupDeActive()
        {
            networkpopup = false;
            messagePopupText.text = "ReConnecting...";
            yield return new WaitForSeconds(3f);
            messagePopupText.text = "You are back online";
            yield return new WaitForSeconds(1f);
            messagePopup.SetActive(false);
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

        public void Reset()
        {
            SelectBet(0);
            Clear();
            totalWinText.text = "0";
            totalUserCoins = UserCoins;

            userCoinsText.text = totalUserCoins.ToString("#0.00");
            sixteencard_Timer.inst.SetDefaultColor();
            multiplierText.text = "";
            canBet = true;
        }

       /* IEnumerator GameLoginStatus(string url, string loginStatus)
        {
            Debug.LogError(PlayerPrefs.GetString(Constants.Token));

            WWWForm form = new WWWForm();
            form.AddField("game_login_status", loginStatus);
            if (SceneManager.GetActiveScene().name == "jokerScenePC")
                form.AddField("recent_game_name", "12cards");
            else
                form.AddField("recent_game_name", "16cards");

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
                string jsonString = System.Text.Encoding.UTF8.GetString(uwr.downloadHandler.data, 3, uwr.downloadHandler.data.Length - 3);
                JSONNode loginInfo = JSON.Parse(uwr.downloadHandler.text);
                Debug.Log($"User Details : {uwr.downloadHandler.text}");
                string msg = loginInfo["message"];
                if (msg.Equals("Success.") || msg.Equals("Success"))
                {
                    print("yes");
                    if (loginStatus == "0")
                        SceneManager.LoadSceneAsync("DashBoard");
                }
                else
                {
                    print("no");
                }
            }
        }*/

        public void AddBetAmounts(int cardNumber, long amount)
        {
            bettingPosition.Add(cardNumber);
            bettingAmount.Add(amount);
            winAmount.Add(0);
            bettingTime.Add(1);
        }

        public void Logout()
        {
            SixteenCardsSocketController.Instance.Leave();
               SceneManager.LoadSceneAsync("DashBoard");
           // StartCoroutine(GameLoginStatus(Constant.KIBaseURL + "game-login-status", "0"));
            //SceneManager.LoadSceneAsync("DashBoard");
            // StartCoroutine(LogOutRutine(PlayerPrefs.GetString(Constants.token),false));
        }

        public void LogoutSilent()
        {
            // StartCoroutine(LogOutRutine(PlayerPrefs.GetString(Constants.token),true));
        }

        public IEnumerator LogOutRutine(string token, bool isSilent)
        {
            loadingPanel.SetActive(true);
            UnityWebRequest unityWebRequest = UnityWebRequest.Get(Sixteen_cards.instance.apiData.logOutApi);
            unityWebRequest.SetRequestHeader(Constants.authorization, "Bearer " + token);
            yield return unityWebRequest.SendWebRequest();
            loadingPanel.SetActive(false);
            if (unityWebRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(unityWebRequest.error);
                if (!isSilent)
                    ShowMessage(unityWebRequest.error);
                Debug.LogError(unityWebRequest.downloadHandler.text);
            }
            else
            {
                try
                {
                    Debug.LogError(unityWebRequest.downloadHandler.text);
                    LogoutData logoutData = JsonConvert.DeserializeObject<LogoutData>(unityWebRequest.downloadHandler.text);
                    if (logoutData.status == 200)
                    {
                        if (!isSilent)
                        {
                            ShowMessage("You have logged out from this device!");
                            SixteenCardsSocketController.Instance.Leave();
                        }

                        PlayerPrefs.DeleteKey(Constants.userName);
                        PlayerPrefs.DeleteKey(Constants.password);
                        PlayerPrefs.DeleteKey(Constants.token);
                        fakeUserPopup.SetActive(true);
                        Clear();
                    }
                    else
                    {
                        if (!isSilent)
                            ShowMessage(logoutData.message);
                    }
                }
                catch (Exception e)
                {
                    if (!isSilent)
                        ShowMessage(e.ToString());
                }
            }
        }

        [ContextMenu("FakeWin")]
        public void FakeWin()
        {
            winNumber = 11;
            totalBetAmount = 80;
            winAmount = new List<long>() { 0, 0, 50, 0, 0 };
            bettingAmount = new List<long>() { 5, 40, 5, 20, 10 };
            bettingPosition = new List<int>() { 12, 23, 11, 21, 13 };
            bettingTime = new List<int>() { 1, 1, 1, 1, 1 };
            SixteenCardsSocketController.Instance.roomIdData.roomId = "45456";
            StartCoroutine(GameDataInsert(50, "loose"));
        }

        public void ShowMessage(string message)
        {
            messagePopupText.text = message;
            messagePopup.SetActive(true);
        }

        public void SelectBet(int id)
        {
            betButtonss.ForEach(x => x.buttonTransfrom.localScale = Vector2.one);

            selectedBetbuttons = betButtonss.Find(x => x.id == id);

           // selectedBetbuttons.buttonTransfrom.localScale = Vector2.one * 1.3f;
        }

        public bool Bet(int amount)
        {
            if (canBet == false)
            {
                return false;
            }

            if (amount > totalUserCoins)
            {
                return false;
            }
            else
            {
                totalBet = totalBet + amount;
                currentTotalBetText.text = totalBet.ToString();
                totalUserCoins = totalUserCoins - amount;
                userCoinsText.text = totalUserCoins.ToString("#0.00");
                return true;
            }
        }

        public void RemoveBet(int amount)
        {
            totalBet = totalBet - amount;
            currentTotalBetText.text = totalBet.ToString();

            totalUserCoins = totalUserCoins + amount;
            userCoinsText.text = totalUserCoins.ToString("#0.00");
        }

        public void Clear()
        {
            if (canBet == false)
            {
                return;
            }

            removeHandlerss.Clear();
            removeCount = 0;
            ButtonHandlerss.ForEach(x => x.Clear());
            totalBet = 0;
            currentTotalBetText.text = totalBet.ToString();
            totalUserCoins = UserCoins;
            userCoinsText.text = totalUserCoins.ToString("#0.00");

            if (ButtonHandlerss.TrueForAll(x => x.CheckifLastSavedDataAvailable() == false))
            {
               // removeButtonObj.SetActive(true);
                //   repeatButtonObj.SetActive(false);
                repeatButton.interactable=false;
            }
            else
            {
               // removeButtonObj.SetActive(true);
                //   repeatButtonObj.SetActive(true);
                repeatButton.interactable=true;
            }
        }
         public void Betclearforprint16card()
        {
if (canBet == false)
            {
                return;
            }
Debug.Log("bet clear after sending in api test card data");
           // removeHandlers.Clear();
            removeCount = 0;
           // ButtonHandlers.ForEach(x => x.Clear());
            totalBet = 0;
            currentTotalBetText.text = totalBet.ToString();
           /* if (ButtonHandlers.TrueForAll(x => x.CheckifLastSavedDataAvailable() == false))
            {
                //  removeButtonObj.SetActive(true);
                //   repeatButtonObj.SetActive(false);
                Repeatbutton.interactable = false;
            }
            else
            {
                //  removeButtonObj.SetActive(false);
                // repeatButtonObj.SetActive(true);
                Repeatbutton.interactable = true;
            }*/
        }
    

        public void Remove()
        {
            if (canBet == false)
            {
                return;
            }

            if (removeHandlerss.Count > 0)
            {
                List<IRemoveHandlers> re = removeHandlerss[removeCount - 1];
                re.ForEach(x => { if (x != null) x.Remove(); });
                re.Clear();

                removeHandlerss.RemoveAt(removeCount - 1);
                removeCount -= 1;
            }
        }

        public void Repeat()
        {
            if (canBet == false)
            {
                return;
            }

            long repeatSum = 0;
            ButtonHandlerss.ForEach(x => repeatSum = repeatSum + x.GetPrevRoundTotalSum());
            Debug.LogError("repeatSum " + repeatSum);
            if (repeatSum <= totalUserCoins)
            {
                ButtonHandlerss.ForEach(x => x.Repeat());

               // removeButtonObj.SetActive(true);
                //  repeatButtonObj.SetActive(false);
                repeatButton.interactable=false;
            }
        }

        public void DoubleUp()
        {
            if (canBet == false)
            {
                return;
            }

            long repeatSum = 0;
            ButtonHandlerss.ForEach(x => repeatSum = repeatSum + x.GetRoundTotalSum());
            if (repeatSum <= totalUserCoins)
            {
                removeHandlerss.Add(new List<IRemoveHandlers>());
                ButtonHandlerss.ForEach(x => x.DoubleUp(removeCount));
                removeCount += 1;
            }
        }

        public void TimerEnd()
        {
            SoundControllerJeeto.Instance.PlayOneShot(SoundControllerJeeto.SoundType.NoBet, noBetSoundIndex);
            canBet = false;
            betButtonss.ForEach(x => x.buttonTransfrom.localScale = Vector2.one);
        }

      /*  public void OnWin(int outerId, int innerId)
        {
            string multiplier = "";
            if (SixteenCardsSocketController.Instance.is_x_excuted.Equals(Constants.zero))
            {
                multiplier = SixteenCardsSocketController.Instance.win_price;

               


            }
            if (string.IsNullOrEmpty(multiplier) ||multiplier=="1x")
            {
                multiplierObject.SetActive(false);
            }
            else
            {
                if(multiplier=="1x")
                {
                    Debug.Log("not show the 1x");
                }
                //multiplierObject.SetActive(true);
                multiplierText.text = multiplier;
            }
            multiplierText.text = SixteenCardsSocketController.Instance.win_price;
            ButtonHandlerss.ForEach(x => x.SavePrevRound());
            Debug.LogError("AllBetData() " + AllBetData());
            Sequence seq = DOTween.Sequence();
            seq.AppendInterval(4f);
            seq.AppendCallback(() =>
            {
                int count = winHandlerss.Count;
                int winCount = 0;
                for (int i = 0; i < count; i++)
                {
                    Debug.Log("calling on win on win handlers");
                    if (winHandlerss[i].OnWin(outerId, innerId))
                        winCount++;
                }

                if (totalBetAmount > 0)
                {
                    if (winCount == 0)
                    {
                        StartCoroutine(GameDataInsert(0, "loose"));
                    }
                }
                //commented by shivamfusi0on07 as commneted on actual priject
              //  cardHistoryDeck.PushCardData(outerId, innerId);
                StartCoroutine(GetUserDetails());
                // winHandlerss.ForEach(x => x.OnWin(outerId, innerId));
            }
            );
            seq.AppendInterval(10f);
            seq.AppendCallback(() =>
            {
                canBet = true;
                SoundControllerJeeto.Instance.PlayOneShot(SoundControllerJeeto.SoundType.PlaceBet, placeBetSoundIndex);
            });
            seq.AppendCallback(() => Clear());
            seq.AppendCallback(() => totalWinText.text = "0");
            seq.AppendCallback(() => SelectBet(0));
            Printer16card.OnGameRestart();
            seq.AppendCallback(() => sixteencard_Timer.inst.StartTimer(90f));
            isSpining = false;
        }*/
        public  void OnWin(int outerId, int innerId)
{
    // string multiplier = "";

    // // Retrieve the multiplier value
    // if (SixteenCardsSocketController.Instance.is_x_excuted.Equals(Constants.zero))
    // {
    //     multiplier = SixteenCardsSocketController.Instance.win_price;
    // }

    // // Check if the multiplier is empty or "1x"
    // if (string.IsNullOrEmpty(multiplier) || multiplier == "1x")
    // {
    //     multiplierObject.SetActive(false); // Hide the multiplier object
    //     Debug.Log("Multiplier is either empty or 1x, not showing.");
    // }
    // else
    // {
    //     multiplierObject.SetActive(true); // Show the multiplier object
    //     multiplierText.text = multiplier; // Update the text with the multiplier value
    //     Debug.Log("multiplier in center:"+multiplierText.text);
    // }

    // Save previous round data
    ButtonHandlerss.ForEach(x => x.SavePrevRound());

    Debug.LogError("AllBetData() " + AllBetData());

    // Create a sequence for animations or delayed actions
    Sequence seq = DOTween.Sequence();
    seq.AppendInterval(4f);
    seq.AppendCallback(() =>
    {
        int count = winHandlerss.Count;
        int winCount = 0;

        for (int i = 0; i < count; i++)
        {
            Debug.Log("Calling OnWin on win handlers");
            if (winHandlerss[i].OnWin(outerId, innerId))
                winCount++;
        }
Debug.Log("total bet amount if bet place and no win:"+totalBetAmount);
        // Handle loss condition if no wins
       /* if (totalBetAmount > 0 && winCount == 0)
        {
            Debug.Log("Game data inser for no win loss show in history");
            StartCoroutine(GameDataInsert(0, "loose"));
        }*/

        // Fetch updated user details
        StartCoroutine(GetUserDetails());
        cardHistoryDeck.PushCardData(outerId, innerId);
    });

    // Additional sequence actions
    seq.AppendInterval(10f);
    seq.AppendCallback(() =>
    {
        canBet = true;
        SoundControllerJeeto.Instance.PlayOneShot(SoundControllerJeeto.SoundType.PlaceBet, placeBetSoundIndex);
    });

    seq.AppendCallback(() => Clear());
    seq.AppendCallback(() => totalWinText.text = "0");
    seq.AppendCallback(() => SelectBet(0));
   // Printer16card.OnGameRestart();
  // await Task.Delay(5000);
    seq.AppendCallback(() => sixteencard_Timer.inst.StartTimer(90f));
    isSpining = false;
   // Reset();
}



        private IEnumerator GetUserDetails()
        {
            //loadingPanel.SetActive(true);
            yield return new WaitForSeconds(2);

            /*string multiplier = "";
            if (SixteenCardsSocketController.Instance.is_x_excuted.Equals(Constants.zero))
            {
                multiplier = SixteenCardsSocketController.Instance.win_price;
              //  Sixteen_cards.instance.xisexecuted.Play();
            }
             if (string.IsNullOrEmpty(multiplier) || multiplier == "1x")
            {
                multiplierObject.SetActive(false);
            }
            else
            {
                //multiplierObject.SetActive(true);
                multiplierText.text = multiplier;
            }
            multiplierText.text = SixteenCardsSocketController.Instance.win_price;*/
            currentWinAmount = 0;
            currentWinNumber = 0;
            bettingAmount.Clear();
            bettingPosition.Clear();
            bettingTime.Clear();
            winAmount.Clear();
            UnityWebRequest unityWebRequest = UnityWebRequest.Get(apiData.userDetailsApi);
            unityWebRequest.SetRequestHeader(Constants.authorization, "Bearer " + _token);
            yield return unityWebRequest.SendWebRequest();
            //loadingPanel.SetActive(false);
            if (unityWebRequest.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    Debug.LogError(unityWebRequest.downloadHandler.text);
                    LoginData loginData = JsonConvert.DeserializeObject<LoginData>(unityWebRequest.downloadHandler.text);
                    if (loginData.status == 200)
                    {
#if UNITY_EDITOR
                        //                     coins = 50000;
#endif
                        //int.TryParse(loginData.walletBlance, out int coins);
                        int.TryParse(loginData.walletBlance.ToString(), out int coins);

                        UserCoins = totalUserCoins = coins;
                        userCoinsText.text = coins.ToString("#0.00");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
        }

       /* public void ShowWinAmount(long amount, int winNumber)
        {
            long winAmount = amount * 14;*/
           // if(winAmount == 0)
           //     return;
          //  Debug.Log("inside show win amount 16 cards");
          /*  if (totalBetAmount > 0 && winAmount > 0)
            { 
                
                currentWinNumber = winNumber;
                Debug.LogError("currentWinNumber " + currentWinNumber);
                if (SixteenCardsSocketController.Instance.is_x_excuted.Equals(Constants.zero))
                {
                    try
                    {
                        int x = int.Parse(SixteenCardsSocketController.Instance.win_price.Replace("x", ""));
                        Debug.Log("before updating win amount " + winAmount);
                        winAmount *= x;
                        Debug.Log("after updating win amount " + winAmount);
                        StartCoroutine(WinnerHotlistUpdate());
                        Debug.Log("after calling hotlist");
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.ToString());
                    }
                }
                currentWinAmount = winAmount;
                int index = bettingPosition.IndexOf(currentWinNumber);
                Debug.LogError("current win number " + currentWinNumber + " winnumber index " + index + " winamount count" + this.winAmount.Count);
                this.winAmount[index] = currentWinAmount;
                Debug.Log("before showing win popup");*/
                // StartCoroutine(ShowWinPopUp(currentWinAmount.ToString()));
                // StartCoroutine(GameDataInsert(currentWinAmount, "win"));
                // StartCoroutine(GameDataInsert(winAmount, "win"));               
          //  }
           /* currentWinAmount = winAmount;    
            StartCoroutine(GameDataInsert(currentWinAmount, "win"));
            totalWinText.text = winAmount.ToString();
            winAmountstored=winAmount.ToString();
            totalUserCoins = totalUserCoins + winAmount;
            // if(currentWinAmount >= 1000)
            //     _heavyCoinsWinEffect.Play();
            userCoinsText.text = totalUserCoins.ToString();
            UserCoins = totalUserCoins;
            if(winAmount>1 &&  winAmount==1000) {
                Debug.Log("Winanimation added on win amount"+winAmount);
                StartCoroutine(winEffect12card(0.8f));
            }
            else*/
               // StartCoroutine(ShowWinPopUp(winAmount.ToString()));
           /* if (winAmount > 1 || winAmount < 999)
            {
                //SoundController.Instance.PlayWinClickSound();
                 StartCoroutine(ShowWinPopUp(winAmount.ToString()));
            }*/
           /* if (winAmount > 1 || winAmount < 999)
{
    if (winAmount > 0)
    {
        StartCoroutine(ShowWinPopUp(winAmount.ToString()));
    }
    else
    {
        Debug.Log("Win amount is 0, not showing win popup...lal h");
    }
}
        }*/


       /* public string winAmountstored;
        //showwinamount function change by shivamfusion07 for mode xwn
         public void ShowWinAmount(long amount)
        {
            long winAmount = amount * 14;
            if (winAmount == 0)
                return;
           // long winAmount = 0;
          //  winAmount = amount * 10;
          //  if(winAmount <= 0)
              //  return;
             if (SixteenCardsSocketController.Instance.is_x_excuted.Equals(Constants.zero))
            {
                 try
                    {
                        int x = int.Parse(SixteenCardsSocketController.Instance.win_price.Replace("x", ""));
                        Debug.Log("before updating win amount " + winAmount);
                        winAmount *= x;
                        Debug.Log("after updating win amount " + winAmount);
                        StartCoroutine(WinnerHotlistUpdate());
                        Debug.Log("after calling hotlist");
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.ToString());
                    }
            }
           // StartCoroutine(GameDataInsert(winAmount, "win"));
             StartCoroutine(GameDataInsert(winAmount, "win"));
            //WinAmount.text = winAmount.ToString();
           winAmountstored=winAmount.ToString();
            totalWinText.text = winAmount.ToString();
            totalUserCoins = totalUserCoins + winAmount;
            userCoinsText.text = totalUserCoins.ToString();
            UserCoins = totalUserCoins;
            if(winAmount>=1000) {
                Debug.Log("Winanimation added on win amount"+winAmount);
                StartCoroutine(winEffect12card(0.8f));
            }
            else
               // StartCoroutine(ShowWinPopupCoroutine());
                 StartCoroutine(ShowWinPopUp(winAmount.ToString()));
            if (winAmount > 1 || winAmount < 999)
            {
                //SoundController.Instance.PlayWinClickSound();
               StartCoroutine(ShowWinPopUp(winAmount.ToString()));
            }
        }*/
        //added by shivamfusion07
        public string winAmountstored;

// Updated ShowWinAmount function
public void ShowWinAmount(long amount)
{
    long winAmount = amount * 14;
    if (winAmount == 0)
            {
                Debug.Log("bet insert but not win user losse......");
                StartCoroutine(GameDataInsert(0, "loose"));
                //canBet = true;
                ////Reset();
                //canBet = false;
                
                return;
            }

    if (SixteenCardsSocketController.Instance.is_x_excuted.Equals(Constants.zero))
    {
        try
        {
            int x = int.Parse(SixteenCardsSocketController.Instance.win_price.Replace("x", ""));
            Debug.Log("Before updating win amount: " + winAmount);
            winAmount *= x;
            Debug.Log("After updating win amount: " + winAmount);
            StartCoroutine(WinnerHotlistUpdate());
            Debug.Log("After calling hotlist");
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }

    StartCoroutine(GameDataInsert(winAmount, "win"));
    Winamount.text = winAmount.ToString();
    winAmountstored = winAmount.ToString();
    totalWinText.text = winAmount.ToString();
    totalUserCoins = totalUserCoins + winAmount;
    userCoinsText.text = totalUserCoins.ToString("#0.00");
    UserCoins = totalUserCoins;
if(winAmount==0)
    {
        Debug.Log("bet insert but not win user losse......");
        StartCoroutine(GameDataInsert(0, "loose"));
                canBet = true;
                Reset();
                canBet = false;
            }
    if (winAmount >= 1000)
    {
        Debug.Log("Win animation added for win amount: " + winAmount);
        StartCoroutine(PlayCoinEffectAndShowPopup(3.5f, 0.2f));
    }
    else
    {
        StartCoroutine(ShowWinPopupCoroutine(6f));
    }
}


       /* private IEnumerator winEffect12card(float fadeOutDuration)
    {
        _heavyWinAnimation.SetActive(true);
        // Animator animator;
        // float waitDuration = 0.0f;
        // animator = _heavyWinAnimation.GetComponent<Animator>();
        // if (animator != null)
        // {
        //     AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        //     if (clipInfo.Length > 0)
        //     {
        //         waitDuration = (clipInfo[0].clip.length / animator.GetFloat("AnimationSpeed")) - fadeOutDuration;
        //         Debug.LogWarning("FadeOutDuration" + waitDuration);
        //     }
        // }
        // if (waitDuration > 0.0f)
            yield return new WaitForSeconds(7f);
            _heavyWinAnimation.gameObject.SetActive(false);
            yield return new WaitForSeconds(2f);
 StartCoroutine(ShowWinPopUp(winAmount.ToString()));*/
          //  Debug.Log("win amount before win pop up.."+winAmountstored.ToString());
          /*  if(winAmountstored.ToInt32()>0)
            {
Debug.Log("win amount greater then 1000");
           
            }*/
        // Image image = _heavyWinAnimation.GetComponent<Image>();
        // if (image != null)
        // {
        //     image.DOFade(0, fadeOutDuration).OnComplete(() =>
        //     {
        //         Debug.Log("win pop up showing if win amount is greter then 1000");
        //         StartCoroutine(ShowWinPopupCoroutine());
        //         Color color = image.color;
        //         color.a = 1.0f;
        //         image.color = color;
        //     });
        // }
        // else
        // {
        //     Debug.LogError("Image Component not found on Win Animation");
        // }
   // }
  private IEnumerator PlayCoinEffectAndShowPopup(float coinEffectDuration, float popupDuration)
{
yield return new WaitForSeconds(1.7f);
    _heavyWinAnimation.SetActive(true); // Activate coin effect
    yield return new WaitForSeconds(coinEffectDuration); // Wait for coin effect duration
    _heavyWinAnimation.SetActive(false); // Deactivate coin effect
    Debug.Log("Coin effect deactivated. Showing win popup...");
   // WinPopUp.SetActive(true); // Show win popup
  //  yield return new WaitForSeconds(popupDuration); // Wait for popup duration
   StartCoroutine(ShowWinPopupCoroutine(popupDuration));
}

      public  IEnumerator ShowWinPopupCoroutine(float delay)
        {
            Debug.Log("win pop enter here.....");
            yield return new WaitForSeconds(delay);
            Winsound.Play();
            WinPopUp.SetActive(true);
            canBet = true;
            Reset();
            canBet = false;
            // Winamount.text = winAmount;
            yield return new WaitForSeconds(4);
            WinPopUp.SetActive(false);
            sixteencard_CardSelect.instance.totalbetdata=0; 

        }
//         public IEnumerator ShowWinPopupCoroutine(float delay)
// {
//     Debug.Log("win pop enter here.....");

//     if (sixteencard_CardSelect.instance.totalbetdata > 0)  // Check if totalbetdata is greater than zero
//     {
//         yield return new WaitForSeconds(delay);
//         Winsound.Play();
//         WinPopUp.SetActive(true);

//         yield return new WaitForSeconds(4);
//         WinPopUp.SetActive(false);
//         sixteencard_CardSelect.instance.totalbetdata = 0;  // Reset total bet data
//     }
//     else
//     {
//         Debug.Log("Total win is zero, popup not shown.");
//     }
// }


      /*  IEnumerator GameDataInsert(long winAmount, string winLoose)
        {
          GameData16card data= new GameData16card
          {
           win_Number = winNumber.ToString(),
            game_name = apiData.gameName,
            start_point = UserCoins,
            game_id = SixteenCardsSocketController.Instance.gameIdjson,         //SocketController.Instance.betData.roomId, // Game room ID as string
            win_Amount =  winAmount,
            draw_time="",
            bonus_spin= "",
            claim_status =1,
            playerId =int.Parse(SixteenCardsSocketController.Instance.joinRoomData.playerId),
            gameData = SixteenCardsSocketController.Instance.betData.cardValueSet,
            bet_ammount = totalBet.ToString(),//betamontwithoutbet, 
          };
            string jsonData = JsonConvert.SerializeObject(data); // More robust serialization
        Debug.Log("Serialized JSON: " + jsonData);
        // Prepare the request
        UnityWebRequest unityWebRequest = new UnityWebRequest(Constant.KIBaseURL + "game_data_insert", "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
        unityWebRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
        unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
        unityWebRequest.SetRequestHeader("Content-Type", "application/json");
        // Send the request
        yield return unityWebRequest.SendWebRequest();
        // Handle response
        if (unityWebRequest.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Request Successful");
            StartCoroutine(WinnerHotlistUpdate());
            try
            {
                Debug.LogError("Game_Data_Insert 16 card : " + unityWebRequest.downloadHandler.text);
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }
        else
        {
            Debug.Log("Request Failed");
            try
            {
                Debug.LogError("shivam"+unityWebRequest.error);
                Debug.LogError("nhichal rha"+unityWebRequest.downloadHandler.text);
            }
            catch (Exception e)
            {
                Debug.LogError("ghlega"+e.ToString());
            }
        }
    }*/



       /* IEnumerator GameDataInsert(long winAmount, string winLoose)
        {
            Debug.LogError(winLoose + "/" + winAmount);
            WWWForm form = new WWWForm();
            form.AddField(Constants.playerId, SixteenCardsSocketController.Instance.joinRoomData.playerId);
            form.AddField(Constants.winNmuber, winNumber);
            form.AddField(Constants.winAmmount, winAmount.ToString());
            form.AddField(Constants.gameName, apiData.gameName);
            Debug.Log(apiData.gameName + "gamename");
            form.AddField(Constants.betAmmount, totalBetAmount.ToString());
            form.AddField(Constants.winLoose, winLoose);
            form.AddField(Constants.gameId, SixteenCardsSocketController.Instance.gameIdjson);
            form.AddField(Constants.startPoint, UserCoins.ToString());
            form.AddField(Constants.betting_time, JsonConvert.SerializeObject(bettingTime));
            form.AddField(Constants.betting_position, JsonConvert.SerializeObject(bettingPosition));
            form.AddField(Constants.betting_amount, JsonConvert.SerializeObject(bettingAmount));
            form.AddField(Constants.win_amount, JsonConvert.SerializeObject(this.winAmount));
            UnityWebRequest unityWebRequest = UnityWebRequest.Post(apiData.gameDataInsertApi, form);
            yield return unityWebRequest.SendWebRequest();
            if (unityWebRequest.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("SSSSS");
                  StartCoroutine(WinnerHotlistUpdate());
                try
                {
                    Debug.LogError(unityWebRequest.downloadHandler.text);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
            else
            {
                Debug.Log("SSSSS");
                try
                {
                    Debug.LogError(unityWebRequest.error);
                    Debug.LogError(unityWebRequest.downloadHandler.text);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
            //StartCoroutine(cardHistoryDeck.GetGameHistoryData());
        }*/
        //gamedtaainsertnewfunctionupdated\
        IEnumerator GameDataInsert(long winAmount, string winLoose)
        {
          GameData16card data= new GameData16card
          {
           win_Number = winNumber.ToString(),
            game_name = apiData.gameName,
            start_point = UserCoins,
            game_id = SixteenCardsSocketController.Instance.gameIdjson,         //SocketController.Instance.betData.roomId, // Game room ID as string
            win_Amount =  winAmount,
            draw_time="",
            bonus_spin= "",
            claim_status =1,
            playerId =int.Parse(SixteenCardsSocketController.Instance.joinRoomData.playerId),
            gameData = SixteenCardsSocketController.Instance.betData.cardValueSet,
            bet_ammount = totalBet.ToString(),//betamontwithoutbet,
          };
            string jsonData = JsonConvert.SerializeObject(data); // More robust serialization
        Debug.Log("Serialized JSON: " + jsonData);
        // Prepare the request
        UnityWebRequest unityWebRequest = new UnityWebRequest(Constant.KIBaseURL + "game_data_insert", "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
        unityWebRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
        unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
        unityWebRequest.SetRequestHeader("Content-Type", "application/json");
        // Send the request
        yield return unityWebRequest.SendWebRequest();
        // Handle response
        if (unityWebRequest.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Request Successful");
            StartCoroutine(WinnerHotlistUpdate());
            try
            {
                Debug.LogError("Game_Data_Insert 16 card : " + unityWebRequest.downloadHandler.text);
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }
        else
        {
            Debug.Log("Request Failed");
            try
            {
                Debug.LogError("shivam"+unityWebRequest.error);
                Debug.LogError("nhichal rha"+unityWebRequest.downloadHandler.text);
            }
            catch (Exception e)
            {
                Debug.LogError("ghlega"+e.ToString());
            }
        }
    }
        public IEnumerator LiveDataInsert()
        {

            WWWForm form = new WWWForm();

            form.AddField(Constants.winNmuber, winNumber);
            form.AddField(Constants.gameName, "16cards");
            form.AddField(Constants.win_amount, currentWinAmount.ToString());
            Debug.Log(winNumber + "winNumberLiveData");
            UnityWebRequest unityWebRequest = UnityWebRequest.Post("https://mplgames.in/admin/api/live-data", form);
            yield return unityWebRequest.SendWebRequest();
            if (unityWebRequest.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("sucess LivedataInsert");
            }
            else
            {
                Debug.LogError(unityWebRequest.error + "fail LivedataInsert");
            }
        }
        //Sixteen_cards.instance.apiData.winnerHotListUpdateApi
        //apiData.hotListApi
        IEnumerator WinnerHotlistUpdate()
        {
            WWWForm form = new WWWForm();
            // form.AddField(Constants.gameName, apiData.gameName);
            form.AddField(Constants.playerId, SixteenCardsSocketController.Instance.joinRoomData.playerId);
            UnityWebRequest unityWebRequest = UnityWebRequest.Post(Constant.KIBaseURL + "winner-hotlist-execution", form);
            yield return unityWebRequest.SendWebRequest();
            if (unityWebRequest.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    Debug.LogError(unityWebRequest.downloadHandler.text);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
            else
            {
                try
                {
                    Debug.LogError(unityWebRequest.error);
                    Debug.LogError(unityWebRequest.downloadHandler.text);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
        }

        public void Initialize(string userName, long totalCoins)
        {
            _token = PlayerPrefs.GetString(Constants.token);
            UserCoins = totalUserCoins = totalCoins;
            userCoinsText.text = totalCoins.ToString("#0.00");
            userNameText.text = userName;
            Debug.Log("user name text in game showing:"+userName);
            cardHistoryDeck.Initialize(userName);
            //StartCoroutine(cardHistoryDeck.GetGameHistoryData());
        }

        public void CheckForRepeatButton()
        {
            if (totalBet == 0)
            {
                if (ButtonHandlerss.TrueForAll(x => x.CheckifLastSavedDataAvailable() == false))
                {
                   // removeButtonObj.SetActive(true);
                    //  repeatButtonObj.SetActive(false);
                    repeatButton.interactable=false;
                }
                else
                {
                  //  removeButtonObj.SetActive(true);
                    //  repeatButtonObj.SetActive(true);
                    repeatButton.interactable=true;
                }
            }
            else
            {
              //  removeButtonObj.SetActive(true);
                // repeatButtonObj.SetActive(false);
                repeatButton.interactable=false;
            }
        }

          public string AllBetData()
        {
            string betData = null;
            List<BetDataHandlers> newBetDatas = new List<BetDataHandlers>();
            winHandlerss.ForEach(x => newBetDatas.Add(x.GetBetData()));

            betData = JsonConvert.SerializeObject(newBetDatas);
             userbetdata = betData;
            Debug.Log(betData + "betdata");

            return betData;
        }

      public void SetBetData(List<JeetoJoker.CardValueSelect> cardValueSet)
        {
            Debug.Log("set bet data call hua..."+ cardValueSet.Count);
            totalBetAmount = 0;
            
            int count = cardValueSet.Count;
            Debug.Log("group card number:" +sixteencard_CardSelect.instance.groupcardNumber);
               // IWinHandlers winHandler = winHandlerss.Find(x => (x as sixteencard_CardSelect).groupcardNumber == cardValueSet[2].card16);
           // Debug.Log("data of win handler" + winHandler);
            for (int i = 0; i < count; i++)
            {
                Debug.Log("..............."+i);
                 IWinHandlers winHandler = winHandlerss.Find(x => (x as sixteencard_CardSelect).groupcardNumber == cardValueSet[i].card);

                if (winHandler != null)
                {
                    Debug.Log("aa gya iske andar..");
                    long betAmount = (winHandler as sixteencard_CardSelect).GetTotalBetAmount();
                    totalBetAmount += betAmount;
                    cardValueSet[i].value = Convert.ToInt32(betAmount);
                }
                else
                {
                    Debug.Log("win handler null....");

                }
            }

            Debug.Log("total amount in 16 card:" + totalBetAmount);
            Debug.Log("total count lal h in 16 card:" + count);
        }
       /* public void SetBetData(List<khelojeetonew.CardValueSelect> cardValueSet)
{
    totalBetAmount = 0;
    int count = cardValueSet.Count;
    
    for (int i = 0; i < count; i++)
    {
        IWinHandlers winHandler = winHandlerss.Find(x => (x as CardSelect).groupcardNumber == cardValueSet[i].card);
        
        if (winHandler != null)
        {
            long betAmount = (winHandler as CardSelect).GetTotalBetAmount();
            
            // Check if betAmount is greater than 0 before assigning
            if (betAmount > 0)
            {
                cardValueSet[i].value = Convert.ToInt32(betAmount);
                totalBetAmount += betAmount;
            }
            else
            {
                Debug.LogWarning($"No bet placed for card: {cardValueSet[i].card}");
            }
        }
        else
        {
            Debug.LogWarning($"winHandler not found for card: {cardValueSet[i].card}");
        }
    }
}*/

         //changes by shivamfusion
       /*  public void SetBetDataBET(List<khelojeetonew.CardValueSelect> cardValueSet)
        {

            // SocketController.Instance.SetCompleteData(cardValueSet);
           SixteenCardsSocketController.Instance.SetCompleteData(cardValueSet);
        }*/

        private Dictionary<string, string> numberToAlphabet = new Dictionary<string, string>
    {
        {"11", "JH"},
        {"12", "JS"},
        {"13", "JD"},
        {"14", "JC"},
        {"21", "QH"},
        {"22", "QS"},
        {"23", "QD"},
        {"24", "QC"},
        {"31", "KH"},
        {"32", "KS"},
        {"33", "KD"},
        {"34", "KC"}
    };
        private Dictionary<string, string> alphabetToNumber = new Dictionary<string, string>
{
    {"JH", "11"},
    {"JS", "12"},
    {"JD", "13"},
    {"JC", "14"},
    {"QH", "21"},
    {"QS", "22"},
    {"QD", "23"},
    {"QC", "24"},
    {"KH", "31"},
    {"KS", "32"},
    {"KD", "33"},
    {"KC", "34"}
};

        public void StartSpinning(int cardData)
        {
            if (!isSpining)
            {
                winNumber = cardData;

                Debug.Log("LevelManager: StartSpinning");
                cardIdLive = cardData / 10;
                suitIDLive = cardData % 10;
                Debug.Log("card id in start spinning of socket slot:"+cardIdLive);
                Debug.Log("suit id in start spinning of socket slot:"+suitIDLive);
                if (cardIdLive != 0 && suitIDLive != 0)
                {
                    Debug.Log("if");
                    //startSpinWheelWithData();
                    StartCoroutine(startSpinWheelWithData());
                }
                else
                {
                    Debug.Log("else");
                    wheelIsSpinning = true;
                    StartSpinWheelWithoutData();
                }
                isSpining = true;
                infoPanel.SetActive(false);
                SetCenterWheelAnimation(true);
                SetCenterWheelTexts(false);

                //if (firstTime)
                //{
                //    firstTime = false;
                //    StartCoroutine(checkWheelSpinAndStop());
                //}
            }
        }
        private void StartSpinWheelWithoutData()
        {
            WheelSpinFoo.spinWheel();
        }
        public IEnumerator startSpinWheelWithData()
        {
            Debug.LogError("startSpinWheelWithData");
            //WheelSpinFoo.StartSpinningWithData(cardIdLive, suitIDLive);

            cardImage.gameObject.SetActive(false);
            suiteImage.gameObject.SetActive(false);
             Xanimation.SetActive(true);
           /* if (SixteenCardsSocketController.Instance.is_x_excuted.Equals(Constants.zero))
            {
                WithoutXanimation.SetActive(false);
                Xanimation.SetActive(true);
            }
            else
            {
                Xanimation.SetActive(false);
                WithoutXanimation.SetActive(true);
            }*/
            WheelSpinFoo.spinWheel();
            yield return new WaitForSeconds(2.5f);
            WheelSpinFoo.faaa(cardIdLive, suitIDLive, () =>
            {
               StartCoroutine(SetCardImage(cardIdLive, suitIDLive));
                Debug.Log("card image set from here..innere wheel"+cardIdLive);
                Debug.Log("card image set from here..innere wheel"+suitIDLive);
            });
              Debug.Log("winning data of 16 card shivamfusion...:"+sixteencard_CardSelect.instance.totalbetdata);
             // ShowWinAmount(sixteencard_CardSelect.instance.totalbetdata);
        }
    public IEnumerator SetCardImage(int cardIdLive, int suitIDLive)
        {

             yield return new WaitForSeconds(4f);
            Xanimation.SetActive(false);
            ShowXMultiplierText();
                // yield return new WaitForSeconds(2f);  
  Debug.Log("set card image function call");
            StartCoroutine(setcardresult());
             string itemCard = SuperJokerConstants.CARDNAMEPREFIX + cardIdLive;
             string itemSuite = SuperJokerConstants.SUITENAMEPREFIX + suitIDLive;
             cardImage.sprite = Resources.Load<Sprite>("SJ_Resources/" + itemCard);
             suiteImage.sprite = Resources.Load<Sprite>("SJ_Resources/" + itemSuite);
             cardImage.gameObject.SetActive(true);
             suiteImage.gameObject.SetActive(true);
        }
        private IEnumerator setcardresult()
        {
            yield return new WaitForSeconds(2.8f);
            string itemCard = SuperJokerConstants.CARDNAMEPREFIX + cardIdLive;
            string itemSuite = SuperJokerConstants.SUITENAMEPREFIX + suitIDLive;
            Xanimation.SetActive(false);
            
            cardImage.sprite = Resources.Load<Sprite>("SJ_Resources/" + itemCard);
            suiteImage.sprite = Resources.Load<Sprite>("SJ_Resources/" + itemSuite);
            cardImage.gameObject.SetActive(true);
            suiteImage.gameObject.SetActive(true);
            if(multiplierObject)
            multiplierObject.SetActive(true);
            WithoutXanimation.SetActive(false);
        }

        private void ShowXMultiplierText() {
            string multiplier = "";
            // Retrieve the multiplier value
            if (SixteenCardsSocketController.Instance.is_x_excuted.Equals(Constants.zero))
            {
               multiplier = SixteenCardsSocketController.Instance.win_price;  
            }

            // Check if the multiplier is empty or "1x"
            if (string.IsNullOrEmpty(multiplier) || multiplier == "1x")
            {
                multiplierObject.SetActive(false); // Hide the multiplier object
                Debug.Log("Multiplier is either empty or 1x, not showing.");
            }
            else
            {
                multiplierObject.SetActive(true); // Show the multiplier object
                multiplierText.text = multiplier; // Update the text with the multiplier value
                Debug.Log("multiplier in center:"+multiplierText.text);
                if(totalBetAmount == 0)
                    StartCoroutine(WinnerHotlistUpdate());
            }
        }

        public void SetCenterWheelAnimation(bool state)
        {
            cardFlashAnimation.SetActive(state);
            if (!state)
            {
                // centerWheelCardAndSuite.SetCardImage((int)newCardData.x, (int)newCardData.y);
                SetSelectionWheelLights(true);
            }
            else
            {
                //SetSelectionWheelLights(false);
                multiplierObject.SetActive(false);
            }
        }

        public void SetCenterWheelTexts(bool state)
        {
            //centerWheelCardAndSuite.gameObject.SetActive(state);
            // N.gameObject.SetActive(state);
        }
        public void SetSelectionWheelLights(bool state)
        {
            wheelLights.SetActive(state);
        }
    }


    [System.Serializable]
    public class BetButtonss
    {
        public int id;
        public Transform buttonTransfrom;
        public int amount;
    }

    public interface ButtonHandlerss
    {
        public void Clear();
        public void Repeat();
        public void DoubleUp(int removeCountIndex);
        public void SavePrevRound();
        public bool CheckifLastSavedDataAvailable();
        public long GetPrevRoundTotalSum();
        public long GetRoundTotalSum();
    }

    public interface IRemoveHandlers
    {
        public static Action<int> refresh;
        public void Remove();
    }

    public interface IWinHandlers
    {
        public bool OnWin(int outerId, int innerId);
        public BetDataHandlers GetBetData();
    }

    [System.Serializable]
    public class BetDataHandlers
    {
        public string Id;
        public long amount;
    }
    public class GameData16card
{
    public string win_Number { get; set; }
    public string game_name { get; set; }
    public long start_point { get; set; }
    public string game_id { get; set; }
    public long win_Amount { get; set; }
     public string draw_time;
    public string bonus_spin;
    public int claim_status;
    public int playerId { get; set; }  // Changed to int as per your requirement
    public List<JeetoJoker.CardValueSelect> gameData { get; set; }
    public string bet_ammount { get; set; }  // Changed to long to match your JSON format
}



}
