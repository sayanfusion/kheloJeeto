using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SimpleJSON;

public class UIHistoryContent1 : MonoBehaviour
{
    [SerializeField] private Text txtSNo;
    [SerializeField] private Text txtHandId;
    [SerializeField] private Text txtResult;
    [SerializeField] private Text txtPlay;


    public void Bind(int index, JSONNode historyData)
    {

        txtSNo.text = index.ToString();
        //txtSNo.text = historyData["game_id"].ToString().Substring(historyData["game_id"].ToString().Length - 5, 4);
        //txtHandId.text = historyData["win_number"].ToString();
        txtHandId.text = historyData["game_id"].ToString().Substring(historyData["game_id"].ToString().Length - 5, 4);
        txtResult.text = historyData["bet_ammount"].ToString();
        txtPlay.text = historyData["win_ammount"].ToString();

        /*txtSNo.text = historyData.game_id.ToString().Substring(Mathf.Max(0, historyData.game_id.ToString().Length - 4));
        txtHandId.text = historyData.win_number;
        txtResult.text = historyData.bet_ammount;
        txtPlay.text = historyData.win_ammount;*/

    }
}
