using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class leanmove : MonoBehaviour
{

    public static leanmove Instance;
    public GameObject resultText;

    public Sprite[] spriteholder;

    // Start is called before the first frame update
    void Start()
    {
       WinSequence.mover = false;
       Debug.Log(WinSequence.mover);
    }

    // Update is called once per frame
    void Update()
    {
        if (WinSequence.mover == true)
        {
            fullmove();
            WinSequence.mover = false;
        }
    }
    public void movetopos()
    {
        resultText.SetActive(true);
        // transform.LeanMoveLocal(new Vector2(-1144.964f, -121.65849f), 2);
        transform.LeanMoveLocal(new Vector2(0f, 0f), 2);

    }

    public void movebacktopos()
    {
        transform.LeanMoveLocal(new Vector2(-1280f, -159.549f), 2);
    }
    public void fullmove()
    {
        this.gameObject.GetComponent<Image>().sprite = spriteholder[WinSequence.resultcarryover];
        movetopos();
        Debug.Log(WinSequence.resultcarryover + "blah");
        transform.LeanMoveLocal(new Vector2(30f, -30f), 2);
        //   Invoke(nameof(movebacktopos), 66);
          // Invoke(nameof(disableResult), 66);
    }

    public void disableResult()
    {
        resultText.SetActive(false);
    }
}
