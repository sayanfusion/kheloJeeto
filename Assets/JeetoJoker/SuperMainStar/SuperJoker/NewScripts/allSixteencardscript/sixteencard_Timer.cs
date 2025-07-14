using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

namespace khelojeetonew
{
    public class sixteencard_Timer : MonoBehaviour
    {  /*/* sixteencard_Timer is copy of Timer controller new*/

        public static sixteencard_Timer inst;
        [SerializeField] private float timeLeft = 25f;
        public Text timerText;
        private bool canStart = false;
        public Text placeyourbet;
        public Color cDefaultCol;
        public Color cWarningColor;
        public Sixteencards_spinwheel innerSpinWheel;
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
        private void Awake()
        {
            inst = this;
            if (Application.isMobilePlatform)
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
        void Update()
        {
            return;
            if (canStart && timeLeft > 0)
            {
                //Debug.Log(((STartDateTime - DateTime.Now).Duration().Seconds));
                timeSpend = timeLeft - (float)((DateTime.Now - STartDateTime).Duration().TotalSeconds);

                //timeLeft -= Time.deltaTime;

                //string min = ((int)timeLeft / 60).ToString("00");
                //string sec = (timeLeft % 60).ToString("00");
                //timerText.text = min + ":" + sec;


                timerText.text = timeSpend.ToString("00");
                if (timeSpend <= 10 && timeSpend > 07)
                {
                    timerText.color = cWarningColor;
                }


                if (timeSpend <= 07)
                {
                    if (!stopBetting)
                    {
                        stopBetting = true;
                        // UIManager.inst.StopTheWonCorutine();
                        //SoundController.instance.PlayAudio(SoundController.ClipType.NO_MORE_BET);
                        timerText.color = Color.red;
                        placeyourbet.text = "NO MORE PLAY!";
                        placeyourbet.fontSize = 30;

                        TimeUp?.Invoke();

                        Sixteen_cards.instance.TimerEnd();
                    }
                }
                if (timeSpend <= 0)
                {
                    timeSpend = 0;
                    //JeetoJokerManager.instance.StartSpinning();
                    canStart = false;
                    WheelSpinTime?.Invoke();
                }
            }
        }

        public void UpdateTimer(float time)
        {
            if (innerSpinWheel != null && !innerSpinWheel.isWheelSpinning())
            {
                timerText.text = time.ToString("00");
                if(time>=10)
                {
                    timerText.color =cDefaultCol;
                }
                if (time <= 10 && time > 07)
                {
                    timerText.color = cWarningColor;
                }

            
                    if (time <= 10)
                {
                    _inputBlocker.SetActive(true);
                    if (!stopBetting)
                    {
                        stopBetting = true;
                        // UIManager.inst.StopTheWonCorutine();
                        //SoundController.instance.PlayAudio(SoundController.ClipType.NO_MORE_BET);
                        timerText.color = Color.red;
                        placeyourbet.text = "NO MORE PLAY!";
                        placeyourbet.fontSize = 30;

                        TimeUp?.Invoke();
                        Debug.Log("bet emit timer in 16 card 10secondss");
                        SixteenCardsSocketController.Instance.Bet();

                        Sixteen_cards.instance.TimerEnd();
                    }
                }
                if (time <= 0)
                {
                    time = 0;
                    //JeetoJokerManager.instance.StartSpinning();
                    canStart = false;
                    WheelSpinTime?.Invoke();
                }
            }

            
        }


        public void StartTimer(float totalTime)
        {
            _inputBlocker.SetActive(false);
            stopBetting = false;
            //Debug.Log("Timer start");
            STartDateTime = DateTime.Now;
            timeLeft = totalTime;
            canStart = true;
            timerText.color = cDefaultCol;
            //UIManager.inst.SetWinAmount();
            placeyourbet.text = "PLACE YOUR CHIPS!";
            //placeyourbet.fontSize = 30;
            //UIManager.inst.SetPlayAmount();
            //LevelManager.inst.SetInputBlocker(false);
        }

        public void SetDefaultColor()
        {
            timerText.color = cDefaultCol;
        }


        public void StopTimer()
        {
            stopBetting = false;
            //UIManager.inst.StopTheWonCorutine();
            canStart = false;
            timeSpend = 90f;
            timerText.text = "90";
            timerText.color = Color.red;
            placeyourbet.text = "NO MORE PLAY!";
            placeyourbet.fontSize = 30;
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause == true)
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
