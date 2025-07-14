using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultDate : MonoBehaviour
{
    public TextMeshProUGUI _single, _double, _triple;
    public void SetResultdata(string sin,string dou,string tri) 
    {
        _single.text = sin;
        _double.text = dou;
        _triple.text = tri;
    }
}
