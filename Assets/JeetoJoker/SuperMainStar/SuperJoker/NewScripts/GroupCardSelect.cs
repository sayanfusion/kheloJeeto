using khelojeetonew;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace khelojeetonew
{
    public class GroupCardSelect : InputDetection, ButtonHandlers, IRemoveHandler
    {
        [SerializeField] private Image chipImage;
        [SerializeField] private Text chipText;
        [SerializeField] internal GameObject playText;

        [SerializeField] private List<CardSelect> cardSelects;

        private List<BetButtons> totalBets = new List<BetButtons>();

        public int TotalGrpBetsCount { get => totalBets.Count; }

        private List<BetButtons> prevRoundTotalBets = new List<BetButtons>();

        private List<int> removeCount = new List<int>();

        private void Start()
        {
            cardSelects.ForEach(x => x.groupCardSelect.Add(this));

            JeetoJokerManager.instance.ButtonHandlers.Add(this);

            ToggleChipVisibility(false);
            // // TogglePlayTextVisibility(true);
            IRemoveHandler.refresh += Refresh;
        }

        private void UpdateChipVisualData(long a_Bet)
        {
            chipImage.sprite = CardDeck.instance.GetChipForValueRange(a_Bet);
            chipText.text = "" + Mathf.Abs(a_Bet).ToString("F0");
        }

        // // private void TogglePlayTextVisibility(bool a_State)
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
            playText.SetActive(false);
            BetButtons bet = JeetoJokerManager.instance.SelectedBetbutton;
            int amt = bet.amount * cardSelects.Count;

            if (!JeetoJokerManager.instance.Bet(amt))
            {
                return;
            }

            totalBets.Add(bet);

            long totalBet = GetTotalBetSum();

            UpdateChipVisualData(totalBet);

            if (totalBets.Count == 1)
            {
                ToggleChipVisibility(true);
                // TogglePlayTextVisibility(false);
            }

            cardSelects.ForEach(x => x.OnClickGroupLeftClick());

            JeetoJokerManager.instance.CheckForRepeatButton();

            JeetoJokerManager.instance.removeHandlers.Add(new List<IRemoveHandler>() { this });
            removeCount.Add(JeetoJokerManager.instance.removeCount);
            JeetoJokerManager.instance.removeCount += 1;
        }

        public override void RightClick(bool ignoreStack = false)
        {
            base.RightClick();

            if (!JeetoJokerManager.instance.canBet)
                return;

            if (totalBets.Count > 0)
            {
                BetButtons bet = totalBets[totalBets.Count - 1];
                totalBets.RemoveAt(totalBets.Count - 1);

                int amt = bet.amount * cardSelects.Count;

                JeetoJokerManager.instance.RemoveBet(amt);

                long totalBet = GetTotalBetSum();

                UpdateChipVisualData(totalBet);

                if (totalBets.Count == 0)
                {
                    ToggleChipVisibility(false);
                    playText.SetActive(true);

                    // TogglePlayTextVisibility(true);
                }

                cardSelects.ForEach(x => x.OnClickGroupRightClick());

                JeetoJokerManager.instance.CheckForRepeatButton();

                if (removeCount.Count > 0)
                {
                    int currentIndex = removeCount[removeCount.Count - 1];
                    List<IRemoveHandler> n = JeetoJokerManager.instance.removeHandlers[currentIndex];
                    n.Remove(this);
                    removeCount.RemoveAt(removeCount.Count - 1);
                    if (n.Count == 0)
                    {
                        JeetoJokerManager.instance.removeHandlers.RemoveAt(currentIndex);
                        JeetoJokerManager.instance.removeCount -= 1;
                        IRemoveHandler.refresh?.Invoke(currentIndex);
                    }


                }
            }
            else { 
            playText.SetActive(true);


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
            playText.SetActive(true);
            ToggleChipVisibility(false);
            // TogglePlayTextVisibility(true);
        }

        public void Remove()
        {
            base.RightClick();

            if (totalBets.Count > 0)
            {
                BetButtons bet = totalBets[totalBets.Count - 1];
                totalBets.RemoveAt(totalBets.Count - 1);

                int amt = bet.amount * cardSelects.Count;

                JeetoJokerManager.instance.RemoveBet(amt);

                long totalBet = GetTotalBetSum();

                UpdateChipVisualData(totalBet);

                if (totalBets.Count == 0)
                {
                    ToggleChipVisibility(false);
                    playText.SetActive(true);

                    // TogglePlayTextVisibility(true);
                }

                cardSelects.ForEach(x => x.OnClickGroupRightClick());

                JeetoJokerManager.instance.CheckForRepeatButton();
            }
            removeCount.RemoveAt(removeCount.Count - 1);
        }

        public void Repeat()
        {
            if (prevRoundTotalBets.Count == 0)
                return;
            Debug.Log("1");

            totalBets.Clear();
            Debug.Log("2");

            totalBets.AddRange(prevRoundTotalBets);
            Debug.Log("3");

            foreach (BetButtons bet in totalBets)
            {
                Debug.Log("4");
                int amt = bet.amount * cardSelects.Count;

                JeetoJokerManager.instance.Bet(amt);
            }
                Debug.Log("5");

            long totalBet = GetTotalBetSum();

            UpdateChipVisualData(totalBet);
            Debug.Log("before chip-visiility repeat");
            ToggleChipVisibility(true);
            Debug.Log("after chip-visiility repeat");
            // TogglePlayTextVisibility(false);
            StartCoroutine(RepeatCoroutine());
            cardSelects.ForEach(x => x.OnClickGroupLeftClick());

        }

        private IEnumerator RepeatCoroutine()
        {

            yield return new  WaitForSeconds(0.1f);

            ToggleChipVisibility(true);
            // ToggleBlueRibbonVisibility(true);
            // TogglePlayTextVisibility(false);

        }

        public void SavePrevRound()
        {
            prevRoundTotalBets.Clear();
            if (totalBets.Count > 0)
            {
                Debug.LogError("totalBets " + totalBets.Count);
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

            foreach (BetButtons bet in totalBets)
            {
                int amt = bet.amount * cardSelects.Count;

                JeetoJokerManager.instance.Bet(amt);
            }

            totalBets.AddRange(totalBets);

            long totalBet = GetTotalBetSum();

            UpdateChipVisualData(totalBet);

            ToggleChipVisibility(true);
            // TogglePlayTextVisibility(false);

            cardSelects.ForEach(x => x.OnClickGroupLeftClick());

            JeetoJokerManager.instance.removeHandlers[removeCountIndex].Add(this);
            removeCount.Add(JeetoJokerManager.instance.removeCount);
        }
         public void NewToggleChipVisibility(bool a_State)
        {
            chipImage.gameObject.SetActive(a_State);
            chipText.text = "Play";
        }
    }
}
