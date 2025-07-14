using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace DevCommon.Extended
{
    public enum TimeFormat
    {
        SS,
        MM_SS,
        HH_MM_SS,
    }

    [DisallowMultipleComponent]
    public class CountdownTimer : MonoBehaviour
    {
        private enum ETimerState
        {
            Idle = 0,
            CountdownActive,
        }

        private ETimerState timerState = ETimerState.Idle;
        private Action<string> onTimerUpdate;
        private Action onCountdownFinished;

        private double millisecondsLeft = 0;
        private TimeFormat timeFormat;

        private bool autoDestroy = false;

        public void StartCountdown(double a_Milliseconds, Action<string> a_OnTimerUpdate, Action a_OnCountdownFinished, TimeFormat a_TimeFormat = TimeFormat.MM_SS, bool a_AutoDestroy = false)
        {
            timeFormat = a_TimeFormat;
            onTimerUpdate = a_OnTimerUpdate;
            onCountdownFinished = a_OnCountdownFinished;
            autoDestroy = a_AutoDestroy;
            timerState = ETimerState.CountdownActive;
            millisecondsLeft = a_Milliseconds;
        }

        public void StopCountdown()
        {
            timerState = ETimerState.Idle;
            onTimerUpdate?.Invoke(getFormatedTime(0, timeFormat));

            onTimerUpdate = null;
            onCountdownFinished = null;

            autoDestroyObj();
        }

        private void Update()
        {
            countdown();
        }

        private void countdown()
        {
            if (timerState == ETimerState.CountdownActive)
            {
                millisecondsLeft -= Time.deltaTime;

                if (millisecondsLeft > 0.0f)
                    onTimerUpdate?.Invoke(getFormatedTime(millisecondsLeft, timeFormat));

                if (millisecondsLeft <= 0.0f)
                    countdownFinished();
            }
        }

        private void countdownFinished()
        {
            timerState = ETimerState.Idle;
            onTimerUpdate?.Invoke(getFormatedTime(0, timeFormat));
            onCountdownFinished?.Invoke();

            autoDestroyObj();
        }

        private void autoDestroyObj()
        {
            if (autoDestroy)
            {
                GameObject t_Go = GetComponent<GameObject>();
                if (t_Go != null)
                    Destroy(this.gameObject);
                else
                    Destroy(this);
            }
        }

        private string getFormatedTime(double a_Milliseconds, TimeFormat a_TimeFormat)
        {
            //@"hh\:mm\:ss\:fff"
            switch (a_TimeFormat)
            {
                case TimeFormat.SS: return TimeSpan.FromSeconds(a_Milliseconds).ToString(@"ss");
                case TimeFormat.MM_SS: return TimeSpan.FromSeconds(a_Milliseconds).ToString(@"mm\:ss");
                case TimeFormat.HH_MM_SS: return TimeSpan.FromSeconds(a_Milliseconds).ToString(@"hh\:mm\:ss");
                default: return "";
            }
        }
    }
}
