using UnityEngine;

namespace DevCommon.Utils
{
    [DisallowMultipleComponent]
    public sealed class BoundedByRadius : MonoBehaviour
    {
        public enum EDimension
        {
            Three = 0,
            Two
        }

        public Transform _target;
        public float _radius = 1.0f; //radius of *circle*
        public EDimension _dimension;

        private Vector3 m_CenterPosition = Vector3.zero;
        private Vector3 m_RequiredPosition = Vector3.zero;

        private void Awake()
        {
            if (_target == null)
                _target = transform;

            m_CenterPosition = transform.position; //center of *circle*
        }

        private void Update()
        {
            if (_dimension == EDimension.Three)
            {
                float distance = Vector3.Distance(_target.position, m_CenterPosition); //distance from ~moving object~ to *circle*

                if (distance > _radius) //If the distance is less than the radius, it is already within the circle.
                {
                    Vector3 fromOriginToObject = _target.position - m_CenterPosition; //~GreenPosition~ - *circle center*
                    fromOriginToObject *= _radius / distance; //Multiply by radius //Divide by Distance
                    m_RequiredPosition = m_CenterPosition + fromOriginToObject; //*circle center* + all that Math
                    _target.position = m_RequiredPosition; //*circle center* + all that Math
                }
            }
            else if (_dimension == EDimension.Two)
            {
                float distance = Vector3.Distance(_target.position, m_CenterPosition); //distance from ~moving object~ to *circle*

                if (distance > _radius) //If the distance is less than the radius, it is already within the circle.
                {
                    Vector3 fromOriginToObject = _target.position - m_CenterPosition; //~GreenPosition~ - *circle center*
                    fromOriginToObject *= _radius / distance; //Multiply by radius //Divide by Distance
                    m_RequiredPosition = m_CenterPosition + fromOriginToObject; //*circle center* + all that Math
                    m_RequiredPosition.z = _target.position.z;
                    _target.position = m_RequiredPosition;
                }
            }
        }
    }
}