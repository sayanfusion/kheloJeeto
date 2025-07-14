using System;
using UnityEngine;
using TMPro;

namespace DevCommon.Utils
{
    public class Toast : MonoBehaviour
    {
        public enum Time
        {
            OneSecond = 1,
            TwoSecond = 2,
            ThreeSecond = 3,
        }
    
        public enum Position
        {
            Top,
            Bottom,
        }
    
        public static void ShowMessage(string a_Message, Toast.Time a_Time = Time.TwoSecond, Toast.Position a_Position = Position.Bottom)
        {
            GameObject t_MessageObj = Resources.Load("ToastMessage") as GameObject;
            RectTransform t_ContainerObj = t_MessageObj.transform.GetChild(0).GetComponent<RectTransform>();
            TMP_Text t_TextObj = t_MessageObj.GetComponentInChildren<TMP_Text>();
    
            t_TextObj.text = a_Message;
            setPosition(t_ContainerObj, a_Position);
    
            GameObject t_Go = Instantiate(t_MessageObj);
            removeClone(t_Go, a_Time);
        }
    
        private static void setPosition(RectTransform a_RectTransform, Position a_Position)
        {
            if (Enum.Equals(a_Position, Position.Top))
            {
                a_RectTransform.anchorMin = new Vector2(0f, 1f);
                a_RectTransform.anchorMax = new Vector2(1f, 1f);
                a_RectTransform.pivot = new Vector2(0.5f, 1f);
                a_RectTransform.anchoredPosition = new Vector3(0f, -50f, 0f);
            }
            else if (Enum.Equals(a_Position, Position.Bottom))
            {
                a_RectTransform.anchorMin = new Vector2(0f, 0f);
                a_RectTransform.anchorMax = new Vector2(1f, 0f);
                a_RectTransform.pivot = new Vector2(0.5f, 0f);
                a_RectTransform.anchoredPosition = new Vector3(0f, 50f, 0f);
            }
        }
    
        private static void removeClone(GameObject a_Go, Time a_Time)
        {
            Destroy(a_Go, (int)a_Time);
        }
    }
}

