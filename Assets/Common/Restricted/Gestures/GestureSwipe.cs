using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace DevCommon.Gesture
{
    class CardinalDirection
    {
        public static readonly Vector2 Up = new Vector2(0, 1);
        public static readonly Vector2 Down = new Vector2(0, -1);
        public static readonly Vector2 Right = new Vector2(1, 0);
        public static readonly Vector2 Left = new Vector2(-1, 0);
        public static readonly Vector2 UpRight = new Vector2(1, 1);
        public static readonly Vector2 UpLeft = new Vector2(-1, 1);
        public static readonly Vector2 DownRight = new Vector2(1, -1);
        public static readonly Vector2 DownLeft = new Vector2(-1, -1);
    }

    public enum Swipe
    {
        None,
        Up,
        Down,
        Left,
        Right,
        UpLeft,
        UpRight,
        DownLeft,
        DownRight
    };

    public class GestureSwipe : Gesture
    {
        private Vector3 startPosition;

        private float minSwipeLength = 0.5f;
        private bool triggerSwipeAtMinLength = false;
        private bool useEightDirections = false;

        private const float eightDirAngle = 0.906f;
        private const float fourDirAngle = 0.5f;
        private const float defaultDPI = 72f;
        private const float dpcmFactor = 2.54f;

        private static Dictionary<Swipe, Vector2> a_CardinalDirections = new Dictionary<Swipe, Vector2>()
        {
            { Swipe.Up, CardinalDirection.Up },
            { Swipe.Down, CardinalDirection.Down },
            { Swipe.Right, CardinalDirection.Right },
            { Swipe.Left, CardinalDirection.Left },
            { Swipe.UpRight, CardinalDirection.UpRight },
            { Swipe.UpLeft, CardinalDirection.UpLeft },
            { Swipe.DownRight, CardinalDirection.DownRight },
            { Swipe.DownLeft, CardinalDirection.DownLeft }
        };

        private float dpcm;
        private float swipeStartTime;
        private float swipeEndTime;
        private bool autoDetectSwipes = false;
        private bool swipeEnded;
        private Swipe swipeDirection;
        private Vector2 firstPressPos;
        private Vector2 secondPressPos;
        private Vector2 swipeVelocity;

        public GestureSwipe()
        {
            GestureType = EGestureType.Swipe;

            float t_Dpi = (Screen.dpi == 0) ? defaultDPI : Screen.dpi;
            dpcm = t_Dpi / dpcmFactor;
            secondPressPos = Vector2.zero;
            firstPressPos = Vector2.zero;
            autoDetectSwipes = true;
        }

        // Update is called once per frame
        public override void CustomUpdate(float a_DeltaTime)
        {
            if (autoDetectSwipes)
            {
                detectSwipe();
            }
        }

        private void detectSwipe()
        {
            if (getTouchInput() || getMouseInput())
            {
                // Swipe already ended, don't detect until a new swipe has begun
                if (swipeEnded)
                {
                    return;
                }

                Vector2 t_CurrentSwipe = secondPressPos - firstPressPos;
                float t_SwipeCm = t_CurrentSwipe.magnitude / dpcm;

                // Check the swipe is long enough to count as a swipe (not a touch, etc)
                if (t_SwipeCm < minSwipeLength)
                {
                    // Swipe was not long enough, abort
                    if (!triggerSwipeAtMinLength)
                    {
                        if (Application.isEditor)
                        {
                            Debug.LogWarning("<color=#fcf003>[SwipeManager] Swipe was not long enough.</color>");
                        }

                        swipeDirection = Swipe.None;
                    }

                    return;
                }

                swipeEndTime = Time.time;
                swipeVelocity = t_CurrentSwipe * (swipeEndTime - swipeStartTime);
                swipeDirection = getSwipeDirByTouch(t_CurrentSwipe);
                swipeEnded = true;

                onReceivedGesture(this, swipeDirection);
            }
            else
            {
                swipeDirection = Swipe.None;
            }
        }

        public bool IsSwiping() { return swipeDirection != Swipe.None; }
        public bool IsSwipingRight() { return isSwipingDirection(Swipe.Right); }
        public bool IsSwipingLeft() { return isSwipingDirection(Swipe.Left); }
        public bool IsSwipingUp() { return isSwipingDirection(Swipe.Up); }
        public bool IsSwipingDown() { return isSwipingDirection(Swipe.Down); }
        public bool IsSwipingDownLeft() { return isSwipingDirection(Swipe.DownLeft); }
        public bool IsSwipingDownRight() { return isSwipingDirection(Swipe.DownRight); }
        public bool IsSwipingUpLeft() { return isSwipingDirection(Swipe.UpLeft); }
        public bool IsSwipingUpRight() { return isSwipingDirection(Swipe.UpRight); }

        #region Helper Functions
        private bool getTouchInput()
        {
            if (Input.touches.Length > 0)
            {
                Touch t_Touch = Input.GetTouch(0);

                // Swipe/Touch started
                if (t_Touch.phase == TouchPhase.Began)
                {
                    firstPressPos = t_Touch.position;
                    swipeStartTime = Time.time;
                    swipeEnded = false;
                    // Swipe/Touch ended
                }
                else if (t_Touch.phase == TouchPhase.Ended)
                {
                    secondPressPos = t_Touch.position;
                    return true;
                    // Still swiping/touching
                }
                else
                {
                    secondPressPos = t_Touch.position;
                    // Could count as a swipe if length is long enough
                    if (triggerSwipeAtMinLength)
                    {
                        return true;
                    }
                }
            }
            else
            {
                secondPressPos = Vector2.zero;
            }

            return false;
        }

        private bool getMouseInput()
        {
            // Swipe/Click started
            if (Input.GetMouseButtonDown(0))
            {
                firstPressPos = (Vector2)Input.mousePosition;
                swipeStartTime = Time.time;
                swipeEnded = false;
                // Swipe/Click ended
            }
            else if (Input.GetMouseButtonUp(0))
            {
                secondPressPos = (Vector2)Input.mousePosition;
                return true;
                // Still swiping/clicking
            }
            else
            {
                secondPressPos = (Vector2)Input.mousePosition;
                // Could count as a swipe if length is long enough
                if (triggerSwipeAtMinLength)
                {
                    return true;
                }
            }

            secondPressPos = Vector2.zero;

            return false;
        }

        private bool isDirection(Vector2 a_Direction, Vector2 a_CardinalDirection)
        {
            var t_Angle = useEightDirections ? eightDirAngle : fourDirAngle;
            return Vector2.Dot(a_Direction, a_CardinalDirection) > t_Angle;
        }

        private Swipe getSwipeDirByTouch(Vector2 a_CurrentSwipe)
        {
            a_CurrentSwipe.Normalize();
            var t_SwipeDir = a_CardinalDirections.FirstOrDefault(dir => isDirection(a_CurrentSwipe, dir.Value));
            return t_SwipeDir.Key;
        }

        private bool isSwipingDirection(Swipe a_SwipeDir)
        {
            detectSwipe();
            return swipeDirection == a_SwipeDir;
        }
        #endregion
    }
}