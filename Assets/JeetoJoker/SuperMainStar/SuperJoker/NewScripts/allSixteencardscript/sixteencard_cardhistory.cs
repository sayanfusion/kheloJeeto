using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace khelojeetonew
{
    public class sixteencard_cardhistory : MonoBehaviour
{
       // public sixteencards_wheelbase foo;

        [SerializeField] private List<sixteencard_cardhistoryContent> cardHistoryContents;

        private List<LastResultData1> lastResultDatas1 = new List<LastResultData1>();

        [SerializeField] private int maxResultCount = 10;
        string key;

        public void Initialize(string userName)
        {
            int count = cardHistoryContents.Count;
            for (int i = 0; i < count; i++)
            {
                cardHistoryContents[i].gameObject.SetActive(true);
            }
            key = $"{Sixteen_cards.instance.gameType}/{userName}/cardHistory";
            if (PlayerPrefs.HasKey(key))
            {
                lastResultDatas1 = JsonConvert.DeserializeObject<List<LastResultData1>>(PlayerPrefs.GetString(key));
                count = lastResultDatas1.Count;
                for (int i = 0; i < count; i++)
                {
                    cardHistoryContents[i].Initialize(lastResultDatas1[i]);
                    cardHistoryContents[i].gameObject.SetActive(true);
                }
            }
        }

        public void PushCardData(int cardId, int suitId)
        {
            string multiplier = "";
            if (SixteenCardsSocketController.Instance.is_x_excuted.Equals(Constants.zero))
            {
                multiplier = SixteenCardsSocketController.Instance.win_price;
            }
            lastResultDatas1.Insert(0, new LastResultData1(cardId, suitId, multiplier));
            if (lastResultDatas1.Count > maxResultCount)
            {
                lastResultDatas1.RemoveAt(maxResultCount);
            }
            int count = lastResultDatas1.Count;
            for (int i = 0; i < count; i++)
            {
                cardHistoryContents[i].Initialize(lastResultDatas1[i]);
                cardHistoryContents[i].gameObject.SetActive(true);
            }

            PlayerPrefs.SetString(key, JsonConvert.SerializeObject(lastResultDatas1));
        }

        public IEnumerator GetGameHistoryData()
        {
            string requestBody = "{\"" + "game_name" + "\":\"" + "16cards" + "\"}";
            UnityWebRequest unityWebRequest = UnityWebRequest.PostWwwForm("https://mplgames.in/admin/api/live-data-history", requestBody);
            unityWebRequest.SetRequestHeader(Constants.authorization, "Bearer " + PlayerPrefs.GetString(Constants.token));
            yield return unityWebRequest.SendWebRequest();

            if (unityWebRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(unityWebRequest.error);
                JeetoJokerManager.instance.ShowMessage(unityWebRequest.error);
                Debug.LogError(unityWebRequest.downloadHandler.text);
            }
            else
            {
                Debug.LogError(unityWebRequest.downloadHandler.text);
                GameHistoryData gameHistoryData = JsonConvert.DeserializeObject<GameHistoryData>(unityWebRequest.downloadHandler.text);
                Debug.LogError("gameHistoryData.list.Count " + gameHistoryData.list.Count);
                for (int i = 0; i < gameHistoryData.list.Count; i++)
                {
                    cardHistoryContents[i].PopulateGameHistoryData(gameHistoryData.list[i]);
                    cardHistoryContents[i].gameObject.SetActive(true);
                }
            }
        }
    }

    [System.Serializable]
    public class LastResultData1
    {
        public int CardValue = 0;
        public int SuiteValue = 0;
        public string multiplier = "";

        public LastResultData1()
        {
        }

        public LastResultData1(int cardValue, int suiteValue, string multiplier)
        {
            CardValue = cardValue;
            SuiteValue = suiteValue;
            this.multiplier = multiplier;
        }
    }

    [SerializeField]
    public class SixteenGameHistoryData
    {
        public List<SixteenGameHistoryItems> list;
        public string status;
        public string message;
    }

    [SerializeField]
    public class SixteenGameHistoryItems
    {
        public string id;
        public string player_id;
        public string win_card;
        public string bonous_spin;
        public string created_at;
        public string updated_at;
    }
}


