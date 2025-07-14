using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultBox : MonoBehaviour {
    public Transform tResultParent;

	private void OnEnable()
	{
        //UpdateResult();//by sayam
    }

    public void UpdateResult()
    {
        int count = tResultParent.childCount;
        for (int i = 0; i < count; i++)
        {
            if (UIControllerTri.instance.sAllResult.Count > i)
                tResultParent.GetChild(i).GetComponent<Result>().SetParameter(UIControllerTri.instance.sAllResult[i]);
        }
    }
	
}
