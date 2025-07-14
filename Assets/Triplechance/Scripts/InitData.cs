using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitData : MonoBehaviour
{
    public List<Text> Datas;

    public void Initialized(string slno,string gameid,string played,string won)
    {
        Datas[0].text = slno;
        Datas[1].text = gameid;
        Datas[2].text = played;
        Datas[3].text = won;
    }
}
