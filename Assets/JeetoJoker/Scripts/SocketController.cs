//using BestHTTP.SocketIO3;
//using BestHTTP.SocketIO3.Events;
using BestHTTP.SocketIO;
using DevCommon;
using System;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using khelojeetonew;
using UnityEngine.Networking;
using System.Collections;
using System.Linq;
using System.Text;
using JeetoJoker;
using SocketManager = BestHTTP.SocketIO.SocketManager;
using SoundControllerJeeto = JeetoJoker.SoundController;
public class SocketController : Singleton<SocketController>
{

    [SerializeField] private string address;
     


    [SerializeField] private GameObject loadingPanel;

    [SerializeField] private bool isLogEnabled;

    public static SocketManager Core;
    public static SocketManager NspCore;
    private static event BestHTTP.SocketIO.Events.SocketIOCallback OnListen;

    // private SocketManager _SocketManager;

    public bool ipAndPortDownloaded { get; private set; }

    public khelojeetonew.RoomIdData roomIdData { get; private set; } = new khelojeetonew.RoomIdData();

    public khelojeetonew.BetData betData { get; private set; } = new khelojeetonew.BetData();

    public khelojeetonew.JoinRoomData joinRoomData { get; private set; } = new khelojeetonew.JoinRoomData();

    public khelojeetonew.SetModeData setMode { get; private set; } = new khelojeetonew.SetModeData();
    public khelojeetonew.LeaveData leaveData { get; private set; } = new khelojeetonew.LeaveData();

    private bool isConnected = false;

    private bool isStarted = false;

    public string is_x_excuted { get; private set; }

    public string win_price { get; private set; }
    public string gameIdjson;
      public string pID;
      public string storedPlayerId;
      public string jsonWinslot;
        public string Windatax;

    protected override void Awake()
    {
        base.Awake();
        // Debug.unityLogger.logEnabled = isLogEnabled;
    }

    private void Start()
    {
       
        loadingPanel.SetActive(true);
        StartCoroutine(DownloadIPAndPort());
    }
   /*  public void StoreContinuousBetData(string jsonData)
    {
        storedContinuousBetData = jsonData;  // Store the continuous bet data here
          Debug.Log("Storing continuous bet data: " + jsonData);
    }*/

    public void Initialize()
    {

        StringBuilder builder = new StringBuilder();

        builder.Append(address);
        builder.Append(address.EndsWith("/") ? "socket.io/" : "/socket.io/");
        SocketOptions option = new SocketOptions();

        Core = new BestHTTP.SocketIO.SocketManager(new Uri(builder.ToString()));
        Core.Socket.AutoDecodePayload = false;
        Core.Socket.On(SocketIOEventTypes.Connect, OnConnected);
        Core.Socket.On(SocketIOEventTypes.Disconnect, OnDisconnected);
        Core.Socket.On(SocketIOEventTypes.Error, OnConnectedError);
        Core.Socket.On(SocketIOEventTypes.Unknown, OnUnknownError);
        Core.Socket.On(Constants.betting, OnBetting);
        Core.Socket.On(Constants.createRoomSuccess, OnCreateRoomeSuccess);
        Core.Socket.On(Constants.errorOccured, OnErrorOccured);
        Core.Socket.On(Constants.mode, OnMode);
        Core.Socket.On(Constants.roomData, OnRoomData);
        Core.Socket.On(Constants.roomMessage, OnRoomMessage);
        Core.Socket.On(Constants.slot, OnSlot);
        Core.Socket.On(Constants.startGame, OnStartGame);
        Core.Socket.On(Constants.timer, OnTimer);
        Core.Socket.On(Constants.updatedPlayer, OnUpdatedPlayer);
        Core.Socket.On(Constants.updatedPlayers, OnUpdatedPlayers);
        Core.Socket.On(Constants.updatedRoom, OnUpdatedRoom);
         //added listerner for game id changes by shivamfusion07
         Core.Socket.On(Constants.gameId,GameIddata);


        /*  _SocketManager = new SocketManager(new Uri(address));
          _SocketManager.Socket.On<ConnectResponse>(SocketIOEventTypes.Connect, OnConnected);
          _SocketManager.Socket.On(SocketIOEventTypes.Disconnect, OnDisconnected);
          _SocketManager.Socket.On<string>(Constants.betting, OnBetting);
          _SocketManager.Socket.On<RoomData>(Constants.createRoomSuccess, OnCreateRoomeSuccess);
          _SocketManager.Socket.On<string>(Constants.errorOccured, OnErrorOccured);
          _SocketManager.Socket.On<string>(Constants.mode, OnMode);
          _SocketManager.Socket.On<RoomData>(Constants.roomData, OnRoomData);
          _SocketManager.Socket.On<string>(Constants.roomMessage, OnRoomMessage);
          _SocketManager.Socket.On<int>(Constants.slot, OnSlot);
          _SocketManager.Socket.On<string>(Constants.startGame, OnStartGame);
          _SocketManager.Socket.On<int>(Constants.timer, OnTimer);
          _SocketManager.Socket.On<Dictionary<string, object>>(Constants.updatedPlayer, OnUpdatedPlayer);
          _SocketManager.Socket.On<object[]>(Constants.updatedPlayers, OnUpdatedPlayers);
          _SocketManager.Socket.On<RoomData>(Constants.updatedRoom, OnUpdatedRoom);*/
    }

    private void OnUpdatedRoom(Socket socket, Packet packet, object[] args)
    {
        Debug.LogError("OnUpdatedRoom " + packet);
        string json = packet.RemoveEventName(true);
        RoomData roomData = JsonConvert.DeserializeObject<RoomData>(json);
        setMode.roomId = roomData._id;
        leaveData.roomId = roomData._id;
        betData.roomId = roomData._id;
    }

    public int gameidstore;
    //for game id response recieved data
  private void GameIddata(Socket socket, Packet packet, object[] args)
    {
        Debug.LogError("Game id data: " + packet);
         JArray dataArray = JArray.Parse(packet.ToString()); // Convert the packet to a JSON array
    string eventName = dataArray[0].ToString(); // "GameId"
    int gameId = dataArray[1].ToObject<int>();  // 1725528573
    gameidstore=gameId;
    Debug.Log("Game id:"+gameidstore);
    Debug.Log($"Event: {eventName}, Game ID: {gameId}");
    }

    private void OnUpdatedPlayers(Socket socket, Packet packet, object[] args)
    {
        Debug.LogError("OnUpdatedPlayers " + packet);

    }

    private void OnUpdatedPlayer(Socket socket, Packet packet, object[] args)
    {
        Debug.LogError("OnUpdatedPlayer " + packet);

    }

    private void OnTimer(Socket socket, Packet packet, object[] args)
    {
        string json = packet.RemoveEventName(true);
         Debug.LogError("OnTimer " + packet);
        json = json.Replace("\"", "");
        TimerControllerNew.inst.UpdateTimer(int.Parse(json));
    }

    private void OnStartGame(Socket socket, Packet packet, object[] args)
    {
        Debug.LogError("OnStartGame " + packet);
        SoundControllerJeeto.Instance.PlayOneShot(SoundControllerJeeto.SoundType.PlaceBet, JeetoJokerManager.instance.placeBetSoundIndex);

    }

    private void OnSlot(Socket socket, Packet packet, object[] args)
    {
        string json = packet.RemoveEventName(true);
        Debug.LogError("OnSlot " + packet);
        json = json.Replace("\"", "");
        JeetoJokerManager.instance.StartSpinning(int.Parse(json));
       
        Debug.Log("whel spining from heree..1111");
        StartCoroutine(DownloadMode());
        StartCoroutine(APICardHistory.Instance.setResultRequest(json));
    }

    private void OnRoomMessage(Socket socket, Packet packet, object[] args)
    {
        Debug.LogError("OnRoomMessage " + packet);

    }

    private void OnRoomData(Socket socket, Packet packet, object[] args)
    {
        Debug.LogError("OnRoomData " + packet);

    }

    private void OnMode(Socket socket, Packet packet, object[] args)
    {
        Debug.LogError("OnMode " + packet);

    }

    private void OnErrorOccured(Socket socket, Packet packet, object[] args)
    {
        Debug.LogError("OnErrorOccured " + packet);

    }

    private void OnCreateRoomeSuccess(Socket socket, Packet packet, object[] args)
    {
        Debug.LogError("OnCreateRoomeSuccess " + packet);
        string json = packet.RemoveEventName(true);
        try
        {

            RoomData roomData = JsonConvert.DeserializeObject<RoomData>(json);
            Debug.LogError(json);
            setMode.roomId = roomData._id;
            leaveData.roomId = roomData._id;
            betData.roomId = roomData._id;
            StartGame(roomData._id);
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }

    private void OnBetting(Socket socket, Packet packet, object[] args)
    {
        Debug.LogError("OnBetting " + packet);

    }

    private void OnUnknownError(Socket socket, Packet packet, object[] args)
    {
        //throw new NotImplementedException();
        Debug.LogError("OnUnknownError " + packet);
    }

    private void OnConnectedError(Socket socket, Packet packet, object[] args)
    {
        Debug.LogError("OnConnectedError " + packet);

    }

    private void OnDisconnected(Socket socket, Packet packet, object[] args)
    {
        isConnected = false;
        Debug.Log("socket disconnected! " + packet);
    }

    private void OnConnected(Socket socket, Packet packet, object[] args)
    {
        Debug.Log("socket connected! " + packet);
        isConnected = true;
    }
    /*
    private void OnUpdatedRoom(RoomData data)
    {
        Debug.LogError("OnUpdatedRoom called! " + JsonConvert.SerializeObject(data));
       
        setMode.roomId = data._id;
        leaveData.roomId = data._id;
        betData.roomId = data._id;
        
    }

    private void OnUpdatedPlayers(object[] data)
    {
        int length = data.Length;
        for(int i = 0; i < length; i++)
        {
            Dictionary<string, object> callbackData = (Dictionary<string, object>)data[i];
            foreach(string key in callbackData.Keys)
            {
               // Debug.Log("OnUpdatedPlayers called! " + key + "/" + callbackData[key]);
            }           
        }
       
    }

    private void OnUpdatedPlayer(Dictionary<string, object> data)
    {
        foreach (string key in data.Keys)
        {
            Debug.Log("OnUpdatedPlayer called! " + key + "/" + data[key]);
        }
    }

    private void OnTimer(int time)
    {
        Debug.Log("OnTimer called! " + time);
        TimerControllerNew.inst.UpdateTimer(time);
    }

    private void OnStartGame(string data)
    {
        Debug.Log("OnStartGame called! " + data);
    }

    private void OnSlot(int data)
    {
        Debug.Log("OnSlot called! " + data);
        JeetoJokerManager.instance.StartSpinning(data);
        StartCoroutine(DownloadMode());
    }

    private void OnRoomMessage(string data)
    {
        Debug.Log("OnRoomMessage called! " + data);
    }

    private void OnRoomData(RoomData data)
    {
        Debug.LogError("OnRoomData called! " + JsonConvert.SerializeObject(data));
    }

    private void OnMode(string data)
    {
        Debug.Log("OnMode called! " + data);
    }

    private void OnErrorOccured(string data)
    {
        Debug.Log("OnErrorOccured called! " + data);
    }

    private void OnCreateRoomeSuccess(RoomData data)
    {
        Debug.LogError("OnCreateRoomeSuccess called! " + JsonConvert.SerializeObject(data));
       
        setMode.roomId = data._id;
        leaveData.roomId = data._id;
        betData.roomId = data._id;
        StartGame(data._id);
    }

    private void OnBetting(string data)
    {
        Debug.Log("OnBetting called! "+ data);
    }

    private void OnConnected(ConnectResponse obj)
    {
        Debug.Log("socket connected!");
        isConnected = true;
        //JoinRoom();
    }

    private void OnDisconnected()
    {
        isConnected = false;
        Debug.Log("socket disconnected!");
    }*/

    public bool StartGame(string roomId)
    {
        if (isConnected)
        {
            roomIdData.roomId = roomId;
            string rawJson = JsonConvert.SerializeObject(roomIdData);
            Debug.LogError("start " + rawJson);
            Dictionary<string, string> keyValuePairs = JsonConvert.DeserializeObject<Dictionary<string, string>>(rawJson);
            Core.Socket.Emit(Constants.start, keyValuePairs);
        }
        return isConnected;
    }

    public bool JoinRoom(string playerId, string playerName, int totalCoin)
    {
        if (isConnected)
        {
             storedPlayerId = playerId;
            // playerId = "309";
          JeetoJokerManager.instance.Reset();  
            //StartCoroutine(DownloadMode());
            Debug.LogError("playerId " + playerId);
            betData.playerId = playerId;
            joinRoomData.name = playerName;
            joinRoomData.playerId = playerId;
            leaveData.playerId = playerId;
            setMode.playerId = playerId;
            setMode.mode = "none";
            joinRoomData.playerStatus = "Good";
            joinRoomData.profileImageUrl = null;
            joinRoomData.totalCoin = totalCoin;
            string rawJson = JsonConvert.SerializeObject(joinRoomData);

            Debug.LogError("joinRoom " + rawJson);
            // Packet packet;
            Dictionary<string, string> keyValuePairs = JsonConvert.DeserializeObject<Dictionary<string, string>>(rawJson);
            Core.Socket.Emit(Constants.joinRoom, keyValuePairs);
        }
        return isConnected;
    }

   public void Bet()
    {
        SetMode();
        JeetoJokerManager.instance.SetBetData(betData.cardValueSet);
        string rawJson = JsonConvert.SerializeObject(betData);
        Debug.LogError("bet " + rawJson);
        // Dictionary<string, object> keyValuePairs = JsonConvert.DeserializeObject<Dictionary<string, object>>(rawJson);
        Debug.Log("showing row json data after 7 seconds:"+ rawJson);
        Core.Socket.Emit(Constants.bet, rawJson);
        // Dictionary<string, object> keyValuePairs = JsonConvert.DeserializeObject<Dictionary<string, object>>(rawJson);
    }


    /* public void Bet()
    {
        SetMode();

        // Pass it to JeetoJokerManager
        JeetoJokerManager.instance.SetBetData(betData.cardValueSet);

        // Serialize to JSON
        string rawJson = JsonConvert.SerializeObject(betData);
        Debug.Log("bet data all 12 cards bet emit..." + rawJson);

        // Emit the serialized bet data to the server
        Core.Socket.Emit(Constants.bet, rawJson);
    }*/
 

    public void SetMode()
    {
        string rawJson = JsonConvert.SerializeObject(setMode);
        Debug.LogError("setMode " + rawJson);
        Dictionary<string, string> keyValuePairs = JsonConvert.DeserializeObject<Dictionary<string, string>>(rawJson);
        Core.Socket.Emit(Constants.setMode, keyValuePairs);
    }

    public void OnApplicationQuit()
    {
        Leave();
    }
    public void Leave()
    {
        string rawJson = JsonConvert.SerializeObject(leaveData);
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
    }

    private IEnumerator DownloadIPAndPort()
    {

        UnityWebRequest unityWebRequest = UnityWebRequest.Get(JeetoJokerManager.instance.apiData.ipAndPortApi);
        yield return unityWebRequest.SendWebRequest();
        if (unityWebRequest.result == UnityWebRequest.Result.Success)
        {
            try
            {
                Debug.LogError(unityWebRequest.downloadHandler.text);
               khelojeetonew.IPAndPortStatus iPAndPortStatus = JsonConvert.DeserializeObject<khelojeetonew.IPAndPortStatus>(unityWebRequest.downloadHandler.text);
                if (iPAndPortStatus.status == 200)
                {
                    //address = $"{iPAndPortStatus.port.ip}:{iPAndPortStatus.port.port}";
                    //address = $"http://13.203.200.43:3005";
                }
                else
                {
                    Debug.LogError("history data error status " + iPAndPortStatus.status);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }
        loadingPanel.SetActive(false);
        ipAndPortDownloaded = true;
        Initialize();
    }
    //changes by shivamfusion07
    public void SetCompleteData(List<khelojeetonew.CardValueSelect> cardValueSet)
        {
            //BetData bd = new BetData();
            //bd.roomId = setMode.roomId;
            //bd.playerId = pID;
            //bd.cardValueSet = cardValueSet;
            //string rawJson = JsonConvert.SerializeObject(bd);
            //Debug.LogError("SetCompleteData " + rawJson);
            //Dictionary<string, string> keyValuePairs = JsonConvert.DeserializeObject<Dictionary<string, string>>(rawJson);
            //Core.Socket.Emit(Constants.bet, keyValuePairs);
            SetMode();
            khelojeetonew.BetData bd = new khelojeetonew.BetData();
            bd.roomId = setMode.roomId;
            bd.playerId = storedPlayerId; 
            Debug.Log("player id:"+bd.playerId);  //pID;
            bd.cardValueSet = cardValueSet;

            // Serialize the BetData object to JSON
            string rawJson = JsonConvert.SerializeObject(bd);
            Debug.Log("bet emit data"+rawJson);
            Debug.LogError("SetCompleteData " + rawJson);

            // Deserialize the JSON back into a BetData object
            //string deserializedData = JsonConvert.DeserializeObject<string>(rawJson);

            // Now you can emit deserializedData to your socket
        Debug.Log("bet 12 card on bet emit card:" + rawJson);
            Core.Socket.Emit(Constants.bet, rawJson);
        }


    private IEnumerator DownloadMode()
    {
        setMode.mode = "none";
        win_price = "";
        is_x_excuted = "";
        WWWForm wWWForm = new WWWForm();
        Debug.Log("player id before download mode call:"+joinRoomData.playerId);
        //wWWForm.AddField("player_id", joinRoomData.playerId);
        UnityWebRequest unityWebRequest = UnityWebRequest.Get(Constant.KIBaseURL + "winning-hotlist?game_name=jeetojoker");
        yield return unityWebRequest.SendWebRequest();
        if (unityWebRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(unityWebRequest.error);
            Debug.LogError(unityWebRequest.downloadHandler.text);
        }
        else
        {
            try
            {
                Debug.LogError("Winning Hotlist : "+unityWebRequest.downloadHandler.text);
               khelojeetonew.HotListStatus hotListStatus = JsonConvert.DeserializeObject<khelojeetonew.HotListStatus>(unityWebRequest.downloadHandler.text);
                if (hotListStatus.status == 200)
                {
                  khelojeetonew.HotList hotList = hotListStatus.list[0];
                    /*#if JeetoJoker || Cards16
                                        HotList hotList = hotListStatus.list;
                    #elif Cards12_IPL
                        HotList hotList = hotListStatus.list.FirstOrDefault(x => x.game_name.Equals(JeetoJokerManager.instance.apiData.gameName));
                    #endif*/

                    if (hotList != null)
                    {
                        // Debug.LogError("Player Id : " + hotList.player_id + " Mode : " + hotList.win_type);
                       // if (hotList.player_id == joinRoomData.playerId)
                       // {
                       //     setMode.mode = hotList.win_type;
                       //     win_price = hotList.win_price;
                       //     is_x_excuted = hotList.is_x_executed;
                       //       Windatax = win_price;
                       //       Debug.Log("win data X:"+Windatax);
                       //Debug.Log(setMode.mode + " " + win_price + " " + is_x_excuted + "abcde");
                       // }
                        if (hotList.game_name == "jeetojoker")
                        {
                            setMode.mode = hotList.win_type;
                            win_price = hotList.win_price;
                            is_x_excuted = hotList.is_x_executed;
                              Windatax = win_price;
                              Debug.Log("win data X:"+Windatax);
                       Debug.Log(setMode.mode + " " + win_price + " " + is_x_excuted + "abcde");
                        }
                    }

                }
                else
                {
                    Debug.LogError("history data error status " + hotListStatus.status);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }
    }
}
