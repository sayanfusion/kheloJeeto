using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HistoryData : MonoBehaviour
{
    [SerializeField] private TMP_Text betNo_Text, result_Text, betAmount_Text, winingAmount_Text;
    

    public void InitData(string betNo, string result,string betAmount, string winingAmount)
    {
        betNo_Text.text = betNo;
        result_Text.text = result;
        betAmount_Text.text = betAmount;
        winingAmount_Text.text = winingAmount;
    }
}
