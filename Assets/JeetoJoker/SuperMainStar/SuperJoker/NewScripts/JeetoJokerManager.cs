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
using System.Threading.Tasks;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Runtime.InteropServices;

namespace khelojeetonew
{
    public class JeetoJokerManager : MonoBehaviour
    {

        public enum GameType
        {
            JeetoJoker,
            Cards16,
            Cards12_IPL
        }
        [SerializeField] public Text betacceptedtext;
        public string userbetdata;
        [SerializeField] private GameType _gameType;
        [SerializeField] private Text WinAmount;
        [SerializeField] private Text totalBetAmountText;

        [SerializeField] private GameObject WinPopUp;
        [SerializeField] private Sprite cardSelected;
        [SerializeField] private Sprite cardDiselect;

        [SerializeField] private List<BetButtons> betButtons;

        [SerializeField] private long UserCoins = 10000;

        [SerializeField] public Text userCoinsText;
        [SerializeField] private Text userCoinsText_COPY;

        [SerializeField] public Text currentTotalBetText;
        [SerializeField] private Text totalWinText;

        [SerializeField] public Text userNameText;

        //  [SerializeField] private GameObject removeButtonObj;
        // [SerializeField] public GameObject repeatButtonObj;
        [SerializeField] public Button Repeatbutton;
        [SerializeField] private SpriteRenderer cardImage;
        [SerializeField] private SpriteRenderer suiteImage;
        [SerializeField] private TMP_Text multiplierText;
        [SerializeField] private GameObject multiplierObject;


        [SerializeField] private Text messagePopupText;
        [SerializeField] private GameObject messagePopup;

        [HideInInspector] public List<ButtonHandlers> ButtonHandlers = new List<ButtonHandlers>();
        [HideInInspector] public List<List<IRemoveHandler>> removeHandlers = new List<List<IRemoveHandler>>();
        [HideInInspector] public List<IWinHandler> winHandlers = new List<IWinHandler>();

        [HideInInspector] public bool canBet = false;

        [SerializeField] private GameObject loadingPanel;
        [SerializeField] private GameObject fakeUserPopup;

        [SerializeField] private APIData _apiData;

        [SerializeField] private CardHistoryDeck cardHistoryDeck;

        public APIData apiData => _apiData;

        public GameType gameType => _gameType;

        public int removeCount = 0;

        private string winNumber;
        private long totalBetAmount = 0;

        private BetButtons selectedBetbutton;
        public long totalBet = 0;
        private long totalUserCoins = 0;

        public Sprite CardSelected { get => cardSelected; }
        public Sprite CardDiselect { get => cardDiselect; }
        public BetButtons SelectedBetbutton { get => selectedBetbutton; }

        public static JeetoJokerManager instance;
        private bool isSpining = false;
        public Foo WheelSpinFoo;
        int cardIdLive;
        int suitIDLive;
        private bool wheelIsSpinning = false;
        // public HistoryCard centerWheelCardAndSuite;
        public GameObject infoPanel;
        public GameObject cardFlashAnimation;
        public GameObject wheelLights;
        public GameObject Xanimation;
        public Text drawTime;

        private string _token;

        private int noBetSoundIndex;
        public int placeBetSoundIndex { get; private set; }
        public Text dateTimeText;

        int winCount = 0;
        [SerializeField]
        private ParticleSystem _heavyCoinsWinEffect;
        [SerializeField]
        private GameObject _heavyWinAnimation;
        public AudioSource Winsound12;

        public GameObject blast;
        private void Awake()
        {
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
        }
        /*
        #if UNITY_EDITOR
                private void OnValidate()
                {
                    List<string> allDefines = new List<string>();
                    allDefines.Add(_gameType.ToString());
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(
                    EditorUserBuildSettings.selectedBuildTargetGroup,
                    string.Join(";", allDefines.ToArray()));
                }
        #endif
        */

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



        private void Update()
        {
            // Get the current date and time
            DateTime currentDateTime = DateTime.Now;
            // Format the date and time to display (e.g., "MM/dd/yyyy HH:mm:ss")
            string formattedDateTime = currentDateTime.ToString("dd-MM-yyyy HH:mm:ss tt");
            dateTimeText.text = formattedDateTime;
            // Assuming userCoinsText.text holds an integer value as a string
            //int balance = int.Parse(userCoinsText.text);

            // Format it to show two decimal places
            //userCoinsText_COPY.text = balance.ToString("#0.00");
            userCoinsText_COPY.text = userCoinsText.text;
            //DateTime drawDateTime;

        }



        public void Reset()
        {
            SelectBet(0);
            Clear();
            totalWinText.text = "0";
            totalUserCoins = UserCoins;
            userCoinsText.text = totalUserCoins.ToString("#0.00");
            TimerControllerNew.inst.SetDefaultColor();
            canBet = true;
        }
        public void Logout()
        {
            SocketController.Instance.Leave();
            SceneManager.LoadSceneAsync("DashBoard");
            // StartCoroutine(LogOutRutine(PlayerPrefs.GetString(Constants.token),false));
        }

        public void LogoutSilent()
        {
            // StartCoroutine(LogOutRutine(PlayerPrefs.GetString(Constants.token),true));
        }

        public IEnumerator LogOutRutine(string token, bool isSilent)
        {
            loadingPanel.SetActive(true);
            UnityWebRequest unityWebRequest = UnityWebRequest.Get(JeetoJokerManager.instance.apiData.logOutApi);
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
                            SocketController.Instance.Leave();
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
            winNumber = "JH";
            totalBetAmount = 50;
            SocketController.Instance.roomIdData.roomId = "roomfake";
            Debug.Log("game 1");
            // StartCoroutine(GameDataInsert(50000, "win"));
        }

        public void ShowMessage(string message)
        {
            messagePopupText.text = message;
            messagePopup.SetActive(true);
        }

        public Button coinbutton;
        public Button[] coinswithamt;
        public void SelectBet(int id)
        {
            betButtons.ForEach(x => x.buttonTransfrom.localScale = Vector2.one);

            selectedBetbutton = betButtons.Find(x => x.id == id);

            // selectedBetbutton.buttonTransfrom.localScale =coinbutton.transform.localScale; //Vector2.one * 1.0f;
            if (coinbutton != null)
            {
                Vector3 targetScale = coinbutton.transform.localScale;

                foreach (Button button in coinswithamt)
                {
                    button.transform.localScale = targetScale;
                }
            }
            else
            {
                Debug.LogWarning("Target object is not assigned!");
            }
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
        public void ClearBet()
        {
            if (canBet == false)
            {
                return;
            }

            removeHandlers.Clear();
            removeCount = 0;
            ButtonHandlers.ForEach(x => x.Clear());
            totalBet = 0;

            currentTotalBetText.text = totalBet.ToString();

            if (ButtonHandlers.TrueForAll(x => x.CheckifLastSavedDataAvailable() == false))
            {
                //  removeButtonObj.SetActive(true);
                // repeatButtonObj.SetActive(false);
                Repeatbutton.interactable = false;
            }
            else
            {
                //  removeButtonObj.SetActive(false);
                //  repeatButtonObj.SetActive(true);
                Repeatbutton.interactable = true;
            }

        }
        //new clear function added by shivamfusion07
        public void Clear()
        {
            if (canBet == false)
            {
                return;
            }

            removeHandlers.Clear();
            removeCount = 0;
            ButtonHandlers.ForEach(x => x.Clear());
            totalBet = 0;
            currentTotalBetText.text = totalBet.ToString();
            totalUserCoins = UserCoins;
            userCoinsText.text = totalUserCoins.ToString("#0.00");

            if (ButtonHandlers.TrueForAll(x => x.CheckifLastSavedDataAvailable() == false))
            {
                // removeButtonObj.SetActive(true);
                // repeatButtonObj.SetActive(false);
                Repeatbutton.interactable = false;
            }
            else
            {
                /* removeButtonObj.SetActive(false);
                 repeatButtonObj.SetActive(true);*/
                Repeatbutton.interactable = true;
            }
        }


        /* public void Clear()
         {
             if (canBet == false)
             {
                 return;
             }

             removeHandlers.Clear();
             removeCount = 0;
             ButtonHandlers.ForEach(x => x.Clear());
               if (BET.instance.finalPlayValue == 0 && totalBet > 0)
             {
                 totalUserCoins = totalUserCoins + totalBet;
                 userCoinsText.text = totalUserCoins.ToString();
             }
             else
             {
                 userCoinsText.text = totalUserCoins.ToString();
             }
             totalBet = 0;
             currentTotalBetText.text = totalBet.ToString();

             // removeButtonObj.SetActive(false);
             // repeatButtonObj.SetActive(true);
             Repeatbutton.interactable = true;
              totalBet = 0;
             currentTotalBetText.text = totalBet.ToString();
             totalUserCoins = UserCoins;
             userCoinsText.text = totalUserCoins.ToString();

             /*if (ButtonHandlers.TrueForAll(x => x.CheckifLastSavedDataAvailable() == false))
             {
                 removeButtonObj.SetActive(true);
                 repeatButtonObj.SetActive(false);
             }
             else
             {
                 removeButtonObj.SetActive(false);
                 repeatButtonObj.SetActive(true);
             }*/
        // }
        public void Betclearforprint()
        {
            if (canBet == false)
            {
                return;
            }
            Debug.Log("bet clear after sending in api test card data");
            removeHandlers.Clear();
            removeCount = 0;
            ButtonHandlers.ForEach(x => x.Clear());
            totalBet = 0;
            currentTotalBetText.text = totalBet.ToString();
            if (ButtonHandlers.TrueForAll(x => x.CheckifLastSavedDataAvailable() == false))
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
            }
        }

        public void Remove()
        {
            if (canBet == false)
            {
                return;
            }

            if (removeHandlers.Count > 0)
            {
                List<IRemoveHandler> re = removeHandlers[removeCount - 1];
                re.ForEach(x => { if (x != null) x.Remove(); });
                re.Clear();

                removeHandlers.RemoveAt(removeCount - 1);
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
            ButtonHandlers.ForEach(x => repeatSum = repeatSum + x.GetPrevRoundTotalSum());
            if (repeatSum <= totalUserCoins)
            {
                ButtonHandlers.ForEach(x => x.Repeat());

                //  removeButtonObj.SetActive(true);
                //  repeatButtonObj.SetActive(false);
                Repeatbutton.interactable = false;
            }
        }

        public void DoubleUp()
        {
            if (canBet == false)
            {
                return;
            }

            long repeatSum = 0;
            ButtonHandlers.ForEach(x => repeatSum = repeatSum + x.GetRoundTotalSum());
            if (repeatSum <= totalUserCoins)
            {
                removeHandlers.Add(new List<IRemoveHandler>());
                ButtonHandlers.ForEach(x => x.DoubleUp(removeCount));
                removeCount += 1;
            }
        }

        public void TimerEnd()
        {
            SoundControllerJeeto.Instance.PlayOneShot(SoundControllerJeeto.SoundType.NoBet, noBetSoundIndex);
            canBet = false;
            betButtons.ForEach(x => x.buttonTransfrom.localScale = Vector2.one);
        }

        public void WithoutwinXexecute()
        {

            string multiplier = "";
            if (SocketController.Instance.is_x_excuted.Equals(Constants.zero))
            {
                multiplier = SocketController.Instance.win_price;
            }
            if (string.IsNullOrEmpty(multiplier) || multiplier == "1x")
            {
                multiplierObject.SetActive(true);

                multiplierText.gameObject.SetActive(false);
            }
            else
            {
                //multiplierObject.SetActive(false);
                multiplierText.gameObject.SetActive(true);
                multiplierText.text = multiplier;
                StartCoroutine(WinnerHotlistUpdate());

            }
        }

        public Sequence OnWin(int outerId, int innerId)
        {
            Debug.Log("inside jjm onwin");
            // string multiplier = "";
            // if (SocketController.Instance.is_x_excuted.Equals(Constants.zero)) 
            // {
            //     multiplier = SocketController.Instance.win_price;
            // }
            // if (string.IsNullOrEmpty(multiplier) || multiplier == "1x")
            // {
            //     multiplierObject.SetActive(false);
            // }
            // else
            // {
            //     multiplierObject.SetActive(true);
            //     multiplierText.text = multiplier;
            // }
            ButtonHandlers.ForEach(x => x.SavePrevRound());
            Debug.LogError("AllBetData() " + AllBetData());
            Sequence seq = DOTween.Sequence();
            // seq.AppendInterval(4f);
            seq.AppendCallback(() =>
            {
                int count = winHandlers.Count;

                for (int i = 0; i < count; i++)
                {
                    if (winHandlers[i].OnWin(outerId, innerId))
                        winCount++;
                }

                Debug.Log("win o and bet emited but not winuser...");
                /*  if (winCount == 0 && totalBet > 0)
                   {
                     StartCoroutine(GameDataInsert(0, "loose"));
                       Debug.Log("game 2");
                   }*/
                //cardHistoryDeck.PushCardData(outerId, innerId);
                APICardHistory.Instance.CardHistory();
                StartCoroutine(GetUserDetails(seq));

                canBet = true;
                //Reset();
                canBet = false;
                //winHandlers.ForEach(x => x.OnWin(outerId, innerId));
            }
            );
            // seq.AppendInterval(10f);
            seq.AppendCallback(() =>
            {
                canBet = true;
                SoundControllerJeeto.Instance.PlayOneShot(SoundControllerJeeto.SoundType.PlaceBet, placeBetSoundIndex);
            });
            //seq.AppendCallback(() => Clear());
            seq.AppendCallback(() => totalWinText.text = "0");
            Debug.Log("win here..");
            //seq.AppendCallback(() => SelectBet(0));
           

            Debug.Log("draw time reset");
            isSpining = false;
            Debug.Log("timer start from jeeto joker");

            return seq;
        }

        private IEnumerator GetUserDetails(Sequence seq)
        {
            //loadingPanel.SetActive(true);
            seq.Pause();
            yield return new WaitForSeconds(2);
            UnityWebRequest unityWebRequest = UnityWebRequest.Get(JeetoJokerManager.instance.apiData.userDetailsApi);
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
                        int.TryParse(loginData.walletBlance, out int coins);
                        UserCoins = totalUserCoins = coins;
                        userCoinsText.text = coins.ToString("#0.00");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
            seq.Play();

        }


        /*public void ShowWinAmount(long amount)
          {
              if (winCount <= 0)
                  return;
              long winAmount = 0;
              winAmount = amount * 10;
              if(winAmount == 0)
                  return;
              if (SocketController.Instance.is_x_excuted.Equals(Constants.zero))
              {
                  try
                  {
                      int x = int.Parse(SocketController.Instance.win_price.Replace("x", ""));
                      winAmount *= x;
                      StartCoroutine(WinnerHotlistUpdate());
                  }
                  catch (Exception e)
                  {
                      Debug.LogError(e.ToString());
                  }
              }
              StartCoroutine(GameDataInsert(winAmount, "win"));
              WinAmount.text = winAmount.ToString();
              totalWinText.text = winAmount.ToString();
              totalUserCoins = totalUserCoins + winAmount;
              userCoinsText.text = totalUserCoins.ToString();
              UserCoins = totalUserCoins;
              if(winAmount>=1000) {
                  Debug.Log("Winanimation added on win amount"+winAmount);
                  StartCoroutine(winEffect12card(0.8f));
              }
              else
                  StartCoroutine(ShowWinPopupCoroutine());
              if (winAmount > 1 || winAmount < 999)
              {
                  //SoundController.Instance.PlayWinClickSound();
                  StartCoroutine(ShowWinPopupCoroutine());
              }
          }*/
        public void ShowWinAmount(long amount)
        {
            if (winCount <= 0)
                return;
            long winAmount = amount * 10;
            if (winAmount == 0)
            {
                Debug.Log("bet insert but not win user losse......");
                StartCoroutine(GameDataInsert(0, "loose"));
                return;
            }
            if (SocketController.Instance.is_x_excuted.Equals(Constants.zero))
            {
                try
                {
                    int x = int.Parse(SocketController.Instance.win_price.Replace("x", ""));
                    winAmount *= x;
                    StartCoroutine(WinnerHotlistUpdate());
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
            StartCoroutine(GameDataInsert(winAmount, "win"));

            WinAmount.text = winAmount.ToString();
            totalBetAmountText.text =totalBetAmount.ToString();
            totalWinText.text = winAmount.ToString();
            totalUserCoins += winAmount;
            userCoinsText.text = totalUserCoins.ToString("#0.00");
            UserCoins = totalUserCoins;
            if (winAmount == 0)
            {
                Debug.Log("bet insert but not win user losse......");
                StartCoroutine(GameDataInsert(0, "loose"));
            }
            if (winAmount >= 1000)
            {
                Debug.Log("Win animation added for win amount: " + winAmount);
                StartCoroutine(ShowCoinEffectAndPopup(3.5f, 0.2f)); // 7s coin effect, 4s popup
            }
            else
            {
                StartCoroutine(ShowWinPopupCoroutine(7f));
            }
        }
        private IEnumerator ShowCoinEffectAndPopup(float coinEffectDuration, float popupDuration)
        {
            yield return new WaitForSeconds(1.7f);
            _heavyWinAnimation.SetActive(true); // Activate coin effect
            yield return new WaitForSeconds(coinEffectDuration); // Wait for coin effect duration
            _heavyWinAnimation.SetActive(false); // Deactivate coin effect
            Debug.Log("Coin effect deactivated. Showing win popup...");
            // WinPopUp.SetActive(true); // Show win popup
            // yield return new WaitForSeconds(popupDuration); // Wait for popup duration
            StartCoroutine(ShowWinPopupCoroutine(popupDuration));
        }
        private IEnumerator ShowWinPopupCoroutine(float delay)
        {
            Debug.Log("win popup showing after win");
            yield return new WaitForSeconds(delay);
            Winsound12.Play();
            WinPopUp.SetActive(true);
            yield return new WaitForSeconds(5f);
            WinPopUp.SetActive(false);
            // }
            // else
            // {
            Debug.Log("Total win is zero, popup not shown.");
            // }
        }
        // private IEnumerator winEffect12card(float fadeOutDuration)
        // {
        //  _heavyWinAnimation.SetActive(true);
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
        /* yield return new WaitForSeconds(7f);
         _heavyWinAnimation.gameObject.SetActive(false);
         yield return new WaitForSeconds(2f);
StartCoroutine(ShowWinPopupCoroutine());
         Debug.Log("win amount before win pop up..");*/
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
        //  public List<CardValueSelect> gamedatacardselect;
        // public string betamontwithoutbet;
        IEnumerator GameDataInsert(long winAmount, string winLoose)
        {
            Debug.Log("enter here gameplay gamedata insert..");
            // if (totalBet > 0)
            // {
            Debug.Log("win number: " + winNumber);
            // Ensure 'num' is correctly fetched from the dictionary
            alphabetToNumber.TryGetValue(winNumber, out string num);
            /*if(winAmount == 0)
            {
            betamontwithoutbet="0";
            Debug.Log("win 0 bet insert with 0 bet amount");
            }*/
            //betamontwithoutbet =totalBetAmount.ToString(); //BET.instance.finalPlayValue.ToString();
            Debug.Log("bet amount succesfully inserted" + totalBetAmount.ToString());
            Debug.Log("win price  bonus spin:" + SocketController.Instance.win_price);
            // Create the GameData object with appropriate values
            GameData data = new GameData
            {
                win_Number = num ?? winNumber.ToString(), // Use 'num' if found, else winNumber as string
                game_name = apiData.gameName, // Game name from apiData
                start_point = UserCoins, // UserCoins (long)
                game_id = SocketController.Instance.gameidstore.ToString(),          //SocketController.Instance.betData.roomId, // Game room ID as string
                win_Amount = winAmount, // Win amount (long)
                draw_time = "",
                bonus_spin = SocketController.Instance.win_price,
                claim_status = 1,
                playerId = Convert.ToInt32(SocketController.Instance.joinRoomData.playerId), // Player ID (int)
                gameData = SocketController.Instance.betData.cardValueSet, // Already structured list of CardValueSelect
                bet_ammount = totalBet.ToString(),//betamontwithoutbet,   //totalBetAmount.ToString(), // Corrected spelling to 'bet_amount'
            };
            Debug.Log("bonusspindata 12 card:" + data.bonus_spin);
            Debug.Log("claim status 12 card:" + data.claim_status);
            /*List<CardValueSelect> gameData =  SocketController.Instance.betData.cardValueSet;
            // Debugging gameData structure
            if (gameData != null && gameData.Count > 0)
            {
                Debug.Log("GameData contains the following entries:");
                foreach (var cardValue in gameData)
                {
                    Debug.Log("Card: " + cardValue.card + ", Value: " + cardValue.value);
                }
            }
            else
            {
                Debug.LogError("GameData is either null or empty.");
            }*/
            List<CardValueSelect> gameData = SocketController.Instance.betData.cardValueSet;
            for (int i = 0; i < gameData.Count; i++)
            {
                // Convert the integer to a string to check its length
                string cardString = gameData[i].card.ToString();
                // If the card has 3 digits, reduce it to the last 2 digits
                if (cardString.Length > 2)
                {
                    // Keep only the last 2 digits and update the card value
                    gameData[i].card = int.Parse(cardString.Substring(1));
                }
                Debug.Log("Corrected Card Data: Card " + gameData[i].card + ", Value: " + gameData[i].value);
            }
            // Populate gameData with default values if needed
            // for (int i = 0; i <= 9; i++)
            // {
            //      gameData.Add(new CardValueSelect { card = i.ToString().PadLeft(2, '0'), value = 2 });
            //  }
            // Update gameData with actual bet amounts using winHandlers
            foreach (var gameDataEntry in gameData)
            {
                // Assuming winHandlers contains the relevant win data
                IWinHandler winHandler = winHandlers.Find(x => (x as CardSelect).groupcardNumber == gameDataEntry.card);
                if (winHandler != null)
                {
                    long betAmount = (winHandler as CardSelect).GetTotalBetAmount();
                    gameDataEntry.value = (int)betAmount; // Update the value with the bet amount
                }
                Debug.Log("Data GameData: Card " + gameDataEntry.card + ", Value: " + gameDataEntry.value);
            }
            // Serialize to JSON using Newtonsoft.Json instead of JsonUtility
            // string jsonData = JsonUtility.ToJson(data);
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
                    Debug.LogError("Game_Data_Insert : " + unityWebRequest.downloadHandler.text);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
            else
            {
                Debug.Log("Request Failed");

            }
        }

        IEnumerator WinnerHotlistUpdate()
        {
            WWWForm form = new WWWForm();
            form.AddField(Constants.playerId, SocketController.Instance.joinRoomData.playerId);
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

        public void Initialize(string userName, int totalCoins)
        {
            _token = PlayerPrefs.GetString(Constants.token);
            UserCoins = totalUserCoins = totalCoins;
            userCoinsText.text = totalCoins.ToString("#0.00");
            userNameText.text = userName;
            cardHistoryDeck.Initialize(userName);
        }

        public void CheckForRepeatButton()
        {
            if (totalBet == 0)
            {
                if (ButtonHandlers.TrueForAll(x => x.CheckifLastSavedDataAvailable() == false))
                {
                    //  removeButtonObj.SetActive(true);
                    //  repeatButtonObj.SetActive(false);
                    Repeatbutton.interactable = false;
                }
                else
                {
                    // removeButtonObj.SetActive(false);
                    //  repeatButtonObj.SetActive(true);
                    Repeatbutton.interactable = true;
                }
            }
            else
            {
                //removeButtonObj.SetActive(true);
                //  repeatButtonObj.SetActive(false);
                Repeatbutton.interactable = false;
            }
        }
        private void FixedUpdate()
        {
            AllBetData();
        }

        public string AllBetData()
        {
            string betData = null;
            List<BetDataHandler> newBetDatas = new List<BetDataHandler>();
            winHandlers.ForEach(x => newBetDatas.Add(x.GetBetData()));

            betData = JsonConvert.SerializeObject(newBetDatas);
            userbetdata = betData;
            // Debug.Log("user bet data:"+userbetdata);

            return betData;
        }

        public void SetBetData(List<CardValueSelect> cardValueSet)
        {
            totalBetAmount = 0;
            int count = cardValueSet.Count;
            for (int i = 0; i < count; i++)
            {

                IWinHandler winHandler = winHandlers.Find(x => (x as CardSelect).groupcardNumber == cardValueSet[i].card);
                Debug.Log("win handlwer data 12 card:" + winHandler);
                if (winHandler != null)
                {
                    long betAmount = (winHandler as CardSelect).GetTotalBetAmount();
                    totalBetAmount += betAmount;
                    Debug.Log("total bet...12 before:" + betAmount);
                    cardValueSet[i].value = Convert.ToInt32(betAmount);
                }
            }
            Debug.Log("total bet...12:" + totalBetAmount);
        }

        //changes by shivamfusion
        public void SetBetDataBET(List<CardValueSelect> cardValueSet)
        {

            SocketController.Instance.SetCompleteData(cardValueSet);
        }

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
                winNumber = cardData.ToString();

                cardIdLive = cardData / 10;
                suitIDLive = cardData % 10;
                Debug.Log("spinning wheel win:" + cardData.ToString());
                Debug.Log("card id live" + cardIdLive);
                Debug.Log("suit id" + suitIDLive);
                string cardDataAsString = cardData.ToString();
                if (numberToAlphabet.ContainsKey(cardDataAsString))
                {
                    winNumber = numberToAlphabet[cardDataAsString];
                    Debug.Log(winNumber + "winNumber");
                }

                if (cardIdLive != 0 && suitIDLive != 0)
                {
                    Debug.Log("if");
                    StartCoroutine(StartSpinWheelWithData());
                    Debug.Log("spin wheel when data hdfsf");
                }
                else
                {
                    Debug.Log("else");
                    wheelIsSpinning = true;
                    //   StartSpinWheelWithoutData();
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
            //  WheelSpinFoo.spinWheel();
        }
        private IEnumerator StartSpinWheelWithData()
        {
            Debug.LogError("startSpinWheelWithData");
            // WheelSpinFoo.StartSpinningWithData(cardIdLive, suitIDLive);

            cardImage.gameObject.SetActive(false);
            suiteImage.gameObject.SetActive(false);
            Xanimation.SetActive(true);
            WheelSpinFoo.spinWheel();

            yield return new WaitForSeconds(2f);
            //
            WheelSpinFoo.faaa(cardIdLive, suitIDLive, () =>
            {

                //StartCoroutine(SetCardImage(cardIdLive, suitIDLive));
            });
            // JeetoJokerManager.instance.ShowWinAmount(CardSelect.instance.totalbetdata);
            Debug.Log("winning data");
        }

        public IEnumerator SetCardImage(int cardIdLive, int suitIDLive)
        {
            string itemCard = SuperJokerConstants.CARDNAMEPREFIX + cardIdLive;
            string itemSuite = SuperJokerConstants.SUITENAMEPREFIX + suitIDLive;
            yield return null;
            Xanimation.SetActive(false);
            ShowXMultiplierText();
            cardImage.sprite = Resources.Load<Sprite>("SJ_Resources/" + itemCard);
            suiteImage.sprite = Resources.Load<Sprite>("SJ_Resources/" + itemSuite);
            CardDeck.instance.HighlightCard(itemCard+itemSuite);
            cardImage.gameObject.SetActive(true);
            suiteImage.gameObject.SetActive(true);
            yield return new WaitForSeconds(1f);
            blast.SetActive(true);
            Invoke(nameof(stopBlast),3f);
        }

        void stopBlast() {

            blast.SetActive(false);
        }
        private void ShowXMultiplierText()
        {
            string multiplier = "";
            if (SocketController.Instance.is_x_excuted.Equals(Constants.zero))
            {
                multiplier = SocketController.Instance.win_price;
            }
            if (string.IsNullOrEmpty(multiplier) || multiplier == "1x")
            {
                multiplierObject.SetActive(true);
                int index =APICardHistory.Instance.GetImageIndex(multiplier);
                multiplierObject.GetComponent<Image>().sprite = APICardHistory.Instance.multiplierImages[index];
                //multiplierText.gameObject.SetActive(false);
            }
            else
            {
                multiplierObject.SetActive(true);
                int index = APICardHistory.Instance.GetImageIndex(multiplier);
                multiplierObject.GetComponent<Image>().sprite = APICardHistory.Instance.multiplierImages[index];
                multiplierText.text = multiplier;
                if (totalBetAmount == 0)
                    StartCoroutine(WinnerHotlistUpdate());
            }
        }

        public void HideMultiplierText() {
            multiplierObject.SetActive(true);
            multiplierText.gameObject.SetActive(false);
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
    public class BetButtons
    {
        public int id;
        public Transform buttonTransfrom;
        public int amount;
    }

    public interface ButtonHandlers
    {
        public void Clear();
        public void Repeat();
        public void DoubleUp(int removeCountIndex);
        public void SavePrevRound();
        public bool CheckifLastSavedDataAvailable();
        public long GetPrevRoundTotalSum();
        public long GetRoundTotalSum();
    }

    public interface IRemoveHandler
    {
        public static Action<int> refresh;
        public void Remove();
    }

    public interface IWinHandler
    {
        public bool OnWin(int outerId, int innerId);
        public BetDataHandler GetBetData();
    }

    [System.Serializable]
    public class BetDataHandler
    {
        public string Id;
        public long amount;
    }
    [System.Serializable]
    public class GameData
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
        public List<CardValueSelect> gameData { get; set; }
        public string bet_ammount { get; set; }  // Changed to long to match your JSON format
    }
}