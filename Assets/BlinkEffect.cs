using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkEffect : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject material;
    public bool isWhite = false;
    public bool isblack = false;
    void Start()
    {
        
    }
    private void OnEnable()
    {
        
        

        
    }

    // Update is called once per frame
    void Update()
    {
        if (isWhite == false)
        {
            StartCoroutine(White());
            
        }
        if (isblack == false)
        {
            StartCoroutine(Black());

        }
    }

    IEnumerator White()
    {
        material.SetActive(true);
        yield return new WaitForSecondsRealtime(0.2f);
        isWhite = true;
        isblack = false;
        //Debug.Log("wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww");
    }
    IEnumerator Black()
    {
        material.SetActive(false);
        yield return new WaitForSecondsRealtime(0.2f);
        isblack = true;
        isWhite = false;
      //  Debug.Log("bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb");
    }

}
