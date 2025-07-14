using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICreatAcc : MonoBehaviour
{
    // Start is called before the first frame update
    string userName, email, pass;
    public void UpdateUsername(string _userName)
    {
        _userName = userName;
    }
    public void UpdateEmail(string _email)
    {
        _email = email;
    }
    public void UpdatePass(string _pass)
    {
        _pass = pass;
    }
    public void CreateAccount()
    {
        Authentication.Instance.CreateAccount(userName, email, pass);
    }

}
