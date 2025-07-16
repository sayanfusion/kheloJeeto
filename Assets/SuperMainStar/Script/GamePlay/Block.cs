using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class Block : InputDetection
{

    public enum BlockType
    {
        SINGLE,
        DOUBLE,
        TRIPLE
    }

    public enum BlockState
    {
        NORMAL,
        CLICKED
    }
    public BlockType blockType;
    public BlockState blockState;
    public int iBetAmount;
    public int limit;

    public GameObject gNormalButton;
    public GameObject gClickedButton;
    public GameObject winState;
    public TMP_Text winStateNum;
    public TMP_Text winStateBet;
    public TMP_Text normalStateNum;
    public TMP_Text clickedStateNum;
    public TMP_Text betAmount;

    public Sprite sHighliteSprite;
    public Sprite sPressedSprite;
    public Sprite sResultState;
    public Sprite sWinSprite;

    public GameObject winEffect;
    public GameObject instantiatedWinEffectObject;

    public KeyCode alphaKeyCode;
    public KeyCode keyBoardKeyCode;


    Image image;
    Sprite sDefaultSprite;


    #region All Delegates

    public delegate void OnSelectBlock(Transform transform);
    public static OnSelectBlock onSelectBlock;

    public delegate void OnDeSelectBlock(Block block);
    public static OnDeSelectBlock onDeSelectBlock;

    public delegate void OnAddBlock(Transform transform);
    public static OnAddBlock onAddBlock;

    public delegate void OnSelectTripleBlock(Block _block);
    public static OnSelectTripleBlock onSelectTripleBlock;

    #endregion
    private void Awake()
    {
        image = transform.GetChild(0).GetComponent<Image>();
        sDefaultSprite = image.sprite;
        if (this.blockType == BlockType.TRIPLE)
        {
            limit = 5000;
        }
        else if (this.blockType == BlockType.DOUBLE)
        {
            limit = 30000;
        }
        else
        {
            limit = 50000;
        }
    }


    private void OnEnable()
    {
        GamePlay.onResultSuccess += OnDecideResult;
    }

    private void OnDisable()
    {
        GamePlay.onResultSuccess -= OnDecideResult;
    }
    private void Update()
    {
        if (Input.GetKeyDown(alphaKeyCode) || Input.GetKeyDown(keyBoardKeyCode))
        {
            LeftClick();
        }

    }


    void ResetData()
    {
        iBetAmount = 0;
    }

    public void UpdateText(string number)
    {
        normalStateNum.text = number;
        clickedStateNum.text = number;

        iBetAmount = 0;
        image.sprite = sDefaultSprite;
        blockState = BlockState.NORMAL;

        gNormalButton.SetActive(true);
        gClickedButton.SetActive(false);
    }

    public void OnAddingBlock()
    {
        if (onAddBlock != null)
        {
            onAddBlock(this.transform);
        }
    }

    public void OnSelectSuccess(int _iBetAmount)
    {
        Debug.Log("IbetAmount : " + iBetAmount);
        // set button to clicked one
        if (_iBetAmount <= limit)
        {
            iBetAmount = _iBetAmount;
            image.sprite = sPressedSprite;
            blockState = BlockState.CLICKED;

            gNormalButton.SetActive(false);
            clickedStateNum.text = normalStateNum.text;
            gClickedButton.SetActive(true);

            betAmount.text = iBetAmount.ToString();
            GamePlay.instance.OnBetData(blockType, iBetAmount, gameObject.name);
            if (blockType == BlockType.TRIPLE && onSelectTripleBlock != null)
            {
                onSelectTripleBlock(this);
            }
        }
    }

    public override void LeftClick()
    {
        base.LeftClick();
        // Audio_Manager.instance.PlayAudio(Audio_Manager.instance.clickSound);

        if (onSelectBlock != null)
        {
            onSelectBlock(this.transform);
        }
    }

    public override void RightClick()
    {
        base.RightClick();
        //  Audio_Manager.instance.PlayAudio(Audio_Manager.instance.unselectSound);

        if (onDeSelectBlock != null)
        {
            Debug.Log("Block Right Click");
            onDeSelectBlock(this);
        }
    }

    public void OnDeselectSuccess(int _gameAmount)
    {
        iBetAmount = _gameAmount;

        GamePlay.instance.OnBetData(blockType, -iBetAmount, gameObject.name);
        if (_gameAmount == 0)
        {
            image.sprite = sDefaultSprite;
            blockState = BlockState.NORMAL;

            gNormalButton.SetActive(true);
            gClickedButton.SetActive(false);
        }
        else
        {
            OnSelectSuccess(_gameAmount);
        }



    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (blockState == BlockState.NORMAL)
        {
            image.sprite = sHighliteSprite;
        }
        else
        {
            GamePlay.instance.EnablePopupForBlock(this.transform);
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (blockState == BlockState.NORMAL)
        {
            image.sprite = sDefaultSprite;
        }

        GamePlay.instance.DisablePopUp();
    }

    public void ResetWinEffect()
    {
        Debug.Log("Reset Win Effect Called");
        if (instantiatedWinEffectObject != null)
        {
            Destroy(instantiatedWinEffectObject);
        }
        if (winState.activeSelf)
        {
            winStateNum.text = normalStateNum.text;
            winStateBet.text = "";
            winState.SetActive(false);
        }

    }

    void SetWinState()
    {
        winStateNum.text = normalStateNum.text;
        winStateBet.text = betAmount.text;
        winState.SetActive(true);
        if (winEffect)
        {
            instantiatedWinEffectObject = Instantiate(winEffect, this.transform);
            instantiatedWinEffectObject.transform.SetLocalPositionAndRotation(new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 1));
            instantiatedWinEffectObject.transform.SetAsLastSibling();
            Debug.Log("Instantiated winEffect Object");
        }
        GamePlay.instance.EnablePopupForBlock(this.transform,true);
        Invoke(nameof(disablePopup),2);
    }

    void disablePopup()
    { 
        GamePlay.instance.DisablePopUp();
        
    }
    public void OnDecideResult(string _sNum)
    {
        if ((blockType == BlockType.TRIPLE && (_sNum == normalStateNum.text))
           || (blockType == BlockType.DOUBLE && (_sNum.Substring(1, 2) == normalStateNum.text))
           || (blockType == BlockType.SINGLE && (_sNum.Substring(2, 1) == normalStateNum.text)))

        {
            GamePlay.instance.lstResultBlock.Add(this);
            if (blockState == BlockState.NORMAL)
            {
                image.sprite = sResultState;
            }
            else
            {
                Debug.Log("number:" + _sNum.Substring(2, 1));
                // image.sprite = sWinSprite;
                SetWinState();

                gNormalButton.SetActive(true);
                gClickedButton.SetActive(false);
#if UNITY_ANDROID
                GamePlay.instance.lstResultBlock.Add(this);
                //GamePlay.instance.ShowWinAmount(blockType, _sNum, iBetAmount);
#endif
            }
        }
    }


}
