using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class textmeshcolour : MonoBehaviour
{
    // Start is called before the first frame update
    public TMP_Text thing;

    void Awake()
    {
        Debug.Log("hello");
        //thing = GetComponent<TextMeshPro>();
    }

    void Start()
    {
        
        thing.color = Color.white;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
