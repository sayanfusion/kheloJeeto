using UnityEngine;
using System.Collections;
using SimpleJSON;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Unity.VisualScripting;
using DevCommon;
using DevCommon.Utils;
using System;
using Random = UnityEngine.Random;

public class AmericanWheel : Wheel
{
    private readonly new byte[] numbers = new byte[] { 37,27, 10, 25, 29, 12, 8, 19, // < "37" is for "00"
        31, 18, 6, 21, 33, 16, 4, 23, 35, 14, 2, 0, 28, 9, 26,
        30, 11, 7, 20, 32, 17, 5, 22, 34, 15, 3, 24, 36, 13, 1 };

    public int Payout = 35;
    // private int id, i;
    private string jsonString;

    private Dictionary<int, int> betNumbers = new Dictionary<int, int>();

    [SerializeField] public string userStokies;
    int result = 0;
    public int stokiesID;

    /////////////////////public static bool stopper = false;
    int first;

    public void SetValue(int id, int value)
    {
        first = 0;
        if (betNumbers.ContainsKey(id))
        {
            Debug.LogError("value " + value);
            if (value < 0)
            {
                betNumbers[id] = betNumbers[id] + value;
            }
            else
            {
                betNumbers[id] = value;
            }
        }
        else
            betNumbers.Add(id, value);




    }


    private void Set_Result()
    {

        switch (userStokies)
        {
            case "High":
                List<int> HighKey = new List<int>();
                foreach (var number in betNumbers)
                {
                    if ((number.Value * 36) >= ResultManager.totalBet)
                    {
                        HighKey.Add(number.Key);
                    }
                }
                result = HighKey[Random.Range(0, HighKey.Count)];
                break;
            case "Low":
                List<int> LowKeys = new List<int>();

                foreach (var number in betNumbers)
                {

                    if ((number.Value * 36) < ResultManager.totalBet || number.Value == 0)
                    {
                        LowKeys.Add(number.Key);
                    }
                }
                result = LowKeys[Random.Range(0, LowKeys.Count)];
                break;
        }

    }

    public void ClearAllData()
    {
        first = 0;
        betNumbers.Clear();
        for (int i = 0; i < numbers.Length; i++)
        {
            betNumbers.Add(i, 0);
        }
    }

    void Start()
    {
        BetSpace.numLenght = Payout;
        resultCheckerObject = new GameObject[numbers.Length];
        for (int i = 0; i < numbers.Length; i++)
        {
            int id = numbers[i];

            resultCheckerObject[id] = new GameObject("resultchecker");
            resultCheckerObject[id].transform.SetParent(transform);
            resultCheckerObject[id].transform.localPosition = new Vector3(0, .215f, 0);
            // resultCheckerObject[id].transform.localPosition = new Vector3(0, 0, 0);
            resultCheckerObject[id].transform.RotateAround(transform.position, Vector3.up * .03f, i * 360 / numbers.Length);

            resultCheckerObject[id].name = id.ToString();
            BetSpace.numLenght = 36;
            betNumbers.Add(i, 0);
        }
        CheckStockies();
        StartCoroutine(SetBlockUser());
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
               // Debug.LogError(uwr.result);
                jsonString = System.Text.Encoding.UTF8.GetString(uwr.downloadHandler.data, 3, uwr.downloadHandler.data.Length - 3);
                JSONNode loginInfo = JSON.Parse(uwr.downloadHandler.text);
                string msg = loginInfo["status"];

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
    void Update()
    {
        // if (stopper == true)
        // {
        //     resultCheckerObject[id].transform.RotateAround(transform.position, Vector3.up * .0f, 0f);
        //     stopper = false;
        // }
        if (timeCounter.instance.secondleft == 5)
        {
            first += 1;
            if (first == 1)
            {
                List<string> keys = new List<string>();
                List<string> values = new List<string>();
                foreach (var item in betNumbers)
                {
                    keys.Add(item.Key.ToString());
                    values.Add(item.Value.ToString());
                }
                Dictionary<string, object> dict = new Dictionary<string, object>();
                dict.Add("playerId", PlayerPrefs.GetInt(Constant.UID));
                dict.Add("name", PlayerPrefs.GetString(Constant.User));
                dict.Add("roomId", PlayerPrefs.GetString(Constants.ROOMID));
                dict.Add("bettedOn", keys.ToArray());
                dict.Add("bettedAmount", values.ToArray());

                Debug.LogError("Keys " + DataConverter.SerializeObject(dict));
                SocketManager.Core.Socket.Emit("bet", dict);
            }

        }

    }

    public override void  Spin()
    {
        print("Spin American");
        base.Spin();
        audios.Instance.WheelRotatingSound();
        Set_Result();
        StartCoroutine(SetResult());
    }
    public int a;
    private IEnumerator SetResult()
    {
        yield return new WaitForSecondsRealtime(5);
        //if (ResultManager.totalBet>0)
        //{
        //    Debug.LogError("result");
        //    a = result;
        //}
        //else
        //{

        //    a = Random.Range(0, 37);            
        //}
        ball.FindNumber(a, false);
        ////ball.FindNumber(37, false);
        CheckStockies();
        ClearAllData();
        //GetStokiesData();

    }


    public void CheckStockies()
    {
        StartCoroutine(getRequest(Constant.KIBaseURL + "winning-hotlists"));
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
            string msg = loginInfo["status"];
            if (msg.Equals("200"))
            {

                print("yes");


                //userStokies = userStokies.Trim('"');
                //Debug.LogError("Hello " + userStokies);
                stokiesID = Convert.ToInt32(loginInfo["list"]["player_id"]);
                if (PlayerPrefs.GetInt(Constant.UID) == stokiesID)
                {
                    Debug.Log("stockiest " + loginInfo["list"]["stockiest"]);
                    userStokies = loginInfo["list"]["win_type"].ToString();
                }
                else
                {
                    userStokies = "Medium";//need to change
                }

            }
            else
            {
                print("no");
            }
            StartCoroutine(WaitingForCall());

        }

    }

    IEnumerator WaitingForCall()
    {
        yield return new WaitForSeconds(5f);
        GetStokiesData();
    }
    public void GetStokiesData()
    {

        Dictionary<string, string> map = new Dictionary<string, string>();
        map.Add("roomId", PlayerPrefs.GetString(Constants.ROOMID));
        map.Add("mode", userStokies.ToLower());
        map.Add("stockistId", PlayerPrefs.GetInt(Constant.STOKIESID).ToString());

        Debug.LogError("setMode " + DataConverter.SerializeObject(map));
        SocketManager.Core.Socket.Emit("setMode", map);
        //Debug.LogError("Winning type " + WinningType);
    }

}