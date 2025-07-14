using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public sealed class SpinWheelNew : MonoBehaviour
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
    [SerializeField] private float spinSpeed, minSpinSpeed;
    [SerializeField][Range(1, 50)] private int spinRounds = 2, totalSlots = 0;
    [SerializeField] private UnityEvent onStartSpinEvent, onStartFinalSpinEvent, onFinishSpinEvent;
    [SerializeField] private bool randomNoSpinWheel = false, clockwiseRotation = false;
    [SerializeField] private List<int> noInWheelSerialWise;
    [SerializeField] public AudioSource finishSound;


    private ESpinState spinState = ESpinState.Idle;
    private EDestinationState destinationState = EDestinationState.None;

    private float itemDegree = 0, rotationSpin = 0, finalRotation = 0, finalSpinSpeed = 0;
    private int spinCount = 0, selectedItem = 0, destinationSlot = 0;

    private List<int> itemChance = new List<int>();
    private Dictionary<int, float> wheelSlotValues = new Dictionary<int, float>();
    private Action<int> onFinishSpinCallback;

    public int TotalSlots { get { return randomNoSpinWheel ? noInWheelSerialWise.Count : totalSlots; } }

    private void Awake()
    {
        calCulateAngleForSlots();
        finishSound = GetComponent<AudioSource>();
    }

    private void calCulateAngleForSlots()
    {
        totalSlots = randomNoSpinWheel ? noInWheelSerialWise.Count : totalSlots;
        itemDegree = 360f / totalSlots;
        if (randomNoSpinWheel)
        {
            for (int i = 0; i < totalSlots; i++)
            {
                if (!wheelSlotValues.ContainsKey(noInWheelSerialWise[i]))
                {
                    wheelSlotValues.Add(noInWheelSerialWise[i], clockwiseRotation ? 360 - (itemDegree * i) : itemDegree * i);
                }

            }
        }
        else
        {
            for (int i = 0; i < totalSlots; i++)
            {
                if (!wheelSlotValues.ContainsKey(i))
                {
                    wheelSlotValues.Add(i, clockwiseRotation ? 360 - (itemDegree * i) : itemDegree * (i - 1));
                }

            }
        }
    }

    public void SpinTheWheel(int a_DestinationSlot)
    {
        if (spinState == ESpinState.Idle)
        {
            if (a_DestinationSlot < 0 || a_DestinationSlot > totalSlots)
            {
                Debug.LogError("Invalid Winning Slot!");
                return;
            }
            destinationSlot = a_DestinationSlot;
            destinationState = EDestinationState.PreAssigned;
            setItemChances(totalSlots, a_DestinationSlot);
            if (randomNoSpinWheel)
                setFinalDestinationForRandom();
            else
                setFinalDestination();
            onStartSpinEvent?.Invoke();
        }
    }

    public SpinWheelNew SpinTheWheel()
    {
        if (spinState == ESpinState.Idle)
        {
            if (!haveError())
            {
                Debug.Log("started spinning: ");
                destinationState = EDestinationState.PostAssigned;
                //onFinishSpinCallback = a_OnFinishSpinCallback;
                spinState = ESpinState.Spinning;
                onStartSpinEvent?.Invoke();
            }
        }
        return this;
    }

    public SpinWheelNew OnComplete(Action<int> a_OnFinishSpinCallback)
    {
        onFinishSpinCallback = a_OnFinishSpinCallback;
        return this;
    }

    public SpinWheelNew SetDestination(int a_DestinationSlot)
    {

        if (spinState == ESpinState.Spinning && destinationState == EDestinationState.PostAssigned)
        {
            if (a_DestinationSlot < 0 || a_DestinationSlot > totalSlots)
            {
                Debug.LogError("Invalid Winning Slot!");
                return default;
            }

            Debug.Log("Destination Slot: " + a_DestinationSlot);

            destinationSlot = a_DestinationSlot;
            setItemChances(totalSlots, a_DestinationSlot);
            setFinalDestinationForRandom();
            spinCount = 0;
            rotationSpin = 0;
            finalSpinSpeed = spinSpeed;
            spinState = ESpinState.FinalSpinning;
            onStartFinalSpinEvent?.Invoke();

        }
        return this;
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
            wheel.localEulerAngles = new Vector3(0, 0, rotationSpin);
        }
        else if (spinState == ESpinState.FinalSpinning)
        {
           // finalSpin();
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

    private int speedOverTime = 0;

    private void finalSpin()
    {
        Debug.Log("Final Spin");
        float t_CurrentProgress = 0;
        float t_FinalProgress = 0;

        if (clockwiseRotation)
        {
            t_CurrentProgress = ((2 - (spinRounds - spinCount)) * -finalRotation) - rotationSpin;
            t_FinalProgress = -finalRotation;
            //Debug.Log(gameObject.name + "((2 - (spinRounds - spinCount)) * 360) - rotationSpin " + (((2 - (spinRounds - spinCount)) * 360) - rotationSpin) + "Spinrounds: " + spinRounds + " " + spinCount + " " + rotationSpin + " "+finalRotation);
        }
        else
        {
            t_CurrentProgress = ((2 - (spinRounds - spinCount)) * finalRotation) + rotationSpin;
            t_FinalProgress = finalRotation;//720 + (finalRotation + 360);
            //Debug.Log(gameObject.name + "((2 - (spinRounds - spinCount)) * 360) - rotationSpin " + ((2 - (spinRounds - spinCount)) * finalRotation) + rotationSpin + "Spinrounds: " + spinRounds + " " + spinCount + " " + rotationSpin + " " + finalRotation);
        }
        float t_SpeedMult = 0;
        if ((gameObject.name == "OuterCircle" && (destinationSlot == 9 || destinationSlot == 6)) || (gameObject.name == "InnerCircle" && (destinationSlot == 7 || destinationSlot == 4)))
        {
            Debug.Log("Speed Up NAME" + gameObject.name);
            t_SpeedMult = 1 - Mathf.Pow(t_CurrentProgress / t_FinalProgress, 8);
        }
        else
        {
            t_SpeedMult = 1 - (t_CurrentProgress / t_FinalProgress);
        }
        finalSpinSpeed = ((spinSpeed - minSpinSpeed) * t_SpeedMult) + minSpinSpeed;
        //Debug.Log(gameObject.name + "(spinSpeed - minSpinSpeed): " + (spinSpeed - minSpinSpeed) + "  " + "minSpinSpeed: " + minSpinSpeed+ "Current Progress: "+t_CurrentProgress);

        //Debug.Log(gameObject.name+"Speed Multiplier: " + t_SpeedMult + "  " + "Final Spin Speed: " + finalSpinSpeed+ "final Progress: " + t_FinalProgress);

        if (clockwiseRotation)
        {
            rotationSpin -= (finalSpinSpeed * Time.deltaTime);
            if (rotationSpin <= finalRotation)
            {
                rotationSpin = finalRotation;
                onCompleteSpin();
            }
            //Debug.Log(gameObject.name + "rotationSpin: " + rotationSpin + " " + finalRotation);
        }
        else
        {
            rotationSpin += (finalSpinSpeed * Time.deltaTime);
            if (rotationSpin >= finalRotation)
            {
                rotationSpin = finalRotation;
                onCompleteSpin();
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
        onFinishSpinCallback = null;
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

    private void setFinalDestinationForRandom()
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
            //Debug.Log("Final Rotation: " +gameObject.name+" "+(clockwiseRotation ? -wheelSlotValues[destinationSlot] : wheelSlotValues[destinationSlot]));
            //Debug.Log("Final Rotation after change: " + gameObject.name + " " + (clockwiseRotation ? -wheelSlotValues[destinationSlot] > -180 ? -wheelSlotValues[destinationSlot] - 360 : -wheelSlotValues[destinationSlot] : wheelSlotValues[destinationSlot] < 180 ? wheelSlotValues[destinationSlot] + 360 : wheelSlotValues[destinationSlot]));
            finalRotation = clockwiseRotation ? (-wheelSlotValues[destinationSlot] > -180 ? (-wheelSlotValues[destinationSlot] - 360) : (-wheelSlotValues[destinationSlot])) : wheelSlotValues[destinationSlot] < 180 ? wheelSlotValues[destinationSlot] + 360 : wheelSlotValues[destinationSlot];
            Debug.Log("Final Rotation :" + finalRotation);
            if (destinationState == EDestinationState.PreAssigned)
            {
                spinState = ESpinState.Spinning;
            }
        }
    }

    public void SetDestinationDirectly(int destinationSlot)
    {
        finalRotation = clockwiseRotation ? -wheelSlotValues[destinationSlot] : wheelSlotValues[destinationSlot];
        wheel.transform.Rotate(new Vector3(0, 0, finalRotation));
    }

    private bool haveError()
    {
        bool t_IsError = false;
        if (totalSlots < 2)
        {
            Debug.LogError("Minimum Items Count Is 2.");
            t_IsError = true;
        }

        if (spinRounds < 2)
        {
            Debug.LogError("Minimum Spin Count Is 2.");
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
