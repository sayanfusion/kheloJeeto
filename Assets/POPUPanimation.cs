using System.Collections;
using UnityEngine;
using DG.Tweening;
public class POPUPanimation : MonoBehaviour
{
    public GameObject[] buttons;
    public float popupDuration = 0.1f;
    public float delayBetweenButtons = 0.0f;

    private Coroutine popupCoroutine;

    void Awake()
    {
        // Start with all buttons hidden
       // ResetAllScales();
    }

    void OnEnable()
    {
        TriggerPopupAnimation();
    }
    private void TriggerPopupAnimation()
    {
        // If already animating, stop and restart
        if (popupCoroutine != null)
            StopCoroutine(popupCoroutine);

        ResetAllScales(); // Reset scales to 0

        popupCoroutine = StartCoroutine(AnimateButtons());
    }

    private void ResetAllScales()
    {
        foreach (GameObject button in buttons)
        {
            if (button != null)
                button.transform.localScale = Vector3.zero;
        }
    }

    IEnumerator AnimateButtons()
    {
        foreach (GameObject button in buttons)
        {
            button.transform.DOScale(1f, 0.3f).SetEase(Ease.Linear).OnStart(() => button.transform.localScale = Vector3.zero);
            yield return new WaitForSeconds(0.15f);
            // if (button != null)
            // {
            //     yield return StartCoroutine(ScaleUp(button.GetComponent<RectTransform>()));
            //     yield return new WaitForSeconds(delayBetweenButtons);
            // }
        }

        popupCoroutine = null;
    }


}
