using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonImageChange : MonoBehaviour
{
    public Sprite[] image;
   public int a = 0, b = 1;
    public void ChangeButton()
    {
        
        Block[] allChildObj = GetComponentsInChildren<Block>();
        for (int i = 0; i < allChildObj.Length; i++)
        {
            if (i % 10 == 0 && i > 0)
            {
                int tempnum = a;
                a = b;
                b = tempnum;
            }
            if (i % 2 == 0)
            {
                Image temp = allChildObj[i].transform.GetChild(0).GetComponent<Image>();
                temp.sprite = image[a];
                temp.SetNativeSize();
            }
            else
            {
                Image temp = allChildObj[i].transform.GetChild(0).GetComponent<Image>();
                temp.sprite = image[b];
                temp.SetNativeSize();
            }
             allChildObj[i].transform.GetChild(1).GetComponent<RectTransform>().sizeDelta=new Vector2(72,72);
        }
    }
}
