using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LobbyLogoSwitch : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Image image;

    // Icon Dynamic Scaling
    private Vector3 originalScale;
    public float scaleFactor = 0.9f;

    // Outline reference
    private Outline outline;

    void Start()
    {
        image = GetComponent<Image>();

        originalScale = transform.localScale;

        // Add Outline component if missing
        outline = GetComponent<Outline>();
        if (outline == null)
            outline = gameObject.AddComponent<Outline>();

        // Set red outline but disable it initially
        outline.effectColor = Color.red;
        outline.effectDistance = new Vector2(10f, 10f);
        outline.enabled = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Mouse Entered on Logo");
        transform.localScale = originalScale * scaleFactor;

        if (outline != null)
            outline.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Mouse Exit from Logo");
        transform.localScale = originalScale;

        if (outline != null)
            outline.enabled = false;
    }
}
