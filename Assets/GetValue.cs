using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GetValue : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject textValue;
    public int value;
    void Start()
    {
        switch (transform.name)
        {
            case "chip1(Clone)":
                value = 1;
                break;
            case "chip2(Clone)":
                value = 2;
                break;
            case "chip5(Clone)":
                value = 5;
                break;
            case "chip10(Clone)":
                value = 10;
                break;
            case "chip50(Clone)":
                value = 50;
                break;
            case "chip100(Clone)":
                value = 100;
                break;
            case "chip500(Clone)":
                value = 500;
                break; 
            case "chip1k(Clone)":
                value = 1000;
                break; 
            case "chip5k(Clone)":
                value = 5000;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
