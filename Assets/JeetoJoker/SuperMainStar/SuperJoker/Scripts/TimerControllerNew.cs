using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using TMPro;
using JeetoJoker;
namespace khelojeetonew
{
    public class TimerControllerNew : MonoBehaviour
    {
        public static TimerControllerNew inst;
        [SerializeField]private float timeLeft = 25f;
        public TMP_Text timerText;
        private bool canStart = false;
        public Text placeyourbet;
        public Color cDefaultCol;

        public UnityAction TimeUp;
        public UnityAction WheelSpinTime;

        //public LevelManager level;
        float timeSpend;

        public Text ststuc;
        public bool isWheelRunning;

        private bool stopBetting = false;
        private bool firstTime = true;

        //new code
        public DateTime STartDateTime;
        [SerializeField]
        private GameObject _inputBlocker;

        public SpinWheelnew innerSpinWheel;

        [SerializeField] private GameObject noMoreBetPlease;

        private void Awake()
        {
            inst = this;
            if(Application.isMobilePlatform)
            {
                Application.runInBackground = true;
            }
           
        }
        // Use this for initialization
        void Start()
        {
            //UIManager.inst.StopTheWonCorutine();
            //UIManager.inst.ResetWinAmoutText();

            placeyourbet.text = "PLACE YOUR CHIPS!";
            placeyourbet.fontSize = 30;
            //timeLeft = timeLeft - Constant.TimerForGame;
           
            if (timeLeft > 0)
                StartTimer(timeLeft);

            isWheelRunning = false;
            ststuc.text = "void";
        }

        // Update is called once per frame
    

        public void UpdateTimer(float time)
        {
            if (time == 90f) {
                StartTimer(time);

            }

            if ( innerSpinWheel != null && !innerSpinWheel.isWheelSpinning())
            {
                timerText.text = time.ToString();
                if (time == 25) JeetoJoker.SoundController.Instance.PlayLastchance();
                if (time == 5) { 
                JeetoJoker.SoundController.Instance.PlayNoMoreBet();
                    ShowNoMoreBetPlease();
                    Invoke(nameof(HideNoMoreBetPlease), 2f);



                }
                if (time <= 5)
                {
                    CardDeck.instance.disableAllText();
                    _inputBlocker.SetActive(true);
                    if (!stopBetting)
                    {
                        stopBetting = true;
                        // UIManager.inst.StopTheWonCorutine();
                        //SoundController.instance.PlayAudio(SoundController.ClipType.NO_MORE_BET);
                        placeyourbet.text = "NO MORE PLAY!";
                        placeyourbet.fontSize = 30;
                        TimeUp?.Invoke();
                        Debug.Log("bet data emit for 5 seconds..");
                        Debug.Log("Calling Bet of socket");
                        SocketController.Instance.Bet();
                        Debug.Log("bet emit in 5 second..");
                        JeetoJokerManager.instance.TimerEnd();
                    }
                }
                if (time <= 0)
                {
                    time = 0;
                    //JeetoJokerManager.instance.StartSpinning();
                    canStart = false;
                    // WheelSpinTime?.Invoke();
                }
            }

            
        }


        public void StartTimer(float totalTime)
        {
            _inputBlocker.SetActive(false);
            stopBetting = false;
            Debug.Log("Timer start");
            STartDateTime = DateTime.Now;
            timeLeft = totalTime;
            canStart = true;
            // timerText.color = cDefaultCol;
            JeetoJoker.SoundController.Instance.PlayPlaceyourChips();
            CardDeck.instance.EnableAllText();
            CardDeck.instance.RemoveHighlightCard();
            //yield return new WaitForSeconds(10f);
            JeetoJokerManager.instance.Clear();
            JeetoJokerManager.instance.SelectBet(0);
            //UIManager.inst.SetWinAmount();
            if (totalTime >20)
            {
                placeyourbet.text = "PLACE YOUR CHIPS!";
            }
         
            //placeyourbet.fontSize = 30;
            //UIManager.inst.SetPlayAmount();
            //LevelManager.inst.SetInputBlocker(false);
        }

        public void SetDefaultColor()
        {
            // timerText.color = cDefaultCol;
        }


        void ShowNoMoreBetPlease() {

            noMoreBetPlease.SetActive(true);


        }

        void HideNoMoreBetPlease() { 
        
            noMoreBetPlease.SetActive(false);


        }

        public void StopTimer()
        {
            stopBetting = false;
            //UIManager.inst.StopTheWonCorutine();
            canStart = false;
            timeSpend = 90f;
            timerText.text = "90";
            placeyourbet.text = "NO MORE PLAY!";
            placeyourbet.fontSize = 30;
        }

        private void OnApplicationPause(bool pause)
        {
            if(pause == true)
            {
              
            }
            else
            {
               // LevelManager.inst.ShowUnexpectedQuitPanel();
                //Reset();
                //if (isWheelRunning)
                //{
                //   // DataCollector.instance.ManualWheelsTOPAfterpAUSE();
                //}
            }
        }
      
        private void Reset()
        { 
            if ((timeSpend) <= 0)
            {
                StopTimer();
            }
        }

    }
}
