using UnityEngine;
using UnityEngine.UI;

namespace DevCommon.GUI
{
    [DisallowMultipleComponent]
    public class RadialLayout : LayoutGroup
    {
        [SerializeField] private float distance;
        [Range(0f, 360f)]
        [SerializeField] private float minAngle, maxAngle, startAngle;

        protected override void OnEnable() { base.OnEnable(); calculateRadial(); }

        public override void SetLayoutHorizontal() { }

        public override void SetLayoutVertical() { }

        public override void CalculateLayoutInputVertical() => calculateRadial();

        public override void CalculateLayoutInputHorizontal() => calculateRadial();

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            calculateRadial();
        }
#endif

        private void calculateRadial()
        {
            m_Tracker.Clear();
            if (transform.childCount == 0)
                return;

            float t_OffsetAngle = ((maxAngle - minAngle)) / (transform.childCount - 1);
            float t_Angle = startAngle;

            for (int i = 0; i < transform.childCount; i++)
            {
                RectTransform t_Child = (RectTransform)transform.GetChild(i);
                if (t_Child != null)
                {
                    //Adding the elements to the tracker stops the user from modifiying their positions via the editor.
                    m_Tracker.Add(this, t_Child,
                    DrivenTransformProperties.Anchors |
                    DrivenTransformProperties.AnchoredPosition |
                    DrivenTransformProperties.Pivot);
                    Vector3 t_Pos = new Vector3(Mathf.Cos(t_Angle * Mathf.Deg2Rad), Mathf.Sin(t_Angle * Mathf.Deg2Rad), 0);
                    t_Child.localPosition = t_Pos * distance;
                    //Force objects to be center aligned, this can be changed however I'd suggest you keep all of the objects with the same anchor points.
                    t_Child.anchorMin = t_Child.anchorMax = t_Child.pivot = new Vector2(0.5f, 0.5f);

                    // LookAt Center
                    //child.LookAt(this.rectTransform.position);
                    //Vector3 relative = transform.InverseTransformPoint(this.rectTransform.position);
                    //float angle = Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg;
                    //Debug.Log("Angle: " + -angle);
                    //child.rotation = Quaternion.Euler(0, 0, -angle);

                    t_Angle += t_OffsetAngle;
                }
            }
        }
    }
}