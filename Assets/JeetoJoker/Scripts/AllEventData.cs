using System;
using System.Collections.Generic;

namespace khelojeetonew
{
    [Serializable]
    public class RoomIdData
    {
        public string roomId { get; set; }
    }

    [Serializable]
    public class JoinRoomData
    {
        public string playerId { get; set; }
        public string name { get; set; }
        public int totalCoin { get; set; }
        public string profileImageUrl { get; set; }
        public string playerStatus { get; set; }
    }

   [Serializable]
public class BetData
{
    public string roomId { get; set; }
    public string playerId { get; set; }
    public List<CardValueSelect> cardValueSet { get; set; }

    public BetData()
    {
        cardValueSet = new List<CardValueSelect>();

        // Only add cards for values 11-34 (excluding cards starting with 4)
        for (int i = 1; i <= 3; i++)
        {
            for (int j = 1; j <= 4; j++)
            {
                int cardValue = i * 10 + j; // Card values: 11, 12, 13, 14, 21, 22, 23, 24, 31, 32, 33, 34
                cardValueSet.Add(new CardValueSelect { card = cardValue, value = 0 });
            }
        }
    }
}

[Serializable]
public class CardValueSelect
{
    public int card { get; set; }
    public int value { get; set; }
}

 /*[Serializable]
    public class BetData16card
    {
        public string roomId { get; set; }
        public string playerId { get; set; }
        public List<CardValueSelect16> cardValueSet { get; set; }

        public BetData16card()
        {
            cardValueSet = new List<CardValueSelect16>();
            for (int i = 11; i <= 14; i++)
            {
                cardValueSet.Add(new CardValueSelect16 { card16 = i, value16 = 0 });
                cardValueSet.Add(new CardValueSelect16 { card16 = i + 10, value16 = 0 });
                cardValueSet.Add(new CardValueSelect16 { card16 = i + 20, value16 = 0 });
                cardValueSet.Add(new CardValueSelect16 { card16 = i + 30, value16 = 0 });


            }
        }
    }

    [Serializable]
    public class CardValueSelect16
    {
        public int card16 { get; set; }
        public int value16 { get; set; }
    }*/

    [Serializable]
    public class RoomData
    {
        public string _id { get; set; }
        public int occupancy { get; set; }
        public bool isJoin { get; set; }
        public bool disconnect { get; set; }
        public int disconnectCount { get; set; }
        public CardValueSelect[] cardsValue { get; set; }
        public int totalBetSum { get; set; }
        public string mode { get; set; }
        public bool cancelBet { get; set; }
        public PlayerData[] players { get; set; }

    }

    [Serializable]
    public class LoginData
    {
        public int status { get; set; }
        public string message { get; set; }
        public string token { get; set; }
        public string id { get; set; }
        public string walletBlance { get; set; }
        public UserDetails userDetails { get; set; }

    }

    [Serializable]
    public class UserDetails
    {
        public int id { get; set; }
        public string name { get; set; }
        public string user_name { get; set; }
        public string email { get; set; }
    }

    [Serializable]
    public class SetModeData
    {
        public string roomId { get; set; }
        public string playerId { get; set; }
        public string mode { get; set; }
    }

    [Serializable]
    public class PlayerData
    {
        public string playerId { get; set; }
        public string socketID { get; set; }
        public string playerStatus { get; set; }
        public string totalCoin { get; set; }
    }

    [Serializable]
    public class ReportData
    {
        public int status { get; set; }
        public int bet_amount { get; set; }
        public int com_pt { get; set; }
        public int win_amount { get; set; }
        public int NTP { get; set; }
    }

    [Serializable]
    public class HistoryDataStatus
    {
        public int status { get; set; }
        public HistoryDataList list { get; set; }
    }

    [Serializable]
    public class HistoryDataList
    {
        public HistoryData[] data { get; set; }
        public string next_page_url { get; set; }
        public string prev_page_url { get; set; }
        public int per_page { get; set; }
        public int current_page { get; set; }
    }

    [Serializable]
    public class HistoryData
    {
        public int id { get; set; }
        public string win_ammount { get; set; }
        public string bet_ammount { get; set; }
        public string win_loose { get; set; }
    }

    [Serializable]
    public class IPAndPortStatus
    {
        public int status { get; set; }
        public IPAndPort port { get; set; }
    }

    [Serializable]
    public class IPAndPort
    {
        public string ip { get; set; }
        public int port { get; set; }
    }

    [Serializable]
    public class HotListStatus
    {
        public int status { get; set; }
        public List<HotList> list { get; set; }
/*#if JeetoJoker || Cards16
    public HotList list { get; set; }
#elif Cards12_IPL
    public HotList[] list { get; set; }
#endif*/
    }

    [Serializable]
    public class HotList
    {
        public int id;
        public string player_id { get; set; }
        public string win_type { get; set; }
        public string game_name { get; set; }
        public string is_x_executed { get; set; }
        public string win_price { get; set; }
    }

    [Serializable]
    public class LogoutData
    {
        public int status { get; set; }
        public string message { get; set; }
    }

    [Serializable]
    public class LeaveData
    {
        public string roomId { get; set; }
        public string playerId { get; set; }
    }
}