using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Chip : MonoBehaviour {
    
    public int value;
    
    public GameObject ring;

    public static Chip Instance;

    private void Select()
    {
        ChipManager.selected = this;
        ChipManager.selected.ring.SetActive(true);
    }

    public void OnClick()
    {
        if (!BetSpace.BetsEnabled)
            return;

        transform.DOComplete();
        if (ChipManager.selected)
        {
            ////////////////BetSpace.count = 0;
            ////////////////BetSpace.count1 = 0;
            AudioManager.SoundPlay(3);
            ChipManager.selected.transform.DOScale(1f, .2f);
            ChipManager.selected.ring.SetActive(false);
        }
        transform.DOShakeScale(.3f, .2f, 10, 0);
        Select();
    }

    public void OnPointEnter()
    {
        if (BetSpace.BetsEnabled)
        {
            transform.DOComplete();
            transform.DOScale(1.2f, .3f);
        }
    }

    public void OnPointExit()
    {
        if (BetSpace.BetsEnabled)
        {
            transform.DOComplete();
            transform.DOScale(1f, .2f);
        }
    }
}
