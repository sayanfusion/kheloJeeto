using UnityEngine;
using UnityEngine.UI;

public class ButtonBlink : MonoBehaviour
{
    public Button button;
    public Image imageToBlink;
    public float blinkDuration = 1f;
    public int maxAlpha = 65;

    private Color initialColor;
    private float timer;

    void Start()
    {
        SetAlphaZero();
    }

    private void SetAlphaZero()
    {
        initialColor = imageToBlink.color;
        imageToBlink.color = new Color(initialColor.r, initialColor.g, initialColor.b, 0f);
    }

    void Update()
    {
        if (button.interactable)
        {
            Blink();
        }
        else
        {
            SetAlphaZero();
        }
    }

    void Blink()
    {
        timer += Time.deltaTime;
        float t = Mathf.PingPong(timer / blinkDuration, 1f);
        float alpha = Mathf.Lerp(0f, maxAlpha / 100f, t); // Convert maxAlpha to range [0, 1]
        Color newColor = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
        imageToBlink.color = newColor;
    }
}
