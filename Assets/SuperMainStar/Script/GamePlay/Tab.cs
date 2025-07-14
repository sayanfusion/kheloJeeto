using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class Tab : MonoBehaviour
{

    public Sprite sSelectedTab;
    public Sprite sSelectedBlockTab;

    public List<BlockData> allBlockData;
    public RectTransform buttonPanel;
    private Sprite sNormalTab;
    private Sprite clickObjectNormalSprite;
    private int index;

    Button button;
    Image image;

    private void Awake()
    {
        button = GetComponent<Button>();
        image = GetComponent<Image>();
        sNormalTab = image.sprite;
        if (SceneManager.GetActiveScene().name == "TripleChanceProGameplay")
        {
            clickObjectNormalSprite = transform.GetComponent<Image>().sprite;
        }
        else
        {
            clickObjectNormalSprite = transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite;
        }
        button.onClick.AddListener(delegate
        {
            OnClick(true);
        });
    }

    private void OnEnable()
    {
        allBlockData = new List<BlockData>();
        allBlockData.Clear();
        Block.onSelectTripleBlock += OnSelectBlock;
        Block.onDeSelectBlock += OnDeSelectBlock;
        GamePlay.onClearClicked += ClearAllData;
    }

    private void OnDisable()
    {
        Block.onSelectTripleBlock -= OnSelectBlock;
        Block.onDeSelectBlock -= OnDeSelectBlock;
        GamePlay.onClearClicked -= ClearAllData;
    }

    public void OnClick(bool _playSound = true)
    {
        // Debug.Log("onclick");
        //if(_playSound)
        //   	SoundController.instance.PlayAudio (SoundController.ClipType.TAB);
        GamePlay.instance.DeSelectAllTab();
        //  image.sprite = sSelectedTab;
        ActiveCLickImage(true);
        RenameAllChild();
        GamePlay.instance.currentSelectedTab = this;
        Debug.LogError("OnClick");

    }

    public void Select()
    {
        ActiveCLickImage(false);
        //image.sprite = sSelectedBlockTab;
        // AddBlockDataToList();
    }

    public void Reset()
    {
        image.sprite = sNormalTab;
        //ActiveCLickImage(false);
        TurnSelectedTabGreen(false);
    }

    public void AddData(BlockData data)
    {
        allBlockData.Add(data);
    }

    public void DeSelect()
    {
        if (allBlockData.Count == 0)
            //image.sprite = sNormalTab;
            ActiveCLickImage(false);
        else
            ActiveCLickImage(false);
        //image.sprite = sSelectedBlockTab;


    }


    void OnSelectBlock(Block _block)
    {
        if (_block.tNormalStateNumber.text[0] == gameObject.name[0])
        {
            BlockData _blockData = allBlockData.Find((BlockData obj) => (obj.game_number == _block.tNormalStateNumber.text));
            if (_blockData != null)
            {
                _blockData.game_amount = _block.iBetAmount;

            }
            else
            {
                allBlockData.Add(new BlockData(_block, _block.tNormalStateNumber.text, _block.iBetAmount));
            }
            image.sprite = sSelectedBlockTab;
            TurnSelectedTabGreen(true);
        }
    }

    public void TurnSelectedTabGreen(bool active)
    {
        if (SceneManager.GetActiveScene().name == "TripleChanceProGameplay")
        {
            if (active == true)
                transform.GetComponent<Image>().sprite = sSelectedBlockTab;
            else
                transform.GetComponent<Image>().sprite = clickObjectNormalSprite;
        }
        else
        {
            if (active == true)
                transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = sSelectedBlockTab;
            else
                transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = clickObjectNormalSprite;
        }
    }

 public void TurnSelectedTabGreenRepeat(bool active)
{
    if (transform.childCount > 0 && transform.GetChild(0).childCount > 0)
    {
        if (active == true)
        {
            image.sprite = sSelectedBlockTab;
            transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = sSelectedBlockTab;
        }
        else
        {
            ActiveCLickImage(false);
            transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = clickObjectNormalSprite;
        }
    }
    else
    {
        Debug.LogError("Transform hierarchy is not as expected. Please check the hierarchy.");
    }
}

    public void ResetTab()
    {
        image.sprite = sNormalTab;
        transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = clickObjectNormalSprite;
    }

    void OnDeSelectBlock(Block _block)
    {

        if (_block.tNormalStateNumber.text[0] == gameObject.name[0])
        {
            Debug.Log("Block Normal State text 0 : " + _block.tNormalStateNumber.text[0] + ",Game Object Name 0 : " + gameObject.name[0]);
            BlockData _blockData = allBlockData.Find((BlockData obj) => (obj.game_number == _block.tNormalStateNumber.text));
            allBlockData.Remove(_blockData);
            if (allBlockData.Count == 0)
            {
                image.sprite = sNormalTab;
                if (GamePlay.instance.currentSelectedTab == this)
                {
                    ActiveCLickImage(true);
                    TurnSelectedTabGreen(false);
                }
                // image.sprite = sSelectedTab;
                else
                {
                    TurnSelectedTabGreen(false);
                    ActiveCLickImage(false);
                }
                //image.sprite = sNormalTab;
            }
        }

    }


    public void OnRandomPickClicked()
    {
        allBlockData.Clear();
        image.sprite = sSelectedTab;
        GamePlay.instance.RemovePreviousSelectedTripleBlock(gameObject.name);
    }

    public void ClearAllData()
    {
        allBlockData.Clear();
        if (GamePlay.instance.currentSelectedTab == this)
            // image.sprite = sSelectedTab;
            ActiveCLickImage(true);

        else
            //image.sprite = sNormalTab;
            ActiveCLickImage(false);
    }
    public void ActiveCLickImage(bool active)
    {
        Debug.Log("Activating child click object");
        if (SceneManager.GetActiveScene().name == "TripleChanceProGameplay")
        {

        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(active);
        }
    }

    public void RenameAllChild()
    {
        float _startingValue = int.Parse(gameObject.name);
        int _iCount = buttonPanel.transform.childCount;

        for (int i = 0; i < _iCount; i++)
        {
            Transform _trans = buttonPanel.transform.GetChild(i);
            _trans.name = (_startingValue + (i)).ToString("000");
            _trans.GetChild(0).GetChild(0).GetComponent<Text>().text = (_startingValue + (i)).ToString("000");
            _trans.GetChild(1).GetChild(0).GetComponent<Text>().text = (_startingValue + (i)).ToString("000");
            _trans.GetComponent<Block>().OnDeselectSuccess(0);
        }

        AddBlockDataToList();

    }

    void AddBlockDataToList()
    {
        allBlockData = GamePlay.instance.GetAllBlockDataForTab(gameObject.name);
        for (int i = 0; i < allBlockData.Count; i++)
        {
            int _iAmount = allBlockData[i].game_amount;

            allBlockData[i].block.OnSelectSuccess(_iAmount);
        }
    }

}
