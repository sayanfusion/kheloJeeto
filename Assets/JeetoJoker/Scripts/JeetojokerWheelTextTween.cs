using UnityEngine;
using DG.Tweening; // Import DOTween namespace

public class JeetojokerWheelTextTween : MonoBehaviour
{
    public float moveDistance = 10f; 
    public float moveDuration = 2f;

    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.position;
        StartLoop();
    }

    private void StartLoop() {
        Vector3 leftPosition = initialPosition + Vector3.left * moveDistance;
        Vector3 rightPosition = initialPosition + Vector3.right * moveDistance;

        transform.position = leftPosition; 
        transform.DOMove(rightPosition, moveDuration)
            .SetEase(Ease.Linear) 
            .OnComplete(() => {
                transform.position = leftPosition;
                StartLoop(); 
            });
    }
}
