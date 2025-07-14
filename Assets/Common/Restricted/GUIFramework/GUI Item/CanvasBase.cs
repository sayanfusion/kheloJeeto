using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevCommon.GUI
{
    [RequireComponent(typeof(CanvasAnimation))]
    [RequireComponent(typeof(CanvasGroup))]
    [DisallowMultipleComponent]
    public class CanvasBase : MonoBehaviour
    {
        public CanvasController Controller { get; set; }

        protected Action onCompleteEntryAnimation;
        protected Action onCompleteExitAnimation;
        protected Action onStopAllAnimation;

        // Called to reset the canvas anchor
        public void ResetCanvasAnchor()
        {
            RectTransform t_Rect = GetComponent<RectTransform>();
            t_Rect.anchoredPosition = Vector2.zero;
        }

        // Enable a canvas interaction
        public void EnableCanvasInteraction()
        {
            CanvasGroup t_CanvasGroup = GetComponent<CanvasGroup>();
            t_CanvasGroup.alpha = 1.0f;
            t_CanvasGroup.blocksRaycasts = true;
            t_CanvasGroup.interactable = true;
        }

        // Disable a canvas interaction
        public void DisableCanvasInteraction()
        {
            CanvasGroup t_CanvasGroup = GetComponent<CanvasGroup>();
            t_CanvasGroup.alpha = 0.0f;
            t_CanvasGroup.blocksRaycasts = false;
            t_CanvasGroup.interactable = false;
        }

        // Put this canvas on top of other canvas
        public void PutOnTop()
        {
            this.transform.SetAsLastSibling();
        }

        // Called to reset the sorting order
        public void ResetSortingOrder()
        {
            Canvas t_Canvas = GetComponent<Canvas>();
            t_Canvas.sortingOrder = 0;
            t_Canvas.overrideSorting = false;
        }

        // Called to set the sorting order
        public void SetSortingOrder(int a_Order)
        {
            Canvas t_Canvas = GetComponent<Canvas>();
            t_Canvas.overrideSorting = true;
            t_Canvas.sortingOrder = a_Order;
        }

        // Start the entry animation
        public void BeginEntryAnimation(Action a_OnComplete)
        {
            CanvasAnimation t_UiAnimation = GetComponent<CanvasAnimation>();
            if (t_UiAnimation != null)
            {
                onCompleteEntryAnimation = a_OnComplete;
                t_UiAnimation.OnEntry(() =>
                {
                    onCompleteEntryAnimation?.Invoke();
                    OnCompleteLoading();
                });
            }
            else
            {
                a_OnComplete?.Invoke();
            }
        }

        // Start the exit animation
        public void BeginExitAnimation(Action a_OnComplete)
        {
            CanvasAnimation t_UiAnimation = GetComponent<CanvasAnimation>();
            if (t_UiAnimation != null)
            {
                onCompleteExitAnimation = a_OnComplete;
                t_UiAnimation.OnExit(() =>
                {
                    onCompleteExitAnimation?.Invoke();
                    OnCompleteUnLoading();
                });
            }
            else
            {
                a_OnComplete?.Invoke();
            }
        }

        // Rewind to original state
        public void BeginRewindAnimationState(Action a_OnComplete)
        {
            CanvasAnimation t_UiAnimation = GetComponent<CanvasAnimation>();
            if (t_UiAnimation != null)
            {
                t_UiAnimation.OnRewind(a_OnComplete);
            }
            else
            {
                a_OnComplete?.Invoke();
            }
        }

        // Stop all animations
        public void StopAllAnimation(Action a_OnComplete)
        {
            CanvasAnimation t_UiAnimation = GetComponent<CanvasAnimation>();
            if (t_UiAnimation != null)
            {
                onStopAllAnimation = a_OnComplete;
                t_UiAnimation.OnStop(() =>
                {
                    onStopAllAnimation?.Invoke();
                    OnStopAllAnimation();
                });
            }
            else
            {
                a_OnComplete?.Invoke();
            }
        }

        private void Reset() => OnReset();

        #region Initialize Methods
        // On Reset canvas
        protected virtual void OnReset() { }

        // On Initialize canvas
        public virtual void OnInitialize() { }

        // On DeInitialize canvas
        public virtual void OnDeInitialize() { }
        #endregion

        #region Animation Event Methods
        // On complete loading page
        protected virtual void OnCompleteLoading() { }

        // On complete unloading page
        protected virtual void OnCompleteUnLoading() { }

        // On stop all animation
        protected virtual void OnStopAllAnimation() { }
        #endregion
    }
}