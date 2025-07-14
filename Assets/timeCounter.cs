using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using System;

public class timeCounter : MonoBehaviour
{
    public static timeCounter instance;
    public GameObject txtdisplay;
    public int secondleft;
    //public int leftmint;
    public bool takingaway = false;
    public bool RollBall = false;
    //public int spawnEnim;
    //public GameObject txtenemy;
    //public int enamkill ;
    //public GameObject levelfail;
    //public GameObject levelcomplete;

    //public GameObject inventory;






    //    var gos : GameObject[];
    //gos = GameObject.FindGameObjectsWithTag("Enemy");
    //if(gos.length > 20)
    //{
    //  // Do Something
    //}

    // Start is called before the first frame update

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {

        txtdisplay.GetComponent<Text>().text = secondleft.ToString();

    }



    // Update is called once per frame
    void Update()
    {
        if (takingaway == false && secondleft > 0)
        {

            //StartCoroutine(timeTake());
        }

        if (secondleft == 40)
        {
            //ResultManager.totalBet = 0;
            RollBall = false;
        }


            checkleveltatuse();

        if (secondleft < 0)
        {
            secondleft = 0;


        }


       
    }

    public void checkleveltatuse()
    {

        //Debug.Log(secondleft + " - " + RollBall);
        if (secondleft == 5 && RollBall == false)
        {
            audios.Instance.NoMoreBetSoundPlay();
            //SceneRoulette._Instance.OnButtonRoll();
            //RollBall = true;
        }
        else if (secondleft <= 0 && RollBall == false)
        {
            SceneRoulette._Instance.OnButtonRoll();
            RollBall = true;
        }

        /* if (secondleft <=3 )
         {

         }*/

    }



    IEnumerator timeTake()
    {


        takingaway = true;

        yield return new WaitForSeconds(1);
        secondleft -= 1;


        if (secondleft < 10)
        {
            txtdisplay.GetComponent<Text>().text = secondleft.ToString();
        }
        else
        {
            txtdisplay.GetComponent<Text>().text = secondleft.ToString();
        }
        takingaway = false;

    }



}
