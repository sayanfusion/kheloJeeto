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


namespace DevCommon
{
    internal sealed class SocketManager : MonoBehaviour
    {

        public static BestHTTP.SocketIO.SocketManager Core;
        public static BestHTTP.SocketIO.SocketManager NspCore;
        private static event SocketIOCallback OnListen;
        public string socketLink;
        public static SocketManager socketManager;
        bool istrue = false;
        public string gameIdjson;
        private void Awake()
        {
            if (socketManager == null)
            {
                socketManager = this;
            }
            else
            {
                Destroy(this);
            }
        }
        private void Start()
        {
            Connect();

        }

        private void Connect()
        {

            //Config.SocketConfig.Host.Trim();
            StringBuilder builder = new StringBuilder();
            builder.Append(socketLink.Trim());
            builder.Append(socketLink.EndsWith("/") ? "socket.io/" : "/socket.io/");
            Debug.Log("url : " + builder.ToString());

            Core = new BestHTTP.SocketIO.SocketManager(new Uri(builder.ToString()));
            Core.Socket.On(SocketIOEventTypes.Connect, onConnected);
            Core.Socket.On(SocketIOEventTypes.Disconnect, onDisconnected);
            Core.Socket.On(SocketIOEventTypes.Error, onConnectedError);
            Core.Socket.On(SocketIOEventTypes.Unknown, onUnknownError);

        }

        private void onConnected(Socket socket, Packet packet, object[] args)
        {
            Debug.LogWarning("SocketConnected : " + socket);
            //addEvents();
            AddAllListner();
            //SocketListener.ActivateListener(ref OnListen);
            if (SceneManager.GetActiveScene().name.Equals("jokerScenePC"))
            {
                EmmiteJoinRoomJoker();
            }
            else
            {
                EmmiteJoinRoom();
            }
            istrue = true;
        }

        private void onDisconnected(Socket socket, Packet packet, object[] args)
        {
            Debug.LogError("Disconnected");
        }

        private void onConnectedError(Socket socket, Packet packet, object[] args)
        {
            Debug.LogError("ConnectedError");
        }

        private void onUnknownError(Socket socket, Packet packet, object[] args)
        {
            Debug.LogError("UnknownError");
        }

        private void addEvents()
        {
            Config.SocketConfig.SocketEvents.ForEach(evt => Core.Socket.On(evt, OnListen, true));
        }

        private void AddAllListner()
        {
            #region roomJoin
            Core.Socket.On("createRoomSuccess", OnRoomCreateSuccess);
            Core.Socket.On("updatedRoom", OnUpdateRoom);
            Core.Socket.On("updatedPlayers", OnUpdatePlayers);
            Core.Socket.On("updatedPlayer", OnUpdatedPlayer);
            Core.Socket.On("roomMessage", OnRoomMessage);
            #endregion
            #region StartEmit
            Core.Socket.On("reset", Onreset);
            Core.Socket.On("winner", OnWinner);
            Core.Socket.On("timer", OnTimer);
            Core.Socket.On(Constants.GameId, OnGameId);

            #endregion
            Core.Socket.On("updatedRoom", OnUpdatedRoom);
            Core.Socket.On("roomData", OnRoomDataRecive);
            Core.Socket.On("leavePlayer", OnLeavePlayer);
            #region JeetoJoker
            Core.Socket.On("room_error", Onroom_error);
            Core.Socket.On("room_info", Onroom_info);
            Core.Socket.On("game_info", Ongame_info);
            Core.Socket.On("winner", Onwinner);
            Core.Socket.On("reset", Onresetgame);

            #endregion
        }
        private void OnUpdatedRoom(Socket socket, Packet packet, params object[] args)
        {
            string s = packet.RemoveEventName(true);
            Debug.Log("OnUpdatedRoom Recieved : " + s);
        }

        private void OnRoomDataRecive(Socket socket, Packet packet, params object[] args)
        {
            string s = packet.RemoveEventName(true);
            Debug.Log("On roomData Recieved : " + s);
            if (SceneManager.GetActiveScene().name.Equals("ForIndianMobile"))
            {
                Debug.Log($"Socket Id : {socket.Id}");
                if (PlayerPrefs.GetInt(Constant.STOKIESID) == SceneRoulette._Instance._AmeWheel.stokiesID)
                {
                    RoomData myDeserializedClass = JsonConvert.DeserializeObject<RoomData>(s);
                    List<Player> players = new List<Player>();
                    for (int i = 0; i < myDeserializedClass.players.Count; i++)
                    {
                        if (myDeserializedClass.players[i].playerStatus.ToLower().Trim().Equals(SceneRoulette._Instance._AmeWheel.stokiesID.ToString().ToLower().Trim()))
                        {
                            players.Add(myDeserializedClass.players[i]);
                        }

                    }
                    string result = Set_Result(SceneRoulette._Instance._AmeWheel.userStokies, players);

                    Dictionary<string, object> dd = new Dictionary<string, object>();
                    dd.Add("roomId", PlayerPrefs.GetString(Constants.ROOMID));
                    dd.Add("winner", result);
                    dd.Add("isWinner", "yes");

                    Debug.LogError($"Set Winner Called :: {JsonConvert.SerializeObject(dd)}");
                    Core.Socket.Emit("setWinner", dd);


                }
                else
                {
                    RoomData myDeserializedClass = JsonConvert.DeserializeObject<RoomData>(s);
                    string result = Set_Result("Medium", myDeserializedClass.players);

                    Dictionary<string, object> dd = new Dictionary<string, object>();
                    dd.Add("roomId", PlayerPrefs.GetString(Constants.ROOMID));
                    dd.Add("winner", result);
                    dd.Add("isWinner", "no");

                    Debug.LogError($"Set Winner Called : {JsonConvert.SerializeObject(dd)}");
                    Core.Socket.Emit("setWinner", dd);
                }
            }
        }

        private string Set_Result(string userStokies, List<Player> players)
        {
            long TotalBet = 0;
            Dictionary<string, int> betNumbers = new Dictionary<string, int>();
            foreach (var item in players)
            {
                for (int i = 0; i < item.bettedOn.Count; i++)
                {

                    if (betNumbers.ContainsKey(item.bettedOn[i]))
                    {
                        if (betNumbers[item.bettedOn[i]] > item.bettedAmount[i])
                        {
                            TotalBet -= betNumbers[item.bettedOn[i]];
                            betNumbers[item.bettedOn[i]] = item.bettedAmount[i];
                            TotalBet += item.bettedAmount[i];
                        }
                    }
                    else
                    {
                        betNumbers.Add(item.bettedOn[i], item.bettedAmount[i]);
                        TotalBet += item.bettedAmount[i];
                    }
                }
            }


            string result = null;
            switch (userStokies)
            {
                case "High":
                    List<string> HighKey = new List<string>();
                    foreach (var number in betNumbers)
                    {
                        if ((number.Value * 36) >= TotalBet)
                        {
                            HighKey.Add(number.Key);
                        }
                    }
                    result = HighKey[Random.Range(0, HighKey.Count)];
                    break;
                case "Low":
                    List<string> LowKeys = new List<string>();

                    foreach (var number in betNumbers)
                    {

                        if ((number.Value * 36) < TotalBet || number.Value == 0)
                        {
                            LowKeys.Add(number.Key);
                        }
                    }
                    result = LowKeys[Random.Range(0, LowKeys.Count)];
                    break;
                case "Medium":
                    List<string> HighKey1 = new List<string>();
                    foreach (var number in betNumbers)
                    {
                        if ((number.Value * 36) >= TotalBet)
                        {
                            HighKey1.Add(number.Key);
                        }
                    }
                    List<string> LowKeys1 = new List<string>();

                    foreach (var number in betNumbers)
                    {

                        if ((number.Value * 36) < TotalBet || number.Value == 0)
                        {
                            LowKeys1.Add(number.Key);
                        }
                    }
                    List<string> MediumKeys = new List<string>(betNumbers.Keys);
                    foreach (var number in betNumbers)
                    {
                        if (HighKey1.Contains(number.Key) || LowKeys1.Contains(number.Key))
                        {
                            MediumKeys.Add(number.Key);
                        }
                    }
                    result = MediumKeys[Random.Range(0, MediumKeys.Count)];
                    break;
            }
            if (result == null)
            {
                result = Random.Range(0, 37).ToString();
            }
            return result;
        }

        #region Join_Room

        private void OnRoomCreateSuccess(Socket socket, Packet packet, params object[] args)
        {
            string s = packet.RemoveEventName(true);
            Debug.Log("Room Create Recieved : " + s);
            JSONNode OnUpdateRoomData = JSON.Parse(s);
            Debug.LogError("Room Id " + OnUpdateRoomData["_id"]);
            PlayerPrefs.SetString(Constants.ROOMID, OnUpdateRoomData["_id"]);
            if (SceneManager.GetActiveScene().name.Equals("TripleChanceGameplay") || SceneManager.GetActiveScene().name.Equals("ForIndianMobile"))
            {
                EmmiteStart();
            }
        }
        private void OnUpdateRoom(Socket socket, Packet packet, params object[] args)
        {
            string s = packet.RemoveEventName(true);
            Debug.LogError("Update Room Recieved : " + s);
            JSONNode OnUpdateRoomData = JSON.Parse(s);
            Debug.LogError("Room Id " + OnUpdateRoomData["_id"]);
            PlayerPrefs.SetString(Constants.ROOMID, OnUpdateRoomData["_id"]);
            if (SceneManager.GetActiveScene().name.Equals("TripleChanceGameplay") && istrue == true)
            {
                istrue = false;
                GamePlay.instance.CheckStockies();
            }
            if (SceneManager.GetActiveScene().name.Equals("TripleChanceProGameplay") && istrue == true)
            {
                istrue = false;
                GamePlay.instance.CheckStockies();
            }
        }
        private void OnUpdatePlayers(Socket socket, Packet packet, params object[] args)
        {
            string s = packet.RemoveEventName(true);
            Debug.Log("Update Players Recieved : " + s);
        }
        private void OnUpdatedPlayer(Socket socket, Packet packet, params object[] args)
        {
            string s = packet.RemoveEventName(true);
            Debug.Log("Updated Player Recieved : " + s);
        }
        private void OnRoomMessage(Socket socket, Packet packet, params object[] args)
        {
            Debug.Log("Room Message Recieved");

        }
        private void EmmiteJoinRoom()
        {
            Dictionary<string, object> dd = new Dictionary<string, object>();
            dd.Add("playerId", PlayerPrefs.GetInt(Constant.UID).ToString());
            dd.Add("name", PlayerPrefs.GetString(Constant.User));
            dd.Add("totalCoin", PlayerPrefs.GetInt(Constants.BALANCE));
            dd.Add("playerStatus", PlayerPrefs.GetInt(Constant.UID).ToString());

            Debug.LogError("joinRoom " + JsonConvert.SerializeObject(dd));
            Core.Socket.Emit("joinRoom", dd);
        }
        private void EmmiteJoinRoomJoker()
        {
            Dictionary<string, object> dd = new Dictionary<string, object>();
            dd.Add("player_name", PlayerPrefs.GetString(Constant.User));
            dd.Add("player_id", PlayerPrefs.GetInt(Constant.UID).ToString());
            dd.Add("profile_url", "img2");

            Debug.LogError("join_room " + JsonConvert.SerializeObject(dd));
            Core.Socket.Emit("join_room", dd);
        }

        private void CheckPlayer()
        {
            Dictionary<string, object> dd = new Dictionary<string, object>();
            dd.Add("playerId", PlayerPrefs.GetInt(Constant.UID).ToString());
            Core.Socket.Emit("checkPlayer", dd);
        }

        private void OnLeavePlayer(Socket socket, Packet packet, params object[] args)
        {
            string s = packet.RemoveEventName(true);
            BackToLoginPage();
        }
        public void BackToLoginPage()
        {
            Dictionary<string, object> dd = new Dictionary<string, object>();
            dd.Add("playerId", PlayerPrefs.GetInt(Constant.UID).ToString());
            dd.Add("name", PlayerPrefs.GetString(Constant.User));
            dd.Add("roomId", PlayerPrefs.GetString(Constants.ROOMID));

            Debug.LogError(JsonConvert.SerializeObject(dd));
            SocketManager.Core.Socket.Emit("leaveRoom", dd);
            SceneManager.LoadScene("login");
        }
        #endregion
        #region StartEmit
        private void EmmiteStart()
        {
            Dictionary<string, object> dd = new Dictionary<string, object>();
            dd.Add("roomId", PlayerPrefs.GetString(Constants.ROOMID));
            Debug.LogError(JsonConvert.SerializeObject(dd));
            Debug.Log("start : ");
            SocketManager.Core.Socket.Emit("start", dd);
        }
        #endregion
        #region StartEvent
        private void Onreset(Socket socket, Packet packet, params object[] args)
        {
            string s = packet.RemoveEventName(true);
            Debug.Log("Onreset : " + s);
            //GamePlay.instance.timerScript.StartTimer(60);
            //GamePlay.instance.CheckStockies();

        }
        #region JeetooJokerlistner
        private void Onroom_error(Socket socket, Packet packet, params object[] args)
        {
            string s = packet.RemoveEventName(true);
            Debug.Log("Onroom_error : " + s);
        }
        private void Onroom_info(Socket socket, Packet packet, params object[] args)
        {
            string s = packet.RemoveEventName(true);
            Debug.Log("Onroom_info : " + s);
        }
        private void Ongame_info(Socket socket, Packet packet, params object[] args)
        {
            string s = packet.RemoveEventName(true);
            Debug.Log("Ongame_info : " + s);
        }
        private void Onwinner(Socket socket, Packet packet, params object[] args)
        {
            string s = packet.RemoveEventName(true);
            Debug.Log("Onwinner : " + s);
        }
        private void Onresetgame(Socket socket, Packet packet, params object[] args)
        {
            string s = packet.RemoveEventName(true);
            Debug.Log("Onresetgame : " + s);
        }
        #endregion
        private void OnWinner(Socket socket, Packet packet, params object[] args)
        {
            string s = packet.RemoveEventName(true);

            JSONNode OnUpdateRoomData = JSON.Parse(s);
            Debug.LogError("sss " + s);
            Debug.LogError("Result :" + int.Parse(s.Trim('"')));
            //Debug.Log("OnWinner : " + OnUpdateRoomData.Value);
            if (SceneManager.GetActiveScene().name.Equals("ForIndianMobile"))
            {
                SceneRoulette._Instance._AmeWheel.a = Convert.ToInt32(s.Trim('"'));
            }
            else
            {
                if (GamePlay.instance.isListen == true)
                {
                    GamePlay.instance.isListen = false;
                    int res = Convert.ToInt32(s.Trim('"'));
                    GamePlay.instance.result = res.ToString("000");
                    GamePlay.instance.SetresultData();
                }

            }
        }
        private void OnTimer(Socket socket, Packet packet, params object[] args)
        {
            string s = packet.RemoveEventName(true);

            //Debug.LogError("OnTimer out side: " + s);
            //Debug.Log("OnTimer : " + OnUpdateRoomData.Value);
            if (SceneManager.GetActiveScene().name.Equals("ForIndianMobile"))
            {
                //Debug.LogError("OnTimer in side: " + s);
                timeCounter.instance.secondleft = Convert.ToInt32(s.Trim('"'));
                timeCounter.instance.txtdisplay.GetComponent<Text>().text = timeCounter.instance.secondleft.ToString();
                Debug.Log("Timer1 "+s);
               
            }
            else
            {
                GamePlay.instance.isListen = true;
                //Debug.LogError("GamePlay.instance.isListen "+ GamePlay.instance.isListen);
                GamePlay.instance.timerScript.timeLeft = Convert.ToInt32(s.Trim('"'));
                Debug.Log("Timer2 " + GamePlay.instance.timerScript.timeLeft );
            }

        }
        private void OnGameId(Socket socket, Packet packet, object[] args)
        {
            try 
            {
                string json = packet.RemoveEventName(true); Debug.Log(json + "Error in OngameId: ");
                json = json.Replace("\"", "");
                gameIdjson = json;
                PlayerPrefs.SetString("GameIdRoulette", gameIdjson);
            }
            catch (Exception e)
            {
                Debug.Log("Error in OngameId: " + e.ToString());
            }
        }
        #endregion


    }
}
[Serializable]
public class Player
{
    public string playerId { get; set; }
    public string name { get; set; }
    public List<string> bettedOn { get; set; }
    public List<int> bettedAmount { get; set; }
    public int totalAmount { get; set; }
    public string playerType { get; set; }
    public string playerStatus { get; set; }
    public string mode { get; set; }
    public string _id { get; set; }
}
[Serializable]
public class RoomData
{
    public Slots slots { get; set; }
    public string _id { get; set; }
    public int occupancy { get; set; }
    public int currentRound { get; set; }
    public bool isJoin { get; set; }
    public long time { get; set; }
    public List<Player> players { get; set; }
    public int __v { get; set; }
}
[Serializable]
public class Slots
{
    public List<string> slotList { get; set; }
    public List<int> slotAmountList { get; set; }
}