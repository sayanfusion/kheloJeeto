using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace DevCommon.Gesture
{
    public sealed class GestureHandler : MonoBehaviour
    {
        private bool rayCastUI = true;
        private List<Gesture> gestureList = new List<Gesture>();

        // Returns a singleton instance of Gesture Handler
        private static GestureHandler instance;

        public static GestureHandler Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject t_Obj = new GameObject("GestureHandler");
                    DontDestroyOnLoad(t_Obj);
                    instance = t_Obj.AddComponent<GestureHandler>();
                }
                return instance;
            }
        }

        // Private constructor
        private GestureHandler() { }

        // Set gesture handler's raycast on UI element status
        public void SetRayCastUI(bool a_RaycastUI)
        {
            rayCastUI = a_RaycastUI;
        }

        // Set a particular getsure type to not activate if overlapping with UI elements
        public void SetRayCastUI(EGestureType a_Type, bool a_RaycastUI)
        {
            Gesture t_RequiredGesture = gestureList.Where(x => x.GestureType == a_Type).FirstOrDefault<Gesture>();
            if (t_RequiredGesture != null)
            {
                t_RequiredGesture.RayCastUI = a_RaycastUI;
            }
        }

        // Register an interface for receiveing particular gestures
        public void RegisterForGesture(EGestureType a_Type, IGesture a_Interface)
        {
            Gesture t_RequiredGesture = gestureList.Where(x => x.GestureType == a_Type).FirstOrDefault<Gesture>();
            if (t_RequiredGesture == null)
            {
                t_RequiredGesture = createNewGesture(a_Type);
            }
            t_RequiredGesture.AddInterfaceObject(a_Interface);
            t_RequiredGesture.RayCastUI = rayCastUI;
        }

        // Unregister an interface from particular gesture
        public void UnRegisterFromGesture(EGestureType a_Type, IGesture a_Interface)
        {
            Gesture t_RequiredGesture = gestureList.Where(x => x.GestureType == a_Type).FirstOrDefault<Gesture>();
            if (t_RequiredGesture != null)
            {
                t_RequiredGesture.RemoveInterfaceObject(a_Interface);
                if (t_RequiredGesture.IGestureSubscribers.Count <= 0)
                    gestureList.Remove(t_RequiredGesture);
            }
        }

        // Unregister an interface from all gesture
        public void UnRegisterFromGestures(IGesture a_Interface)
        {
            List<Gesture> t_ListGestures = gestureList.Where(x => x.IGestureSubscribers.Contains(a_Interface)).ToList<Gesture>();
            for (int i = 0; i < t_ListGestures.Count; i++)
            {
                t_ListGestures[i].RemoveInterfaceObject(a_Interface);
            }

            t_ListGestures = gestureList.Where(x => x.IGestureSubscribers.Count <= 0).ToList<Gesture>();
            for (int i = 0; i < t_ListGestures.Count; i++)
            {
                gestureList.Remove(t_ListGestures[i]);
            }
        }

        // Create a new gesture type
        private Gesture createNewGesture(EGestureType a_Type)
        {
            Gesture t_Gesture = null;
            switch (a_Type)
            {
                case EGestureType.None:
                    Debug.Log("Gesture none");
                    break;
                case EGestureType.Tap:
                    t_Gesture = new GestureTap();
                    break;
                case EGestureType.TapAndHold:
                    t_Gesture = new GestureTapAndHold();
                    break;
                case EGestureType.Drag:
                    t_Gesture = new GestureDrag();
                    break;
                case EGestureType.Pinch:
                    t_Gesture = new GesturePinch();
                    break;
                case EGestureType.Rotate:
                    t_Gesture = new GestureRotate();
                    break;
                case EGestureType.Swipe:
                    t_Gesture = new GestureSwipe();
                    break;
            }

            if (t_Gesture != null)
            {
                gestureList.Add(t_Gesture);
            }

            return t_Gesture;
        }

        // Update is called once per frame
        private void Update()
        {
            float t_DeltaTime = Time.deltaTime;
            for (int i = 0; i < gestureList.Count; i++)
            {
                gestureList[i].CustomUpdate(t_DeltaTime);
            }
        }
    }
}