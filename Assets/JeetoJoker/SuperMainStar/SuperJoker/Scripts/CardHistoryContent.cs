using khelojeetonew;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CardHistoryContent : MonoBehaviour
{
    [SerializeField] private Image cardImage;
    [SerializeField] private Image suiteImage;
    [SerializeField] private GameObject multiplier;
    [SerializeField] private Text multiplierText;
    [SerializeField] private Text timerText;

    public void Initialize(LastResultData lastResultData)
    {
        string itemCard = SuperJokerConstants.CARDNAMEPREFIX + lastResultData.CardValue;
        string itemSuite = SuperJokerConstants.SUITENAMEPREFIX + lastResultData.SuiteValue;
        cardImage.sprite = Resources.Load<Sprite>("SJ_Resources/" + itemCard);
        suiteImage.sprite = Resources.Load<Sprite>("SJ_Resources/" + itemSuite);
        StartCoroutine(ShowXMultiplierText(lastResultData));
    }

    public IEnumerator ShowXMultiplierText(LastResultData lastResultData) {
        yield return new WaitForSeconds(3f);
        if (string.IsNullOrEmpty(lastResultData.multiplier) || lastResultData.multiplier == "1x")
        {
           multiplier.SetActive(false);          
        }
        else
        {
           multiplierText.text = lastResultData.multiplier;
           multiplier.SetActive(true);
        }
 }

    public void PopulateGameHistoryData(GameHistoryItems gameHistoryItems)
    {
        //Debug.Log("PopulateGameHistoryData " +  gameHistoryItems.win_card);
        //Debug.Log("PopulateGameHistoryData0 " +  gameHistoryItems.win_card.Split(',')[0]);
        //Debug.Log("PopulateGameHistoryData1 " +  gameHistoryItems.win_card.Split(',')[1]);
        string itemCard = /*SuperJokerConstants.CARDNAMEPREFIX + */gameHistoryItems.win_card.Split(',')[0];
        string itemSuite = /*SuperJokerConstants.SUITENAMEPREFIX + */gameHistoryItems.win_card.Split(',')[1];
        cardImage.sprite = Resources.Load<Sprite>("SJ_Resources/" + itemCard);
        cardImage.gameObject.SetActive(true);
        suiteImage.sprite = Resources.Load<Sprite>("SJ_Resources/" + itemSuite);
        suiteImage.gameObject.SetActive(true);
        DateTime dateTime = DateTime.Parse(gameHistoryItems.created_at);
        timerText.text = dateTime.ToString("HH:mm");

        //Debug.Log("multiplier3 " + itemCard + " - " + itemSuite + " - " + gameHistoryItems.bonous_spin);
        if (string.IsNullOrEmpty(gameHistoryItems.bonous_spin) || gameHistoryItems.bonous_spin == "1x")
        {
            multiplier.SetActive(false);
        }
        else
        {
            multiplierText.text = gameHistoryItems.bonous_spin;
            multiplier.SetActive(true);
        }
    }
}
