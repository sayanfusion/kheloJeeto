using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using System.Linq;

namespace DevCommon.GUI
{
    [DisallowMultipleComponent]
    public sealed class CanvasAnimation : MonoBehaviour
    {
        private int entryAnimationCallbackCount = 0;
        private int exitAnimationCallbackCount = 0;

        private Action onEntryAnimationComplete;
        private Action onExitAnimationComplete;

        // Play entry animation
        public void OnEntry(Action a_OnComplete)
        {
            List<DOTweenAnimation> t_EntryAnimations = GetComponentsInChildren<DOTweenAnimation>().Where(x => x.AnimationMode.Equals(DOTweenAnimation.EAnimationMode.OnEntry)).ToList();
            entryAnimationCallbackCount = t_EntryAnimations.Count();
            if (t_EntryAnimations.Any())
            {
                onEntryAnimationComplete = a_OnComplete;
                t_EntryAnimations.ForEach(x =>
                {
                    x.DORestartById(x.AnimationMode.ToString());
                    x.onComplete.AddListener(OnEntryCompleteAnimation);
                });
            }
            else
            {
                //No animating objects
                a_OnComplete?.Invoke();
            }
        }

        // Play exit animation
        public void OnExit(Action a_OnComplete)
        {
            List<DOTweenAnimation> t_ExitAnimations = GetComponentsInChildren<DOTweenAnimation>().Where(x => x.AnimationMode.Equals(DOTweenAnimation.EAnimationMode.OnExit)).ToList();
            exitAnimationCallbackCount = t_ExitAnimations.Count();

            if (t_ExitAnimations.Any())
            {
                onExitAnimationComplete = a_OnComplete;
                t_ExitAnimations.ForEach(x =>
                {
                    x.DORestartById(x.AnimationMode.ToString());
                    x.onComplete.AddListener(OnExitCompleteAnimation);
                });
            }
            else
            {
                //No animating objects
                a_OnComplete?.Invoke();
            }
        }

        // Rewind animation state
        public void OnRewind(Action a_OnComplete)
        {
            List<DOTweenAnimation> t_AllAnimations = GetComponentsInChildren<DOTweenAnimation>().ToList();
            if (t_AllAnimations.Any())
            {
                t_AllAnimations.ForEach(x =>
                {
                    if (x.isActive)
                        x.DORewind();
                });
            }
            a_OnComplete?.Invoke();
        }

        // On enty animation complete invoke onComplete callback
        private void OnEntryCompleteAnimation()
        {
            entryAnimationCallbackCount--;
            if (entryAnimationCallbackCount == 0)
            {
                onEntryAnimationComplete?.Invoke();
            }
        }

        // On exit animation complete invoke onComplete callback
        private void OnExitCompleteAnimation()
        {
            exitAnimationCallbackCount--;
            if (exitAnimationCallbackCount == 0)
            {
                onExitAnimationComplete?.Invoke();
            }
        }

        // Stop all playing animation
        public void OnStop(Action a_OnComplete)
        {
            List<DOTweenAnimation> t_AllAnimations = GetComponentsInChildren<DOTweenAnimation>().ToList();
            if (t_AllAnimations.Any())
            {
                t_AllAnimations.ForEach(x =>
                {
                    if (x.isActive)
                        x.DOComplete(true);
                });
            }
            a_OnComplete?.Invoke();
        }
    }
}
