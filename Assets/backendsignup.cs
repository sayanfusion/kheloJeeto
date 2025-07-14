using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class backendsignup : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private InputField userName, email, password, gender, dob;

    backendfunctions backendObj;
    private string nm;
    // Start is called before the first frame update
    void Start()
    {
        backendObj = FindObjectOfType<backendfunctions>();
    }

    public void Register()
    {
        nm = userName.text;
        print("test"+nm);

        backendObj.Add(userName.text, email.text, password.text, gender.text, dob.text);
        backendObj.registration();
    }
}
