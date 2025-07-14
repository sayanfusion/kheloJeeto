using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevCommon.Gesture
{
    public class GesturePinch : Gesture
    {
        public float PinchValue { get; private set; } = 0;

        private bool gestureActive = false;
        private float startDifference = 0;

        public GesturePinch()
        {
            GestureType = EGestureType.Pinch;
        }

        // Update is called once per frame
        public override void CustomUpdate(float a_DeltaTime)
        {
            HandleThroughTouch();
        }

        void HandleThroughTouch()
        {
            if (Input.touchCount == 2 && !gestureActive)
            {
                Touch t_TouchOne = Input.GetTouch(0);
                Vector3 t_TouchOnePosition = t_TouchOne.position;

                Touch t_TouchTwo = Input.GetTouch(1);
                Vector3 t_TouchTwoPosition = t_TouchTwo.position;

                bool t_TouchOneBlocksUI = checkPointerOverUIObject(t_TouchOne.position);
                bool t_TouchTwoBlocksUI = checkPointerOverUIObject(t_TouchOne.position);

                if (!(t_TouchOneBlocksUI || t_TouchTwoBlocksUI))
                {
                    gestureActive = true;
                    startDifference = Vector3.Distance(t_TouchOnePosition, t_TouchTwoPosition);
                    onGestureStarted(this, Vector3.Lerp(t_TouchOnePosition, t_TouchTwoPosition, 0.5f));
                    return;
                }
            }
            else if (gestureActive)
            {
                if (Input.touchCount < 2)
                {
                    gestureActive = false;
                    onGestureEnded(this);
                    return;
                }
                else
                {
                    Touch t_TouchOne = Input.GetTouch(0);
                    Vector3 t_TouchOnePosition = t_TouchOne.position;

                    Touch t_TouchTwo = Input.GetTouch(1);
                    Vector3 t_TouchTwoPosition = t_TouchTwo.position;

                    float t_TouchDiff = Vector3.Distance(t_TouchOnePosition, t_TouchTwoPosition);

                    if (startDifference != 0)
                        PinchValue = t_TouchDiff / startDifference;

                    onReceivedGesture(this);
                }
            }
        }
    }
}