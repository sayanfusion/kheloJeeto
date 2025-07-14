
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System;
//using UnityEditorInternal;

namespace khelojeetonew
{
    public class Card : InputDetection
    {
        public int id;
        public int suiteID;

        public int cardNumber;
        public int groupcardNumber;

        public SuperJokerConstants.CardData cardData;


        Image playImage;
        Image chipImage;
        [HideInInspector] public Animator anim;
        Text chipText;
        Text playText;
       // CardDeck cardDeck;
        private float cardInGroup;

        private List<CardInfo> cardInfoDatas = new List<CardInfo>();
        private List<CardBetData> cardBetDatas = new List<CardBetData>();

        private void AddIntoCardInfo(CardInfo a_CardInfo) => cardInfoDatas.Add(a_CardInfo);
        private CardInfo GetMatchingCardInfo(float a_BetAmount) => cardInfoDatas.Where(x => x.Amount == a_BetAmount).LastOrDefault();

        private void SetCardBet(float a_SingleValue, float a_GroupValue)
        {
            CardBetData t_CardValueData = new CardBetData(a_SingleValue, a_GroupValue);
            cardBetDatas.Add(t_CardValueData);
        }

        private float GetTotalSingleBet() => cardBetDatas.Any() ? cardBetDatas.Sum(x => x.SingleBetValue) : 0.0f;
        private float GetTotalGroupBet() => cardBetDatas.Any() ? cardBetDatas.Sum(x => x.GroupBetValue) : 0.0f;


        private void DoubleTheSingleBet() => cardBetDatas.ForEach(x => x.SingleBetValue += x.SingleBetValue);
        private void DoubleTheGroupBet() => cardBetDatas.ForEach(x => x.GroupBetValue += x.GroupBetValue);

        private void DoubleTheBet() => cardBetDatas.ForEach(x =>
        {
            x.SingleBetValue += x.SingleBetValue;
            x.GroupBetValue += x.GroupBetValue;
        });

        private CardBetData GetLastSingleBet() => cardBetDatas.Where(x => x.SingleBetValue >= 5.0f).LastOrDefault();
        private CardBetData GetLastGroupBet() => cardBetDatas.Where(x => x.GroupBetValue >= 2.5f).LastOrDefault();

        private void RemoveLastSingleBet()
        {
            CardBetData t_BetData = cardBetDatas.Where(x => x.SingleBetValue >= 5.0f).LastOrDefault();
            if (t_BetData != null)
            {
                t_BetData.SingleBetValue = 0.0f;
                if (t_BetData.GroupBetValue < 2.5f)
                    cardBetDatas.Remove(t_BetData);
            }
        }

        private void RemoveLastGroupBet()
        {
            CardBetData t_BetData = cardBetDatas.Where(x => x.GroupBetValue >= 2.5f).LastOrDefault();
            if (t_BetData != null)
            {
                t_BetData.GroupBetValue = 0.0f;
                if (t_BetData.SingleBetValue < 5.0f)
                    cardBetDatas.Remove(t_BetData);
            }
        }

        public void HideCardAndChip()
        {
            if (cardBetDatas.Any())
            {
                float t_TotalSingleBet = GetTotalSingleBet();
                float t_TotalGroupBet = GetTotalGroupBet();

                if (t_TotalSingleBet < 5.0f && t_TotalGroupBet < 2.5f)
                {
                    ToggleChipVisibility(false);
                    ToggleBlueRibbonVisibility(false);
                    TogglePlayTextVisibility(true);
                }
                else if (t_TotalSingleBet < 5.0f && t_TotalGroupBet >= 2.5f)
                {

                    ToggleChipVisibility(false);
                    ToggleBlueRibbonVisibility(true);
                    //TogglePlayTextVisibility(false);
                    TogglePlayTextVisibility(true);

                }
                else if (t_TotalSingleBet >= 5.0f && t_TotalGroupBet < 2.5f)
                {
                    ToggleChipVisibility(true);
                    ToggleBlueRibbonVisibility(true);
                    TogglePlayTextVisibility(false);
                }
            }
            else
            {
                ToggleChipVisibility(false);
                ToggleBlueRibbonVisibility(false);
                TogglePlayTextVisibility(true);
            }
        }

        private void ToggleChipVisibility(bool a_State)
        {
            Debug.LogError("ToggleChipVisibility");
            chipImage.gameObject.SetActive(a_State);
            chipText.gameObject.SetActive(a_State);
        }

        private void ToggleBlueRibbonVisibility(bool a_State)
        {
            //if (a_State)
            //{
            //    playImage.sprite = LevelManager.inst.cardSelected;
            //}
            //else
            //{
            //    playImage.sprite = LevelManager.inst.cardDiselect;
            //}
        }

        private void TogglePlayTextVisibility(bool a_State)
        {
            playText.gameObject.SetActive(a_State);
        }

        private void Start()
        {
            //cardDeck = GetComponentInParent<CardDeck>();
            playImage = transform.GetChild(1).GetComponent<Image>();
            chipImage = transform.GetChild(2).GetComponent<Image>();
            chipText = transform.GetChild(3).GetComponent<Text>();
            playText = transform.GetChild(4).GetComponent<Text>();
            anim = GetComponent<Animator>();

            cardBetDatas = new List<CardBetData>();

            ToggleChipVisibility(false);
            ToggleBlueRibbonVisibility(false);
            TogglePlayTextVisibility(true);
        }

        public void OnRebet(int chipValue)
        {
            Clicked(1, false, chipValue);
        }

        public void RemoverButton(CardInfo cardInfo, Action<CardInfo> callback)
        {
            Clicked(-1, false, cardInfo.Amount);
            callback?.Invoke(cardInfo);
        }

        public void Clicked(int modifier, bool ignoreStack = false, float chipValue = 0)
        {
            if (modifier > 0)
            {
                if (!ignoreStack)
                {
                    Debug.Log($"Single BetValue: {chipValue}");
                    BetSingleChip(chipValue);
                    //CardInfo dataExtract = new CardInfo(Utility.RandomString(5), cardNumber, (float)chipValue, false, 0, (int)chipValue);
                    //ButtonManager.instance.UpdateCardNumber(dataExtract);
                    //AddIntoCardInfo(dataExtract);
                }
                else
                {
                    //group added
                    //float value = chipValue / cardInGroup;
                    //value = (float)Math.Round(value, 1);

                    //float fValue = (float)Math.Round(chipValue / cardInGroup, 1);
                    //int iValue = (int)fValue;
                    //float dValue = fValue - iValue;
                    //float betValue = dValue > 0.5f ? iValue + 0.5f : iValue + dValue;
                    //Debug.Log($"fValue: {fValue}, iValue: {iValue}, dValue: {dValue}, betValue: {betValue}");

                    //float valueMultiplier = cardInGroup == 3 ? 3.3f : 2.5f;
                    //float value = (chipValue / 10) * valueMultiplier;
                    //float valueMultiplier = cardInGroup == 3 ? 3 : 4f;
                    //float value = (chipValue) * valueMultiplier;
                    Debug.Log($"group BetValue: {chipValue}");
                    BetGroupChip(chipValue);

                    //CardInfo dataExtract = new CardInfo(Utility.RandomString(5), cardNumber, (float)chipValue, true, 0, (int)chipValue);
                    //ButtonManager.instance.UpdateCardNumber(dataExtract);
                    //AddIntoCardInfo(dataExtract);
                }
            }
            else
            {
                if (!ignoreStack)
                {
                    //single remove
                    if (cardBetDatas.Any())
                    {
                        CardBetData t_CardBetData = GetLastSingleBet();
                        if (t_CardBetData != null)
                        {
                            float amountToDeduct = t_CardBetData.SingleBetValue;
                            //DataCollector.instance.AddCashBack(amountToDeduct);
                            //UIManager.inst.SetPlayAmount(-amountToDeduct);
                            RemoveLastSingleBet();
                            LiveDataSingle(amountToDeduct, -1);
                            float t_TotalBet = GetTotalSingleBet();
                            if (t_TotalBet >= 5.0f)
                            {
                                UpdateChipVisualData(t_TotalBet);
                            }
                            else
                            {
                                HideCardAndChip();
                            }
                            //ButtonManager.instance.RemoveFromBaseCards(GetMatchingCardInfo(amountToDeduct), (x) => cardInfoDatas.Remove(x));
                        }
                    }
                }
                else
                {
                    //group remove
                    if (cardBetDatas.Any())
                    {

                        CardBetData t_CardBetData = GetLastGroupBet();
                        //LiveDataSingle(t_CardBetData.GroupBetValue, -1);
                       // Debug.Log(t_CardBetData.GroupBetValue);
                        if (t_CardBetData != null)
                        {
                            RemoveLastGroupBet();
                            float t_TotalBet = GetTotalGroupBet();
                            Debug.Log($"Card Id: {id}, Total Bet: {t_TotalBet}");

                            if (t_TotalBet < 5f)
                            {
                                HideCardAndChip();
                            }
                        }
                    }
                }
            }
        }

        public void IsCleared(bool a_SessionCompleted)
        {
            float t_TotalBet = GetTotalSingleBet();
            LiveDataSingle(t_TotalBet, -1);

            if (a_SessionCompleted == false)
            {

               // DataCollector.instance.AddCashBack(t_TotalBet);
            }

            ToggleChipVisibility(false);
            ToggleBlueRibbonVisibility(false);
            TogglePlayTextVisibility(true);

            //UIManager.inst.totalBetAmount = 0;

            cardBetDatas.Clear();
            cardBetDatas = new List<CardBetData>();

            if (!a_SessionCompleted)
            {
                if (cardInfoDatas.Any())
                {
                    for (int i = cardInfoDatas.Count - 1; i >= 0; i--)
                    {
                       // ButtonManager.instance.RemoveFromBaseCards(cardInfoDatas[i], (x) => cardInfoDatas.Remove(x));
                    }
                }
            }
            cardInfoDatas.Clear();
            cardInfoDatas = new List<CardInfo>();
        }

        private void BetSingleChip(float value)
        {
            SetCardBet(value, 0.0f);
            UpdateChipVisualData(GetTotalSingleBet());

            ToggleChipVisibility(true);
            ToggleBlueRibbonVisibility(true);
            TogglePlayTextVisibility(false);

            //LiveDataSingle(value, 1);
        }

        float betvalue;
        private void BetGroupChip(float value)
        {
          //  betvalue += value;
          
            SetCardBet(0.0f, value);
            ToggleBlueRibbonVisibility(true);
            TogglePlayTextVisibility(false);
            Debug.LogError("GetLastGroupBet().GroupBetValue " + GetTotalGroupBet());
            UpdateChipVisualData(GetTotalGroupBet());
            ToggleChipVisibility(true);
            //if (GetTotalSingleBet() > 0f)
            //{
            //    TogglePlayTextVisibility(false);
            //}
            //else
            //{
            //    TogglePlayTextVisibility(true);
            //}

           // LiveDataGroup(value, 1);
        }

        public void CardInGroupPresent(float value, int GroupcardNumber)
        {
            cardInGroup = value;
            groupcardNumber = GroupcardNumber;
        }

        public void DoubleUp()
        {
            if (GetTotalSingleBet() >= 5.0f)
            {
                for (int i = cardBetDatas.Count - 1; i >= 0; i--)
                {
                    Clicked(1, false, cardBetDatas[i].SingleBetValue);
                }
            }
        }

        private void UpdateChipVisualData(float a_Bet)
        {
            chipImage.sprite = CardDeck.instance.GetChipForValueRange(Mathf.Abs((int)a_Bet));
            chipText.text = "" + Mathf.Abs(a_Bet).ToString("F0");
        }

        public override void LeftClick(bool ignoreStack = false)
        {
            //if (ButtonManager.instance.removeModifier < 0)
            //{
            //    //Debug.Log("Is RightClick");
            //    RightClick(ignoreStack);
            //    return;
            //}

            //Debug.Log("Is LeftClick "+ GetTotalGroupBet());

            if (!ignoreStack)
            {
                //if (DataCollector.instance.CheckEnoughCash(CardDeck.instance.GetChipSelected()))
                //{
                    
                //    Clicked(ButtonManager.instance.removeModifier, ignoreStack, CardDeck.instance.GetChipSelected());
                //}
            }
            else
            {
                //float value = CardDeck.instance.GetChipSelected() / cardInGroup;
                //value = (float)Math.Round(value, 2);

                //if (DataCollector.instance.CheckEnoughCash(value))
                //{
                //    Clicked(ButtonManager.instance.removeModifier, ignoreStack, CardDeck.instance.GetChipSelected());
                //}
               
                //Clicked(ButtonManager.instance.removeModifier, ignoreStack, CardDeck.instance.GetChipSelected());
            }
        }

        public override void RightClick(bool ignoreStack = false)
        {
            Debug.Log("Is RightClick");
            if (!ignoreStack)
            {
                if (GetTotalSingleBet() >= 5.0f)
                {
                    Clicked(-1, ignoreStack, GetLastSingleBet().SingleBetValue);
                }
            }
            else
            {
                if (GetTotalGroupBet() >= 2.5f)
                {
                   // Debug.LogError("GetLastGroupBet().GroupBetValue "+GetLastGroupBet().GroupBetValue);
                    Clicked(-1, ignoreStack, GetLastGroupBet().GroupBetValue);
                }
            }
        }

        public void LiveDataSingle(float amount, int Modifier)
        {

            //if (id == 1 && suiteID == 1 || id == 2 && suiteID == 1 || id == 3 && suiteID == 1)
            //{
            //    CardinRow.Livedataheart += amount * Modifier;
            //    BetDataDisplayer.BroadcastSingleBetData(amount, Modifier, BetDataDisplayer.ECardType.Heart);
            //}
            //else if (id == 1 && suiteID == 2 || id == 2 && suiteID == 2 || id == 3 && suiteID == 2)
            //{
            //    CardinRow.Livedataspades+= amount * Modifier;
            //    BetDataDisplayer.BroadcastSingleBetData(amount, Modifier, BetDataDisplayer.ECardType.Spades);
            //}
            //else if (id == 1 && suiteID == 3 || id == 2 && suiteID == 3 || id == 3 && suiteID == 3)
            //{
            //    CardinRow.Livedatadiamond += amount * Modifier;
            //    BetDataDisplayer.BroadcastSingleBetData(amount, Modifier, BetDataDisplayer.ECardType.Diamond);
            //}
            //else if (id == 1 && suiteID == 4 || id == 2 && suiteID == 4 || id == 3 && suiteID == 4)
            //{
            //    CardinRow.Livedataclub += amount * Modifier;
            //    BetDataDisplayer.BroadcastSingleBetData(amount, Modifier, BetDataDisplayer.ECardType.Club);
            //}
        }
        public void LiveDataGroup(float amount, int Modifier)
        {
            if (id == 1 && suiteID == 1 || id == 2 && suiteID == 1 || id == 3 && suiteID == 1 || id == 4 && suiteID == 1)
            {
                //CardinRow.Livedataheart += amount * Modifier;
                //BetDataDisplayer.BroadcastBetData(amount, Modifier, BetDataDisplayer.ECardType.Heart);
            }
            else if (id == 1 && suiteID == 2 || id == 2 && suiteID == 2 || id == 3 && suiteID == 2 || id == 4 && suiteID == 2)
            {
                //CardinRow.Livedataspades += amount * Modifier;
                //BetDataDisplayer.BroadcastBetData(amount, Modifier, BetDataDisplayer.ECardType.Spades);
            }
            else if (id == 1 && suiteID == 3 || id == 2 && suiteID == 3 || id == 3 && suiteID == 3 || id == 4 && suiteID == 3)
            {
                //CardinRow.Livedatadiamond += amount * Modifier;
                //BetDataDisplayer.BroadcastBetData(amount, Modifier, BetDataDisplayer.ECardType.Diamond);
            }
            else if (id == 1 && suiteID == 4 || id == 2 && suiteID == 4 || id == 3 && suiteID == 4 || id == 4 && suiteID == 4)
            {
                //CardinRow.Livedataclub += amount * Modifier;
                //BetDataDisplayer.BroadcastBetData(amount, Modifier, BetDataDisplayer.ECardType.Club);
            }
        }
        

      
    }
}
[Serializable]
public class CardInfo
{
    public string UniqueId;
    public int CardNumber;
    public float Amount;
    public bool IsGroup;
    public int GroupCardNumber;
    public int ChipSelectedValue;

    public CardInfo(string uniqueId, int cardNumber, float amount, bool isGroup, int groupCardNumber, int chipSelectedValue)
    {
        UniqueId = uniqueId;
        CardNumber = cardNumber;
        Amount = amount;
        IsGroup = isGroup;
        GroupCardNumber = groupCardNumber;
        ChipSelectedValue = chipSelectedValue;
    }
}

[Serializable]
public class CardBetData
{
    public float SingleBetValue = 0;
    public float GroupBetValue = 0;

    public CardBetData(float a_SingleBetValue, float a_GroupBetValue)
    {
        SingleBetValue = a_SingleBetValue;
        GroupBetValue = a_GroupBetValue;
    }
}
