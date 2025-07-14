using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevCommon.Gesture
{
    public class GestureRotate : Gesture
    {
        public float Value { get; private set; } = 0;

        private Touch touchOne;
        private Touch touchTwo;
        private Vector3 startSlope;
        private Vector3 currentSlope;
        private bool gestureActive = false;

        public GestureRotate()
        {
            GestureType = EGestureType.Rotate;
        }

        // Update is called once per frame
        public override void CustomUpdate(float a_DeltaTime)
        {
            handleThroughTouch();
        }

        private void handleThroughTouch()
        {
            if (Input.touchCount == 2 && !gestureActive)
            {
                touchOne = Input.GetTouch(0);
                Vector3 t_TouchOnePosition = touchOne.position;

                touchTwo = Input.GetTouch(1);
                Vector3 t_TouchTwoPosition = touchTwo.position;

                bool t_TouchOneBlocksUI = checkPointerOverUIObject(t_TouchOnePosition);
                bool t_TouchTwoBlocksUI = checkPointerOverUIObject(t_TouchTwoPosition);

                if (!(t_TouchOneBlocksUI || t_TouchTwoBlocksUI))
                {
                    gestureActive = true;
                    startSlope = t_TouchOnePosition - t_TouchTwoPosition;
                    onGestureStarted(this);
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
                    touchOne = Input.GetTouch(0);
                    Vector3 t_TouchOnePosition = touchOne.position;

                    touchOne = Input.GetTouch(1);
                    Vector3 t_TouchTwoPosition = touchTwo.position;

                    currentSlope = t_TouchOnePosition - t_TouchTwoPosition;

                    float t_Dot = startSlope.x * currentSlope.x + startSlope.y * currentSlope.y;      // dot product
                    float t_Det = startSlope.x * currentSlope.y - startSlope.y * currentSlope.x;      // determinant
                    Value = Mathf.Atan2(t_Det, t_Dot) * Mathf.Rad2Deg;                                     // atan2(y, x) or atan2(sin, cos)
                    startSlope = currentSlope;
                    onReceivedGesture(this);
                }
            }
        }
    }
}