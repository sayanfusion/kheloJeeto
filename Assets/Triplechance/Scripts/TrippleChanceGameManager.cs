using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace tripplechance
{
    public class TrippleChanceGameManager : MonoBehaviour
    {
        [SerializeField] private SpinWheelNew innerWheel;
        [SerializeField] private SpinWheelNew outerWheel;
        [SerializeField] private SpinWheelNew middleWheel;

        [SerializeField] private Text timerText;

        [SerializeField] private int betTime;
        [SerializeField] private int stopWheelTime;

        private void Start()
        {
            StartCoroutine(StartBetTimer());
        }

        private IEnumerator StartBetTimer()
        {
            float time = 0;
            int secondCounter = betTime;
            timerText.text = secondCounter.ToString("00");
            while (secondCounter > 0)
            {
                time += Time.deltaTime;

                if(time >= 1f)
                {
                    time = 0;
                    secondCounter -= 1;
                    timerText.text = secondCounter.ToString("00");
                }
                yield return new WaitForEndOfFrame();
            }

            timerText.text = "00";
            innerWheel.SpinTheWheel();
            outerWheel.SpinTheWheel();
            middleWheel.SpinTheWheel();
            StartCoroutine(StopWheel());
        }

        private IEnumerator StopWheel()
        {
            float time = 0;
            int secondCounter = 0;
            //timerText.text = secondCounter.ToString("00");
            while (secondCounter <= stopWheelTime)
            {
                time += Time.deltaTime;

                if (time >= 1f)
                {
                    time = 0;
                    secondCounter += 1;
                    //timerText.text = secondCounter.ToString("00");

                    if(secondCounter == (stopWheelTime - 2f))
                    {
                        innerWheel.SetDestination(Random.Range(0, 8));
                        outerWheel.SetDestination(Random.Range(0, 8));
                        middleWheel.SetDestination(Random.Range(0, 8));
                    }
                }

                yield return new WaitForEndOfFrame();
            }

            timerText.text = "00";
            yield return new WaitForSeconds(5f);
            StartCoroutine(StartBetTimer());
        }
    }
}
