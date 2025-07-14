using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class timer : MonoBehaviour
{
    public Text timeshower;
    public Sprite red;
    public Sprite green;
    public static float score8;

    public static bool betallowerd = true;

    public Button chiparray1;
    public Button chiparray2;
    public Button chiparray5;
    public Button chiparray10;
    public Button chiparray50;
    public Button chiparray100;
    public Button chiparray500;
    public Button chiparray1000;
    public Button chiparray5000;
    // Start is called before the first frame update
  /*  void Start()
    {
        score8 = timeCounter.instance.secondleft;
    }*/

    // Update is called once per frame
    void Update()
    {
        score8 = timeCounter.instance.secondleft;
        // Debug.Log("Timer " + timeCounter.instance.secondleft);

        if (score8 <= 5)
        {
            //this.gameObject.GetComponent<Image>().sprite = red;

            chiparray1.interactable = false;
            chiparray2.interactable = false;
            chiparray5.interactable = false;
            chiparray10.interactable = false;
            chiparray50.interactable = false;
            chiparray100.interactable = false;
            chiparray500.interactable = false;
            chiparray1000.interactable = false;
            chiparray5000.interactable = false;

            betallowerd = false;

        }

        if (score8 > 5)
        {
            //this.gameObject.GetComponent<Image>().sprite = red;

            chiparray1.interactable = true;
            chiparray5.interactable = true;
            chiparray10.interactable = true;
            chiparray50.interactable = true;
            chiparray100.interactable = true;
            chiparray2.interactable = true;
            chiparray500.interactable = true;
            chiparray1000.interactable = true;
            chiparray5000.interactable = true;

            betallowerd = true;
            SceneRoulette.numDisplayed = false;

        }


        if (timeCounter.instance.secondleft <= 5)
        {
            this.gameObject.GetComponent<Image>().sprite = red;
            ///////////////////Invoke(nameof(redder), 55);
            WinSequence.greenredder = false;
        }
        else
        {
            this.gameObject.GetComponent<Image>().sprite = green;
        }
    }

    public void redder()
    {
        this.gameObject.GetComponent<Image>().sprite = red;
    }
}
