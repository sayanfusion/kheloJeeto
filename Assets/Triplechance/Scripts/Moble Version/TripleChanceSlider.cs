using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace tripplechance
{
    public class TripleChanceSlider : MonoBehaviour
    {
        [SerializeField] private RectTransform[] allMoveContent;
        [SerializeField] private float[] allMoveContentEndPos;
        [SerializeField] private GameObject[] allButtonParrent;
        [SerializeField] private List<float> defaultPos=new();
        [SerializeField] private List<GameObject> allArrow;
        private int? previousValue=null;
        private bool isopen;
        [SerializeField] private float time=0.2f;
        private void Start()
        {
            for (int i = 0; i < allMoveContent.Length; i++)
            {
                defaultPos.Add( allMoveContent[i].anchoredPosition.x);
            }
        }
        private void OnEnable()
        {
            TripleChanceSocketManager.closeSlider += CloseSlider;
        }
        private void OnDisable()
        {
            TripleChanceSocketManager.closeSlider -= CloseSlider;
        }
        public void OnSliderButtonClick(int value)
        {
            if (previousValue!=null&&previousValue!=value)
            {
                CloseParent((int)previousValue);
                StartCoroutine(DelayedOpenParent(value));
            }
            else
            {
                (isopen ? (System.Action<int>)CloseParent : OpenParent)(value);
            }
        }
        private void OpenParent(int value)
        {
            isopen = true;
            allMoveContent[value].DOAnchorPosX(allMoveContentEndPos[value], time);
            allButtonParrent[value].SetActive(true);
            previousValue = value;
            allArrow[value].transform.localScale = Vector3.one*-1;
        }
        private void CloseParent(int value)
        {
            isopen = false;
            allMoveContent[value].DOAnchorPosX(defaultPos[value], time).OnComplete(()=> { 
                allButtonParrent[value].SetActive(false);
                previousValue = null;
                for (int i = 0; i < allArrow.Count; i++)
                {
                    allArrow[i].transform.localScale = Vector3.one;
                }
            });
           
        }
        private IEnumerator DelayedOpenParent(int value)
        {
            yield return new WaitForSeconds(time);
            OpenParent(value);
        }
        private void CloseSlider()
        {
            if (previousValue != null )
            {
                CloseParent((int)previousValue);
            }
        }
    }
}