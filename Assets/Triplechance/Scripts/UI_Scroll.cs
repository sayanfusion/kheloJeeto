using UnityEngine;
using UnityEngine.UI;

public class UI_Scroll : MonoBehaviour
{
    public ScrollRect scrollRect;
    public float scrollAmount;
    public Button upButton;
    public Button downButton;

    public void ScrollUp()
    {
        ChangeScrollHorizontal(scrollAmount);
    }
    public void ScrollDown()
    {
        ChangeScrollHorizontal(-scrollAmount);
    }

    private void ChangeScrollHorizontal(float amount)
    {
        scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition + amount);
        Debug.Log("New Normalized position : " + scrollRect.verticalNormalizedPosition);
    }

}