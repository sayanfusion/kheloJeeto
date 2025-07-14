using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class textcontrol : MonoBehaviour
{
    public Text yellowthing;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (BalanceManager.Balance == 0)
        {
            yellowthing.text = "NO POINT AVAILABLE. PLESE LOAD POINTS TO CONTINUE PLAY";
        }
        else
        {
            yellowthing.text = ""
;        }
    }
}
