//using BestHTTP.SocketIO3;
//using BestHTTP.SocketIO3.Events;
using BestHTTP.SocketIO;
using DevCommon;
using System;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using khelojeetonew;
using UnityEngine.Networking;
using System.Collections;
using System.Linq;
using System.Text;
using JeetoJoker;
using SocketManager = BestHTTP.SocketIO.SocketManager;
using SoundControllerJeeto = JeetoJoker.SoundController;
public class SixteenCardsSocketController : Singleton<SixteenCardsSocketController>
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

   //public BetData betData { get; private set; } = new khelojeetonew.BetData();
   // public khelojeetonew.BetData16card betData { get; private set; } = new khelojeetonew.BetData16card();
   public JeetoJoker.BetData16card betData { get; private set; } = new JeetoJoker.BetData16card();
  // public  BetData betData = new BetData16card();
    public JeetoJoker.JoinRoomData joinRoomData { get; private set; } = new JeetoJoker.JoinRoomData();

    public JeetoJoker.SetModeData setMode { get; private set; } = new JeetoJoker.SetModeData();
    public JeetoJoker.LeaveData leaveData { get; private set; } = new JeetoJoker.LeaveData();

    public int playerId { get; private set; }

    private bool isConnected = false;

    private bool isStarted = false;

    public string is_x_excuted { get; private set; }

    public string win_price { get; private set; }
    public string gameIdjson;

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
        Core.Socket.On(Constants.GameId, OnGameId);
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
       // Debug.LogError("OnTimer " + packet);
        json = json.Replace("\"", "");
        sixteencard_Timer.inst.UpdateTimer(int.Parse(json));
       
    }
    private void OnGameId(Socket socket, Packet packet, object[] args)
    {
        try
        {
            string json = packet.RemoveEventName(true);
            json = json.Replace("\"", "");
            gameIdjson = json;
            Debug.Log(json + "json");
        }
        catch (Exception e)
        {
            Debug.LogError("Error in OngameId: " + e.ToString());
        }
    }
    private void OnStartGame(Socket socket, Packet packet, object[] args)
    {
        Debug.LogError("OnStartGame " + packet);
        SoundControllerJeeto.Instance.PlayOneShot(SoundControllerJeeto.SoundType.PlaceBet, Sixteen_cards.instance.placeBetSoundIndex);

    }

    private void OnSlot(Socket socket, Packet packet, object[] args)
    {
        string json = packet.RemoveEventName(true);
        Debug.LogError("OnSlot " + packet);
        json = json.Replace("\"", "");
        Debug.Log("printing json from onslot" + json);

        Sixteen_cards.instance.StartSpinning(int.Parse(json));
        Debug.Log("in slot socket enter for check winning hotlist");
                StartCoroutine(DownloadMode16()); 
        //StartCoroutine(DownloadX());
       // StartCoroutine(DownloadModeGameWise());
        StartCoroutine(CardHistory());
    }
    public IEnumerator CardHistory()
    {
        yield return new WaitForSeconds(12);
        APICardHistory16.Instance.CardHistory();
    }
    private void OnRoomMessage(Socket socket, Packet packet, object[] args)
    {
        Debug.LogError("OnRoomMessage " + packet);
        Sixteen_cards.instance.multiplierObject.SetActive(false);
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

          //  Debug.Log(roomData._id + "123456789");
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
        Debug.LogError("OnUnknownError "+packet);
    }

    private void OnConnectedError(Socket socket, Packet packet, object[] args)
    {
        Debug.LogError("OnConnectedError "+ packet);
       
    }

    private void OnDisconnected(Socket socket, Packet packet, object[] args)
    {
       
        isConnected = false;
        Debug.Log("socket disconnected! " + packet);
    }

    private void OnConnected(Socket socket, Packet packet, object[] args)
    {
      
        Debug.Log("socket connected! "+packet);
        isConnected = true;
       // JoinRoom(playerId.ToString(),"shivam",10000);
       Debug.Log("token in 16 card:"+PlayerPrefs.GetString(Constants.token));
        //StartCoroutine(SixteenFakeUserPopup.instance.AutoLogin(PlayerPrefs.GetString(Constants.token)));

        if (PlayerPrefs.HasKey(Constants.token))
        {
           Debug.Log("join room call when socket connected");
        }
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
        Sixteen_cards.instance.StartSpinning(data);
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

    public bool JoinRoom(string playerId,string playerName, long totalCoin)
    {
        if (isConnected)
        {
            // playerId = "309";
            Debug.Log("bet reset called");
            Sixteen_cards.instance.Reset();
            Debug.Log("download mode call when join room callled");
            StartCoroutine(DownloadMode16());
            //StartCoroutine(DownloadModeGameWise());
            Debug.LogError("playerId " + playerId);
            betData.playerId = playerId;
            joinRoomData.name = playerName;
            joinRoomData.playerId = playerId;
            leaveData.playerId = playerId;
            setMode.playerId = playerId;
            this.playerId = int.Parse(playerId);
            setMode.mode = "none";
            joinRoomData.playerStatus = "Good";
            joinRoomData.profileImageUrl = null;
           // joinRoomData.totalCoin = totalCoin;
            string rawJson = JsonConvert.SerializeObject(joinRoomData);
           
            Debug.LogError("joinRoom " + rawJson);
            // Packet packet;
            Dictionary<string, string> keyValuePairs =  JsonConvert.DeserializeObject<Dictionary<string, string>>(rawJson);
            Core.Socket.Emit(Constants.joinRoom, keyValuePairs);
        }
        return isConnected;
    }

    //iplgamecode of bet emit
    /* public void Bet()
    {
        Debug.Log("bet emittttttttttttttttttt..........");
        // Log all the cards and their values in cardValueSet
        if (betData.cardValueSet != null && betData.cardValueSet.Count > 0)
        {
            Debug.Log("Debugging betData.cardValueSet:");
            foreach (var cardValue in betData.cardValueSet)
            {
                Debug.Log($"Card: {cardValue.card}, Value: {cardValue.value}");
            }
        }
        else
        {
            Debug.LogWarning("betData.cardValueSet is null or empty.");
        }
        Sixteen_cards.instance.SetBetData(betData.cardValueSet);

        // Sort cardValueSet by card
        betData.cardValueSet.Sort((x, y) => x.card.CompareTo(y.card));

        // Serialize to JSON
        string rawJson = JsonConvert.SerializeObject(betData);

        // Emit the bet data to the server
        Core.Socket.Emit(Constants.bet, rawJson);
        Debug.Log("Serialized bet data 16 card for mode execution: " + rawJson);
    }*/
    //new bet fubnction
    public void Bet()
    {
        Debug.Log("bet emittttttttttttttttttt..........");

        // Update card values before sorting and emitting
        
        Sixteen_cards.instance.SetBetData(betData.cardValueSet);


        // Log all the cards and their values in cardValueSet
       /* if (betData.cardValueSet != null && betData.cardValueSet.Count > 0)
        {
            Debug.Log("Debugging betData.cardValueSet (After SetBetData):");
            foreach (var cardValue in betData.cardValueSet)
            {
                Debug.Log($"Card: {cardValue.card16}, Value: {cardValue.value16}");
            }
        }
        else
        {
            Debug.LogWarning("betData.cardValueSet is null or empty.");
        }*/

         betData.cardValueSet.Sort((x, y) => x.card.CompareTo(y.card));

        // Serialize to JSON
        string rawJson = JsonConvert.SerializeObject(betData);
          Debug.Log("bet 16 " + rawJson);
        // Emit the bet data to the server
        
        Core.Socket.Emit(Constants.bet, rawJson);
        Debug.Log("raw json in 16card:"+rawJson);

    }
    // public void SetCompleteData(List<khelojeetonew.CardValueSelect> cardValueSet)
    // {
    //     //BetData bd = new BetData();
    //     //bd.roomId = setMode.roomId;
    //     //bd.playerId = pID;
    //     //bd.cardValueSet = cardValueSet;
    //     //string rawJson = JsonConvert.SerializeObject(bd);
    //     //Debug.LogError("SetCompleteData " + rawJson);
    //     //Dictionary<string, string> keyValuePairs = JsonConvert.DeserializeObject<Dictionary<string, string>>(rawJson);
    //     //Core.Socket.Emit(Constants.bet, keyValuePairs);
    //     SetMode();
    //    BetData bd = new BetData();
    //    bd.roomId = setMode.roomId;
    //     bd.playerId = joinRoomData.playerId;
    //     Debug.Log("player id:" + bd.playerId);  //pID;
    //     bd.cardValueSet = cardValueSet;

    //     // Serialize the BetData object to JSON
    //     string rawJson = JsonConvert.SerializeObject(bd);
    //     Debug.Log("bet emit data" + rawJson);
    //     Debug.LogError("SetCompleteData " + rawJson);

    //     // Deserialize the JSON back into a BetData object
    //     //string deserializedData = JsonConvert.DeserializeObject<string>(rawJson);

    //     // Now you can emit deserializedData to your socket
    //     Core.Socket.Emit(Constants.bet, rawJson);
    // }




    public void SetMode()
    {
        string rawJson = JsonConvert.SerializeObject(setMode);
        Debug.Log("setMode " + rawJson);
        Dictionary<string, string> keyValuePairs = JsonConvert.DeserializeObject<Dictionary<string, string>>(rawJson);
        Core.Socket.Emit(Constants.setMode, keyValuePairs);
    }

    protected new void OnApplicationQuit()
    {
        Debug.Log("OnApplicationQuit called.");
        Leave(); 
    }

    void OnDestroy()
    {
        Debug.Log("OnDestroy called.");
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
        } catch(Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }

   private IEnumerator DownloadIPAndPort()
{
    string url = Sixteen_cards.instance.apiData.ipAndPortApi; // Get the URL from the instance
    Debug.Log("Request URL: " + url); // Debug the complete URL

    UnityWebRequest unityWebRequest = UnityWebRequest.Get(url);
    yield return unityWebRequest.SendWebRequest();

    if (unityWebRequest.result == UnityWebRequest.Result.Success)
    {
        try
        {
            Debug.LogError(unityWebRequest.downloadHandler.text);
           JeetoJoker.IPAndPortStatus iPAndPortStatus = JsonConvert.DeserializeObject<JeetoJoker.IPAndPortStatus>(unityWebRequest.downloadHandler.text);
            if (iPAndPortStatus.status == 200)
            {
                address = $"{iPAndPortStatus.port.ip}:{iPAndPortStatus.port.port}";
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
    else
    {
        Debug.LogError("Request failed: " + unityWebRequest.error);
    }

    loadingPanel.SetActive(false);
    ipAndPortDownloaded = true;
    Initialize();
}

  public string Windatax;

//new function fro winning hotlist 16 card added by shivamfusion07
 private IEnumerator DownloadMode16()
{
    Debug.Log("mode x execution for 16 cards");
    setMode.mode = "none";
    win_price = "";
    is_x_excuted = "";
    WWWForm wWWForm = new WWWForm();
    wWWForm.AddField("player_id", PlayerPrefs.GetInt(Constant.UID));
    UnityWebRequest unityWebRequest = UnityWebRequest.Get(Constant.KIBaseURL + "winning-hotlist?game_name=16cards");
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
            Debug.LogError(unityWebRequest.downloadHandler.text);

            // Deserialize correctly
          JeetoJoker.HotListStatus hotListStatus = JsonConvert.DeserializeObject<JeetoJoker.HotListStatus>(unityWebRequest.downloadHandler.text);

            if (hotListStatus.status == 200 && hotListStatus.list.Count > 0)
            {
              JeetoJoker.HotList hotList = hotListStatus.list[0];

                if (hotList != null)
                {
                        // Handle potential null values
                        if (hotList.game_name == "16cards")
                        {
                            win_price = hotList.win_price ?? "0";  // Default to 0 if null
                            is_x_excuted = hotList.is_x_executed.ToString();  // Use correct field name from JSON

                            Debug.LogError("win_price : " + win_price + " is_x_excuted : " + is_x_excuted);
                            Debug.Log("win_price : " + win_price + " is_x_excuted : " + is_x_excuted + "  123456");
                            setMode.mode = hotList.win_type;
                            win_price = hotList.win_price;
                            is_x_excuted = hotList.is_x_executed;
                            Windatax = win_price;
                            Debug.Log("win data X:" + Windatax);
                            Debug.Log(setMode.mode + " " + win_price + " " + is_x_excuted + "abcde");
                        }
                    
                }
            }
            else
            {
                Debug.LogError("history data error status " + hotListStatus.status + " 123456");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Deserialization Error: " + e.ToString());
        }
    }
}
}


// Update the class to reflect correct field names


    
   /* public IEnumerator DownloadX()
    {
        Debug.Log("enter in mode multiplier function shivamfusion07");
      //  setMode.mode = "none";
        win_price = "";
        is_x_excuted = "";
        WWWForm form4 = new WWWForm();
        form4.AddField("player_id", PlayerPrefs.GetInt(Constant.UID));
        form4.AddField("game_name", Sixteen_cards.instance.apiData.gameName);

        Debug.Log(Sixteen_cards.instance.apiData.hotListGamewiseApi + "  123456");
        UnityWebRequest unityWebRequest = UnityWebRequest.Post(Sixteen_cards.instance.apiData.hotListApi, form4);
        yield return unityWebRequest.SendWebRequest();
        if (unityWebRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(unityWebRequest.error);           
            Debug.LogError(unityWebRequest.downloadHandler.text);
        }
        else
        {
            Debug.Log("DownloadX " + unityWebRequest.downloadHandler.text);
            try
            {
                Debug.LogError(unityWebRequest.downloadHandler.text);
               JeetoJoker.HotListStatus hotListStatus = JsonConvert.DeserializeObject<JeetoJoker.HotListStatus>(unityWebRequest.downloadHandler.text);
                if (hotListStatus.status == 200)
                {

                    JeetoJoker.HotList hotList = hotListStatus.list[0];
                    // JeetoJoker.HotList hotList = hotListStatus.data.FirstOrDefault(x => x.game_name.Equals(Sixteen_cards.instance.apiData.gameName));

                    Debug.LogError("hotList " + hotList.win_price);
                    Debug.Log(hotList.win_price + "  123456");
                    if (hotList != null)
                    {


                        win_price = hotList.win_price;
                        is_x_excuted = hotList.is_x_excuted;
                         Debug.LogError("win_price : " + hotList.win_price + " is_x_excuted : " + hotList.is_x_excuted);
                        Debug.Log("win_price : " + hotList.win_price + " is_x_excuted : " + hotList.is_x_excuted + "  123456");
                    }
                                    
                }
                else
                {
                    Debug.LogError("history data error status " + hotListStatus.status+" 123456");
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }
    }*/

   /* private IEnumerator DownloadModeGameWise()
    {
        setMode.mode = "none";

        UnityWebRequest unityWebRequest = UnityWebRequest.Get(Sixteen_cards.instance.apiData.hotListApi);
     
        yield return unityWebRequest.SendWebRequest();*/
      /*  if (unityWebRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(unityWebRequest.error);
            Debug.LogError(unityWebRequest.downloadHandler.text);
        }
        else
        {
            try
            {
                Debug.LogError(unityWebRequest.downloadHandler.text);
                WinnerHotListStatus hotListStatus = JsonConvert.DeserializeObject<WinnerHotListStatus>(unityWebRequest.downloadHandler.text);
                if (hotListStatus.status == 200)
                {


                    WinnerHotListData hotList = hotListStatus.winner_hotlist.FirstOrDefault(x => x.player_id.Contains(playerId));


                    if (hotList != null)
                    {
                        Debug.LogError("win_type : " + hotList.stokez_data.win_type);

                        setMode.mode = hotList.stokez_data.win_type;

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
    }*/
//}
//}
