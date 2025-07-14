using UnityEngine;
using DG.Tweening;

public class CustomAnimator : MonoBehaviour
{

    public enum AnimationType
    {
        Position,
        Scale,
        PositionAndScale
    }

    public enum AnimationLocation
    {
        Left,
        Right,
        Top,
        Bottom,
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    public TweenComponent[] allComponent;

    private void OnEnable()
    {
        //   DOTween.timeScale = 1;
     //   Debug.LogError(Time.timeScale);
        int length = allComponent.Length;
        for(int i = 0; i < length; i++)
        {
            if (allComponent[i].animationType == AnimationType.Position)
            {
                Vector2 toPos = allComponent[i].rectTransform.anchoredPosition;
                allComponent[i].rectTransform.anchoredPosition = allComponent[i].rectTransform.anchoredPosition + GetPosition(allComponent[i].showingLocaion);
                allComponent[i].rectTransform.DOAnchorPos(toPos, allComponent[i].animationTime).SetEase(allComponent[i].easeType).SetDelay(allComponent[i].delayTime);
            }
            else if (allComponent[i].animationType == AnimationType.Scale)
            {
                Vector2 toScale = allComponent[i].rectTransform.localScale;
                allComponent[i].rectTransform.localScale = allComponent[i].fromScale;
                allComponent[i].rectTransform.DOScale(toScale, allComponent[i].animationTime).SetEase(allComponent[i].easeType).SetDelay(allComponent[i].delayTime);
            }
            else
            {
                Vector2 toPos = allComponent[i].rectTransform.anchoredPosition;
                allComponent[i].rectTransform.anchoredPosition = allComponent[i].rectTransform.anchoredPosition + GetPosition(allComponent[i].showingLocaion);
                allComponent[i].rectTransform.DOAnchorPos(toPos, allComponent[i].animationTime).SetEase(allComponent[i].easeType).SetDelay(allComponent[i].delayTime);
                Vector2 toScale = allComponent[i].rectTransform.localScale;
                allComponent[i].rectTransform.localScale = allComponent[i].fromScale;
                allComponent[i].rectTransform.DOScale(toScale, allComponent[i].animationTime).SetEase(allComponent[i].easeType).SetDelay(allComponent[i].delayTime);
            }
        }
    }

    private void OnDisable()
    {
        //   DOTween.timeScale = 1;
  //      Debug.LogError(Time.timeScale);
        int length = allComponent.Length;
        for (int i = 0; i < length; i++)
        {
            allComponent[i].rectTransform.DOComplete();          
        }
    }

    Vector2 GetPosition(AnimationLocation animationLocation)
    {
        switch (animationLocation)
        {
            case AnimationLocation.Left:
                return new Vector2(-1000, 0);
            case AnimationLocation.Right:
                return new Vector2(1000, 0);
            case AnimationLocation.Top:
                return new Vector2(0, 1000);
            case AnimationLocation.Bottom:
                return new Vector2(0, -1000);
            case AnimationLocation.TopLeft:
                return new Vector2(-1000, 1000);
            case AnimationLocation.TopRight:
                return new Vector2(1000, 1000);
            case AnimationLocation.BottomLeft:
                return new Vector2(-1000, -1000);
            case AnimationLocation.BottomRight:
                return new Vector2(1000, -1000);
            default:
                return new Vector2(0, 0);
        }
    }
}

[System.Serializable]
public class TweenComponent
{
    public CustomAnimator.AnimationType animationType;
    public CustomAnimator.AnimationLocation showingLocaion;
    public RectTransform rectTransform;
    public Ease easeType;
    public float animationTime,delayTime;
    public Vector2 fromScale;
}
