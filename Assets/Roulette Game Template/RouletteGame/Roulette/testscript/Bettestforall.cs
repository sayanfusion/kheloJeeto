using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*public class CardDatabet
{
    public string card { get; set; }
    public float value { get; set; }

    public CardDatabet(string card, float value)
    {
        this.card = card;
        this.value = value;
    }
}
 public enum BetTypeforbet
{
    Red,//
    Black,
    FirstTwelve,
    SecondTwelve,
    ThirdTwelve,
    OnetoEighteen,
    NineteentoThirtySix,
    Odd,//
    Even,
    twotoOnelower,
    twotoOnemiddle,
    twotoOnehigher,

 }*/

public class Bettestforall : MonoBehaviour
{

/*private List<CardDatabet> cardList;
   public void Start()
   {
     CreateCardArray();
     PlaceBet(BetTypeforbet.Red, 18); // Place 100 on Red
    PlaceBet(BetTypeforbet.Odd, 50);
    }
  void CreateCardArray()
{
    List<CardDatabet> cardList = new List<CardDatabet>();
    System.Text.StringBuilder debugInfo = new System.Text.StringBuilder();
    // Loop through card values from 1 to 37
    for (int i = 1; i <= 36; i++)
    {
        string cardNumber = i.ToString();
        float cardValue = 0f;
        // Add a new CardData object to the list
        cardList.Add(new CardDatabet(cardNumber, cardValue));
        // Append card and value data to the debugInfo string
        debugInfo.AppendLine($"Card: {cardNumber}, Value: {cardValue}");
    }
    Debug.Log(debugInfo.ToString());
}
 void PlaceBet(BetTypeforbet betType, float amount)
 {
        List<CardDatabet> affectedCards = new List<CardDatabet>();
        // Determine the affected cards based on the bet type
        switch (betType)
        {
            case BetTypeforbet.Red:
               affectedCards = FindRedCards();
                break;
            case BetTypeforbet.Black:
         affectedCards = FindBlackCards();
                break;
            case BetTypeforbet.FirstTwelve:
                  affectedCards = FindRangeCards(1, 12);
                break;
            case BetTypeforbet.SecondTwelve:
                     affectedCards = FindRangeCards(13, 24);
                break;
                case BetTypeforbet.ThirdTwelve:
                affectedCards = FindRangeCards(25,36);
                break;
                case BetTypeforbet.OnetoEighteen:
                affectedCards = FindRangeCards(1,18);
                break;
                  case BetTypeforbet.NineteentoThirtySix:
                affectedCards = FindRangeCards(1,18);
                break;
            case BetTypeforbet.Odd:
                   affectedCards = FindOddCards();
                break;
            case BetTypeforbet.Even:
                 affectedCards = FindEvenCards();
                break;
                 case BetTypeforbet.twotoOnelower:
                 affectedCards = FindtwotoOnelower();
                break;
                 case BetTypeforbet.twotoOnemiddle:
                 affectedCards = FindtwotoOnemiidle();
                break;
                 case BetTypeforbet.twotoOnehigher:
                 affectedCards = FindtwotoOnehigher();
                break;

        }

        // Distribute the bet amount across the affected cards
        float amountPerCard = amount / affectedCards.Count;
       foreach (CardDatabet card in affectedCards)
{
    card.value += amountPerCard; // Update card value directly
}
        // Debug the updated cardList
        DebugUpdatedCardList();
    }

    // Function to return the list of red cards (for example, assume even numbers are red)
    List<CardDatabet> FindRedCards()
    {
        List<CardDatabet> redCards = new List<CardDatabet>();
        // Define the specific red card numbers
        HashSet<int> redCardNumbers = new HashSet<int> { 1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36 };

        foreach (var card in cardList)
        {
            int cardNumber = int.Parse(card.card);
            if (redCardNumbers.Contains(cardNumber)) // Check if the card number is in the red card set
            {
                redCards.Add(card);
            }
        }
        return redCards;
    }

    // Function to find cards that are considered "black" (for example, odd-numbered cards)
    List<CardDatabet> FindBlackCards()
{
    List<CardDatabet> blackCards = new List<CardDatabet>();
    // Define the specific black cards
    HashSet<int> blackCardNumbers = new HashSet<int> { 2, 4, 6, 8, 10, 11, 13, 15, 17, 20, 22, 24, 26, 28, 29, 31, 33, 35 };
    
    foreach (var card in cardList)
    {
        int cardNumber = int.Parse(card.card);
        if (blackCardNumbers.Contains(cardNumber)) // Check if the card number is in the black cards list
        {
            blackCards.Add(card);
        }
    }
    return blackCards;
}
  List<CardDatabet> FindtwotoOnelower()
{
    List<CardDatabet> TwotoOne = new List<CardDatabet>();
    // Define the specific black cards
    HashSet<int> bettwoToone = new HashSet<int> { 3, 6, 9,12,15,18,21,24,27,30,33,36 };
    
    foreach (var card in cardList)
    {
        int cardNumber = int.Parse(card.card);
        if (bettwoToone.Contains(cardNumber)) // Check if the card number is in the black cards list
        {
            TwotoOne.Add(card);
        }
    }
    return TwotoOne;
}
 List<CardDatabet> FindtwotoOnemiidle()
{
    List<CardDatabet> TwotoOnemiddle = new List<CardDatabet>();
    // Define the specific black cards
    HashSet<int> bettwoToonemidle = new HashSet<int> {2,5,8,11,14,17,20,23,26,29,32,35 };
    
    foreach (var card in cardList)
    {
        int cardNumber = int.Parse(card.card);
        if (bettwoToonemidle.Contains(cardNumber)) // Check if the card number is in the black cards list
        {
            TwotoOnemiddle.Add(card);
        }
    }
    return TwotoOnemiddle;
}
List<CardDatabet> FindtwotoOnehigher()
{
    List<CardDatabet> TwotoOnehigher = new List<CardDatabet>();
    // Define the specific black cards
    HashSet<int> bettwoToonehigher = new HashSet<int> {1,4,7,10,13,16,19,22,25,28,31,34 };
    
    foreach (var card in cardList)
    {
        int cardNumber = int.Parse(card.card);
        if (bettwoToonehigher.Contains(cardNumber)) // Check if the card number is in the black cards list
        {
            TwotoOnehigher.Add(card);
        }
    }
    return TwotoOnehigher;
}


    // Function to find odd-numbered cards
    List<CardDatabet> FindOddCards()
    {
        List<CardDatabet> oddCards = new List<CardDatabet>();
        foreach (var card in cardList)
        {
            int cardNumber = int.Parse(card.card);
            if (cardNumber % 2 != 0) // Odd cards
            {
                oddCards.Add(card);
            }
        }
        return oddCards;
    }

    // Function to find even-numbered cards
    List<CardDatabet> FindEvenCards()
    {
        List<CardDatabet> evenCards = new List<CardDatabet>();
        foreach (var card in cardList)
        {
            int cardNumber = int.Parse(card.card);
            if (cardNumber % 2 == 0) // Even cards
            {
                evenCards.Add(card);
            }
        }
        return evenCards;
    }

    // Function to find cards in a given range (inclusive)
    List<CardDatabet> FindRangeCards(int start, int end)
    {
        List<CardDatabet> rangeCards = new List<CardDatabet>();
        foreach (var card in cardList)
        {
            int cardNumber = int.Parse(card.card);
            if (cardNumber >= start && cardNumber <= end)
            {
                rangeCards.Add(card);
            }
        }
        return rangeCards;
    }

    // Function to debug the updated cardList
    void DebugUpdatedCardList()
    {
        System.Text.StringBuilder debugInfo = new System.Text.StringBuilder();

        foreach (CardDatabet cardData in cardList)
        {
            debugInfo.AppendLine($"Card: {cardData.card}, Value: {cardData.value}");
        }

        Debug.Log(debugInfo.ToString());
    }
}*/
}
//code for compile
/*using System;
using System.Collections.Generic;

public class CardDatabet
{
    public string card { get; set; }
    public float value { get; set; }

    public CardDatabet(string card, float value)
    {
        this.card = card;
        this.value = value;
    }
}

public enum BetTypeforbet
{
    Red,
    Black,
    FirstTwelve,
    SecondTwelve,
    ThirdTwelve,
    OnetoEighteen,
    NineteentoThirtySix,
    Odd,
    Even,
    twotoOnelower,
    twotoOnemiddle,
    twotoOnehigher,
}

public class Bettestforall
{
    private List<CardDatabet> cardList;

    public Bettestforall()
    {
        CreateCardArray();
        PlaceBet(BetTypeforbet.Red, 18); // Place 18 on Red
        PlaceBet(BetTypeforbet.Odd, 36);
        PlaceBet(BetTypeforbet.Black, 18);
        PlaceBet(BetTypeforbet.FirstTwelve, 18);
        PlaceBet(BetTypeforbet.SecondTwelve, 18);
        PlaceBet(BetTypeforbet.ThirdTwelve, 18);
        PlaceBet(BetTypeforbet.OnetoEighteen, 18);
        PlaceBet(BetTypeforbet.NineteentoThirtySix, 18);
        PlaceBet(BetTypeforbet.Even, 18);
        PlaceBet(BetTypeforbet.twotoOnelower, 18);
        PlaceBet(BetTypeforbet.twotoOnemiddle, 18);
        PlaceBet(BetTypeforbet.twotoOnehigher, 18);  
              
        
    }

    void CreateCardArray()
    {
        cardList = new List<CardDatabet>();
        System.Text.StringBuilder debugInfo = new System.Text.StringBuilder();
        
        // Loop through card values from 1 to 37
        for (int i = 1; i <= 36; i++)
        {
            string cardNumber = i.ToString();
            float cardValue = 0f;
            // Add a new CardData object to the list
            cardList.Add(new CardDatabet(cardNumber, cardValue));
            // Append card and value data to the debugInfo string
            debugInfo.AppendLine($"Card: {cardNumber}, Value: {cardValue}");
        }
        Console.WriteLine(debugInfo.ToString());
    }

    void PlaceBet(BetTypeforbet betType, float amount)
    {
        List<CardDatabet> affectedCards = new List<CardDatabet>();

        // Determine the affected cards based on the bet type
        switch (betType)
        {
            case BetTypeforbet.Red:
                affectedCards = FindRedCards();
                break;
            case BetTypeforbet.Black:
                affectedCards = FindBlackCards();
                break;
            case BetTypeforbet.FirstTwelve:
                affectedCards = FindRangeCards(1, 12);
                break;
            case BetTypeforbet.SecondTwelve:
                affectedCards = FindRangeCards(13, 24);
                break;
            case BetTypeforbet.ThirdTwelve:
                affectedCards = FindRangeCards(25, 36);
                break;
            case BetTypeforbet.OnetoEighteen:
                affectedCards = FindRangeCards(1, 18);
                break;
            case BetTypeforbet.NineteentoThirtySix:
                affectedCards = FindRangeCards(19, 36);
                break;
            case BetTypeforbet.Odd:
                affectedCards = FindOddCards();
                break;
            case BetTypeforbet.Even:
                affectedCards = FindEvenCards();
                break;
            case BetTypeforbet.twotoOnelower:
                affectedCards = FindtwotoOnelower();
                break;
            case BetTypeforbet.twotoOnemiddle:
                affectedCards = FindtwotoOnemiidle();
                break;
            case BetTypeforbet.twotoOnehigher:
                affectedCards = FindtwotoOnehigher();
                break;
        }

        // Distribute the bet amount across the affected cards
        float amountPerCard = amount / affectedCards.Count;

        foreach (CardDatabet card in affectedCards)
        {
            card.value += amountPerCard; // Update card value directly
        }

        // Debug the updated cardList
        DebugUpdatedCardList();
    }

    List<CardDatabet> FindRedCards()
    {
        List<CardDatabet> redCards = new List<CardDatabet>();
        // Define the specific red card numbers
        HashSet<int> redCardNumbers = new HashSet<int> { 1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36 };

        foreach (var card in cardList)
        {
            int cardNumber = int.Parse(card.card);
            if (redCardNumbers.Contains(cardNumber)) // Check if the card number is in the red card set
            {
                redCards.Add(card);
            }
        }
        return redCards;
    }

    List<CardDatabet> FindBlackCards()
    {
        List<CardDatabet> blackCards = new List<CardDatabet>();
        // Define the specific black cards
        HashSet<int> blackCardNumbers = new HashSet<int> { 2, 4, 6, 8, 10, 11, 13, 15, 17, 20, 22, 24, 26, 28, 29, 31, 33, 35 };

        foreach (var card in cardList)
        {
            int cardNumber = int.Parse(card.card);
            if (blackCardNumbers.Contains(cardNumber)) // Check if the card number is in the black cards list
            {
                blackCards.Add(card);
            }
        }
        return blackCards;
    }

    List<CardDatabet> FindtwotoOnelower()
    {
        List<CardDatabet> TwotoOne = new List<CardDatabet>();
        // Define the specific lower cards
        HashSet<int> bettwoToone = new HashSet<int> { 3, 6, 9, 12, 15, 18, 21, 24, 27, 30, 33, 36 };

        foreach (var card in cardList)
        {
            int cardNumber = int.Parse(card.card);
            if (bettwoToone.Contains(cardNumber)) // Check if the card number is in the list
            {
                TwotoOne.Add(card);
            }
        }
        return TwotoOne;
    }

    List<CardDatabet> FindtwotoOnemiidle()
    {
        List<CardDatabet> TwotoOnemiddle = new List<CardDatabet>();
        // Define the specific middle cards
        HashSet<int> bettwoToonemidle = new HashSet<int> { 2, 5, 8, 11, 14, 17, 20, 23, 26, 29, 32, 35 };

        foreach (var card in cardList)
        {
            int cardNumber = int.Parse(card.card);
            if (bettwoToonemidle.Contains(cardNumber)) // Check if the card number is in the list
            {
                TwotoOnemiddle.Add(card);
            }
        }
        return TwotoOnemiddle;
    }

    List<CardDatabet> FindtwotoOnehigher()
    {
        List<CardDatabet> TwotoOnehigher = new List<CardDatabet>();
        // Define the specific higher cards
        HashSet<int> bettwoToonehigher = new HashSet<int> { 1, 4, 7, 10, 13, 16, 19, 22, 25, 28, 31, 34 };

        foreach (var card in cardList)
        {
            int cardNumber = int.Parse(card.card);
            if (bettwoToonehigher.Contains(cardNumber)) // Check if the card number is in the list
            {
                TwotoOnehigher.Add(card);
            }
        }
        return TwotoOnehigher;
    }

    List<CardDatabet> FindOddCards()
    {
        List<CardDatabet> oddCards = new List<CardDatabet>();
        foreach (var card in cardList)
        {
            int cardNumber = int.Parse(card.card);
            if (cardNumber % 2 != 0) // Odd cards
            {
                oddCards.Add(card);
            }
        }
        return oddCards;
    }

    List<CardDatabet> FindEvenCards()
    {
        List<CardDatabet> evenCards = new List<CardDatabet>();
        foreach (var card in cardList)
        {
            int cardNumber = int.Parse(card.card);
            if (cardNumber % 2 == 0) // Even cards
            {
                evenCards.Add(card);
            }
        }
        return evenCards;
    }

    List<CardDatabet> FindRangeCards(int start, int end)
    {
        List<CardDatabet> rangeCards = new List<CardDatabet>();
        foreach (var card in cardList)
        {
            int cardNumber = int.Parse(card.card);
            if (cardNumber >= start && cardNumber <= end)
            {
                rangeCards.Add(card);
            }
        }
        return rangeCards;
    }

    void DebugUpdatedCardList()
    {
        System.Text.StringBuilder debugInfo = new System.Text.StringBuilder();
        foreach (var card in cardList)
        {
            debugInfo.AppendLine($"Card: {card.card}, Value: {card.value}");
        }
        Console.WriteLine(debugInfo.ToString());
    }

    public static void Main(string[] args)
    {
        new Bettestforall();
    }
}*/

