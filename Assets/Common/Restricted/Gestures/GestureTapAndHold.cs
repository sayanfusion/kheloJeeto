using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevCommon.Gesture
{
    public class GestureTapAndHold : Gesture
    {
        private Vector3 startPosition;
        private float tolerance = 5; //In pixels
        private float holdTime = 3.0f;
        private float count = 0;
        private bool touched = false;

        public GestureTapAndHold()
        {
            GestureType = EGestureType.TapAndHold;
        }

        // Update is called once per frame
        public override void CustomUpdate(float a_DeltaTime)
        {
            if (Input.GetMouseButtonDown(0))
            {
                touched = true;
                startPosition = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0) && touched)
            {
                onInteractionClosed();
            }

            if (touched)
            {
                Vector3 t_TouchPos = Input.mousePosition;
                if (Vector3.Distance(startPosition, t_TouchPos) > tolerance)
                {
                    onInteractionClosed();
                }
                else
                {
                    count += a_DeltaTime;
                    if (count >= holdTime)
                    {
                        onInteractionClosed();
                        onReceivedGesture(this);
                        return;
                    }
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