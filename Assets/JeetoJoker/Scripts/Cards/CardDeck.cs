using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace khelojeetonew
{
    public class CardDeck : MonoBehaviour
    {
        public static CardDeck instance;
        Card[] deckCards;
        Sprite playNormal;
        Sprite playSelected;
        CardSelector[] allCARDS;
        public Sprite[] chipImages;
       
        int chipSelected;

        

        public Transform glowBackground;
        //color change glow effect on card after win
        Color lerpedColor = Color.white;
        public bool isReset;
        Card cardColor;
        public float Duration;
        private void Awake()
        {
            instance = this;
        }
        // Use this for initialization
        void Start()
        {
            //cardSelectingButtons = FindObjectsOfType<CardSelectingButton>();
            deckCards    = GetComponentsInChildren<Card>();
            playNormal   = Resources.Load<Sprite>("SJ_Resources/play_normal");
            playSelected = Resources.Load<Sprite>("SJ_Resources/play_selected");
           // chipImages   = Resources.LoadAll<Sprite>("SJ_Resources/ChipImages/newChipImages");
            chipImages   = Resources.LoadAll<Sprite>("SJ_Resources/ChipImages");
            chipSelected = 5;
            //DataCollector.instance.SetCards(deckCards, groupCard);
            //DataCollector.instance.SetCardCollectingButtons(cardSelectingButtons);
            isReset = true;        
        }

        //public GroupCardsSelector GetGroup(int groupId)
        //{
        //    GroupCardsSelector groupSelector = null;
        //    for (int i = 0; i < groupCard.Length; i++)
        //    {
        //        if (groupCard[i].groupCardNumber == groupId)
        //        {
        //            groupSelector = groupCard[i];
        //            break;
        //        }
        //    }
        //    return groupSelector;
        //}

        void ShowWinner()
        {
            ShowWinningCard(2, 2);
        }

        public Sprite GetChipImage(int value)
        {
            for (int i = 0; i < chipImages.Length; i++)
            {
                if(chipImages[i].name == value.ToString())
                {
                    return chipImages[i];
                }
            }
Debug.Log("Chip image get in 12 card in get chip images");
            return chipImages[0];
        }

        public Sprite GetChipForValueRange(long value)
        {
            Debug.Log("Value " + value);
            if (value == 5)
                return GetChipImage(5);
            else if (value >= 10 && value < 20)
                return GetChipImage(10);
            else if (value >= 20 && value < 50)
                return GetChipImage(20);
            else if (value >= 50 && value < 100)
                return GetChipImage(50);
            else if (value >= 100 && value < 500)
                return GetChipImage(100);
            else if (value >= 500)
                return GetChipImage(500);
            else
                return GetChipImage(10);
        }

        public Sprite GetPlayImageSelected()
        {
            return playSelected;            
        }

        public Sprite GetPlayImageNormal()
        {
            return playNormal;
        }

        public void SetChipSelected(int value)
        {
            chipSelected = value;
        }

        public int GetChipSelected()
        {
            return chipSelected;
        }

        //public void SelectCards(int cardValue, int suiteValue)
        //{
        //    foreach (Card item in deckCards)
        //    {
        //        //item.TrySelecting(cardValue, suiteValue);
        //    }
        //}

        //public void DeSelectCards(int cardValue, int suiteValue)
        //{
        //    foreach (Card item in deckCards)
        //    {
        //      //  item.TryDeSelecting(cardValue, suiteValue);
        //    }
        //}
    
        public void ClearAll(bool a_SessionCompleted)
        {
            foreach (Card item in deckCards)
            {
                item.IsCleared(a_SessionCompleted);
            }
            //foreach (GroupCardsSelector item in groupCard)
            //{
            //    item.IsCleared(a_SessionCompleted);
            //}
        }

        public void DoubleAll()
        {
            foreach (Card item in deckCards)
            {
                item.DoubleUp();
            }
            //foreach (GroupCardsSelector item in groupCard)
            //{
            //    item.DoubleUp();
            //}
        }
        
      
       
        public void RepeatCardsSelectionStorageSingle(List<CardInfo> cardInfo)
        {
            //gameObject.BroadcastMessage("TryAsRepeatCardTwo", cardInfo, SendMessageOptions.RequireReceiver);

            for (int i = 0; i < cardInfo.Count; i++)
            {
                if (cardInfo[i].IsGroup)
                {
                   // GroupCardsSelector groupCardsSelector = GetGroup(cardInfo[i].GroupCardNumber);
                   // groupCardsSelector.OnRebet((int)cardInfo[i].Amount);
                }
                else
                {
                    for (int j = 0; j < deckCards.Length; j++)
                    {
                        if (cardInfo[i].CardNumber == deckCards[j].cardNumber)
                        {
                            deckCards[j].OnRebet((int)cardInfo[i].Amount);
                            break;
                        }
                    }
                }
            }
        }

        public void RepeatSelectingButtons()//SuiteData[] suiteData)
        {
            //foreach (SuiteData item in suiteData)
            //{
            //    cardSelectingButtons[0].transform.parent.BroadcastMessage("TryAsRepeatSuite", item, SendMessageOptions.RequireReceiver);
            //}
        }

        public void ShowWinningCard(int cardValue, int cardSuite)
        {
            //CardData cardData = new CardData(cardValue, cardSuite, 0, 0, 0);
            //gameObject.BroadcastMessage("SetAsWinningCard", cardData, SendMessageOptions.RequireReceiver);
        }

        public void SetWinningGlow(Card card)
        {
            glowBackground.gameObject.SetActive(true);
            //glowBackground.transform.position = card.transform.position;
            glowBackground.transform.position = card.transform.GetChild(1).GetComponent<Image>().transform.position;
            //Debug.LogError(card.transform.GetChild(1).GetComponent<Image>().gameObject.name);
            cardColor = card;
            //UIManager.inst.wonCard(cardColor);
        }
       
        private void Update()
        {
            if(cardColor != null)
            {
                if (isReset == false)
                {
                   // Debug.Log("win" + isReset);
                    //lerpedColor = Color.Lerp(Color.white, Color.green, Mathf.PingPong(Time.time, Duration));
                    //cardColor.transform.GetChild(1).GetComponent<Image>().color = lerpedColor;

                   
                }
                
            }
            
           
        }
       
        public void DisableWinningCard()
        {
            glowBackground.gameObject.SetActive(false);
     
        }
    }
}

public class Utility
{
    private static System.Random random = new System.Random();
    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
          .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
