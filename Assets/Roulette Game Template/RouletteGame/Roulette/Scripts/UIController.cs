using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public static List<BetSpace> BetSpace = new List<BetSpace>();
    public BetSpace betSpace;

    public void OnClickDoubleBet()
    {
        //foreach (var item in BetSpace)
        //{
        //    if (item != null)
        //    {
        //        item.DoubleBet();
        //    }
        //}
        if (betSpace != null)
        {
            betSpace.DoubleBet();
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (!BetSpace.Contains(hit.transform.GetComponent<BetSpace>()))
                {
                    BetSpace.Add(hit.transform.GetComponent<BetSpace>());
                }
                    betSpace = hit.transform.GetComponent<BetSpace>();
            }
        }

    }

}
