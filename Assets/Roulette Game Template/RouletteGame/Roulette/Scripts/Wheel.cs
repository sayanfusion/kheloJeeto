using UnityEngine;

public class Wheel : MonoBehaviour
{
    protected byte[] numbers;
    protected bool spinning = true;

    public BallManager ball;
    public GameObject[] resultCheckerObject;

    private readonly Vector3 pivot = Vector3.back;
    public float speed = 0.9f;
    public static bool slower = false;
    public static bool faster = false;

    public static float lastBet = 0;

    void Awake()
    {
        //Invoke(nameof(stopper), 5);
    }

    void FixedUpdate()
    {
        if (spinning)
            transform.Rotate(pivot * speed);

        if (faster == true)
        {
            speed = 2f;
            faster = false;
        }

        if (slower == true)
        {
            if (speed > 0.5)
            {
                speed -= 0.2f*Time.deltaTime;
            }
            else
            {
                slower = false;
            }
            
        }
        
    }

    public virtual void Spin()
    {
        
        SceneRoulette.GameStarted = true;
        ball.StartSpin();
        BetSpace.EnableBets(false);
    }

    public void stopper()
    {
        Debug.Log("stopped");
        //speed -= 1f;
        slower = true;
    }
    public void backtospeed()
    {
        speed = 0.9f;
    }
}
