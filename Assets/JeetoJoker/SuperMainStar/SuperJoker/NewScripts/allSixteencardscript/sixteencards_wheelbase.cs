using khelojeetonew;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sixteencards_wheelbase : MonoBehaviour
{
    [SerializeField] private Sixteencards_spinwheel outerSpinWheel, innerSpinWheel;
    [SerializeField] private int totalSlots = 0;

    public AudioSource audiostop;
    [SerializeField] private List<OuterWheelData1> outerWheelDatas1 = new List<OuterWheelData1>();
    [SerializeField] private List<InnerWheelData1> innerWheelDatas1 = new List<InnerWheelData1>();
    //public CenterWheelAnimator anim;
    public float timedifference;

    int t_OuterWheelSlotId;
    int t_InnerWheelSlotId;

    Action onAssignSlotId;

    //public void StartSpinningWithData(int cardID, int suitID)
    //{
    //    if (cardID == 0)
    //    {
    //        cardID = 1;
    //    }
    //    if (suitID == 0)
    //    {
    //        suitID = 1;
    //    }
    //    int t_OuterWheelCardType = cardID - 1;
    //    int t_InnerWheelCardType = suitID - 1;

    //    t_OuterWheelSlotId = outerWheelDatas[t_OuterWheelCardType].Segments[Random.Range(0, outerWheelDatas[t_OuterWheelCardType].Segments.Count)];
    //    t_InnerWheelSlotId = innerWheelDatas[t_InnerWheelCardType].Segments[Random.Range(0, innerWheelDatas[t_InnerWheelCardType].Segments.Count)];

    //    StartCoroutine(timeDifferenceWithData(t_OuterWheelSlotId, t_InnerWheelSlotId));
    //}

    //private IEnumerator timeDifferenceWithData(int a_OuterWheelSlotId, int a_InnerWheelSlotId)
    //{
    //    outerSpinWheel.Initialize(totalSlots, a_OuterWheelSlotId, onOuterWheelSpinComplete);
    //    yield return new WaitForSeconds(timedifference);
    //    innerSpinWheel.Initialize(totalSlots, a_InnerWheelSlotId, onInnerWheelSpinComplete);
    //}

    private void Start()
    {
        
    }

    public void faaa(int cardID, int suitID, Action a_OnAssignSlotId)
    {
        Debug.Log($"CardId: {cardID}, SuiteId: {suitID}");
        onAssignSlotId = a_OnAssignSlotId;

        if (cardID == 0)
        {
            cardID = 1;
        }
        if (suitID == 0)
        {
            suitID = 1;
        }
        int t_OuterWheelCardType = cardID - 1;
        int t_InnerWheelCardType = suitID - 1;

        //int t_OuterWheelCardType = UnityEngine.Random.Range(0, outerWheelDatas.Count);
        //int t_InnerWheelCardType = UnityEngine.Random.Range(0, innerWheelDatas.Count);

        //t_OuterWheelSlotId = outerWheelDatas[t_OuterWheelCardType].Segments[UnityEngine.Random.Range(0, outerWheelDatas[t_OuterWheelCardType].Segments.Count)];
        //t_InnerWheelSlotId = innerWheelDatas[t_InnerWheelCardType].Segments[UnityEngine.Random.Range(0, innerWheelDatas[t_InnerWheelCardType].Segments.Count)];

        t_OuterWheelSlotId = outerWheelDatas1[t_OuterWheelCardType].Segments[UnityEngine.Random.Range(0, outerWheelDatas1[t_OuterWheelCardType].Segments.Count)];
        t_InnerWheelSlotId = innerWheelDatas1[t_InnerWheelCardType].Segments[UnityEngine.Random.Range(0, innerWheelDatas1[t_InnerWheelCardType].Segments.Count)];
        //Debug.Log($"OuterWheelSlotId: {t_OuterWheelSlotId}, InnerWheelSlotId: {t_InnerWheelSlotId}");

        StartCoroutine(TimeDifference(cardID, suitID));
    }

    public void SetDirectDestination(int cardID, int suitID)
    {
        Debug.Log($"CardId: {cardID}, SuiteId: {suitID}");
        if (cardID == 0)
        {
            cardID = 1;
        }
        if (suitID == 0)
        {
            suitID = 1;
        }
        //int t_OuterWheelCardType = cardID - 1;
        //int t_InnerWheelCardType = suitID - 1;
        int t_OuterWheelCardType = UnityEngine.Random.Range(0, outerWheelDatas1.Count);
        int t_InnerWheelCardType = UnityEngine.Random.Range(0, innerWheelDatas1.Count);
        t_OuterWheelSlotId = outerWheelDatas1[t_OuterWheelCardType].Segments[UnityEngine.Random.Range(0, outerWheelDatas1[t_OuterWheelCardType].Segments.Count)];
        t_InnerWheelSlotId = innerWheelDatas1[t_InnerWheelCardType].Segments[UnityEngine.Random.Range(0, innerWheelDatas1[t_InnerWheelCardType].Segments.Count)];

        Debug.Log($"OuterWheelSlotId: {t_OuterWheelSlotId}, InnerWheelSlotId: {t_InnerWheelSlotId}");

        outerSpinWheel.DirectlySetDestination(totalSlots, t_OuterWheelSlotId);
        innerSpinWheel.DirectlySetDestination(totalSlots, t_InnerWheelSlotId);
    }

    private IEnumerator TimeDifference(int cardID, int suitID)
    {
        outerSpinWheel.AssignWinningSlot(t_OuterWheelSlotId);
        yield return new WaitForSeconds(timedifference);
        innerSpinWheel.AssignWinningSlot(t_InnerWheelSlotId);
        StartCoroutine(WaitForWinShow(cardID, suitID));
    }

    private IEnumerator WaitForWinShow(int cardID, int suitID)
    {

        yield return new WaitForSeconds(1f);
        // onAssignSlotId?.Invoke();
       Sixteen_cards.instance.OnWin(cardID, suitID);
        yield return new WaitForSeconds(2f);
        onAssignSlotId?.Invoke();
    }

    public void spinWheel()
    {
        outerSpinWheel.SpinTheWheel(totalSlots, onOuterWheelSpinComplete);
        innerSpinWheel.SpinTheWheel(totalSlots, onInnerWheelSpinComplete);
    }
    public void Freeze()
    {
        outerSpinWheel.FreezeWheel();
        innerSpinWheel.FreezeWheel();
    }
    private void onOuterWheelSpinComplete(int a_WinningSlot)
    {
        Debug.Log("first");

        //Debug.Log($"Outer Winning Slot: {a_WinningSlot}");
        outerSpinWheel.WheelSpinStoppped();
        // anim.A();
    }

    private void onInnerWheelSpinComplete(int a_WinningSlot)
    {
        // audiostop.Play();
        //Debug.Log($"Inner Winning Slot: {a_WinningSlot}");
        innerSpinWheel.WheelSpinStoppped();
        Debug.Log("final");
        //LevelManager.inst.SetStopEffects();
        //anim.B();
    }

    public void restarttt()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

[System.Serializable]
public class OuterWheelData1
{
    public enum ECardType
    {
        None = 0,
        C1, C2, C3, C4
    }

    public ECardType CardType = ECardType.None;
    public List<int> Segments = new List<int>();
}

[System.Serializable]
public class InnerWheelData1
{
    public enum ECardType
    {
        None = 0,
        S1, S2, S3, S4,
    }

    public ECardType CardType = ECardType.None;
    public List<int> Segments = new List<int>();
}
