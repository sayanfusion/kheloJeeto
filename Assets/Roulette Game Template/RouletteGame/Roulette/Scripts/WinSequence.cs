using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Networking;
using SimpleJSON;
using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
public class WinSequence : MonoBehaviour
{

    private readonly byte[] redNumbers = new byte[] { 1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36 };
    //
    public List<Text> History;
    private string GamedataInsertapi = "https://mplgames.in/admin/api/roulette-game_data_inserts";
    public GameObject WinPopUp;
    public Text WinText;
    //

    public GameObject winPanel;
    public AudioSource Winsound;
    public TMP_Text winText;
    public TMP_Text resultText;

    public GameObject historyPrefab;
    public Transform historyContent;

    public static bool mover = false;

    public static bool numberdisplayed = false;

    public static int resultcarryover;

    public static bool greenredder = true;

    public static bool ballstopper = false;

    public GameObject[] chessRook;

    public int Result, TotalWin;

    public string winLoss;

    public static WinSequence Instance;

    public static int totalBettingAmount;
    string numbers;
    public string winnumber;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
       
        string historyData = PlayerPrefs.GetString("HistoryData");


        string[] historyArray = historyData.Split(',');


        for (int i = 0; i < History.Count && i < historyArray.Length; i++)
        {
            History[i].text = historyArray[i];
            int intValue;
            if (int.TryParse(History[i].text, out intValue))
            {
                if (new List<int> { 1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36 }.Contains(intValue))
                {
                    History[i].color = Color.red;
                }
                else if (intValue == 0 || intValue == 00)
                {
                    History[i].color = Color.green;
                }
                else
                {
                    History[i].color = Color.black;
                }
            }
        }


        SetResults();
    }

    public void ShowResult(int result, int totalWin)
    {
        BetPool.Instance.ResetStatus();
        SceneRoulette._Instance.camCtrl?.GoToOrigin();

        print(totalWin + " with " + result);
        Result = result;
        TotalWin = totalWin;
        numbers = Result.ToString();
        chessRook[result].SetActive(true);
        string sRes;
        if (result != -1 && result != 37)
        {
            sRes = result.ToString();
            resultcarryover = result;
        }
        else
            sRes = "00";



        winnumber = sRes;
        resultText.text = sRes;
        HistorywinnumberText();
        show();
        numberdisplayed = true;
        greenredder = true;
        ballstopper = true;
        //ball.StartSpin();
        timer.score8 = 68f;
        SceneRoulette._Instance.undoButton.gameObject.SetActive(true);
        //////////////////AmericanWheel.stopper = true;
        StartCoroutine(RookDisabler());
        bool win = totalWin > 0;
        if (win && ResultManager.totalBet > 0)
        {
            winPanel.SetActive(true);
            StartCoroutine(WinDisabler());
            winText.text = string.Format("<color=#yellow></color> {0}", totalWin.ToString());
            AudioManager.SoundPlay(0);
            winLoss = "win";
            Winsound.Play();

            //  getuserdetails();
            StartCoroutine(GameDataInsert(totalWin, "win"));
            StartCoroutine(ShowWinPopUp(totalWin.ToString()));

        }
        else if (ResultManager.totalBet > 0)
        {
            winLoss = "lost";
            //  getuserdetails();
            StartCoroutine(GameDataInsert(0, "loose"));
        }
        SendUserResult();

      
        SetResults();
        StartCoroutine(EnableBets());
    }
    IEnumerator ShowWinPopUp(string winAmount)
    {


        WinPopUp.SetActive(true);

        WinText.text = winAmount;
        yield return new WaitForSeconds(4);

        WinPopUp.SetActive(false);
    }

    private IEnumerator EnableBets()
    {
        yield return new WaitForSecondsRealtime(8);
        BetSpace.EnableBets(true);
        SceneRoulette.GameStarted = false;
        ////////////////////SceneRoulette._Instance.rebetButton.gameObject.SetActive(true);
        //SceneRoulette._Instance.undoButton.gameObject.SetActive(true);

    }
    private IEnumerator WinDisabler()
    {

        yield return new WaitForSecondsRealtime(68);
        winPanel.SetActive(false);

    }

    private IEnumerator RookDisabler()
    {
        yield return new WaitForSecondsRealtime(8);
        for (int i = 0; i <= 37; i++)
        {
            chessRook[i].SetActive(false);
        }
    }

    public void show()
    {
        mover = true;
    }

    public void getuserdetails()
    {
        StartCoroutine(getuserdetailsrequest(Constant.KIBaseURL + "roulette-game_data_inserts"));
    }


    IEnumerator getuserdetailsrequest(string url)
    {
        string gameId = DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString()
            + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
        Debug.LogError("TotalWin " + TotalWin);
        WWWForm form4 = new WWWForm();

        form4.AddField("player_id", PlayerPrefs.GetInt(Constants.UID));
        form4.AddField("win_number", Result);
        form4.AddField("win_ammount", TotalWin);
        form4.AddField("game_name", "Roullete");
        form4.AddField("bet_ammount", ResultManager.totalBet);
        form4.AddField("win_loose", winLoss);
        form4.AddField("game_id", gameId);
        form4.AddField("start_point", (int)BalanceManager.ConstBalance);

        UnityWebRequest uwr = UnityWebRequest.Post(url, form4);
        uwr.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString(Constants.Token));
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


    IEnumerator GameDataInsert(int winAmount, string winLoose)
    {
        Debug.Log("Sucess GameDatainsert:0"+ "betAmmount: "+ ResultManager.totalBet.ToString()+ "startPoint: "+ (int)BalanceManager.ConstBalance);
        WWWForm form = new WWWForm();
        form.AddField(Constants.playerId, PlayerPrefs.GetInt(Constants.UID));
        form.AddField(Constants.gameId, PlayerPrefs.GetString("GameIdRoulette"));
        form.AddField(Constants.winAmmount, winAmount.ToString());
        form.AddField(Constants.betAmmount, ResultManager.totalBet.ToString());
        form.AddField(Constants.gameName, "roulette");
        form.AddField(Constants.winLoose, winLoose);
        form.AddField(Constants.startPoint, (int)BalanceManager.ConstBalance);
        form.AddField(Constants.winNmuber, winnumber);
        //  form.AddField(Constants.bonusspin, "");
        form.AddField(Constants.betting_position, "[0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37]");
        form.AddField(Constants.betting_amount, "[5,0,0,0,0,0,0,0,0,0,0,0,0,0,0,5,0,0,5,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0]");







        Debug.Log("Sucess GameDatainsert:1");
        UnityWebRequest unityWebRequest = UnityWebRequest.Post(GamedataInsertapi, form);

        yield return unityWebRequest.SendWebRequest();
        if (unityWebRequest.result == UnityWebRequest.Result.Success)
        {
            Debug.Log(unityWebRequest.downloadHandler.text + "Sucess GameDatainsert: Yes");
            SceneRoulette._Instance.GetDetails();

        }
        else
        {

            Debug.Log(unityWebRequest.error + "Sucess GameDatainsert: No");

        }
    }


    public void HistorywinnumberText()
    {
        for (int i = History.Count - 1; i > 0; i--)
        {
            History[i].text = History[i - 1].text;
        }

        if (History.Count > 0)
        {
            if (Result !=37)
            {
                History[0].text = Result.ToString();
            }
            else
            {
                History[0].text = "00";
            }
           
           
        }


        for (int i = 0; i < History.Count ; i++)
        {
         
            int intValue;
            if (int.TryParse(History[i].text, out intValue))
            {
                if (new List<int> { 1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36 }.Contains(intValue))
                {
                    History[i].color = Color.red;
                }
                else if (intValue == 0 || intValue == 00)
                {
                    History[i].color = Color.green;
                }
                else
                {
                    History[i].color = Color.black;
                }
            }
        }

        string historyData = "";


        for (int i = 0; i < History.Count; i++)
        {
            historyData += History[i].text;


            if (i < History.Count - 1)
            {
                historyData += ",";
            }
        }
       

        PlayerPrefs.SetString("HistoryData", historyData);
    }



    public void SendUserResult()
    {
        //StartCoroutine(SendUserResultrequest(Constant.KIBaseURL+"live-data"));
    }
    IEnumerator SendUserResultrequest(string url)
    {
        WWWForm form4 = new WWWForm();
        form4.AddField("win_number", numbers);
        form4.AddField("game_name", SceneManager.GetActiveScene().name.ToLower());
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
        //uwr.Dispose();

    }
    void SetResults()
    {
        StartCoroutine(getResultRequest(Constant.KIBaseURL + "live-data-history"));
    }
    IEnumerator getResultRequest(string url)
    {
        string jsonString;
        WWWForm form4 = new WWWForm();
        form4.AddField("game_name", "roulettegameplay");
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
                for (int i = 0; i < historyContent.childCount; i++)
                {
                    Destroy(historyContent.GetChild(i).gameObject);
                }
                for (int i = 0; i < loginInfo["list"].Count; i++)
                {
                    SetResultHistory(int.Parse(loginInfo["list"][i]["win_number"]));
                }
            }
            else
            {
                print("no");
            }
        }
        uwr.Dispose();
    }

    void SetResultHistory(int result)
    {

        string sRes = result.ToString();


        bool isRed = false;

        for (int i = 0; i < redNumbers.Length; i++)
        {
            if (redNumbers[i] == result)
            {
                isRed = true;
                break;
            }
        }


        GameObject hOb = Instantiate(historyPrefab, historyContent);
        hOb.transform.SetAsFirstSibling();

        //if (historyContent.childCount > 6)
        //    Destroy(historyContent.GetChild(6).gameObject);

        if (isRed)
        {
            hOb.transform.GetChild(1).GetComponent<TMP_Text>().text = sRes;
            resultText.color = Color.red;
        }
        else
        {
            TMP_Text blackHistoryText = hOb.transform.GetChild(0).GetComponent<TMP_Text>();

            if (sRes.Equals("0") || sRes.Equals("00"))
            {
                blackHistoryText.color = Color.green;
                resultText.color = Color.green;
            }
            else
            {
                blackHistoryText.color = Color.white;
                resultText.color = Color.white;
            }

            blackHistoryText.text = sRes;
        }
    }
}





