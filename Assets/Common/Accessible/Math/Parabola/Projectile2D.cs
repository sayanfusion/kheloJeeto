using System;
using UnityEngine;

namespace DevCommon.Math.Parabola
{
    [DisallowMultipleComponent]
    public sealed class Projectile2D : MonoBehaviour
    {
        public enum EProjectileState
        {
            Idle = 0,
            Moving,
            AtDestination,
        }

        public Projectile2DData _projectile2DData;
        public float _closeDistance = 0.1f;
        public bool _autoInitialize = false;

        private Vector3 m_StartPos = new Vector3(0, 0, 0);
        private Vector3 m_NextPos = new Vector3(0, 0, 0);

        private EProjectileState m_ProjectileState = EProjectileState.Idle;
        private Action m_OnArrivedCallback = null;
        private Transform m_Transform;

        private void Start()
        {
            if (_autoInitialize && m_ProjectileState != EProjectileState.Moving)
            {
                m_StartPos = transform.position;
                m_Transform = transform;
                m_ProjectileState = EProjectileState.Moving;
            }
        }

        public void Initialize(Projectile2DData projectile2DData = null, Action onArrivedCallback = null, float closeDistance = 0.1f)
        {
            if (m_ProjectileState != EProjectileState.Moving)
            {
                if (projectile2DData != null)
                    _projectile2DData = projectile2DData;

                _closeDistance = closeDistance;
                m_OnArrivedCallback = onArrivedCallback;

                m_StartPos = transform.position;
                m_Transform = transform;
                m_ProjectileState = EProjectileState.Moving;
            }
        }

        Vector3 offset = new Vector3(0, 0, 0);
        private void Update()
        {
            if (m_ProjectileState == EProjectileState.Moving)
            {
                // Compute the next position, with arc added in
                float x0 = m_StartPos.x;
                float x1 = _projectile2DData._targetPos.x;
                float dist = x1 - x0;
                float nextX = Mathf.MoveTowards(m_Transform.position.x, x1, _projectile2DData._speed * Time.deltaTime);
                float baseY = Mathf.Lerp(m_StartPos.y, _projectile2DData._targetPos.y, (nextX - x0) / dist);
                float arc = _projectile2DData._arcHeight * (nextX - x0) * (nextX - x1) / (-0.25f * dist * dist);
                m_NextPos = new Vector3(nextX, baseY + arc, m_Transform.position.z);

                // Rotate to face the next position, and then move there
                m_Transform.rotation = LookAt2D(m_NextPos - m_Transform.position);
                m_Transform.position = m_NextPos;

                // Do something when we reach the target
                offset = _projectile2DData._targetPos - m_NextPos;
                float sqrLen = offset.sqrMagnitude;
                // square the distance we compare with
                if (sqrLen < _closeDistance)
                {
                    m_ProjectileState = EProjectileState.AtDestination;
                    m_OnArrivedCallback?.Invoke();
                }
                //if (m_NextPos == _projectile2DData._targetPos)
            }
        }

        // This is a 2D version of Quaternion.LookAt; it returns a quaternion
        // that makes the local +X axis point in the given forward direction.
        // forward direction
        // Quaternion that rotates +X to align with forward
        private Quaternion LookAt2D(Vector2 forward)
        {
            return Quaternion.Euler(0, 0, Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg);
        }
    }

    [System.Serializable]
    public class Projectile2DData
    {
        [Tooltip("Position we want to hit")]
        public Vector3 _targetPos = new Vector3(0, 0, 0);

        [Tooltip("Horizontal speed, in units/sec")]
        public float _speed = 10;

        [Tooltip("How high the arc should be, in units")]
        public float _arcHeight = 1;
    }
}