using System.Collections;
using UnityEngine;

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

    public void TriggerPopupAnimation()
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
                button.GetComponent<RectTransform>().localScale = Vector3.zero;
        }
    }

    IEnumerator AnimateButtons()
    {
        foreach (GameObject button in buttons)
        {
            if (button != null)
            {
                yield return StartCoroutine(ScaleUp(button.GetComponent<RectTransform>()));
                yield return new WaitForSeconds(delayBetweenButtons);
            }
        }

        popupCoroutine = null;
    }

    IEnumerator ScaleUp(RectTransform rect)
    {
        float elapsedTime = 0f;
        Vector3 startScale = Vector3.zero;
        Vector3 targetScale = Vector3.one;

        while (elapsedTime < popupDuration)
        {
            rect.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime / popupDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rect.localScale = targetScale;
    }
}
