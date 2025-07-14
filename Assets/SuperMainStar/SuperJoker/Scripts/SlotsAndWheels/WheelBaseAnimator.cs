using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SuperJoker {
    public class WheelBaseAnimator : MonoBehaviour {

        WheelBase wheelBase;
        Animator animator;


        private void Awake()
        {
            wheelBase = GetComponent<WheelBase>();
            animator = GetComponent<Animator>();
            Subscribe();
        }

        void Subscribe()
        {
            if (wheelBase)
            {
                wheelBase.WheelStart += PlayAnimation;
                wheelBase.WheelStop += StopAnimation;
            }
        }

        public void PlayAnimation()
        {            
            animator.SetBool("blink", true);
        }

        public void StopAnimation()
        {
            animator.SetBool("blink", false);           
        }

        private void OnDestroy()
        {
            if (wheelBase)
            {
                wheelBase.WheelStart -= PlayAnimation;
                wheelBase.WheelStop -= StopAnimation;
            }
        }
    }
}
