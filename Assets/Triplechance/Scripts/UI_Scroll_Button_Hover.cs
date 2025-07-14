using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Scroll_Button_Hover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public UI_Scroll uI_Scroll;
    public bool pointerHover;
    private delegate void ScrollDelegate();
    private ScrollDelegate scrollDelegate;
    private void Start()
    {
        Button button = gameObject.GetComponent<Button>();
        if (button == uI_Scroll.upButton)
        {
            scrollDelegate = uI_Scroll.ScrollUp;
        }
        if (button == uI_Scroll.downButton)
        {
            scrollDelegate = uI_Scroll.ScrollDown;
        }
    }

    private void Update()
    {
        if (pointerHover)
        {
            Debug.Log("Invoking Delegate");
            scrollDelegate.Invoke();
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerHover = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerHover = false;
    }
}