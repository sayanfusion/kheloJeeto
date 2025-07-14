using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
[System.Serializable]
public class ApiData16
{
    public int id;
    public string win_number;
    public string win_card;
    public string game_name;
    public string bonous_spin;
    public string created_at;
    public string updated_at;
}

[System.Serializable]
public class ApiResponse16
{
    public ApiData16[] list;
}



public class APICardHistory16 : MonoBehaviour
{
    public static APICardHistory16 Instance;
    public bool isstart = true;
    public SpriteRenderer c1;
    public SpriteRenderer c2;
    public SpriteRenderer c3;
    public SpriteRenderer c4;
    public SpriteRenderer s1;
    public SpriteRenderer s2;
    public SpriteRenderer s3;
    public SpriteRenderer s4;
    public List<string> WinNumber;

 
    public List<Image> cardimage;
    public List<Image> suiteimage;
    public List<string> Timer;
    public List<GameObject> multiPlayers;

    public string apiUrl;
    public string gameName;
    public string key;
    private bool isRequestInProgress = false;
    private void Awake()
    {
        Instance = this;

    }
    private void Start()
    {
        CardHistory();
    }
    private void Update()
    {
        /*if (isstart == true && !isRequestInProgress) // Check if a request is not in progress
        {
            CardHistory();
            StartCoroutine(falseistrue());
        }*/
    }
    IEnumerator falseistrue()
    {
        yield return new WaitForSeconds(3);
        isstart = false;

    }

    public void CardHistory()
    {
        StartCoroutine(PostRequest());
    }
    // IEnumerator PostRequest(string url, string key, string value)
    // {
    //     Debug.Log(key + " Result_History " + value);
    //     string requestBody = "{\"" + key + "\":\"" + value + "\"}";
    //     using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
    //     {
    //         byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(requestBody);
    //         webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
    //         webRequest.downloadHandler = new DownloadHandlerBuffer();
    //         webRequest.SetRequestHeader("Content-Type", "application/json");

    //         var operation = webRequest.SendWebRequest();
    //         while (!operation.isDone)
    //         {
    //             yield return null;
    //         }
    //         isRequestInProgress = false;//

    //         /*   if (webRequest.responseCode == 429) // Check for rate limit exceeded (HTTP 429)
    //            {
    //                Debug.LogWarning("Rate limit exceeded. Waiting before making the next request.");
    //                yield return new WaitForSeconds(7.0f); // Wait for 7h l seconds (adjust as needed)
    //                CardHistory(); // Retry the request after waiting
    //                yield break; // Exit the coroutine to avoid processing the response
    //            }*/

    //         if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
    //             webRequest.result == UnityWebRequest.Result.ProtocolError)
    //         {
    //             Debug.LogError("Error: " + webRequest.error);
    //         }
    //         else
    //         {
    //             WinNumber.Clear();

    //             Debug.Log("Data " + webRequest.downloadHandler.text);

    //             string responseBody = webRequest.downloadHandler.text;
    //             ApiResponse16 apiResponse = JsonUtility.FromJson<ApiResponse16>(responseBody);

    //             ApiData16[] dataList = apiResponse.list;
    //             Debug.Log("Win Number: Main");
    //             foreach (ApiData16 data in dataList)
    //             {
    //                 Debug.Log("Win Number: " + data.win_card);
    //                 WinNumber.Add(data.win_card);


    //             }
    //             //Debug.Log("Timer " + Timer.Count);



    //             MapWinNumbersToSprites16(dataList);


    //         }
    //     }
    // }
    IEnumerator PostRequest()
{

    // Prepare the form data with a fixed key ("value")
    Dictionary<string, string> formData = new Dictionary<string, string>();
    formData.Add("game_name", gameName.ToString());
    formData.Add("player_id", PlayerPrefs.GetInt(Constant.STOKIESID.ToString()).ToString());
    WWWForm form = new WWWForm();
    foreach (var entry in formData)
    {
        form.AddField(entry.Key, entry.Value);
    }

    // Create the UnityWebRequest for POST
    UnityWebRequest request = UnityWebRequest.Post(apiUrl, form);

    request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

    yield return request.SendWebRequest(); // Send the request and wait for it to complete

    // Check for rate limit (HTTP 429)
    if (request.responseCode == 429)
    {
        Debug.LogWarning("Rate limit exceeded. Waiting before making the next request.");
        yield return new WaitForSeconds(7.0f); // Wait for 7 seconds (adjust as needed)
        StartCoroutine(PostRequest()); // Retry the request after waiting
        yield break; // Exit the coroutine to avoid processing the response
    }

    // Check for connection errors or protocol errors
    if (request.result == UnityWebRequest.Result.ConnectionError ||
        request.result == UnityWebRequest.Result.ProtocolError)
    {
        Debug.LogError("Error: " + request.error);
    }
    else
    {
        // Clear previous results
        WinNumber.Clear();
        Timer.Clear();

        // Log the response text for debugging
        Debug.Log("Data " + request.downloadHandler.text);

        // Process the response (assumed to be JSON formatted)
        string responseBody = request.downloadHandler.text;
        ApiResponse16 apiResponse = JsonUtility.FromJson<ApiResponse16>(responseBody);

        ApiData16[] dataList = apiResponse.list;
        Debug.Log("Win Number: Main");

        // Loop through the data and add win numbers to the list
        foreach (ApiData16 data in dataList)
        {
            Debug.Log("Win Number: " + data.win_card);
            WinNumber.Add(data.win_card);
            DateTime dateTime = DateTime.Parse(data.created_at);
            string formattedTime = dateTime.ToString("h:mm");
            Timer.Add(formattedTime);
        }

        // Map win numbers to sprites (assuming this is a separate method)
        MapWinNumbersToSprites16(dataList);
    }
}

    public List<Text> HistoryCardTimer;
    private void MapWinNumbersToSprites16(ApiData16[] dataList)
    {
        for (int i = 0; i < WinNumber.Count; i++)
        {
            Debug.Log("HelloWorld16 " + WinNumber[i]);
            // string itemCard = WinNumber[i].Split(',')[0];
            // string itemSuite = WinNumber[i].Split(',')[1];
            // cardimage[i].sprite = Resources.Load<Sprite>("SJ_Resources/" + "Black_" + itemCard);
            // suiteimage[i].sprite = Resources.Load<Sprite>("SJ_Resources/" + itemSuite);
            // Debug.Log("HelloWorld16 " + dataList[i].bonous_spin);
            // if (string.IsNullOrEmpty(dataList[i].bonous_spin) || dataList[i].bonous_spin == "1x")
            // {
            //     multiPlayers[i].SetActive(false);
            // }
            // else
            // {
            //     multiPlayers[i].transform.GetChild(0).GetComponent<Text>().text = dataList[i].bonous_spin;
            //     multiPlayers[i].SetActive(true);
            // }
            HistoryCardTimer[i].text = Timer[i];
            int winNum = int.Parse(WinNumber[i]);
            if (winNum == 11)
            {
                cardimage[i].sprite = c1.sprite;
                suiteimage[i].sprite = s1.sprite;
            }
            else if (winNum == 12)
            {
                cardimage[i].sprite = c1.sprite;
                suiteimage[i].sprite = s2.sprite;
            }
            else if (winNum == 13)
            {

                cardimage[i].sprite = c1.sprite;
                suiteimage[i].sprite = s3.sprite;
            }
            else if (winNum == 14)
            {

                cardimage[i].sprite = c1.sprite;
                suiteimage[i].sprite = s4.sprite;
            }
            else if (winNum == 21)
            {
                cardimage[i].sprite = c2.sprite;
                suiteimage[i].sprite = s1.sprite;
            }
            else if (winNum == 22)
            {
                cardimage[i].sprite = c2.sprite;
                suiteimage[i].sprite = s2.sprite;
            }
            else if (winNum == 23)
            {

                cardimage[i].sprite = c2.sprite;
                suiteimage[i].sprite = s3.sprite;
            }
            else if (winNum == 24)
            {
                cardimage[i].sprite = c2.sprite;
                suiteimage[i].sprite = s4.sprite;
            }
            else if (winNum == 31)
            {

                cardimage[i].sprite = c3.sprite;
                suiteimage[i].sprite = s1.sprite;
            }
            else if (winNum == 32)
            {

                cardimage[i].sprite = c3.sprite;
                suiteimage[i].sprite = s2.sprite;
            }
            else if (winNum == 33)
            {

                cardimage[i].sprite = c3.sprite;
                suiteimage[i].sprite = s3.sprite;
            }
            else if (winNum == 34)
            {

                cardimage[i].sprite = c3.sprite;
                suiteimage[i].sprite = s4.sprite;
            }
            else if (winNum == 41)
            {

                cardimage[i].sprite = c4.sprite;
                suiteimage[i].sprite = s1.sprite;
            }
            else if (winNum == 42)
            {

                cardimage[i].sprite = c4.sprite;
                suiteimage[i].sprite = s2.sprite;
            }
            else if (winNum == 43)
            {

                cardimage[i].sprite = c4.sprite;
                suiteimage[i].sprite = s3.sprite;
            }
            else if (winNum == 44)
            {

                cardimage[i].sprite = c4.sprite;
                suiteimage[i].sprite = s4.sprite;
            }
            
        }
    }
}

