using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIHistoryContent : MonoBehaviour
{
    [SerializeField] private Image imgCard;
    [SerializeField] private Image imgSuit;
    [SerializeField] private Text txtGameid;
   // [SerializeField] private Text txtStatus;
    [SerializeField] private Text txtResult;
    [SerializeField] private Text txtPlay;
    [SerializeField] private Text txtWon;
     //[SerializeField] private Image imgCardinfodata; // For card value (e.g., J, Q, K, A, numeric cards)
   // [SerializeField] private Image imgSuitinfodata; // For suit value (e.g., Hearts, Spades, Diamonds, Clubs)
    //[SerializeField] private Text txtWinnumber;
   /* public void Bind(int index, HistoryDataListItem historyData)
    {
        txtTicketid.text = index.ToString();
        txtStatus.text = historyData.id.ToString();
        txtResult.text = historyData.win_loose.ToString();
        txtPlay.text = historyData.bet_ammount.ToString();
        txtWon.text = historyData.win_ammount.ToString();
        txtLoss.text=historyData.win_loose.ToString();
    }*/
   /* public void Bind(int index, HistoryDataListItem historyData)
{
    if (historyData == null)
    {
        Debug.LogError("Bind failed: historyData is null.");
        return;
    }
    // Assign the game_id to txtTicketid
    txtGameid.text = historyData.game_id ?? "Unknown";
   // txtStatus.text = historyData.id.ToString();
    // Ensure null-safe access for string fields
    txtResult.text = historyData.win_loose ?? "Unknown";
    txtPlay.text = historyData.bet_ammount ?? "0";
    txtWon.text = historyData.win_ammount ?? "0";
    txtWinnumber.text = historyData.win_number ?? "Unknown";
    Debug.Log($"Data bound successfully for index {index}: {JsonUtility.ToJson(historyData)}");
}*/
   public void Bind(int index, HistoryDataListItemJeeto historyData)
    {
        if (historyData == null)
        {
            Debug.LogError("Bind failed: historyData is null.");
            return;
        }
        // Set the basic text fields
        txtGameid.text = historyData.game_id ?? "Unknown";
        txtResult.text = historyData.win_loose ?? "Unknown";
        txtPlay.text = historyData.bet_ammount ?? "0";
        txtWon.text = historyData.win_ammount ?? "0";
        // Update card and suit images
        if (!string.IsNullOrEmpty(historyData.win_number))
        {
            UpdateWinNumber(historyData.win_number);
        }
        // Debug.Log($"Data bound successfully for index {index}: {JsonUtility.ToJson(historyData)}");
    }
    private void UpdateWinNumber(string winNumberStr)
    {
        if (int.TryParse(winNumberStr, out int winNumber))
        {
            int cardValue = winNumber / 10; // Extract card value
            int suitValue = winNumber % 10; // Extract suit value
            // Load sprites from Resources folder
            Sprite cardSprite = Resources.Load<Sprite>($"Cards/{GetCardName(cardValue)}");
            Sprite suitSprite = Resources.Load<Sprite>($"Suits/{GetSuitName(suitValue)}");
            if (cardSprite == null || suitSprite == null)
            {
                Debug.LogError($"Failed to load sprites for win_number: {winNumberStr}");
                return;
            }
            // Assign sprites to the respective image fields
            imgCard.sprite = cardSprite;
            imgSuit.sprite = suitSprite;
        }
        else
        {
            Debug.LogError($"Invalid win_number format: {winNumberStr}");
        }
    }
    private string GetCardName(int cardValue)
    {
        return cardValue switch
        {
            11 => "J",
            12 => "Q",
            13 => "K",
            14 => "A",
            _ => cardValue.ToString() // For numeric cards
        };
    }
    private string GetSuitName(int suitValue)
    {
        return suitValue switch
        {
            1 => "Heart",
            2 => "Spade",
            3 => "Diamond",
            4 => "Club",
            _ => "Unknown"
        };
    }
}