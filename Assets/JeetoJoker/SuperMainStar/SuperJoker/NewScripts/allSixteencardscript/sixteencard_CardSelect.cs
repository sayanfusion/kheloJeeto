using khelojeetonew;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


namespace khelojeetonew
{
    public class sixteencard_CardSelect : InputDetection, ButtonHandlerss, IRemoveHandlers, IWinHandlers
    {
         public static sixteencard_CardSelect instance;
        public int id;
        public int suiteID;

        public int cardNumber;
        public int groupcardNumber;

        [SerializeField] private Image playImage;
        [SerializeField] private Image chipImage;
        public Animator anim;
        [SerializeField] private Text chipText;
        // [SerializeField] private Text playText;

        [HideInInspector] public List<sixteencards_groupcardselector> groupCardSelects;

        private List<BetButtonss> totalBets = new List<BetButtonss>();

        private List<BetButtonss> prevRoundTotalBets = new List<BetButtonss>();

        private List<int> removeCount = new List<int>();

        private static int zero = 0;
        
        private void Awake()
        {
            instance=this;
        }   

        private void Start()
        {
            Sixteen_cards.instance.ButtonHandlerss.Add(this);
           Sixteen_cards.instance.winHandlerss.Add(this);
         //  JeetoJokerManager.instance.winHandlers.Add(this);

            ToggleChipVisibility(false);
            ToggleBlueRibbonVisibility(false);
            // TogglePlayTextVisibility(true);
            IRemoveHandlers.refresh += Refresh;
        }

        public  void ToggleChipVisibility(bool a_State)
        {
            chipImage.gameObject.SetActive(a_State);
            chipText.gameObject.SetActive(a_State);
        }

        public void ToggleBlueRibbonVisibility(bool a_State)
        {
            if (a_State)
            {
                playImage.sprite = Sixteen_cards.instance.CardSelected;
            }
            else
            {
                playImage.sprite = Sixteen_cards.instance.CardDiselect;
            }
        }

        // // public  void TogglePlayTextVisibility(bool a_State)
        // {
        //     playText.gameObject.SetActive(a_State);
        // }

        private void UpdateChipVisualData(long a_Bet)
        {
            chipImage.sprite = sixteencards_CardDeck.instance.GetChipForValueRange(a_Bet);
            chipText.text = "" + Mathf.Abs(a_Bet).ToString("F0");
        }

        private long GetTotalBetSum()
        {
            long sum = zero;
            totalBets.ForEach(x => sum = x.amount + sum);
            return sum;
        }

        private long GetTotalGroupBetSum()
        {
            long sum = zero;
            groupCardSelects.ForEach(x => sum = sum + x.GetTotalBetSum());
            return sum;
        }

        public override void LeftClick(bool ignoreStack = false)
        {
            base.LeftClick();

            BetButtonss bet = Sixteen_cards.instance.SelectedBetbuttons;
Debug.Log("bet left click:::::"+bet);
            if (!Sixteen_cards.instance.Bet(bet.amount))
            {
                return;
            }

            totalBets.Add(bet);

            long totalBet = GetTotalBetSum() + GetTotalGroupBetSum();
            UpdateChipVisualData(totalBet);

            if (totalBets.Count == 1 || groupCardSelects.Find(x => x.TotalGrpBetsCount == 1) != null)
            {
                ToggleChipVisibility(true);
                ToggleBlueRibbonVisibility(true);
                // TogglePlayTextVisibility(false);
            }

           Sixteen_cards.instance.CheckForRepeatButton();

            Sixteen_cards.instance.removeHandlerss.Add(new List<IRemoveHandlers>() { this });
            removeCount.Add(Sixteen_cards.instance.removeCount);
           Sixteen_cards.instance.removeCount += 1;
        }

        public override void RightClick(bool ignoreStack = false)
        {
            base.RightClick();
            if (!Sixteen_cards.instance.canBet)
                return;
            if (totalBets.Count > zero)
            {
                BetButtonss bet = totalBets[totalBets.Count - 1];
                totalBets.RemoveAt(totalBets.Count - 1);

              Sixteen_cards.instance.RemoveBet(bet.amount);

                long totalBet = GetTotalBetSum() + GetTotalGroupBetSum();
                UpdateChipVisualData(totalBet);

                if (totalBets.Count == zero && groupCardSelects.TrueForAll(x => x.TotalGrpBetsCount == zero))
                {
                    ToggleChipVisibility(false);
                    ToggleBlueRibbonVisibility(false);
                    // TogglePlayTextVisibility(true);
                }

                Sixteen_cards.instance.CheckForRepeatButton();

                if (removeCount.Count > zero)
                {
                    int currentIndex = removeCount[removeCount.Count - 1];
                    // Debug.LogError(currentIndex + "/"+ JeetoJokerManager.instance.removeHandlers.Count);
                    List<IRemoveHandlers> n = Sixteen_cards.instance.removeHandlerss[currentIndex];
                    n.Remove(this);
                    removeCount.RemoveAt(removeCount.Count - 1);

                    if (n.Count == zero)
                    {
                       Sixteen_cards.instance.removeHandlerss.RemoveAt(currentIndex);
                        Sixteen_cards.instance.removeCount -= 1;
                        IRemoveHandler.refresh?.Invoke(currentIndex);
                    }

                }
            }
        }

        private void Refresh(int index)
        {
            int count = removeCount.Count;
            for (int i = 0; i < count; i++)
            {
                if (removeCount[i] > index)
                    removeCount[i]--;
            }
        }

        public void OnClickGroupLeftClick()
        {
            long totalBet = GetTotalBetSum() + GetTotalGroupBetSum();

            UpdateChipVisualData(totalBet);

            // if (totalBets.Count == 1 || groupCardSelects.Find(x => x.TotalGrpBetsCount == 1) != null)
            // {
                ToggleChipVisibility(true);
                ToggleBlueRibbonVisibility(true);
                // TogglePlayTextVisibility(false);
            // }
        }

        public void OnClickGroupRightClick()
        {
            long totalBet = GetTotalBetSum() + GetTotalGroupBetSum();

            UpdateChipVisualData(totalBet);

            if (totalBets.Count == zero && groupCardSelects.TrueForAll(x => x.TotalGrpBetsCount == zero))
            {
                ToggleChipVisibility(false);
                ToggleBlueRibbonVisibility(false);
                // TogglePlayTextVisibility(true);
            }
        }

        public void Clear()
        {
            totalBets.Clear();
            removeCount.Clear();

            ToggleChipVisibility(false);
            ToggleBlueRibbonVisibility(false);
            // TogglePlayTextVisibility(true);
        }

        public void Remove()
        {
            base.RightClick();

            if (totalBets.Count > zero)
            {
                BetButtonss bet = totalBets[totalBets.Count - 1];
                totalBets.RemoveAt(totalBets.Count - 1);

                Sixteen_cards.instance.RemoveBet(bet.amount);

                long totalBet = GetTotalBetSum() + GetTotalGroupBetSum();

                UpdateChipVisualData(totalBet);

                if (totalBets.Count == zero && groupCardSelects.TrueForAll(x => x.TotalGrpBetsCount == zero))
                {
                    ToggleChipVisibility(false);
                    ToggleBlueRibbonVisibility(false);
                    // TogglePlayTextVisibility(true);
                }

                Sixteen_cards.instance.CheckForRepeatButton();
            }

            removeCount.RemoveAt(removeCount.Count - 1);
        }

        public void Repeat()
        {
            if (prevRoundTotalBets.Count == zero)
                return;

            totalBets.Clear();

            totalBets.AddRange(prevRoundTotalBets);

            foreach (BetButtonss bet in totalBets)
            {
                int amt = bet.amount;

                Sixteen_cards.instance.Bet(amt);
            }

            long totalBet = GetTotalBetSum() + GetTotalGroupBetSum();

            UpdateChipVisualData(totalBet);

            ToggleChipVisibility(true);
            ToggleBlueRibbonVisibility(true);
            // TogglePlayTextVisibility(false);
            StartCoroutine(RepeatCoroutine());
        }

        private IEnumerator RepeatCoroutine()
        {

            yield return new  WaitForSeconds(0.1f);

            ToggleChipVisibility(true);
            ToggleBlueRibbonVisibility(true);
            // TogglePlayTextVisibility(false);

        }

        public void SavePrevRound()
        {
            prevRoundTotalBets.Clear();
            if (totalBets.Count > zero)
            {
                prevRoundTotalBets.AddRange(totalBets);
            }
        }

        public bool CheckifLastSavedDataAvailable()
        {
            if (prevRoundTotalBets.Count > zero)
            {
                return true;
            }

            return false;
        }

        public long GetRoundTotalSum()
        {
            long sum = zero;
            totalBets.ForEach(x => sum = sum + x.amount);
            return sum;
        }

        public long GetPrevRoundTotalSum()
        {
            long sum = zero;
            prevRoundTotalBets.ForEach(x => sum = sum + x.amount);
            return sum;
        }

        public void DoubleUp(int removeCountIndex)
        {
            if (totalBets.Count == zero)
                return;

            foreach (BetButtonss bet in totalBets)
            {
                int amt = bet.amount;

                Sixteen_cards.instance.Bet(amt);
            }

            totalBets.AddRange(totalBets);

            long totalBet = GetTotalBetSum() + GetTotalGroupBetSum();

            UpdateChipVisualData(totalBet);

            ToggleChipVisibility(true);
            ToggleBlueRibbonVisibility(true);
            // TogglePlayTextVisibility(false);

            Sixteen_cards.instance.removeHandlerss[removeCountIndex].Add(this);
            removeCount.Add(Sixteen_cards.instance.removeCount);
        }
public long totalbetdata;
        public bool OnWin(int outerId, int innerId)
        {
            long totalBet = GetTotalBetSum() + GetTotalGroupBetSum();
             totalbetdata=totalBet;
            if (totalBet > zero)
            {
                Sixteen_cards.instance.AddBetAmounts(groupcardNumber, totalBet);
            }
            if (outerId == id && innerId == suiteID)
            {

                Sequence seq = DOTween.Sequence();
                seq.AppendCallback(() => Sixteen_cards.instance.ShowWinAmount(totalBet));

                for (int i = zero; i < 20; i++)
                {
                    seq.AppendCallback(() => ToggleBlueRibbonVisibility(true));
                    seq.AppendInterval(0.2f);
                    seq.AppendCallback(() => ToggleBlueRibbonVisibility(false));
                    seq.AppendInterval(0.2f);
                }
                if (totalBet > zero)
                    return true;
            }
            return false;
        }

        public BetDataHandlers GetBetData()
        {
            string _id = id.ToString();
            string _suit = suiteID.ToString();
            switch (_id)
            {
                case "1":
                    _id = "J";
                    break;
                case "2":
                    _id = "Q";
                    break;
                case "3":
                    _id = "K";
                    break;
                case "4":
                    _id = "A";
                    break;
                default:
                    break;
            }
            switch (_suit)
            {
                case "1":
                    _suit = "H";
                    break;
                case "2":
                    _suit = "S";
                    break;
                case "3":
                    _suit = "D";
                    break;
                case "4":
                    _suit = "C";
                    break;
                default:
                    break;
            }
            BetDataHandlers betDatas = new BetDataHandlers();
            betDatas.Id = _suit + "-" + _id;
            betDatas.amount = GetTotalBetSum() + GetTotalGroupBetSum();
            return betDatas;
        }

        public long GetTotalBetAmount()
        {
            return GetTotalBetSum() + GetTotalGroupBetSum();
        }
          public void PlaceBet(int betAmount)
        {
    Debug.Log("place bet function");     
          
        }
    }


    }
   
    
