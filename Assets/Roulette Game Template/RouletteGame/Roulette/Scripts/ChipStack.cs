using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using TMPro;

public class ChipStack : MonoBehaviour
{

    public static ChipStack Instance;
    public static readonly int[] CHIP_VALUES = new int[] { 1, 2, 5, 10, 50, 100, 500, 1000, 5000 };
    public static readonly Vector3 CollectPosition = new Vector3(0, 0, -3);

    public GameObject newChip;
    // [SerializeField] private List<int> slotValues;

    private Vector3 initialPosition;
    private int value = 0;
    private int DoubleValue = 0;
    [SerializeField] public List<GameObject> chips;

    void Start()
    {
        Instance = this;
        initialPosition = transform.position;
    }

    public void SetInitialPosition(Vector3 pos)
    {
        transform.position = pos;
        initialPosition = pos;
    }

    public void Add(int id ,int value)
    {
        //SetValue(this.value*2);
        SetValue(id, this.value + value);
    }
    public void Add2(int id)
    {
        //SetValue(this.value*2);
        SetValue(id, this.value*2);
    }
    public void Remove( int id, int value)
    {
        //SetValue(this.value - value);
        SetValue(id,value);
    }

    public int Clear()
    {

        int lastBet = value;
        value = 0;
        transform.position = initialPosition;
        //Debug.Log("Clear " + chips != null);

        if (chips != null)
        {
            //Debug.Log("des");
            foreach (GameObject chip in chips)
            {
                //Debug.Log("foreach");
                Destroy(chip);
            }
        }
        chips = null;
        return lastBet;
    }



    public int GetValue()
    {
        return value;
    }
    int additionvalue = 0;


    public void SetValue(int id,int value)
    {
        //Debug.LogError("Amount " + value);
        int lastBetValue = value;
        Clear();
        if (value <= 0)
        {
            return;
        }

        this.value = value;
        chips = new List<GameObject>();

        int currentChipIndex = CHIP_VALUES.Length - 1;

        while (value > 0)
        {
            int nextValue = value - CHIP_VALUES[currentChipIndex];

            if (nextValue < 0)
            {
                currentChipIndex--;
                if (currentChipIndex < 0)
                {
                    throw new Exception("Impossible value");
                }
                continue;
            }
            //Debug.Log("out if Value: " + value);

            value = nextValue;

            //if (BetSpace.isDoublePressed == false)
            //    {
            //Debug.LogError("currentChipIndex " + currentChipIndex);
            newChip = ChipManager.InstantiateChip(currentChipIndex);

            newChip.transform.parent = this.gameObject.transform;
            Debug.LogError("chips Parent name----------------------------------- " + this.gameObject.transform.parent.name);
            newChip.transform.localPosition = new Vector3(0, .0025f * (chips.Count + 1), 0);
            chips.Add(newChip);
            additionvalue += newChip.GetComponent<GetValue>().value;
            newChip.GetComponent<GetValue>().textValue.GetComponent<TextMeshPro>().text = lastBetValue.ToString();
            //Debug.LogError("transform name "+newChip.transform.name);
            Debug.LogError("newChip.GetComponent<GetValue>().textValue.GetComponent<TextMeshPro>().text " + newChip.GetComponent<GetValue>().textValue.GetComponent<TextMeshPro>().text);
            SceneRoulette._Instance._AmeWheel.SetValue(id,lastBetValue);
            // Debug.Log("inner if Value: " + BetSpace.isDoublePressed);

            //slotValues.Insert(0, newChip.GetComponent<GetValue>().value);
            //if (BetSpace.isDoublePressed == true)
            //{
            //    newChip.transform.localPosition=Vector3.
            //}


            //Debug.Log("additional value"+ BetSpace.chipValue.ToString());


            //}
        }
        //Debug.LogError("additionvalue " + additionvalue);
    }

    public int Win(int multiplier)
    {
        int winAmount=0;
        if (BetSpace.isPressing == true)
        {
            winAmount = (value * multiplier) * BetSpace.counter;
            Debug.Log($"when true value {value}  multiplier {multiplier} BetSpace.counter {BetSpace.counter} winAmount {winAmount}");
        }
        else
        {
            winAmount = value * multiplier;
            Debug.Log($"when false value {value}  multiplier {multiplier} winAmount {winAmount}");
        }
        SetValue(multiplier, winAmount);

        if (winAmount > 0)
        {
            CollectChips();
        }
        value = 0;
        return winAmount;
    }

    public void CollectChips()
    {
        transform.DOMove(CollectPosition, 1).SetEase(Ease.InSine).SetDelay(1.5f).OnComplete(() => { Clear(); });
    }

    public void SetPositionNull()
    {
        gameObject.transform.position = new Vector3(0, 0, 0);
    }
}
