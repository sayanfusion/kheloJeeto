using DevCommon.Utils;
using khelojeetonew;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace khelojeetonew
{
    public class sixteencards_groupcardselector : InputDetection, ButtonHandlerss, IRemoveHandlers
    {

        [SerializeField] private Image chipImage;
        [SerializeField] private Text chipText;
        // [SerializeField] private Text playText;

        [SerializeField] private List<sixteencard_CardSelect> sixteencard_CardSelect;

        private List<BetButtonss> totalBets = new List<BetButtonss>();

        public int TotalGrpBetsCount { get => totalBets.Count; }

        private List<BetButtonss> prevRoundTotalBets = new List<BetButtonss>();

        private List<int> removeCount = new List<int>();

        private void Start()
        {
            sixteencard_CardSelect.ForEach(x => x.groupCardSelects.Add(this));

           Sixteen_cards.instance.ButtonHandlerss.Add(this);

            ToggleChipVisibility(false);
            // // TogglePlayTextVisibility(true);
            IRemoveHandlers.refresh += Refresh;
        }

        private void UpdateChipVisualData(long a_Bet)
        {
            chipImage.sprite = sixteencards_CardDeck.instance.GetChipForValueRange(a_Bet);
            chipText.text = "" + Mathf.Abs(a_Bet).ToString("F0");
        }

        // private void TogglePlayTextVisibility(bool a_State)
        // {
        //     playText.gameObject.SetActive(a_State);
        // }

        private void ToggleChipVisibility(bool a_State)
        {
            chipImage.gameObject.SetActive(a_State);
            chipText.gameObject.SetActive(a_State);
        }

        public long GetTotalBetSum()
        {
            long sum = 0;
            totalBets.ForEach(x => sum = x.amount + sum);
            return sum;
        }

        public override void LeftClick(bool ignoreStack = false)
        {
            base.LeftClick();

            BetButtonss bets = Sixteen_cards.instance.SelectedBetbuttons;
            int amt = bets.amount * sixteencard_CardSelect.Count;

            if (!Sixteen_cards.instance.Bet(amt))
            {
                return;
            }

            totalBets.Add(bets);

            long totalBet = GetTotalBetSum();

            UpdateChipVisualData(totalBet);

            if (totalBets.Count == 1)
            {
                ToggleChipVisibility(true);
                // TogglePlayTextVisibility(false);
            }

            sixteencard_CardSelect.ForEach(x => x.OnClickGroupLeftClick());

            Sixteen_cards.instance.CheckForRepeatButton();

            Sixteen_cards.instance.removeHandlerss.Add(new List<IRemoveHandlers>() { this });
            removeCount.Add(Sixteen_cards.instance.removeCount);
            Sixteen_cards.instance.removeCount += 1;
        }

        public override  void RightClick(bool ignoreStack = false)
        {
            base.RightClick();
            if (!Sixteen_cards.instance.canBet)
                return;

            if (totalBets.Count > 0)
            {
                BetButtonss bet = totalBets[totalBets.Count - 1];
                totalBets.RemoveAt(totalBets.Count - 1);

                int amt = bet.amount * sixteencard_CardSelect.Count;

                Sixteen_cards.instance.RemoveBet(amt);

                long totalBet = GetTotalBetSum();

                UpdateChipVisualData(totalBet);

                if (totalBets.Count == 0)
                {
                    ToggleChipVisibility(false);
                    // TogglePlayTextVisibility(true);
                }

                sixteencard_CardSelect.ForEach(x => x.OnClickGroupRightClick());

               Sixteen_cards.instance.CheckForRepeatButton();

                if (removeCount.Count > 0)
                {
                    int currentIndex = removeCount[removeCount.Count - 1];
                    List<IRemoveHandlers> n = Sixteen_cards.instance.removeHandlerss[currentIndex];
                    n.Remove(this);
                    removeCount.RemoveAt(removeCount.Count - 1);
                    if (n.Count == 0)
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

        public void Clear()
        {
            totalBets.Clear();
            removeCount.Clear();

            ToggleChipVisibility(false);
            // TogglePlayTextVisibility(true);
        }

        public void Remove()
        {
            base.RightClick();

            if (totalBets.Count > 0)
            {
                BetButtonss bet = totalBets[totalBets.Count - 1];
                totalBets.RemoveAt(totalBets.Count - 1);

                int amt = bet.amount * sixteencard_CardSelect.Count;

               Sixteen_cards.instance.RemoveBet(amt);

                long totalBet = GetTotalBetSum();

                UpdateChipVisualData(totalBet);

                if (totalBets.Count == 0)
                {
                    ToggleChipVisibility(false);
                    // TogglePlayTextVisibility(true);
                }

                sixteencard_CardSelect.ForEach(x => x.OnClickGroupRightClick());

               Sixteen_cards.instance.CheckForRepeatButton();
            }
            removeCount.RemoveAt(removeCount.Count - 1);
        }

        public void Repeat()
        {
            if (prevRoundTotalBets.Count == 0)
                return;

            totalBets.Clear();

            totalBets.AddRange(prevRoundTotalBets);

            foreach (BetButtonss bet in totalBets)
            {
                int amt = bet.amount * sixteencard_CardSelect.Count;

                Sixteen_cards.instance.Bet(amt);
            }

            long totalBet = GetTotalBetSum();

            UpdateChipVisualData(totalBet);

            ToggleChipVisibility(true);
            // TogglePlayTextVisibility(false);

            sixteencard_CardSelect.ForEach(x => x.OnClickGroupLeftClick());
        }

        public void SavePrevRound()
        {
            prevRoundTotalBets.Clear();
            if (totalBets.Count > 0)
            {
                prevRoundTotalBets.AddRange(totalBets);
            }
        }

        public bool CheckifLastSavedDataAvailable()
        {
            if (prevRoundTotalBets.Count > 0)
            {
                return true;
            }

            return false;
        }

        public long GetPrevRoundTotalSum()
        {
            long sum = 0;
            prevRoundTotalBets.ForEach(x => sum = sum + x.amount);
            return sum;
        }

        public long GetRoundTotalSum()
        {
            long sum = 0;
            totalBets.ForEach(x => sum = sum + x.amount);
            return sum;
        }

        public void DoubleUp(int removeCountIndex)
        {
            if (totalBets.Count == 0)
                return;

            foreach (BetButtonss bet in totalBets)
            {
                int amt = bet.amount * sixteencard_CardSelect.Count;

                Sixteen_cards.instance.Bet(amt);
            }

            totalBets.AddRange(totalBets);

            long totalBet = GetTotalBetSum();

            UpdateChipVisualData(totalBet);

            ToggleChipVisibility(true);
            // TogglePlayTextVisibility(false);

            sixteencard_CardSelect.ForEach(x => x.OnClickGroupLeftClick());

            Sixteen_cards.instance.removeHandlerss[removeCountIndex].Add(this);
            removeCount.Add(Sixteen_cards.instance.removeCount);
        }
         public void NewToggleChipVisibility(bool a_State)
        {
            chipImage.gameObject.SetActive(a_State);
            chipText.text = "Play";
        }
    }

}
