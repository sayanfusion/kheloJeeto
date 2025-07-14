using System;
using System.Collections;
using System.Collections.Generic;
namespace tripplechance
{
    public class ResponseData
    {
        [System.Serializable]
        public class CardsValue1
        {
            public string _id;
            public string card;
            public int value;
        }
        [System.Serializable]
        public class Player
        {
            public string playerId;
            public string socketID;
            public int wonCoin;
            public string totalCoin;
            public string playerStatus;
            public int bet;
            public int bet_Amount;
            public string mode;
            public string _id;
            public List<object> cardSetValue;
        }
        [System.Serializable]
        public class CreateRoomSuccess
        {
            public int occupancy;
            public bool isJoin;
            public bool disconnect;
            public int disconnectCount;
            public int gameId;
            public int totalBetSum;
            public string mode;
            public bool cancelBet;
            public bool disconnectPlayer;
            public long time;
            public string game_Name;
            public string _id;
            public List<Player> players;
            public List<CardsValue1> cardsValue1;
        }
        [System.Serializable]
        public class UpdatedPlayers
        {
            public string playerId;
            public string socketID;
            public int wonCoin;
            public string totalCoin;
            public string playerStatus;
            public int bet;
            public int bet_Amount;
            public string mode;
            public string _id;
            public List<object> cardSetValue;
        }
        [System.Serializable]
        public class UpdatedRoom
        {
            public string _id;
            public int occupancy;
            public bool isJoin;
            public bool disconnect;
            public int disconnectCount;
            public int gameId;
            public int totalBetSum;
            public string mode;
            public bool cancelBet;
            public bool disconnectPlayer;
            public long time;
            public string game_Name;
            public List<Player> players;
            public List<CardsValue1> cardsValue1;
            public int draw_time;
        }
        [System.Serializable]
        public class List
        {
            public int id;
            public int player_id;
            public string win_price;
            public bool is_x_executed;
            public string win_type;
            public int is_deleted;
            public DateTime created_at;
            public DateTime updated_at;
        }
        [System.Serializable]
        public class WinningHotlist
        {
            public int status;
            public List<List> list = new();
        }
        [System.Serializable]
        public class AllWindetail
        {
            public List<WinDetail> winDetails = new();
        }
        [System.Serializable]
        public class WinDetail
        {
            public string playerId;
            public string gamedata;
            public int bet_ammount;
            public int win_ammount;
            public string win_number;
            public string game_name;
            public int start_point;
            public int game_id;
        }
        [System.Serializable]
        public class SumDetails
        {
            public List<SumDetail> sumDetails = new();
        }
        [System.Serializable]
        public class SumDetail
        {
            public int trippleDigitSum;
            public int doubleDigitSum;
            public int singleDigitSum;
            public string playerId;
        }

    }


}