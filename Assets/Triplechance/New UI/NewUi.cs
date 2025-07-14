using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class NewUi : MonoBehaviour
{
    public static NewUi instance;
    public GameObject SingleBoard;
    public GameObject DoubleBoard;
    public GameObject BtnS;
    public GameObject BtnD;
    public GameObject BlockPopUp;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        HideSingleAndDouble();
    }

    public void HideSingleAndDouble()
    {
        SingleBoard.SetActive(false);
        DoubleBoard.SetActive(false);
    }


    public void DoubleBoardFunc()
    {
        BlockPopUp.SetActive(false);
        if (SingleBoard.activeInHierarchy)
        {
            SingleBoard.SetActive(false);
           
            BtnS.transform.DORotate(new Vector3(0f, 0f, 0f), 0.3f);
          
        }
        if(DoubleBoard.activeInHierarchy)
        {
            DoubleBoard.SetActive(false);
            BtnD.transform.DORotate(new Vector3(0f, 0f, 0f), 0.3f);
        }
        else
        {
            DoubleBoard.SetActive(true);
            BtnD.transform.DORotate(new Vector3(0f, 0f, 180f), 0.3f);
        }
     
      
    }

    public void SingleBoardFunc()
    {
        BlockPopUp.SetActive(false);
        Debug.Log("SingleBoardFunc " + SingleBoard.activeInHierarchy);
        if (DoubleBoard.activeInHierarchy)
        {
            DoubleBoard.SetActive(false);
            BtnD.transform.DORotate(new Vector3(0f, 0f, 0f), 0.3f);

        }
        if (SingleBoard.activeInHierarchy)
        {
            SingleBoard.SetActive(false);
            BtnS.transform.DORotate(new Vector3(0f, 0f, 0f), 0.3f);
        }
        else
        {
            SingleBoard.SetActive(true);
            BtnS.transform.DORotate(new Vector3(0f, 0f, 180f), 0.3f);
        }
    }


}
