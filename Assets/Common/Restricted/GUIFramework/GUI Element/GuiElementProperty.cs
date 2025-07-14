using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DevCommon.GUI
{
    [DisallowMultipleComponent]
    public class GuiElementProperty : MonoBehaviour
    {
        [Tooltip("If true, GUI Framework will ignore this element while Refreshing GUI Assembly.")]
        [SerializeField] private bool ignored = false;

        [Tooltip("This can be achieved by enabling Read / Write enabled in the advanced Texture Import Settings for the sprite and disabling atlassing for the sprite.")]
        [SerializeField] private bool modifyAlphaHitThreshold = false;

        [ConditionalField(nameof(modifyAlphaHitThreshold), true)]
        [Range(0.0f, 1.0f)]
        [SerializeField] private float alphaHitMinimumThreshold = 1.0f;

        public bool Ignored { get => ignored; }

        private void Start()
        {
            updateAlphaHitThreshold();
        }

        // This can be achieved by enabling Read / Write enabled in the advanced Texture Import Settings for the sprite and disabling atlassing for the sprite.
        private void updateAlphaHitThreshold()
        {
            if (modifyAlphaHitThreshold)
            {
                Image t_ObjImage = GetComponent<Image>();
                if (t_ObjImage != null)
                    t_ObjImage.alphaHitTestMinimumThreshold = alphaHitMinimumThreshold;
            }
        }
    }
}