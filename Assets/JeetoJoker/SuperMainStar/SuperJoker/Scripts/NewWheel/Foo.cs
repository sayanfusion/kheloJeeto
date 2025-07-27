using khelojeetonew;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Foo : MonoBehaviour
{
    [SerializeField] internal SpinWheelnew outerSpinWheel, innerSpinWheel;
    [SerializeField] private int totalSlots = 0;

    public AudioSource audiostop;
    [SerializeField] private List<OuterWheelData> outerWheelDatas = new List<OuterWheelData>();
    [SerializeField] private List<InnerWheelData> innerWheelDatas = new List<InnerWheelData>();
    //public CenterWheelAnimator anim;
    public float timedifference = 15f;

    int t_OuterWheelSlotId;
    int t_InnerWheelSlotId;

    Action onAssignSlotId;
    [SerializeField] GameObject winHighlighter;

    [SerializeField] Transform scroll;
    int CARDID, SUITID;

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

        CARDID = cardID;
        SUITID = suitID;
        //int t_OuterWheelCardType = UnityEngine.Random.Range(0, outerWheelDatas.Count);
        //int t_InnerWheelCardType = UnityEngine.Random.Range(0, innerWheelDatas.Count);

        //t_OuterWheelSlotId = outerWheelDatas[t_OuterWheelCardType].Segments[UnityEngine.Random.Range(0, outerWheelDatas[t_OuterWheelCardType].Segments.Count)];
        //t_InnerWheelSlotId = innerWheelDatas[t_InnerWheelCardType].Segments[UnityEngine.Random.Range(0, innerWheelDatas[t_InnerWheelCardType].Segments.Count)];

        t_OuterWheelSlotId = outerWheelDatas[t_OuterWheelCardType].Segments[UnityEngine.Random.Range(0, outerWheelDatas[t_OuterWheelCardType].Segments.Count)];
        t_InnerWheelSlotId = innerWheelDatas[t_InnerWheelCardType].Segments[UnityEngine.Random.Range(0, innerWheelDatas[t_InnerWheelCardType].Segments.Count)];
        //Debug.Log($"OuterWheelSlotId: {t_OuterWheelSlotId}, InnerWheelSlotId: {t_InnerWheelSlotId}");
        //StartCoroutine(JeetoJokerManager.instance.LiveDataInsert());
        StartCoroutine(TimeDifference(CARDID, SUITID));


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

        int t_OuterWheelCardType = UnityEngine.Random.Range(0, outerWheelDatas.Count);
        int t_InnerWheelCardType = UnityEngine.Random.Range(0, innerWheelDatas.Count);
        t_OuterWheelSlotId = outerWheelDatas[t_OuterWheelCardType].Segments[UnityEngine.Random.Range(0, outerWheelDatas[t_OuterWheelCardType].Segments.Count)];
        t_InnerWheelSlotId = innerWheelDatas[t_InnerWheelCardType].Segments[UnityEngine.Random.Range(0, innerWheelDatas[t_InnerWheelCardType].Segments.Count)];

        Debug.Log($"OuterWheelSlotId: {t_OuterWheelSlotId}, InnerWheelSlotId: {t_InnerWheelSlotId}");
       

        outerSpinWheel.DirectlySetDestination(totalSlots, t_OuterWheelSlotId);
        innerSpinWheel.DirectlySetDestination(totalSlots, t_InnerWheelSlotId);
    }
    
    private IEnumerator TimeDifference(int cardID, int suitID)
    {
        Debug.Log(t_OuterWheelSlotId + " TimeDifference " + t_InnerWheelSlotId + " milan " + timedifference);
        outerSpinWheel.AssignWinningSlot(t_OuterWheelSlotId);
        //yield return new WaitForSeconds(timedifference);
        yield return new WaitForSeconds(1f);
        innerSpinWheel.AssignWinningSlot(t_InnerWheelSlotId);
        yield return new WaitForSeconds(1f);
        onAssignSlotId?.Invoke();
        yield return new WaitForSeconds(3);
        yield return JeetoJokerManager.instance.OnWin(cardID, suitID);
    }

  

    public void spinWheel()
    {
        winHighlighter.SetActive(false);
        outerSpinWheel.audioSourcerunning.Play();
        innerSpinWheel.audioSourcerunning.Stop();
        JeetoJokerManager.instance.HideMultiplierText();
        scrollAnimationStart();
        outerSpinWheel.SpinTheWheel(totalSlots, onOuterWheelSpinComplete);
        innerSpinWheel.SpinTheWheel(totalSlots, onInnerWheelSpinComplete);
    }

    private void onOuterWheelSpinComplete(int a_WinningSlot)
    {
        Debug.Log("first");
        outerSpinWheel.audioSourcerunning.Stop();
        ScrollAnimationStop();

        //Debug.Log($"Outer Winning Slot: {a_WinningSlot}");
        outerSpinWheel.WheelSpinStoppped();       
       // anim.A();
    }

    private void onInnerWheelSpinComplete(int a_WinningSlot)
    {
        // audiostop.Play();
        //Debug.Log($"Inner Winning Slot: {a_WinningSlot}");
        winHighlighter.SetActive(true);
 

        StartCoroutine(JeetoJokerManager.instance.SetCardImage(CARDID, SUITID));
        innerSpinWheel.WheelSpinStoppped();

        //StartCoroutine(JeetoJokerManager.instance.startSpinWheelWithData());
        //innerSpinWheel.StopInnerWheel();       
        //LevelManager.inst.SetStopEffects();
        //anim.B();
    }

    void scrollAnimationStart()
    {
        scroll.DOLocalMoveX(-4.7f, 2.5f).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);

    }

    void ScrollAnimationStop() {

        DOTween.Kill(scroll);
        scroll.transform.localPosition = new Vector3(6.5f, 0, 0);
    }
    public void restarttt()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

[System.Serializable]
public class OuterWheelData
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
public class InnerWheelData
{
    public enum ECardType
    {
        None = 0,
        S1, S2, S3, S4,
    }

    public ECardType CardType = ECardType.None;
    public List<int> Segments = new List<int>();
}