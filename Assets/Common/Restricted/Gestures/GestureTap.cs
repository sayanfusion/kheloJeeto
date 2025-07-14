using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevCommon.Gesture
{
    public class GestureTap : Gesture
    {
        private Vector3 startPosition;
        private Vector3 endPosition;
        private float tolerance = 10;
        private float holdTime = 0.3f;
        private float count = 0;
        private bool touched = false;

        public GestureTap()
        {
            GestureType = EGestureType.Tap;
        }

        // Update is called once per frame
        public override void CustomUpdate(float a_DeltaTime)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!checkPointerOverUIObject())
                {
                    touched = true;
                    startPosition = Input.mousePosition;
                }
            }
            else if (Input.GetMouseButtonUp(0) && touched)
            {
                onInteractionClosed();
                endPosition = Input.mousePosition;
                if (Vector3.Distance(startPosition, endPosition) <= tolerance)
                {
                    onReceivedGesture(this);
                    return;
                }
            }

            if (touched)
            {
                count += a_DeltaTime;
                if (count >= holdTime)
                {
                    onInteractionClosed();
                }
            }
        }

        private void onInteractionClosed()
        {
            count = 0.0f;
            touched = false;
        }
    }
}