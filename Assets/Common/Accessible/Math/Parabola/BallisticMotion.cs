using UnityEngine;

namespace DevCommon.Math.Parabola
{
    [DisallowMultipleComponent]
    public sealed class BallisticMotion : MonoBehaviour
    {
        private Transform m_Transform;
        private Vector3 m_LastPos = new Vector3(0, 0, 0);
        private Vector3 m_Impulse = new Vector3(0, 0, 0);
        private float m_Gravity = 0.0f;
        private bool m_IsActive = false;

        private void Awake()
        {
            m_Transform = transform;
            m_LastPos = transform.position;
        }

        public void Initialize(Vector3 pos, float gravity)
        {
            m_IsActive = true;
            m_Transform.position = pos;
            m_LastPos = m_Transform.position;
            m_Gravity = gravity;
        }

        public void StopMotion()
        {
            m_IsActive = false;
            m_Impulse = Vector3.zero;
        }

        void FixedUpdate()
        {
            if (m_IsActive)
            {
                // Simple verlet integration
                float dt = Time.fixedDeltaTime;
                Vector3 accel = -m_Gravity * Vector3.up;

                Vector3 curPos = m_Transform.position;
                Vector3 newPos = curPos + (curPos - m_LastPos) + m_Impulse * dt + accel * dt * dt;
                m_LastPos = curPos;
                m_Transform.position = newPos;
                m_Transform.forward = newPos - m_LastPos;

                m_Impulse = Vector3.zero;
            }
        }

        public void AddImpulse(Vector3 impulse)
        {
            m_Impulse += impulse;
        }
    }
}