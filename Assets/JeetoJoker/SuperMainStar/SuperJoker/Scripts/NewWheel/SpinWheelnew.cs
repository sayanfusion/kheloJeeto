using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.Events;
using khelojeetonew;
using UnityEngine.SceneManagement;

public class SpinWheelnew : MonoBehaviour
{
    [SerializeField] private Transform wheel;
    [SerializeField] public float spinSpeed = 400;
    [SerializeField] private float minSpinSpeed = 40f;

    //[Range(1, 50)]
    [SerializeField] private int spinRounds = 7;
    [SerializeField] private bool clockwiseRotation = true;

    private bool isSpinning = false;
    private bool isSpinningFinal = false;
    private float itemDegree = 0;
    private int spinCount = 0;
    private float rotationSpin = 0;
    private float finalRotation;
    private float finalSpinSpeed;

    private int totalSlots = 0;
    private List<int> itemChance = new List<int>();
    private int selectedItem;
    private Action<int> onComplete;
   
    public AudioSource audioSourcerunning;
    public UnityAction OnWheelStart;
    public UnityAction OnWheelStop;
    public bool isStop = true;
    public enum type
    {
        innerwheel,
        outerwheel
    }

    public type wheeltype;
    public void DirectlySetDestination(int a_TotalSlots, int a_WinningSlot)
    {
        Debug.Log($"TotalSlots: {a_TotalSlots}, WinningSlot: {a_WinningSlot}");

        totalSlots = a_TotalSlots;
        setItemChances(totalSlots, a_WinningSlot);
        itemDegree = (float)(360f / itemChance.Count);
        setFinalDestination();

        float t_FinalProgress = 0;

        if (clockwiseRotation)
        {
            t_FinalProgress = finalRotation;
        }
        else
        {
            t_FinalProgress = (finalRotation + 360);
        }
        Debug.Log("finalRotation rotation" + t_FinalProgress);
        wheel.eulerAngles = new Vector3(0, 0, t_FinalProgress);
    }

    public void SpinTheWheel(int a_TotalSlots, Action<int> a_OnCompleteCallback = null)
    {
        Debug.Log("a_TotalSlots "+ a_TotalSlots);
        Debug.Log("isWheelSpinning() " + isWheelSpinning());
        if (!isWheelSpinning())
        {
            Debug.Log($"Spin {gameObject.name} Wheel.");
            wheel.eulerAngles = Vector3.zero;
            totalSlots = a_TotalSlots;
            onComplete = a_OnCompleteCallback;
            wheelSpinStarted();

            //finalSpinSpeed = spinSpeed;
            isSpinningFinal = false;
            isSpinning = true;
        }
    }

    public void AssignWinningSlot(int a_WinningSlot)
    {
        if (isWheelSpinning())
        {
            Debug.Log($"AssignWinningSlot in: {gameObject.name}, WinningSlot: {a_WinningSlot}");
            if (a_WinningSlot < 0 || a_WinningSlot > totalSlots)
            {
                Debug.LogError("Invalid Winning Slot!");
                return;
            }

            setItemChances(totalSlots, a_WinningSlot);
            itemDegree = (float)(360f / itemChance.Count);
            setFinalDestination();

            isSpinning = false;
            isSpinningFinal = true;
        }
    }

    private void LateUpdate()
    {
        if (isSpinning)
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
        else if (isSpinningFinal)
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
                        isSpinningFinal = false;
                        onComplete?.Invoke(selectedItem + 1);
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
                        isSpinningFinal = false;
                        onComplete?.Invoke(selectedItem + 1);
                    }
                }
            }
            wheel.eulerAngles = new Vector3(0, 0, rotationSpin);
        }
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

    public bool isWheelSpinning()
    {
        if (isSpinning || isSpinningFinal) return true;
        return false;
    }

    private void setFinalDestination()
    {
        if (itemChance.Count < 3)
        {
            Debug.LogError("Minimum Items Count Is 3.");
            return;
        }
        if (spinSpeed <= 0 || minSpinSpeed <= 0)
        {
            Debug.LogError("Negative speed or 0 value will not work.");
            return;
        }

        
        wheel.eulerAngles = Vector3.zero;
        spinCount = 0;
        rotationSpin = 0;
        selectedItem = UnityEngine.Random.Range(0, 1000);

        int t_AllChances = itemChance.Sum();
        float t_ChancePart = 1000f / t_AllChances;
        float t_CheckedChances = 0f;

        for (int i = 0; i < itemChance.Count; i++)
        {
            t_CheckedChances += t_ChancePart * itemChance[i];
            if (selectedItem < t_CheckedChances)
            {
                selectedItem = i;
                break;
            }
        }

        finalRotation = -(selectedItem * itemDegree) - (itemDegree / 2f);
    }
   
    public void WheelSpinStoppped()
    {
        isStop = true;
        if (SceneManager.GetActiveScene().name == "jokerScenePC")
            TimerControllerNew.inst.isWheelRunning = false;
        else
            sixteencard_Timer.inst.isWheelRunning = false;


        OnWheelStop?.Invoke();
    }

    public void StopInnerWheel()
    {
        Invoke("WheelSpinStoppped", 120f);
    }

    private void wheelSpinStarted()
    {
        isStop = false;
        if (SceneManager.GetActiveScene().name == "jokerScenePC")
            TimerControllerNew.inst.isWheelRunning = false;
        else
            sixteencard_Timer.inst.isWheelRunning = false;


            
        OnWheelStart?.Invoke();
    }
}

