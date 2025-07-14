using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class OpeScene : MonoBehaviour
{
    [SerializeField]
    private bool isLogEnabled;
    // Start is called before the first frame update
    void Start()
    {
        Debug.unityLogger.logEnabled = isLogEnabled;
        Invoke("OpenScene", 0.0f);
        //Debug.LogError("Hello");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenScene()
    {
        //SceneManager.LoadScene(4);
        if (PlayerPrefs.GetInt(Constants.LoginStatus) == 0)
        {
            SceneManager.LoadSceneAsync(4);
        }
        else
        {
            SceneManager.LoadSceneAsync("DashBoard");
        }

    }
  
}
