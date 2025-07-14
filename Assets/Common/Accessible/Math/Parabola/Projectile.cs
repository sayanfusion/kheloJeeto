using System;
using UnityEngine;

namespace DevCommon.Math.Parabola
{
    [RequireComponent(typeof(BallisticMotion))]
    [DisallowMultipleComponent]
    public sealed class Projectile : MonoBehaviour
    {
        public enum EProjectileState
        {
            Idle = 0,
            Moving,
            AtDestination,
        }

        public BallisticMotion _ballisticMotion;
        public ProjectileData _projectileData;
        public float _closeDistance = 0.1f;
        public bool _autoInitialize = false;

        private EProjectileState m_ProjectileState = EProjectileState.Idle;
        private Action m_OnArrivedCallback = null;
        private Transform m_Transform;

        private void Awake()
        {
            _ballisticMotion = GetComponent<BallisticMotion>();
            m_Transform = transform;
        }

        private void Start()
        {
            if (_autoInitialize && m_ProjectileState != EProjectileState.Moving)
            {
                switch (_projectileData._aimMode)
                {
                    case ProjectileData.EAimMode.Normal:
                        NormalAim();
                        break;

                    case ProjectileData.EAimMode.Lateral:
                        LateralAim();
                        break;
                }
                m_ProjectileState = EProjectileState.Moving;
            }
        }

        public void Initialize(ProjectileData projectileData = null, Action onArrivedCallback = null, float closeDistance = 0.1f)
        {
            if (m_ProjectileState != EProjectileState.Moving)
            {
                if (projectileData != null)
                    _projectileData = projectileData;

                _closeDistance = closeDistance;
                m_OnArrivedCallback = onArrivedCallback;

                switch (_projectileData._aimMode)
                {
                    case ProjectileData.EAimMode.Normal:
                        NormalAim();
                        break;

                    case ProjectileData.EAimMode.Lateral:
                        LateralAim();
                        break;
                }
                m_ProjectileState = EProjectileState.Moving;
            }
        }

        private void Update()
        {
            CheckIfAtDestination();
        }

        Vector3 offset = new Vector3(0, 0, 0);
        private void CheckIfAtDestination()
        {
            if (m_ProjectileState == EProjectileState.Moving)
            {
                // Do something when we reach the target
                offset = _projectileData._targetData._transform.position - m_Transform.position;
                float sqrLen = offset.sqrMagnitude;
                // square the distance we compare with
                // cSoumen checking with z position as well just to be sure.
                if (sqrLen < _closeDistance || m_Transform.position.z >= _projectileData._targetData._transform.position.z)
                {
                    m_ProjectileState = EProjectileState.AtDestination;
                    _ballisticMotion.StopMotion();
                    m_OnArrivedCallback?.Invoke();
                }
            }
        }

        public void NormalAim()
        {
            uint solutionIndex = 0;
            Vector3 targetPos = _projectileData._targetData._transform.position;
            Vector3 diff = targetPos - _projectileData._projectile.position;
            Vector3 diffGround = new Vector3(diff.x, 0f, diff.z);

            Vector3[] solutions = new Vector3[2];
            int numSolutions;
            float gravity = _projectileData._gravity;

            if (_projectileData._targetData._velocity.sqrMagnitude > 0.0f)
                numSolutions = BallisticTrajectoryEquation.SolveBallisticArc(_projectileData._projectile.position, _projectileData._projSpeed, targetPos, _projectileData._targetData._velocity, gravity, out solutions[0], out solutions[1]);
            else
                numSolutions = BallisticTrajectoryEquation.SolveBallisticArc(_projectileData._projectile.position, _projectileData._projSpeed, targetPos, gravity, out solutions[0], out solutions[1]);

            if (numSolutions > 0)
            {
                transform.forward = diffGround;
                _ballisticMotion.Initialize(_projectileData._projectile.position, gravity);

                var index = solutionIndex % numSolutions;
                var impulse = solutions[index];
                ++solutionIndex;
                _ballisticMotion.AddImpulse(impulse);
            }
        }

        public void LateralAim()
        {
            Vector3 targetPos = _projectileData._targetData._transform.position;
            Vector3 diff = targetPos - _projectileData._projectile.position;
            Vector3 diffGround = new Vector3(diff.x, 0f, diff.z);

            Vector3 fireVel, impactPos;
            float gravity = _projectileData._gravity;

            if (BallisticTrajectoryEquation.SolveBallisticArcLateral(_projectileData._projectile.position, _projectileData._projSpeed, targetPos, _projectileData._targetData._velocity, _projectileData._arcPeak, out fireVel, out gravity, out impactPos))
            {
                transform.forward = diffGround;
                _ballisticMotion.Initialize(_projectileData._projectile.position, gravity);
                _ballisticMotion.AddImpulse(fireVel);
            }
        }
    }

    [System.Serializable]
    public class ProjectileData
    {
        public enum EAimMode
        {
            Normal,
            Lateral,
        }

        [Range(0.01f, 50.0f)] public float _projSpeed = 1.0f;
        [Range(0.0f, 20.0f)] public float _gravity = 0.0f;
        [Range(0.0f, 10.0f)] public float _arcPeak = 0.0f;
        public Transform _projectile;
        public EAimMode _aimMode = EAimMode.Lateral;
        public TargetData _targetData;
    }

    [System.Serializable]
    public class TargetData
    {
        public Transform _transform;
        public Vector3 _velocity = new Vector3(0, 0, 0);
    }
}