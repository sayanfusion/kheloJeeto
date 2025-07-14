using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;

public class ChangeRequestBody
{
    public string old_password;
    public string new_password;
    public string confirm_password;
}

public class ChangeRequestResonseData
{
    public int status;
    public string[] message;
}

public class ChangePassword : MonoBehaviour
{
    [SerializeField] private TMP_InputField oldPassword;
    [SerializeField] private TMP_InputField newPassword;
    [SerializeField] private TMP_InputField confirmPassword;
    [SerializeField] private GameObject changepasswordObject;
    [SerializeField] private TextMeshProUGUI errorText;

    public void ChangePass()
    {
        StartCoroutine(SendPasswordChangeRequest());
    }

    public IEnumerator SendPasswordChangeRequest()
    {
        string url = Constant.KIBaseURL + "change-password";
        string bearer = PlayerPrefs.GetString(Constants.Token);

        // Create the request body object
        ChangeRequestBody changeRequestBody = new ChangeRequestBody
        {
            old_password = oldPassword.text,
            new_password = newPassword.text,
            confirm_password = confirmPassword.text
        };

        // Serialize the request body to JSON
        string jsonData = JsonUtility.ToJson(changeRequestBody);

        using (UnityWebRequest webRequest = UnityWebRequest.PostWwwForm(url, "POST"))
        {
            // Set headers
            webRequest.SetRequestHeader("Authorization", "Bearer " + bearer);
            webRequest.SetRequestHeader("Content-Type", "application/json");

            // Attach JSON data to the request
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();

            // Send the request
            yield return webRequest.SendWebRequest();

            // Check for errors
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + webRequest.error);
            }
            else
            {
                ChangeRequestResonseData changeRequestResonseData = JsonUtility.FromJson<ChangeRequestResonseData>(webRequest.downloadHandler.text);
                if(changeRequestResonseData.status == 200)
                {
                    Debug.Log("Request sent successfully!");
                // Handle response here if needed
                changepasswordObject.SetActive(false);
                Debug.Log("Response: " + webRequest.downloadHandler.text);
                }
                else
                {
                    errorText.text = "Please enter valid new Pssword";
                }
            }
        }
    }
}