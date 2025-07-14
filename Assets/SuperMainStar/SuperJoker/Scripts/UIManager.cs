using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;

namespace SuperJoker
{
    public class UIManager : MonoBehaviour, IConnectivityEvent,IBalanceRecharge
    {
        public static UIManager inst;

        public Text pointBalance;
        public GameObject notEnoughCashPanel;

        public Transform  parentCanvas;
        public GameObject NoInternetPopUp;
        public GameObject WaitForNextSession;
        public GameObject WinPopup;

        public Text playValue;
        public Text winValue;
        public Text winPopOfText;
        public ButtonManager buttonManager;

        public Text welcomeText;
        public Button soundButton;
        public Sprite soundOn;
        public Sprite soundOff;
        bool isSoundOn;
        AudioListener audioListener;
        public ParticleSystem coinSpurt;
        float win;

        public GameObject minimizeButton;
        public GameObject closeButton;

        public UnityEvent onInternetLost;

        private void Awake()
        {
            inst = this;
        }

        // Use this for initialization
        void Start()
        {
            audioListener = FindObjectOfType<AudioListener>();
            //ShowNoInternetPopup();
            isSoundOn = true;
            SetWelcomTexts(Constant.Name);
            SetPointBalance();

#if UNITY_ANDROID
            minimizeButton.SetActive(false);
            closeButton.SetActive(false);
#endif
            ConnectivityUtils.Instance.SubscribeToEvents(this, true);
            BalanceRecharge.Instance.AddListener(this);
        }

        private void OnDestroy()
        {
            ConnectivityUtils.Instance.SubscribeToEvents(this, false);
            BalanceRecharge.Instance.RemoveListener(this);
        }

        public void SetPlayAmount()
        {
            double totalBetAmount = Constant.PointBalance - SuperJokerConstants.PointBalance;
           
            playValue.text = "" + totalBetAmount;

#if UNITY_ANDROID
            if(totalBetAmount == 0)
            {
                ButtonManager.instance.removeModifier = 1;
                ButtonManager.instance.ToggleRepeatRemove(true);
               // ButtonManager.instance.OnRemoveClicked();
            }
            else
            {
                ButtonManager.instance.ToggleRepeatRemove(false);
            }
#endif

            if (totalBetAmount == 0)
                ButtonManager.instance.SetInteractiveStates(true);
            else
                ButtonManager.instance.SetInteractiveStates(false);

        }

        public double GetPlayAmount()
        {
            return Constant.PointBalance - SuperJokerConstants.PointBalance;
        }

        public void SetPointBalance()
        {
            pointBalance.text = "" + SuperJokerConstants.PointBalance;
        }

        public void SetNotEnoughCash(bool state)
        {
            notEnoughCashPanel.SetActive(state);
        }

        public void CloseNotEnoughCashPanel()
        {
            SetNotEnoughCash(false);
        }

        public void ShowNoInternetPopup()
        {
            LevelManager.inst.cardFlashAnimation.transform.parent.gameObject.SetActive(false);
            SoundOnOff();
            NoInternetPopUp.SetActive(true);
        }

        public void ShowWaitForNextSession(bool state)
        {
            WaitForNextSession.SetActive(state);
        }

        public void SetWinAmount(float amount)
        {
            winValue.text = "" + amount;
            winPopOfText.text = "" + amount;
            win = amount;
            if (amount > 0)
            {
                ShowWinPopup(true);
                TriggerCointSpurt();
            }
        }

        public float GetWinAmount()
        {
            return win;
        }
        public void ShowWinPopup(bool state)
        {
            WinPopup.SetActive(state);
        }

        public void SetClearDoubleRepeatButtons(bool state)
        {
            buttonManager.ToggleClearDoubleRepeat(state);
        }

        public void SetWelcomTexts(string i)
        {
            welcomeText.text = "WELCOME " + i;
        }
        bool toggleSound;
        public void SoundOnOff()
        {
            toggleSound = !toggleSound;
            if (toggleSound)
            {
                AudioListener.volume = 0;
                soundButton.GetComponent<Image>().sprite = soundOff;
            }
            else
            {
                AudioListener.volume = 1;
                soundButton.GetComponent<Image>().sprite = soundOn;
            }
        }      

        void TriggerCointSpurt()
        {
            if (LevelManager.inst.IsSuperBumper())
            {
                coinSpurt.Play();
                AudioManagerTri.Main.PlayNewSound("win");
            }
        }

        public void InternetStatus(bool a_Status)
        {
            if (!a_Status)
            {
                if (!NoInternetPopUp.activeSelf)
                    ShowNoInternetPopup();
                onInternetLost?.Invoke();
            }
        }

        public void ApplicationFocus(bool a_HasFocus) { }

        public void ApplicationPause(bool a_PauseStatus)
        {
            if (a_PauseStatus)
            {
                if (!NoInternetPopUp.activeSelf)
                    ShowNoInternetPopup();
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
            SuperJokerConstants.pointBalance += updatedBalance;
            pointBalance.text = SuperJokerConstants.pointBalance.ToString();
        }
    }
}
