
using System.Collections.Generic;

namespace tripplechance
{
    public class SendData
    {
        [System.Serializable]
        public class JoinRoom
        {
            public string playerId;
            public string name;
            public int totalCoin;
            public string profileImageUrl;
            public string playerStatus;
        }
        [System.Serializable]
        public class Start
        {
            public string roomId;
        }
        
        [System.Serializable]
        public class CardValueSet
        {
            public string card ;
            public int value ;
        }
        [System.Serializable]
        public class Bet
        {
            public string roomId ;
            public string playerId ;
            public int start_point ;
            public string playerBetSum;
            public List<CardValueSet> cardValueSet =new();
        }
        [System.Serializable]
        public class Mode
        {
            public string roomId;
            public string mode;
        }
        [System.Serializable]
        public class Leave
        {
            public string roomId ;
            public string playerId ;
        }

    }
}