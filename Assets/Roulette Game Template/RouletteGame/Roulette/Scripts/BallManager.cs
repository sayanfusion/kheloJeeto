using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class BallManager : MonoBehaviour
{
    public static BallManager Instance;
    public bool spinning = false;
    public Rigidbody ball;
    public Transform resultPoint;
    public Transform originPoint;

    public Transform pivotTransform;
    public Transform pivotWheelTransform;

    public Rigidbody ballthing;

    public GameObject ball2;
    public GameObject target2;

    private float ballTimeSpeed = 1.3f;

    public Wheel wheel;

    private Transform Target;

    public static bool ballmovestart = false;

    private static readonly Vector3 axis = Vector3.up;
    private float angularSpeed = 5f;
    private bool stopping = false;

    private Vector3 deltaAngularCross = Vector3.zero;

    private bool trigger_animateBall = true;

    private int res = -1;

    public List<RectTransform> ballPositions = new List<RectTransform>();
    void Start()
    {
        Instance = this;
        ball.isKinematic = true;
    }

    void Update()
    {
        if (WinSequence.ballstopper == true)
        {
            //angularSpeed = 0;
            ballthing.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionX;
            //ballthing.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX;
            //ballthing.isKinematic = false;
            ballmovestart = true;
            //ball2.transform.position = Vector3.MoveTowards(ball2.transform.position, target2.transform.position, 1f*Time.deltaTime);
            WinSequence.ballstopper = false;
            Debug.Log("this function");
        }
    }

    public void StartSpin()
    {
        ball.isKinematic = true;
        ball.transform.SetParent(originPoint);
        ball.transform.localPosition = Vector3.zero;
        ball.transform.localScale = new Vector3(.03f, .03f, .03f);
        transform.SetParent(pivotTransform);
        transform.localRotation = Quaternion.identity;
        angularSpeed = 5f;
        spinning = true;
        StartCoroutine(AnimationEnabler());
        StartCoroutine(AnimationDisabler());
        trigger_animateBall = true;

        Wheel.faster = true;

    }

    public void FindNumber(int result, bool isEuropean)
    {
        result = result == -1 && !isEuropean ? 37 : result;
        Target = wheel.resultCheckerObject[result].transform;
        res = result;
        DOTween.To(() => angularSpeed, x => angularSpeed = x, 1.5f, 5).OnComplete(() =>
        {
            stopping = true;
        });
    }

    private bool bouncing = false;
    public void PlaceToResult(float angleRatio)
    {
     
        Debug.LogError("resultPoint " + resultPoint.position);
        ball.transform.SetParent(resultPoint);//by sayam
        Debug.LogError("target position " + Target.position);
        
        Vector3 direction = (Target.position - resultPoint.position);
        AudioManager.StopAuxiliar();
        bouncing = true;

        StartCoroutine(BounceSound());
        //////////////////////ball.transform.DOLocalJump(Vector3.zero, .04f, 5, ballTimeSpeed).SetEase(Ease.Linear).OnComplete(() => { bouncing = false; });
    }

    private IEnumerator BounceSound()
    {
        while (bouncing)
        {
            yield return new WaitForSeconds(0f);
            //////////////////////////AudioManager.SoundPlay(1);
        }
    }

    private float CalculateAngleRatio(Vector3 angularCross)
    {

        deltaAngularCross = angularCross - deltaAngularCross;

        Vector3 targetVector = (Target.position - transform.position);
        Vector3 ballVector = (ball.position - transform.position);

        targetVector.y = ballVector.y = 0;

        return (Vector3.Angle(ballVector, targetVector) / 180f);
    }

    private void FixedUpdate()
    {
        if (!spinning)
            return;

        transform.Rotate(axis, angularSpeed);

        if (stopping)
        {
            Vector3 angularCross = Vector3.Cross(transform.forward, (Target.position - transform.position).normalized);
            float angle = Vector3.SignedAngle(transform.forward, (Target.position - transform.position), transform.up);
            float angleRatio = CalculateAngleRatio(angularCross);

            if (deltaAngularCross.y > 0f)
            {
                if (angle < 35 && angle > 0)
                {
                    Debug.LogError("angularSpeed  ");
                    angularSpeed = angleRatio * 2f;
                    ////////////////////////////ball.GetComponent<Animator>().enabled = true;
                }
                if (angleRatio <= 0.2f && trigger_animateBall && angle > 5)
                {
                    Debug.LogError("angleRatio <= 0.2f  ");
                    trigger_animateBall = false;
                    PlaceToResult(angleRatio);
                    Debug.LogError("If....1");
                }
                else if (angleRatio <= 0.02f && !trigger_animateBall)
                {
                    Debug.LogError("If....2");
                    Debug.LogError("angleRatio <= 0.01f  ");
                    spinning = false;
                    audios.Instance.WheelRotatingSoundStop();
                    Wheel.slower = true;
                    transform.SetParent(pivotWheelTransform);
                    ball.isKinematic = true;
                    //res ;
                    Debug.LogError("res " + res);
                    //ball.transform.localPosition = new Vector3(0, ball.transform.localPosition.y, ball.transform.localPosition.z);
                    //Closed by Sayam

                    #region BallPositionsSet
                  
                    ball.transform.SetParent(ballPositions[res]);
                    ball.transform.localPosition = new Vector3(0, -0.4f, -0.15f);
                    ball.transform.localScale = new Vector3(0.27522f, .3f, 0.194112f);
                    #endregion
                    stopping = false;
                    ResultManager.SetResult(res);
                }
            }
            Debug.DrawRay(transform.position, angularCross, Color.white);
            Debug.DrawRay(transform.position, (Target.position - transform.position), Color.yellow);
            Debug.DrawRay(ball.transform.position, (Target.position - resultPoint.position), Color.green);
        }

    }

    private IEnumerator AnimationEnabler()
    {
        yield return new WaitForSecondsRealtime(5f);
        ball.GetComponent<Animator>().enabled = true;
    }
    private IEnumerator AnimationDisabler()
    {
        yield return new WaitForSecondsRealtime(8f);
        ball.GetComponent<Animator>().enabled = false;
    }

    public void ChipsDelay()
    {
        ChipManager.EnableChips(true);
    }

    public void InvokingChipsDelay()
    {
        Invoke("ChipsDelay", 5);
    }

}
