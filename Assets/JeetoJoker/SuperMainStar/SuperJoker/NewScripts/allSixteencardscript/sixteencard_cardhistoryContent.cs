using khelojeetonew;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
namespace khelojeetonew
{

public class sixteencard_cardhistoryContent : MonoBehaviour
{
    [SerializeField] private Image cardImage;
    [SerializeField] private Image suiteImage;
    [SerializeField] private GameObject multiplier;
    [SerializeField] private Text multiplierText;
    [SerializeField] private Text timerText;

    public void Initialize(LastResultData1 lastResultData1)
    {
        string itemCard = SuperJokerConstants.CARDNAMEPREFIX + lastResultData1.CardValue;
        string itemSuite = SuperJokerConstants.SUITENAMEPREFIX + lastResultData1.SuiteValue;

        Debug.Log(itemCard + "ItemCard");
        cardImage.sprite = Resources.Load<Sprite>("SJ_Resources/" + itemCard);
        suiteImage.sprite = Resources.Load<Sprite>("SJ_Resources/" + itemSuite);
        StartCoroutine(ShowXMultiplierText(lastResultData1));
    }

    public IEnumerator ShowXMultiplierText(LastResultData1 lastResultData1) {
        yield return new WaitForSeconds(3f);
        if (string.IsNullOrEmpty(lastResultData1.multiplier) || lastResultData1.multiplier == "1x")
        {
           multiplier.SetActive(false);          
        }
        else
        {
           multiplierText.text = lastResultData1.multiplier;
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
}