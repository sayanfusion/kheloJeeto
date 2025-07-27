using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using khelojeetonew;
[System.Serializable]
public class ApiData
{
    public int id;
    public string win_card;
    public string game_name;
    public string created_at;
    public string updated_at;
    public string bonous_spin;
}

[System.Serializable]
public class ApiResponse
{
    public ApiData[] list;
}



public class APICardHistory : MonoBehaviour
{
    public static APICardHistory Instance;
    public bool isstart = true;
    public SpriteRenderer c1;
    public SpriteRenderer c2;
    public SpriteRenderer c3;
    public SpriteRenderer s1;
    public SpriteRenderer s2;
    public SpriteRenderer s3;
    public SpriteRenderer s4;
    public List<string> WinNumber;
    public List<string> Timer;
    public List<GameObject> nullImage;
    public List<Image> cardimage;
    public List<Image> suiteimage;

    public List<Sprite> multiplierImages;
    public List<Text> HistoryCardTimer;
    public string apiUrl;
    public string livedatasapi;

    public string gameName = "jeetoJoker";

    private void Awake()
    {
        Instance = this;

    }
    private void Start()
    {
        CardHistory();
    }
public IEnumerator setResultRequest(string result)
{
    int ISBET;

    if (JeetoJokerManager.instance.totalBet > 0)
    {
        ISBET = 1;
    }
    else
    {
        ISBET = 0;
    }

    WWWForm form4 = new WWWForm();
    form4.AddField("win_number", result);
    form4.AddField("game_name", gameName);
    form4.AddField("user_id", PlayerPrefs.GetInt(Constant.STOKIESID.ToString()).ToString());
    form4.AddField("is_beted", ISBET.ToString());

    Debug.Log($"User ID: {PlayerPrefs.GetInt(Constant.STOKIESID.ToString())}");

    UnityWebRequest uwr = UnityWebRequest.Post(livedatasapi, form4);

    yield return uwr.SendWebRequest();

    if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
    {
        Debug.LogError($"Error While Sending: {uwr.error}");
    }
    else
    {
        if (uwr.responseCode == 200)
        {
            Debug.Log($"Request succeeded. Response: {uwr.downloadHandler.text}");
            StartCoroutine(CardHistoryDelay());
        }
        else
        {
            Debug.LogError($"Unexpected HTTP Response: {uwr.responseCode}");
        }
    }
}

    public IEnumerator CardHistoryDelay()
    {
        //changes by shivamfusion07
        //card history result showing in 3 seconds
        yield return new WaitForSeconds(15);
        Debug.Log("delay in showing card histroy live 12 seconds");
        CardHistory();
    }
    public void CardHistory()
    {
        StartCoroutine(PostRequest());
    }
    public IEnumerator PostRequest()
    {
        Debug.Log(PlayerPrefs.GetInt(Constant.STOKIESID.ToString()).ToString() + "CardHistoryLive");

        Dictionary<string, string> formData = new Dictionary<string, string>();
        formData.Add("game_name", gameName.ToString());
        formData.Add("player_id", PlayerPrefs.GetInt(Constant.STOKIESID.ToString()).ToString());
        WWWForm form = new WWWForm();
        foreach (var entry in formData)
        {
            form.AddField(entry.Key, entry.Value);
        }

        UnityWebRequest request = UnityWebRequest.Post(apiUrl, form);
        request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

        yield return request.SendWebRequest(); // Send the request and wait for it to complete

        if (request.responseCode == 429) // Check for rate limit exceeded (HTTP 429)
        {
            Debug.LogWarning("Rate limit exceeded. Waiting before making the next request.");
            yield return new WaitForSeconds(7.0f); // Wait for 7 seconds (adjust as needed)
            CardHistory(); // Retry the request after waiting
            yield break; // Exit the coroutine to avoid processing the response
        }

        if (request.result == UnityWebRequest.Result.ConnectionError ||
            request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error:CardHistoryLive " + request.error);
        }
        else
        {
            WinNumber.Clear();
            Timer.Clear();

            string responseBody = request.downloadHandler.text;
            Debug.Log(responseBody + "CardHistoryLive");
            ApiResponse apiResponse = JsonUtility.FromJson<ApiResponse>(responseBody);

            ApiData[] dataList = apiResponse.list;

            foreach (ApiData data in dataList)
            {
                WinNumber.Add(data.win_card);
                //DateTime dateTime = DateTime.Parse(data.bonous_spin);
                //string formattedTime = dateTime.ToString("h:mm");
                Timer.Add(data.bonous_spin);
            }

            

            MapWinNumbersToSprites();

            Debug.Log(WinNumber.Count + "xyzz");
        }

    }
    private void MapWinNumbersToSprites()
    {
      
        for (int i = 0; i < nullImage.Count; i++)
        {
            int index = GetImageIndex(Timer[i]);
            nullImage[i].GetComponent<Image>().sprite = multiplierImages[index];
            string winNum = WinNumber[i];
            if (winNum == "C_1,S_1")
            {
                cardimage[i].sprite = c1.sprite;
                suiteimage[i].sprite = s1.sprite;
            }
            else if (winNum == "C_1,S_2")
            {
                cardimage[i].sprite = c1.sprite;
                suiteimage[i].sprite = s2.sprite;
            }
            else if (winNum == "C_1,S_3")
            {

                cardimage[i].sprite = c1.sprite;
                suiteimage[i].sprite = s3.sprite;
            }
            else if (winNum == "C_1,S_4")
            {

                cardimage[i].sprite = c1.sprite;
                suiteimage[i].sprite = s4.sprite;
            }
            else if (winNum == "C_2,S_1")
            {
                cardimage[i].sprite = c2.sprite;
                suiteimage[i].sprite = s1.sprite;
            }
            else if (winNum == "C_2,S_2")
            {
                cardimage[i].sprite = c2.sprite;
                suiteimage[i].sprite = s2.sprite;
            }
            else if (winNum == "C_2,S_3")
            {

                cardimage[i].sprite = c2.sprite;
                suiteimage[i].sprite = s3.sprite;
            }
            else if (winNum == "C_2,S_4")
            {
                cardimage[i].sprite = c2.sprite;
                suiteimage[i].sprite = s4.sprite;
            }
            else if (winNum == "C_3,S_1")
            {

                cardimage[i].sprite = c3.sprite;
                suiteimage[i].sprite = s1.sprite;
            }
            else if (winNum == "C_3,S_2")
            {

                cardimage[i].sprite = c3.sprite;
                suiteimage[i].sprite = s2.sprite;
            }
            else if (winNum == "C_3,S_3")
            {

                cardimage[i].sprite = c3.sprite;
                suiteimage[i].sprite = s3.sprite;
            }
            else if (winNum == "C_3,S_4")
            {

                cardimage[i].sprite = c3.sprite;
                suiteimage[i].sprite = s4.sprite;
            }
        }
    }

    public int GetImageIndex(string mult) {

        if (mult == "1x") return 0;
        else if (mult == "2x") return 1;
        else if (mult == "3x") return 2;
        else if (mult == "4x") return 3;
        else if (mult == "5x") return 4;
        else if (mult == "6x") return 5;
        else if (mult == "7x") return 6;
        else if (mult == "8x") return 7;
        else if (mult == "9x") return 8;
        else if (mult == "10x") return 9;
        else if (mult == "11x") return 10;
        else if (mult == "12x") return 11;
        else if (mult == "13x") return 12;
        else if (mult == "14x") return 13;
        else if (mult == "15x") return 14;
        else if (mult == "16x") return 15;
        else if (mult == "17x") return 16;
        else if (mult == "18x") return 17;
        else if (mult == "19x") return 18;
        else if (mult == "20x") return 19;

        else return 0;


    }

}
