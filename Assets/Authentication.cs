using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class Authentication : MonoBehaviour
{
    public static Authentication Instance;
    // Start is called before the first frame update

    private void Awake()
    {
        Instance = this;
    }
    public void CreateAccount(string userName, string email, string pass)
    {
        PlayFabClientAPI.RegisterPlayFabUser(
            new RegisterPlayFabUserRequest()
            {
                Username = userName,
                Email = email,
                Password = pass,
                RequireBothUsernameAndEmail = true
            },
            response =>
            {
                Debug.Log("Account Creation Successfull");
            },
            error=>
            {
                Debug.Log("Account Creation unSuccessfull"+error.ErrorMessage);
            }

            );
    }
}
