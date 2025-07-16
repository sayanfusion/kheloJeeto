using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LobbyLogoSwitch : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{


    public GameObject imageOutline;

    void Start()
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (imageOutline)
        {
            imageOutline.SetActive(true);
        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (imageOutline)
        {
            imageOutline.SetActive(false);
        }

    }
}
