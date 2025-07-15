using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUp : MonoBehaviour {

    public enum ERROR
    {
        Insufficient_Balance,
        Maximum_Bet
    }
    public Image iPopUp;
    public RectTransform tAllTexts;

    public TMP_Text number;
    public TMP_Text betAmount;
    public TMP_Text totalPay;
    public TMP_Text winText;
    public TMP_Text redPopUp;


    public Sprite sNormal;
    public Sprite sInsufficient;


    void DisableAllTexts()
    {
        for (int i = 0; i < 3; i++)
        {
            tAllTexts.GetChild(i).gameObject.SetActive(false);
        }
    }

    void SetBlockPosition(Block.BlockType _blockType)
    {
        Vector3 pos = tAllTexts.anchoredPosition;
        pos.x = Mathf.Abs(pos.x);

        if (_blockType == Block.BlockType.TRIPLE)
        {
            pos.x = -Mathf.Abs(pos.x);
            iPopUp.transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            iPopUp.transform.localScale = new Vector3(1, 1, 1);

        }

        tAllTexts.anchoredPosition = pos;
    }

    public void SetBlockData(Block.BlockType _blockType, string _number, int _betAmount)
    {
        DisableAllTexts();
        tAllTexts.GetChild(0).gameObject.SetActive(true);
        iPopUp.sprite = sNormal;
        number.text = _number;
        betAmount.text = _betAmount.ToString();
        //Debug.LogError("--"+ Constant.GetWinValue(_blockType, _number, _betAmount));
        float _winValue = Constant.GetWinValue(_blockType, _number, _betAmount);

        winText.text = _winValue.ToString();

        SetBlockPosition(_blockType);
    }

    

    public void SetBlockDataForRow(Block.BlockType _blockType, string _betAmount)
    {
        DisableAllTexts();
        tAllTexts.GetChild(2).gameObject.SetActive(true);
        iPopUp.sprite = sNormal;
        totalPay.text = _betAmount;

        SetBlockPosition(_blockType);
    }

    public void ShowInsufficientPopUp(Block.BlockType _blockType, ERROR _error)
    {
        DisableAllTexts();
        tAllTexts.GetChild(1).gameObject.SetActive(true);
        iPopUp.sprite = sInsufficient;

        SetBlockPosition(_blockType);

        if(_error == ERROR.Insufficient_Balance)
            redPopUp.text = "Insufficient Balance";
        else
            redPopUp.text = "Limit Exceed";
    }
}
