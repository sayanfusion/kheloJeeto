using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class automove : MonoBehaviour
{
    public Transform ball2;
    public GameObject target;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (BallManager.ballmovestart == true)
        {
            ball2.position = Vector3.MoveTowards(ball2.position, target.transform.position, 0.3f*Time.deltaTime);
            BallManager.ballmovestart = false;
            Debug.Log(BallManager.ballmovestart + "abcd");
        }
        
    }
}
