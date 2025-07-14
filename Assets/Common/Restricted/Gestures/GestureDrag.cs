using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevCommon.Gesture
{
    public class GestureDrag : Gesture
    {
        public Vector3 DragValue { get => dragValue; }

        private Vector3 dragValue = new Vector3(0, 0, 0);
        private Vector3 touchStart = new Vector3(0, 0, 0);
        private bool useTouch = false;
        private bool touchActive;

        public GestureDrag()
        {
            useTouch = (SystemInfo.deviceType == DeviceType.Handheld);
            GestureType = EGestureType.Drag;
        }

        // Update is called once per frame
        public override void CustomUpdate(float a_DeltaTime)
        {
            if (!useTouch)
            {
                handleMouseClick();
            }
            else
            {
                handleTouch();
            }
        }

        private void handleTouch()
        {
            if (Input.touchCount == 1 && !touchActive)
            {
                Touch t_TouchOne = Input.GetTouch(0);
                if (!checkPointerOverUIObject(t_TouchOne.position) && t_TouchOne.phase == TouchPhase.Began)
                {
                    touchStart = t_TouchOne.position;
                    touchActive = true;
                    onGestureStarted(this);
                    return;
                }
            }

            if (touchActive)
            {
                if (Input.touchCount != 1)
                {
                    touchActive = false;
                    onGestureEnded(this);
                    return;
                }
                else
                {
                    Touch t_TouchOne = Input.GetTouch(0);
                    Vector3 t_CurPosition = t_TouchOne.position;
                    dragValue.x = t_CurPosition.x - touchStart.x;
                    dragValue.y = t_CurPosition.y - touchStart.y;
                    touchStart = t_CurPosition;
                    onReceivedGesture(this);
                    return;
                }
            }
        }

        private void handleMouseClick()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!checkPointerOverUIObject())
                {
                    touchStart = Input.mousePosition;
                    dragValue.x = 0;
                    dragValue.y = 0;
                    touchActive = true;
                }
            }

            if (touchActive)
            {
                Vector3 t_CurPosition = Input.mousePosition;
                dragValue.x = t_CurPosition.x - touchStart.x;
                dragValue.y = t_CurPosition.y - touchStart.y;
                onReceivedGesture(this);
                touchStart = t_CurPosition;
            }

            if (Input.GetMouseButtonUp(0))
            {
                dragValue.x = 0;
                dragValue.y = 0;
                touchActive = false;
            }
        }
    }
}