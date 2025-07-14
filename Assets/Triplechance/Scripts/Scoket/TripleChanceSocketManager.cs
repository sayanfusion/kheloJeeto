using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using BestHTTP.SocketIO;
using BestHTTP.SocketIO.Events;
using System.Text;
using Newtonsoft.Json;
using SimpleJSON;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using System.Linq;
using UnityEngine.Networking;
using BestHTTP.JSON;

namespace tripplechance
{
    public class TripleChanceSocketManager : MonoBehaviour
    {

        public static SocketManager Core;
        public static SocketManager NspCore;
        private static event SocketIOCallback OnListen;
        public string socketLink;
        public static TripleChanceSocketManager tripleChanceSocketManager;
        bool istrue = false;
        public static event System.Action closeSlider;

        private string roomid;
        public GameObject loadingPanel;
        [SerializeField] private GamePlay gamePlay;
        [SerializeField] private Timer timer;
        private void Awake()
        {
            if (tripleChanceSocketManager == null)
            {
                tripleChanceSocketManager = this;
            }
            else
            {
                Destroy(this);
            }
        }
        private void Start() => Connect();

        private void Connect()
        {

            //Config.SocketConfig.Host.Trim();
            StringBuilder builder = new StringBuilder();
            builder.Append(socketLink.Trim());
            builder.Append(socketLink.EndsWith("/") ? "socket.io/" : "/socket.io/");

            Core = new SocketManager(new Uri(builder.ToString()));
            Core.Socket.On(SocketIOEventTypes.Connect, onConnected);
            Core.Socket.On(SocketIOEventTypes.Disconnect, onDisconnected);
            Core.Socket.On(SocketIOEventTypes.Error, onConnectedError);
            Core.Socket.On(SocketIOEventTypes.Unknown, onUnknownError);
        }
        private void onUnknownError(Socket socket, Packet packet, object[] args)
        {

        }
        private void onConnectedError(Socket socket, Packet packet, object[] args)
        {

        }
        private void onDisconnected(Socket socket, Packet packet, object[] args)
        {
            Debug.LogError("onDisconnected : " + socket);
        }
        private void onConnected(Socket socket, Packet packet, object[] args)
        {
            Debug.Log("SocketConnected : " + socket);
            AddAllListner();
        }
        private void AddAllListner()
        {
            #region roomJoin
            Core.Socket.On("createRoomSuccess", CreateRoomSuccess);
            Core.Socket.On("startGame", StartGame);
            Core.Socket.On("updatedPlayers", UpdatedPlayers);
            Core.Socket.On("updatedRoom", UpdatedRoom);
            Core.Socket.On("roomMessage", RoomMessage);
            Core.Socket.On("updatedPlayer", UpdatedPlayer);
            Core.Socket.On("errorOccured", ErrorOccured);
            #endregion
            #region Start

            Core.Socket.On("timer", Timer);
            Core.Socket.On("roomData", RoomData);
            Core.Socket.On("betting", Betting);
            Core.Socket.On("slot", Slot);
            #endregion
            Core.Socket.On("mode", Mode);
            Core.Socket.On("playersBetInfo", PlayersBetInfo);
            Core.Socket.On("gameId", SetGameId);
            // Core.Socket.On("winDetails", WinDetails);
            // Core.Socket.On("sumDetails", SumDetails);
            EmmiteJoinRoom();

        }

        private void SetGameId(Socket socket, Packet packet, object[] args)
        {
            string s = packet.RemoveEventName(true);
            gamePlay.gameId = s;
        }



        
        private void EmmiteJoinRoom()
        {
            Debug.Log("EMITE JOIN ROOM");
            SendData.JoinRoom joinRoom = new SendData.JoinRoom();
            joinRoom.playerId = PlayerPrefs.GetInt(Constant.UID).ToString();
            joinRoom.name = PlayerPrefs.GetString(Constant.User);
            joinRoom.totalCoin = PlayerPrefs.GetInt(Constants.BALANCE);

            string rawJson = JsonConvert.SerializeObject(joinRoom);
            Debug.LogError(rawJson);
            Dictionary<string, object> keyValuePairs = JsonConvert.DeserializeObject<Dictionary<string, object>>(rawJson);
            Core.Socket.Emit("joinRoom", keyValuePairs);
           
        }
        
        private void EmmiteStartGame()
        {
            SendData.Start startgame = new SendData.Start();
            startgame.roomId = Constants.ROOMID;
            string rawJson = JsonConvert.SerializeObject(startgame);
            Dictionary<string, object> keyValuePairs = JsonConvert.DeserializeObject<Dictionary<string, object>>(rawJson);
            Debug.Log("Emit Start");
            Core.Socket.Emit("start", keyValuePairs);
        }

        private void EmmiteBet()
        {
            SendData.Bet bet = new SendData.Bet();
            bet.roomId = Constants.ROOMID;
            bet.playerId = PlayerPrefs.GetInt(Constant.UID).ToString();
            bet.start_point = Convert.ToInt32(gamePlay.IPointBalance);
            bet.playerBetSum = gamePlay.playValue.ToString();
            Debug.Log("player bet sum:"+ bet.playerBetSum);

            // Initialize the cardValueSet list
            bet.cardValueSet = new List<SendData.CardValueSet>();

            // Serialize 'allDatas' separately
            string allDatasJson = JsonConvert.SerializeObject(gamePlay.allDatas);
            // Deserialize 'allDatasJson' back into an object
            var allDatas = JsonConvert.DeserializeObject<AllDatas>(allDatasJson);

            foreach (var data in allDatas.singleDatas)
            {
                SendData.CardValueSet cardValue = new SendData.CardValueSet();
                if (Convert.ToInt32(data.value) > 0)
                {
                    cardValue.card = data.card;
                    cardValue.value = Convert.ToInt32(data.value);
                    bet.cardValueSet.Add(cardValue);
                }
            }

            foreach (var data in allDatas.doubleDatas)
            {
                SendData.CardValueSet cardValue = new SendData.CardValueSet();
                if (Convert.ToInt32(data.value) > 0)
                {
                    cardValue.card = data.card;
                    cardValue.value = Convert.ToInt32(data.value);
                    bet.cardValueSet.Add(cardValue);
                }
            }

            foreach (var data in allDatas.tripleDatas)
            {
                SendData.CardValueSet cardValue = new SendData.CardValueSet();
                if (Convert.ToInt32(data.value) > 0)
                {
                    cardValue.card = data.card;
                    cardValue.value = Convert.ToInt32(data.value);
                    bet.cardValueSet.Add(cardValue);
                }
            }
            string rawJson = JsonConvert.SerializeObject(bet);
            Debug.LogError(rawJson);
            Debug.Log("bet emitted data triplechance timer shivamfusion07"+rawJson);
            Core.Socket.Emit("bet", rawJson);
            Debug.LogError("Bet event emitted successfully.");
        }
        private void EmmiteMode()
        {
            SendData.Mode mode = new SendData.Mode();
            mode.roomId = Constants.ROOMID;
            mode.mode = gamePlay.WinningType;
            string rawJson = JsonConvert.SerializeObject(mode);
            Debug.LogError(rawJson);
            Dictionary<string, object> keyValuePairs = JsonConvert.DeserializeObject<Dictionary<string, object>>(rawJson);
            Core.Socket.Emit("mode", keyValuePairs);
        }
        public void OnApplicationQuit()
        {
            EmmiteLeave();
        }
        public void EmmiteLeave()
        {
            SendData.Leave leave = new SendData.Leave();
            leave.roomId = Constants.ROOMID;
            leave.playerId = PlayerPrefs.GetInt(Constant.UID).ToString();
            string rawJson = JsonConvert.SerializeObject(leave);
            Debug.LogError("leave " + rawJson);
            Dictionary<string, string> keyValuePairs = JsonConvert.DeserializeObject<Dictionary<string, string>>(rawJson);
            try
            {
                Core.Socket.Emit(Constants.leave, keyValuePairs);
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
            /* SendData.Leave leave = new SendData.Leave();
            leave.roomId = Constants.ROOMID;
            leave.playerId = PlayerPrefs.GetInt(Constant.UID).ToString();
            string rawJson = JsonConvert.SerializeObject(leave);
            Dictionary<string, object> keyValuePairs = JsonConvert.DeserializeObject<Dictionary<string, object>>(rawJson);
            Core.Socket.Emit("leave", keyValuePairs); */
        }

        #region roomJoin

        private void CreateRoomSuccess(Socket socket, Packet packet, params object[] args)
        {
            string s = packet.RemoveEventName(true);
            Debug.Log("CreateRoomSuccess : " + s);
            ResponseData.CreateRoomSuccess createRomm = JsonUtility.FromJson<ResponseData.CreateRoomSuccess>(s);
            Constants.ROOMID = createRomm._id;
        }
        
        private void StartGame(Socket socket, Packet packet, params object[] args)
        {
            string s = packet.RemoveEventName(true);
            Debug.Log("StartGame : " + s);
            EmmiteStartGame();      
        }
        private void UpdatedPlayers(Socket socket, Packet packet, params object[] args)
        {
            string s = packet.RemoveEventName(true);
            Debug.Log("UpdatedPlayers : " + s);

        }
        private void UpdatedRoom(Socket socket, Packet packet, params object[] args)
        {
            string s = packet.RemoveEventName(true);
            Debug.Log("UpdatedRoom : " + s);
            ResponseData.UpdatedRoom createRomm = JsonUtility.FromJson<ResponseData.UpdatedRoom>(s);
            Constants.ROOMID = createRomm._id;
            Debug.Log("UpdatedRoom success : " + createRomm._id);
        }
        private void UpdatedPlayer(Socket socket, Packet packet, params object[] args)
        {
            string s = packet.RemoveEventName(true);
            Debug.Log("UpdatedPlayer : " + s);
        }
        private void RoomMessage(Socket socket, Packet packet, params object[] args)
        {
            string s = packet.RemoveEventName(true);
            Debug.Log("RoomMessage : " + s);
        }
        private void ErrorOccured(Socket socket, Packet packet, params object[] args)
        {
            string s = packet.RemoveEventName(true);
            Debug.Log("errorOccured : " + s);
        }
        #endregion
        #region Start
        public bool isGameStart;
        private void Timer(Socket socket, Packet packet, params object[] args)
        {
            loadingPanel?.SetActive(false);
            string s = packet.RemoveEventName(true);
            Debug.Log("timer :" + int.Parse(s));
            timer.UpdateTimer(int.Parse(s));
            // Debug.Log("Timer " + int.Parse(s) + "isGameStarted = " + isGameStart);
            if (int.Parse(s) > 5 /*&& int.Parse(s) <= 90*/)
            {
                if (!isGameStart)
                {
                    isGameStart = true;
                    Debug.Log("Starting Game : " + isGameStart);
                    gamePlay.StartGame(float.Parse(s));
                }
            }
            if (int.Parse(s) == 0)
            {
                isGameStart = false;
            }
             if(int.Parse(s) == 8)
            {
                gamePlay.GETbetData();
            }

            if (int.Parse(s) == 6)
            {
                // EmmiteMode();
                EmmiteBet();
#if UNITY_ANDROID

               gamePlay.alertPanel?.SetActive(false);
                closeSlider?.Invoke();
#endif
            }
        }
        private void RoomData(Socket socket, Packet packet, params object[] args)
        {
            string s = packet.RemoveEventName(true);
            Debug.Log("RoomData : " + s);
        }
        private void Betting(Socket socket, Packet packet, params object[] args)
        {
            string s = packet.RemoveEventName(true);
            Debug.Log("Betting : " + s);
        }
        private void Slot(Socket socket, Packet packet, params object[] args)
        {
            string s = packet.RemoveEventName(true);
            string number = s.ToString().Trim('"');
            if (number != null)
            {
                Constant.ResultNumber = number;
               
               // GamePlay.instance.StartSpinning();
                if ( GamePlay.instance.timerScript.timeLeft <= 0)
              {
               {
                GamePlay.instance.StartSpinning();
                Debug.Log("Spin triggered when timer is 00");
            }
        }

                DataClass gameWinTripleDataClass = gamePlay.allDatas.tripleDatas.Find(x => x.card.Equals(number));
                if (gameWinTripleDataClass != null)
                {
                    DataClass dd = gamePlay.allDatas.doubleDatas.Find(x => x.card.Equals(gameWinTripleDataClass.card.Substring(1)));
                    DataClass ss = gamePlay.allDatas.singleDatas.Find(x => x.card.Equals(gameWinTripleDataClass.card.Substring(2)));

                    long totalWinAmount = 0;
                    if (SceneManager.GetActiveScene().name == "TripleChanceProGameplay")
                    {
                        totalWinAmount = (gameWinTripleDataClass.value * 1000) + (dd.value * 100) + (ss.value * 10);
                    }
                    else
                    {
                        totalWinAmount = (gameWinTripleDataClass.value * 900) + (dd.value * 90) + (ss.value * 9);
                    }

                    Debug.Log("Slot : " + s + "Total Win Amount : " + totalWinAmount);
                    // if (SceneManager.GetActiveScene().name.Equals("TripleChanceProGameplay"))
                    // {
                    //     if (gamePlay.multiplier >= 1)
                    //     {
                    //         totalWinAmount *= gamePlay.multiplier;
                    //         Constant.WinAmount = totalWinAmount.ToString();
                    //     }
                    //     Constant.WinAmount = totalWinAmount.ToString();
                    // }
                    // else
                    // {
                    //     Constant.WinAmount = totalWinAmount.ToString();
                    // }
                    Constant.WinAmount = totalWinAmount.ToString();

                    gamePlay.SetWinValue();

                    int playerId = PlayerPrefs.GetInt(Constant.UID);
                    long start_point = (long)gamePlay.IPointBalance + (long)gamePlay.playValue;
                    string win_number = number;
                    string Game_Id = gamePlay.gameId;
                    long win_Amount = totalWinAmount;
                    long bet_amount =   (long)gamePlay.playValue;
                    List<DataClass> betData = new List<DataClass>();

                    foreach (var data in gamePlay.allDatas.singleDatas)
                    {
                        DataClass cardValue = new();
                        if (Convert.ToInt32(data.value) > 0)
                        {
                            cardValue.card = data.card;
                            cardValue.value = Convert.ToInt32(data.value);
                            betData.Add(cardValue);
                        }
                    }

                    foreach (var data in gamePlay.allDatas.doubleDatas)
                    {
                        DataClass cardValue = new();
                        if (Convert.ToInt32(data.value) > 0)
                        {
                            cardValue.card = data.card;
                            cardValue.value = Convert.ToInt32(data.value);
                            betData.Add(cardValue);
                        }
                    }

                    foreach (var data in gamePlay.allDatas.tripleDatas)
                    {
                        DataClass cardValue = new();
                        if (Convert.ToInt32(data.value) > 0)
                        {
                            cardValue.card = data.card;
                            cardValue.value = Convert.ToInt32(data.value);
                            betData.Add(cardValue);
                        }
                    }
                    // gamePlay.IPointBalance += totalWinAmount;
                    string game_name;
                    if (SceneManager.GetActiveScene().name.Equals("TripleChanceProGameplay"))
                    {
                        game_name = "tripleChancePro";
                    }
                    else
                    {
                        game_name = "tripleChance";
                    }
                    StartCoroutine(SendGameData(playerId, start_point, Game_Id, win_number, win_Amount, betData, bet_amount, game_name));
                    // if (gamePlay.multiplier > 1)
                    // {
                    //     gamePlay.ResetWinningCount();
                    // }
                }
            }

        }

        private IEnumerator SendGameData(int playerId, long start_point, string GameID, string win_number, long win_amount, List<DataClass> gameData, long bet_amount, string game_name = "tripleChance")
        {
            // win_number
            // game_name
            // start_point
            // game_id
            // win_ammount
            // playerId
            // gamedata
            // bet_ammount
            string url = Constant.KIBaseURL + "game_data_insert";
            Debug.Log("Game Data Insert URL : " + url);

            UnityWebRequest request = new UnityWebRequest(url, "POST");

            GameDataInsertClass gdi = new GameDataInsertClass
            {
                playerId = playerId,
                win_Number = win_number,
                game_name = game_name,
                start_point = start_point,
                game_id = GameID,
                win_Amount = win_amount,
                draw_time ="",
                bonus_spin="",
                claim_status=1,
                gameData = gameData,
                bet_ammount = bet_amount
            };

            string json = JsonConvert.SerializeObject(gdi);
            Debug.Log("Game Data Insert JSON " + json);

            byte[] bodyData = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyData);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

            // Set the content type header
            request.SetRequestHeader("Content-Type", "application/json");

            // Send the request
            yield return request.SendWebRequest();
            try
            {
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Game Data Insert Error sending request: " + request.error);
                }
                else
                {
                    // Request was successful, print the response
                    Debug.Log("Game Data Insert JSON Successfully: " + request.downloadHandler.text);
                }
            }
            catch (Exception ex)
            {
                Debug.Log("Game Data Insert Exception " + ex);
            }
        }


        #endregion
        #region mode
        private void Mode(Socket socket, Packet packet, params object[] args)
        {
            string s = packet.RemoveEventName(true);
            Debug.Log("Mode : " + s);
        }
        #endregion
        private void PlayersBetInfo(Socket socket, Packet packet, params object[] args)
        {
            string s = packet.RemoveEventName(true);
            Debug.Log("PlayersBetInfo : " + s);
        }
        // private void WinDetails(Socket socket, Packet packet, params object[] args)
        // {
        //     string s = packet.RemoveEventName(true);
        //     //s = " {\"winDetails\":[{\"playerId\":\"9\",\"gamedata\":\"[]\",\"bet_ammount\":0,\"win_ammount\":0,\"win_number\":\"107\",\"game_name\":\"trippleChance\",\"start_point\":1000,\"game_id\":1708965590}]}";
        //     Debug.LogError("WinDetails : " + s);
        //     ResponseData.AllWindetail allWindetail = JsonUtility.FromJson<ResponseData.AllWindetail>(s);
        //     Debug.LogError(allWindetail.winDetails.Count);
        //     for (int i = 0; i < allWindetail.winDetails.Count; i++)
        //     {

        //         if (allWindetail.winDetails[i].playerId == PlayerPrefs.GetInt(Constant.UID).ToString())
        //         {
        //             if (allWindetail.winDetails[i].win_ammount >= 0)
        //             {

        //                 Constant.WinAmount = allWindetail.winDetails[i].win_ammount.ToString();
        //                 gamePlay.SetWinValue();
        //             }

        //         }
        //     }
        // }
        // private void SumDetails(Socket socket, Packet packet, params object[] args)
        // {
        //     string s = packet.RemoveEventName(true);
        //     Debug.LogError("SumDetails : " + s);
        //     ResponseData.SumDetails sumDetails = JsonUtility.FromJson<ResponseData.SumDetails>(s);
        //     Debug.LogError(sumDetails.sumDetails.Count);
        //     for (int i = 0; i < sumDetails.sumDetails.Count; i++)
        //     {
        //         if (sumDetails.sumDetails[i].playerId == PlayerPrefs.GetInt(Constant.UID).ToString())
        //         {
        //             Debug.LogError("S"+sumDetails.sumDetails[i].singleDigitSum);
        //             Debug.LogError("D"+sumDetails.sumDetails[i].doubleDigitSum);
        //             Debug.LogError("T"+sumDetails.sumDetails[i].trippleDigitSum);
        //         }
        //     }
        // }
    }

    internal class GameDataInsertClass
    {
        public string win_Number;
        public string game_name;
        public long start_point;
        public string game_id;
        public long win_Amount;
        public string draw_time;
        public string bonus_spin;
        public int claim_status;
        public int playerId;
        public List<DataClass> gameData;
        public long bet_ammount;
    }
}
