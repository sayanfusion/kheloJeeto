using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] Image loader;
    void Start()
    {
        StartCoroutine(loadingRoutine());
    }

    IEnumerator loadingRoutine() {

        loader.DOFillAmount(1, 0.8f);
        yield return new WaitForSeconds(0.8f);
        SceneManager.LoadScene(1);
    
    }
    
}
