using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace DevCommon.Extended
{
    public sealed class SpinWheel : MonoBehaviour
    {
        private enum ESpinState
        {
            Idle = 0,
            Spinning,
            FinalSpinning,
        }

        private enum EDestinationState
        {
            None = 0,
            PreAssigned,
            PostAssigned,
        }

        [SerializeField] private Transform wheel;
        [SerializeField] private float spinSpeed = 400, minSpinSpeed = 40f;
        [SerializeField] [Range(1, 50)] private int spinRounds = 3;
        [SerializeField] private UnityEvent onStartSpinEvent, onStartFinalSpinEvent, onFinishSpinEvent;

        private ESpinState spinState = ESpinState.Idle;
        private EDestinationState destinationState = EDestinationState.None;

        private float itemDegree = 0, rotationSpin = 0, finalRotation = 0, finalSpinSpeed = 0;
        private int spinCount = 0, selectedItem = 0, totalSlots = 0;
        private bool clockwiseRotation = true;

        private List<int> itemChance = new List<int>();
        private Action<int> onFinishSpinCallback;

        public void SpinTheWheel(int a_TotalSlots, int a_DestinationSlot, Action<int> a_OnFinishSpinCallback = default, bool a_ClockwiseRotation = true)
        {
            if (spinState == ESpinState.Idle)
            {
                if (a_DestinationSlot < 0 || a_DestinationSlot > a_TotalSlots)
                {
                    Debug.LogError("Invalid Winning Slot!");
                    return;
                }

                destinationState = EDestinationState.PreAssigned;
                clockwiseRotation = a_ClockwiseRotation;
                totalSlots = a_TotalSlots;
                onFinishSpinCallback = a_OnFinishSpinCallback;

                itemDegree = (float)(360f / a_TotalSlots);
                setItemChances(a_TotalSlots, a_DestinationSlot);
                setFinalDestination();
                onStartSpinEvent?.Invoke();
            }
        }

        public void SpinTheWheel(int a_TotalSlots, Action<int> a_OnFinishSpinCallback = default, bool a_ClockwiseRotation = true)
        {
            if (spinState == ESpinState.Idle)
            {
                totalSlots = a_TotalSlots;
                if (!haveError())
                {
                    destinationState = EDestinationState.PostAssigned;
                    clockwiseRotation = a_ClockwiseRotation;
                    onFinishSpinCallback = a_OnFinishSpinCallback;

                    itemDegree = (float)(360f / a_TotalSlots);
                    spinState = ESpinState.Spinning;
                    onStartSpinEvent?.Invoke();
                }
            }
        }

        public void SetDestination(int a_DestinationSlot)
        {
            if (spinState == ESpinState.Spinning && destinationState == EDestinationState.PostAssigned)
            {
                if (a_DestinationSlot < 0 || a_DestinationSlot > totalSlots)
                {
                    Debug.LogError("Invalid Winning Slot!");
                    return;
                }

                setItemChances(totalSlots, a_DestinationSlot);
                setFinalDestination();

                spinCount = spinRounds - 2;
                rotationSpin = 0;
                finalSpinSpeed = spinSpeed;
                spinState = ESpinState.FinalSpinning;
                onStartFinalSpinEvent?.Invoke();
            }
        }

        private void Update()
        {
            if (destinationState == EDestinationState.PreAssigned)
            {
                preDefineSpin();
            }
            else if (destinationState == EDestinationState.PostAssigned)
            {
                postDefineSpin();
            }
        }

        private void preDefineSpin()
        {
            if (spinState == ESpinState.Spinning)
            {
                float t_Speed = spinSpeed;
                if (clockwiseRotation)
                {
                    rotationSpin -= (t_Speed * Time.deltaTime);
                    if (rotationSpin <= -360f)
                    {
                        rotationSpin += 360f;
                        spinCount++;
                        if (spinCount == spinRounds - 2)
                        {
                            rotationSpin = 0;
                            finalSpinSpeed = spinSpeed;
                            spinState = ESpinState.FinalSpinning;
                            onStartFinalSpinEvent?.Invoke();
                        }
                    }
                }
                else
                {
                    rotationSpin += (t_Speed * Time.deltaTime);
                    if (rotationSpin >= 360f)
                    {
                        rotationSpin -= 360f;
                        spinCount++;
                        if (spinCount == spinRounds - 2)
                        {
                            rotationSpin = 0;
                            finalSpinSpeed = spinSpeed;
                            spinState = ESpinState.FinalSpinning;
                        }
                    }
                }
                wheel.eulerAngles = new Vector3(0, 0, rotationSpin);
            }
            else if (spinState == ESpinState.FinalSpinning)
            {
                finalSpin();
            }
        }

        private void postDefineSpin()
        {
            if (spinState == ESpinState.Spinning)
            {
                float t_Speed = spinSpeed;
                if (clockwiseRotation)
                {
                    rotationSpin -= (t_Speed * Time.deltaTime);
                    if (rotationSpin <= -360f)
                    {
                        rotationSpin += 360f;
                    }
                }
                else
                {
                    rotationSpin += (t_Speed * Time.deltaTime);
                    if (rotationSpin >= 360f)
                    {
                        rotationSpin -= 360f;
                    }
                }
                wheel.eulerAngles = new Vector3(0, 0, rotationSpin);
            }
            else if (spinState == ESpinState.FinalSpinning)
            {
                finalSpin();
            }
        }

        private void finalSpin()
        {
            float t_CurrentProgress = 0;
            float t_FinalProgress = 0;

            if (clockwiseRotation)
            {
                t_CurrentProgress = ((2 - (spinRounds - spinCount)) * 360) - rotationSpin;
                t_FinalProgress = 720 - finalRotation;
            }
            else
            {
                t_CurrentProgress = ((2 - (spinRounds - spinCount)) * 360) + rotationSpin;
                t_FinalProgress = 720 + (finalRotation + 360);
            }

            float t_SpeedMult = 1 - (t_CurrentProgress / t_FinalProgress);
            finalSpinSpeed = ((spinSpeed - minSpinSpeed) * (t_SpeedMult)) + minSpinSpeed;

            if (clockwiseRotation)
            {
                rotationSpin -= (finalSpinSpeed * Time.deltaTime);
                if (spinCount < spinRounds)
                {
                    if (rotationSpin <= -360f)
                    {
                        rotationSpin += 360f;
                        spinCount++;
                    }
                }
                else
                {
                    if (rotationSpin <= finalRotation)
                    {
                        rotationSpin = finalRotation;
                        onCompleteSpin();
                    }
                }
            }
            else
            {
                rotationSpin += (finalSpinSpeed * Time.deltaTime);
                if (spinCount < spinRounds)
                {
                    if (rotationSpin >= 360f)
                    {
                        rotationSpin -= 360f;
                        spinCount++;
                    }
                }
                else
                {
                    if (rotationSpin >= finalRotation + 360)
                    {
                        rotationSpin = finalRotation + 360;
                        onCompleteSpin();
                    }
                }
            }
            wheel.eulerAngles = new Vector3(0, 0, rotationSpin);
        }

        private void onCompleteSpin()
        {
            spinState = ESpinState.Idle;
            destinationState = EDestinationState.None;
            onFinishSpinCallback?.Invoke(selectedItem + 1);
            onFinishSpinEvent?.Invoke();
        }

        private void setItemChances(int a_TotalSlots, int a_WinningSlot)
        {
            itemChance.Clear();
            for (int i = 0; i < a_TotalSlots; i++)
            {
                if (i == a_WinningSlot - 1)
                    itemChance.Add(100);
                else
                    itemChance.Add(0);
            }
        }

        private void setFinalDestination()
        {
            if (!haveError())
            {
                wheel.eulerAngles = Vector3.zero;
                spinCount = 0;
                rotationSpin = 0;
                selectedItem = UnityEngine.Random.Range(0, 1000);

                int t_AllChances = itemChance.Sum();
                float t_ChancePart = 1000f / t_AllChances;
                float t_CheckedChances = 0f;

                for (int i = 0; i < totalSlots; i++)
                {
                    t_CheckedChances += t_ChancePart * itemChance[i];
                    if (selectedItem < t_CheckedChances)
                    {
                        selectedItem = i;
                        break;
                    }
                }

                finalRotation = -(selectedItem * itemDegree) - (itemDegree / 2f);
                if (destinationState == EDestinationState.PreAssigned)
                {
                    spinState = ESpinState.Spinning;
                }
            }
        }

        private bool haveError()
        {
            bool t_IsError = false;
            if (totalSlots < 3)
            {
                Debug.LogError("Minimum Items Count Is 3.");
                t_IsError = true;
            }

            if (spinRounds < 3)
            {
                Debug.LogError("Minimum Spin Count Is 3.");
                t_IsError = true;
            }

            if (spinSpeed <= 0 || minSpinSpeed <= 0)
            {
                Debug.LogError("Negative speed or 0 value will not work.");
                t_IsError = true;
            }
            return t_IsError;
        }
    }
}