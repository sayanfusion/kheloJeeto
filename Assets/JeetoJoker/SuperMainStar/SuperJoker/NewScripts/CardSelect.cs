using khelojeetonew;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using JeetoJoker;
using TMPro; // Required for TextMeshPro components



namespace khelojeetonew
{
    public class CardSelect : InputDetection, ButtonHandlers, IRemoveHandler, IWinHandler
    {

        public static CardSelect instance;
        public int id;
        public int suiteID;

        public int cardNumber;
        public int groupcardNumber;

        [SerializeField] internal Image playImage;
        [SerializeField] private Image chipImage;
        [SerializeField] internal GameObject playText;
        
        public Animator anim;
        //[SerializeField] public Text chipText;
[SerializeField] public TextMeshProUGUI chipText;
        // [SerializeField] private Text playText;

        [HideInInspector] public List<GroupCardSelect> groupCardSelect;

        public List<BetButtons> totalBets = new List<BetButtons>();

        private List<BetButtons> prevRoundTotalBets = new List<BetButtons>();

        private List<int> removeCount = new List<int>();

        private static int zero = 0;

        public Action onrightCLick;
        private void Awake()
        {
            instance=this;
        }       

        private void Start()
        {
            JeetoJokerManager.instance.ButtonHandlers.Add(this);
            JeetoJokerManager.instance.winHandlers.Add(this);

            ToggleChipVisibility(false);
            ToggleBlueRibbonVisibility(false);
            // TogglePlayTextVisibility(true);
            IRemoveHandler.refresh += Refresh;
        }

        public void ToggleChipVisibility(bool a_State)
        {
            chipImage.gameObject.SetActive(a_State);
            chipText.gameObject.SetActive(a_State);
        }

        public void ToggleBlueRibbonVisibility(bool a_State)
        {
            if (a_State)
            {
                playImage.sprite = JeetoJokerManager.instance.CardSelected;
                playText.gameObject.SetActive(false);
            }
            else
            {

                playText.gameObject.SetActive(true);
                playImage.sprite = JeetoJokerManager.instance.CardDiselect;
            }
        }

        // public void  TogglePlayTextVisibility(bool a_State)
        // {
        //     playText.gameObject.SetActive(a_State);
        // }

        private void UpdateChipVisualData(long a_Bet)
        {
            chipImage.sprite = CardDeck.instance.GetChipForValueRange(a_Bet);
            chipText.text = "" + Mathf.Abs(a_Bet).ToString("F0");
        }

        private long GetTotalBetSum()
        {
            long sum = zero;
            this.totalBets.ForEach(x => sum = x.amount + sum);
            return sum;
        }

        private long GetTotalGroupBetSum()
        {
            long sum = zero;
            groupCardSelect.ForEach(x => sum = sum + x.GetTotalBetSum());
            return 0;
            // return sum;
        }

        public override  void LeftClick(bool ignoreStack = false)
        {
            base.LeftClick();
            OnLeftClick();
        }

        public bool OnLeftClick()
        {
            BetButtons bet= JeetoJokerManager.instance.SelectedBetbutton;

            Debug.Log("clicked twice");

            totalBets.Add(bet);

            long totalBet = GetTotalBetSum();

            if (totalBet > 500) {

                totalBets.Remove(bet);
                return false;
            }
            if (!JeetoJokerManager.instance.Bet(bet.amount))
            {
                return false;
            }
            
            JeetoJoker.SoundController.Instance.PlayAudiojeetojoker(JeetoJoker.SoundController.Instance.clickSound);


            UpdateChipVisualData(totalBet);

            if (totalBets.Count == 1 || groupCardSelect.Find(x => x.TotalGrpBetsCount == 1) != null)
            {
                ToggleChipVisibility(true);
                ToggleBlueRibbonVisibility(true);
                // TogglePlayTextVisibility(false);
            }

            JeetoJokerManager.instance.CheckForRepeatButton();

            JeetoJokerManager.instance.removeHandlers.Add(new List<IRemoveHandler>() { this });
            removeCount.Add(JeetoJokerManager.instance.removeCount);
            JeetoJokerManager.instance.removeCount += 1;
            return true;
        }

        public override  void RightClick(bool ignoreStack = false)
        {
            base.RightClick();
            OnRightClick();
        }

        public void OnRightClick(BetButtons betArg=null, bool invokeRightChik=true)
        {
            JeetoJoker.SoundController.Instance.PlayAudiojeetojoker(JeetoJoker.SoundController.Instance.clickSound);
            // JeetoJoker.SoundController.Instance.PlayAudiojeetojoker(JeetoJoker.SoundController.Instance.unselectSound);
            Debug.Log(" entered uppr levl");
            if (!JeetoJokerManager.instance.canBet)
                return;

            Debug.Log(" crossed return");

            if (totalBets.Count > zero)
            {
                Debug.Log(" ttotal bet count >0");

                if (betArg != null)
                {
                    totalBets.Remove(betArg);
                    JeetoJokerManager.instance.RemoveBet(betArg.amount);

                }
                else
                {

                    BetButtons bet = totalBets[totalBets.Count - 1];
                    totalBets.RemoveAt(totalBets.Count - 1);
                    JeetoJokerManager.instance.RemoveBet(bet.amount);

                }


            long totalBet = GetTotalBetSum() + GetTotalGroupBetSum();
                UpdateChipVisualData(totalBet);
                if(invokeRightChik)
                onrightCLick?.Invoke();

                if (totalBets.Count == zero && groupCardSelect.TrueForAll(x => x.TotalGrpBetsCount == zero))
                {
                    Debug.Log(" ttotal bet count = 0");

                    ToggleChipVisibility(false);
                    ToggleBlueRibbonVisibility(false);
                    // TogglePlayTextVisibility(true);

                }

                JeetoJokerManager.instance.CheckForRepeatButton();

                if (removeCount.Count > zero)
                {
                    Debug.Log(" remove count > 0");

                    int currentIndex = removeCount[removeCount.Count - 1];
                    // Debug.LogError(currentIndex + "/"+ JeetoJokerManager.instance.removeHandlers.Count);
                    List<IRemoveHandler> n = JeetoJokerManager.instance.removeHandlers[currentIndex];
                    n.Remove(this);
                    removeCount.RemoveAt(removeCount.Count - 1);

                    if (n.Count == zero)
                    {

                        JeetoJokerManager.instance.removeHandlers.RemoveAt(currentIndex);
                        JeetoJokerManager.instance.removeCount -= 1;
                        IRemoveHandler.refresh?.Invoke(currentIndex);
                    }

                }
            }
        }

        private void Refresh(int index)
        {
            int count = removeCount.Count;
            for(int i = 0; i < count; i++)
            {
                if (removeCount[i] > index)
                    removeCount[i]--;
            }
        }

        public void OnClickGroupLeftClick(BetButtons bet=null)
        {
            Debug.Log("entered OnClickGroupLeftClick");
            long totalBet = GetTotalBetSum() + GetTotalGroupBetSum();
            if (bet != null) totalBets.Add(bet);
            UpdateChipVisualData(totalBet);

            // if (totalBets.Count == 1 || groupCardSelect.Find(x => x.TotalGrpBetsCount == 1) != null)
            // {
                ToggleChipVisibility(true);
                ToggleBlueRibbonVisibility(true);
                // TogglePlayTextVisibility(false);
            // }
            Debug.Log("exit OnClickGroupLeftClick");
        }

        public void OnClickGroupRightClick()
        {
            long totalBet = GetTotalBetSum() + GetTotalGroupBetSum();

            UpdateChipVisualData(totalBet);

            if (totalBets.Count == zero && groupCardSelect.TrueForAll(x => x.TotalGrpBetsCount == zero))
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
                BetButtons bet = totalBets[totalBets.Count - 1];
                totalBets.RemoveAt(totalBets.Count - 1);

                JeetoJokerManager.instance.RemoveBet(bet.amount);

                long totalBet = GetTotalBetSum() + GetTotalGroupBetSum();

                UpdateChipVisualData(totalBet);

                if (totalBets.Count == zero && groupCardSelect.TrueForAll(x => x.TotalGrpBetsCount == zero))
                {
                    ToggleChipVisibility(false);
                    ToggleBlueRibbonVisibility(false);
                    // TogglePlayTextVisibility(true);
                }

                JeetoJokerManager.instance.CheckForRepeatButton();
            }

            removeCount.RemoveAt(removeCount.Count - 1);
        }

        public void Repeat()
        {
            if (prevRoundTotalBets.Count == zero)
                return;
            Debug.Log("1");

            totalBets.Clear();
            Debug.Log("2");

            totalBets.AddRange(prevRoundTotalBets);
            Debug.Log("3");

            foreach (BetButtons bet in totalBets)
            {
                Debug.Log("4");
                int amt = bet.amount;

                JeetoJokerManager.instance.Bet(amt);
            }
            Debug.Log("5");

            long totalBet = GetTotalBetSum() + GetTotalGroupBetSum();

            UpdateChipVisualData(totalBet);
            Debug.Log("before chip-visiility repeat");
            ToggleChipVisibility(true);
            Debug.Log("after chip-visiility repeat");
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
            if(prevRoundTotalBets.Count > zero)
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

            foreach (BetButtons bet in totalBets)
            {
                int amt = bet.amount;

                JeetoJokerManager.instance.Bet(amt);
            }

            totalBets.AddRange(totalBets);

            long totalBet = GetTotalBetSum() + GetTotalGroupBetSum();

            UpdateChipVisualData(totalBet);

            ToggleChipVisibility(true);
            ToggleBlueRibbonVisibility(true);
            // TogglePlayTextVisibility(false);

            JeetoJokerManager.instance.removeHandlers[removeCountIndex].Add(this);
            removeCount.Add(JeetoJokerManager.instance.removeCount);
        }
        // public long totalbetdata;
        public long OnWin(int outerId, int innerId)
        {       
            Debug.Log("enter in show winamount function");
            Debug.Log("outer id"+outerId);  
              Debug.Log("outer id"+innerId);

            if (outerId == id && innerId == suiteID)
            {
                long totalBet = GetTotalBetSum();
                // totalbetdata=totalBet;
                Debug.Log("total bet for winpopup" + totalBet);
                Debug.Log("inside win data card select.....");
                // seq.AppendCallback(() => JeetoJokerManager.instance.ShowWinAmount(totalBet));

                /* for (int i = zero; i < 20; i++)
                 {
                     seq.AppendCallback(() => ToggleBlueRibbonVisibility(true));
                     seq.AppendInterval(0.2f);
                     seq.AppendCallback(() => ToggleBlueRibbonVisibility(false));
                     seq.AppendInterval(0.2f);
                 }*/

                    return totalBet;

            }
            else
                return 0;
        }

        public BetDataHandler GetBetData()
        {
            string _id=id.ToString();
            string _suit=suiteID.ToString();
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
            BetDataHandler betData = new BetDataHandler();
            betData.Id = _suit + _id;
            betData.amount=GetTotalBetSum()+GetTotalGroupBetSum();
            return betData;
        }

        public long GetTotalBetAmount()
        {
            return GetTotalBetSum() + GetTotalGroupBetSum();
        }
//changes by shivamfusion
         public void EmptytheImage()
        {
            Sprite transparentSprite = Resources.Load<Sprite>("TransparentImage");
            chipImage.sprite = transparentSprite;
            chipText.text = "Play";
        }
        
        public void PlaceBet(int betAmount)
        {


            BetButtons bet = new BetButtons();
            bet.amount = betAmount;

            totalBets.Add(bet);

            long totalBet = GetTotalBetSum() + GetTotalGroupBetSum();
            UpdateChipVisualData(totalBet);

            if (totalBets.Count == 1 || groupCardSelect.Find(x => x.TotalGrpBetsCount == 1) != null)
            {
                ToggleChipVisibility(true);
                ToggleBlueRibbonVisibility(true);
                // TogglePlayTextVisibility(false);
            }

            JeetoJokerManager.instance.CheckForRepeatButton();

            JeetoJokerManager.instance.removeHandlers.Add(new List<IRemoveHandler>() { this });
            removeCount.Add(JeetoJokerManager.instance.removeCount);
            JeetoJokerManager.instance.removeCount += 1;
        }
    }
}
