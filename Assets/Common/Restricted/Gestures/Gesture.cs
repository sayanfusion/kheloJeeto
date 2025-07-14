using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

namespace DevCommon.Gesture
{
    public enum EGestureType
    {
        None,
        Tap,
        TapAndHold,
        Swipe,
        Drag,
        Rotate,
        Pinch,
    }

    public interface IGesture
    {
        void OnGestureStart(Gesture a_Gesture, object a_Data = null);
        void ReceivedGesture(Gesture a_Gesture, object a_Data = null);
        void OnGestureEnded(Gesture a_Gesture, object a_Data = null);
    }

    public class Gesture
    {
        public EGestureType GestureType { get; set; }
        public List<IGesture> IGestureSubscribers { get; private set; } = new List<IGesture>();
        public bool RayCastUI { get; set; } = true;

        public virtual void AddInterfaceObject(IGesture a_InterfaceObject)
        {
            if (!IGestureSubscribers.Contains(a_InterfaceObject))
                IGestureSubscribers.Add(a_InterfaceObject);
        }

        public virtual void RemoveInterfaceObject(IGesture a_InterfaceObject)
        {
            if (IGestureSubscribers.Contains(a_InterfaceObject))
                IGestureSubscribers.Remove(a_InterfaceObject);
        }

        public virtual void CustomUpdate(float a_DeltaTime)
        {

        }

        protected bool checkPointerOverUIObject()
        {
            List<RaycastResult> t_Results = new List<RaycastResult>();
            if (RayCastUI)
            {
                PointerEventData t_EventDataCurrentPosition = new PointerEventData(EventSystem.current);
                t_EventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                EventSystem.current.RaycastAll(t_EventDataCurrentPosition, t_Results);
            }
            return t_Results.Count > 0;
        }

        protected bool checkPointerOverUIObject(Vector3 a_InputPosition)
        {
            List<RaycastResult> t_Results = new List<RaycastResult>();
            if (RayCastUI)
            {
                PointerEventData t_EventDataCurrentPosition = new PointerEventData(EventSystem.current);
                t_EventDataCurrentPosition.position = new Vector2(a_InputPosition.x, a_InputPosition.y);
                EventSystem.current.RaycastAll(t_EventDataCurrentPosition, t_Results);
            }
            return t_Results.Count > 0;
        }

        protected virtual void onGestureStarted(Gesture a_Gesture, object a_Data = null)
        {
            for (int i = 0; i < IGestureSubscribers.Count; i++)
            {
                IGestureSubscribers[i].OnGestureStart(a_Gesture, a_Data);
            }
        }

        protected virtual void onReceivedGesture(Gesture a_Gesture, object a_Data = null)
        {
            for (int i = 0; i < IGestureSubscribers.Count; i++)
            {
                IGestureSubscribers[i].ReceivedGesture(a_Gesture, a_Data);
            }
        }

        protected virtual void onGestureEnded(Gesture a_Gesture, object a_Data = null)
        {
            for (int i = 0; i < IGestureSubscribers.Count; i++)
            {
                IGestureSubscribers[i].OnGestureEnded(a_Gesture, a_Data);
            }
        }
    }
}