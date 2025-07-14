using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace khelojeetonew
{
    public class CardHistoryDeck : MonoBehaviour
    {

        public SixteenCars_Foo foo;

        [SerializeField] private List<CardHistoryContent> cardHistoryContents;

        private List<LastResultData> lastResultDatas = new List<LastResultData>();

        [SerializeField] private int maxResultCount = 10;
        string key;
        IEnumerator tet()
        {
            yield return new WaitForSeconds(3);
           
        }
        public void Initialize(string userName)
        {
            StartCoroutine(tet());
            int count = cardHistoryContents.Count;
            for (int i = 0; i < count; i++)
            {
                cardHistoryContents[i].gameObject.SetActive(false);
            }
            key = $"{JeetoJokerManager.instance.gameType}/{userName}/cardHistory";
            if (PlayerPrefs.HasKey(key))
            {
                lastResultDatas = JsonConvert.DeserializeObject<List<LastResultData>>(PlayerPrefs.GetString(key));
                count = lastResultDatas.Count;
                for (int i = 0; i < count; i++)
                {
                    cardHistoryContents[i].Initialize(lastResultDatas[i]);
                    cardHistoryContents[i].gameObject.SetActive(true);
                }
            }
        }


        public void PushCardData(int cardId, int suitId)
        {
            string multiplier = "";
            if (SocketController.Instance.is_x_excuted.Equals(Constants.zero))
            {
                multiplier = SocketController.Instance.win_price;                
            }
            lastResultDatas.Insert(0, new LastResultData(cardId, suitId, multiplier));
            if (lastResultDatas.Count > maxResultCount)
            {
                lastResultDatas.RemoveAt(maxResultCount);
            }
            int count = lastResultDatas.Count;
            for(int i = 0; i < count; i++)
            {
                cardHistoryContents[i].Initialize(lastResultDatas[i]);
                cardHistoryContents[i].gameObject.SetActive(true);
            }

            PlayerPrefs.SetString(key, JsonConvert.SerializeObject(lastResultDatas));
        }

        public IEnumerator GetGameHistoryData()
        {
            UnityWebRequest unityWebRequest = UnityWebRequest.Get("https://mplgames.in/admin/api/live-data-history");
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
    public class LastResultData
    {
        public int CardValue = 0;
        public int SuiteValue = 0;
        public string multiplier = "";

        public LastResultData()
        {
        }

        public LastResultData(int cardValue, int suiteValue, string multiplier)
        {
            CardValue = cardValue;
            SuiteValue = suiteValue;
            this.multiplier = multiplier;
        }
    }

    [SerializeField]
    public class GameHistoryData
    {
        public List<GameHistoryItems> list;
        public string status;
        public string message;
    }

    [SerializeField]
    public class GameHistoryItems
    {
        public string id;
        public string player_id;
        public string win_card;
        public string bonous_spin;
        public string created_at;
        public string updated_at;
    }
}
