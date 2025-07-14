using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadLogin : MonoBehaviour
{
    public Image fillImage;
    private void Start() {
        StartCoroutine(LoadLoginScene());
    }
    public IEnumerator LoadLoginScene()
    {
        yield return new WaitForSeconds(2.0f);
        AsyncOperation op = SceneManager.LoadSceneAsync("login");

        while(!op.isDone)
        {
            float progress = Mathf.Clamp01(op.progress /0.9f);

            fillImage.fillAmount = progress;
            yield return new WaitForSeconds(0.02f); 
        }
    }
}
