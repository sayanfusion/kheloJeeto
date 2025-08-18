using khelojeetonew;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class Timer : MonoBehaviour
{
    public float timeLeft = 25f;
    public TextMeshProUGUI timerText;
    public GameObject blankPanel;
    private bool playAnimOnce;
    private bool canStart = false;
    private Color cDefaultCol;
    public Color cWarningColor;
    [SerializeField] private GamePlay gamePlay;
    public AudioClip AudioClip, placeyourbet;

    public Image zeroth;
    public Image oneth;

    public Sprite[] red0_5;
    public Sprite[] orange0_10;

    public Sprite[] yellow0_10;
    // public Sprite[] digits;
    bool gameStarted;
    private void OnEnable()
    {
        cDefaultCol = timerText.color;
        blankPanel.SetActive(true);
    }
    // Update is called once per frame
    //void Update () {//prabir
    //  if(canStart && timeLeft > 0)
    //  {
    //      timeLeft -= Time.deltaTime;
    //      //string min = ((int) timeLeft/60).ToString("00");
    //      string sec = (timeLeft).ToString("00");
    //      //timerText.text = min + ":" + sec;
    //           timerText.text = sec;
    //           if (timeLeft <= 10 && timeLeft > 06)
    //           {
    //               timerText.color = cWarningColor;
    //           }
    //           if(timeLeft <= 06 && !playAnimOnce)
    //           {
    //               timerText.color = Color.red;
    //               playAnimOnce = true;
    //               Audio_Manager.instance.PlayAudio(AudioClip);
    //               BlockAllButton();
    //               //GamePlay.instance.SendGameData();
    //           }
    //           if (timeLeft <= 0)
    //      {
    //          canStart = false;
    //               GamePlay.instance.StartSpinning();
    //      }
    //  }
    //}// END OF UPDATE FUNCTION
    public void StartTimer(float totalTime = 0)
    {
        Debug.Log("timer data:" + totalTime);
        gamePlay.ShowMessage("Place your chips");
        Audio_Manager.instance.PLayPlaceYourChips();
        timeLeft = totalTime;
        canStart = true;
        playAnimOnce = false;
        blankPanel.SetActive(false);
        timerText.color = cDefaultCol;
        Debug.Log("Enabling all buttons");
        GamePlay.instance.bInfo.interactable = true;
        gamePlay.RestartGame();
        // GamePlay.instance.bClear.interactable = true;
        // GamePlay.instance.bDoubleUp.interactable = true;
        // GamePlay.instance.bRepeat.interactable = true;
        // Audio_Manager.instance.PlayAudio(placeyourbet);
    }
    public void StopTimer()
    {
        canStart = false;
        timerText.text = "00:00";
        timerText.color = Color.red;
    }
    public void UpdateTimer(int timeValue)
    {
        canStart = true;
        timeLeft = timeValue;

        int ones = timeValue % 10;
        int tens = (timeValue / 10) % 10;

        GamePlay.instance.CheckButtons();

        if (timeValue>10) zeroth.transform.localPosition=new Vector2(35, zeroth.transform.localPosition.y);
        else zeroth.transform.localPosition = new Vector2(38, zeroth.transform.localPosition.y);




        if(timeValue==15) Audio_Manager.instance.PLayLastChance();

        if (timeValue < 6)
        {
            zeroth.sprite = red0_5[ones];
            oneth.sprite = red0_5[tens];

        }
        else if (timeValue < 16)
        {
            zeroth.sprite = orange0_10[ones];
            oneth.sprite = orange0_10[tens];
        }
        else if (timeValue < 91)
        {
            zeroth.sprite = yellow0_10[ones];
            oneth.sprite = yellow0_10[tens];
        }
        // Debug.Log("timer for window version: "+timeLeft);
        timerText.text = timeValue.ToString();
        if (canStart && timeValue >= 0)
        {
            if (timeValue < 10)
            {
                string timeTextValue = Constants.zero + timeValue.ToString();
                timerText.text = timeTextValue.ToString();
            }
            else
            {
                timerText.text = timeValue.ToString();
            }

            if (timeValue <= 15 && timeValue >= 06)
            {
                timerText.color = cWarningColor;
            }

            if (timeValue <= 05 && !playAnimOnce)
            {
                GamePlay.instance.CloseInfo();
                timerText.color = Color.red;
                playAnimOnce = true;
                gamePlay.ShowMessage("No More Play");
                Audio_Manager.instance.PlayAudio(AudioClip);
                Debug.Log("Disabling all buttons");
                GamePlay.instance.bInfo.interactable = false;
                GamePlay.instance.bClear.interactable = false;
                GamePlay.instance.bDoubleUp.interactable = false;
                GamePlay.instance.bRepeat.interactable = false;
                BlockAllButton();
            }
            if (timeValue <= 0)
            {
                canStart = false;
                // GamePlay.instance.StartSpinning();//prabir
            }
        }
    }
    public void BlockAllButton()
    {
        blankPanel.SetActive(true);
    }
}// END OF Timer CLASS