using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using SimpleJSON;

public class backendfunctions : MonoBehaviour
{
    // Start is called before the first frame update

    private string UserName, Email, Password, Gender, Dob;
    private string _message;
    
    public void Add(string userName, string email, string password, string gender, string dob)
    {
        this.UserName = userName;
        this.Email = email;
        this.Password = password;
        this.Gender = gender;
        this.Dob = dob;

      
    
    }
    public void registration()
    {
        StartCoroutine(postRequest("https://admin.khelojeetogame.com/api/user-signup"));

    }
    IEnumerator postRequest(string url)
    {
        Debug.Log("...........................");
        WWWForm form = new WWWForm();
        form.AddField("user_name", UserName);
        form.AddField("gender", Gender);
        form.AddField("date_of_birth", Dob);
        form.AddField("email", Email);
        /////////////form.AddField("password", Password);
 
        UnityWebRequest uwr = UnityWebRequest.Post(url, form);
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            Debug.Log("1111111111111111111111111111111111");
            Debug.Log("Received authentication for OTP: " + uwr.downloadHandler.text);

            JSONNode loginInfo1 = JSON.Parse(uwr.downloadHandler.text);
            _message = loginInfo1[0]["status"];
            print("status is" + _message);

            if (_message == "1")
            {
                Debug.Log("1010101010");
                print("if statement is working");
                SceneManager.LoadScene("ForIndian Mobile");
            }
        }

    }
}
