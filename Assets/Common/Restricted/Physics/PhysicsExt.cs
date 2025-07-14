using System.Collections;
using UnityEngine;

namespace DevCommon.Physics
{
    public sealed class PhysicsExt
    {
        public static bool CheckBounds(Vector3 position, Collider collider, int layerMask)
        {
            Vector3 boundsSize = GetBoundingBox(collider);
            Bounds boxBounds = new Bounds(position, boundsSize);

            float sqrHalfBoxSize = boxBounds.extents.sqrMagnitude;
            float overlapingSphereRadius = Mathf.Sqrt(sqrHalfBoxSize + sqrHalfBoxSize);

            Collider[] hitColliders = UnityEngine.Physics.OverlapSphere(position, overlapingSphereRadius, layerMask);
            foreach (Collider otherCollider in hitColliders)
            {
                if (otherCollider.bounds.Intersects(boxBounds))
                    return (false);
            }
            return (true);
        }

        public static bool CheckBounds2D(Vector2 position, Vector2 boundsSize, int layerMask)
        {
            Bounds boxBounds = new Bounds(position, boundsSize);

            float sqrHalfBoxSize = boxBounds.extents.sqrMagnitude;
            float overlapingCircleRadius = Mathf.Sqrt(sqrHalfBoxSize + sqrHalfBoxSize);

            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(position, overlapingCircleRadius, layerMask);
            foreach (Collider2D otherCollider in hitColliders)
            {
                if (otherCollider.bounds.Intersects(boxBounds))
                    return (false);
            }
            return (true);
        }

        public static Vector3 GetBoundingBox(Collider collider)
        {
            Vector3 boundsSize = Vector3.one;

            if (collider is BoxCollider)
                boundsSize = ((BoxCollider)collider).size;
            else if (collider is SphereCollider)
            {
                var radius = ((SphereCollider)collider).radius;
                boundsSize = new Vector3(radius * 2, radius * 2, radius * 2);
            }
            else if (collider is CapsuleCollider)
            {
                var radius = ((CapsuleCollider)collider).radius;
                var height = ((CapsuleCollider)collider).height;
                var direction = ((CapsuleCollider)collider).direction;

                var directionArray = new Vector3[] { Vector3.right, Vector3.up, Vector3.forward };
                var result = new Vector3();
                for (int i = 0; i < 3; i++)
                {
                    if (i == direction)
                        result += directionArray[i] * height;
                    else
                        result += directionArray[i] * radius * 2;
                }
                boundsSize = result;
            }
            else if (collider is MeshCollider)
            {
                boundsSize = ((MeshCollider)collider).sharedMesh.bounds.size;
            }
            return boundsSize;
        }

        static float parabolaProgress = 0.0f;
        public static IEnumerator DoParabola(Transform projectile, Vector3 destination, float maxHeight, float time)
        {
            var startPos = projectile.position;
            while (parabolaProgress <= 1.0)
            {
                parabolaProgress += Time.deltaTime / time;
                var height = Mathf.Sin(Mathf.PI * parabolaProgress) * maxHeight;
                if (height < 0f)
                {
                    height = 0f;
                }
                projectile.position = Vector3.Lerp(startPos, destination, parabolaProgress) + Vector3.up * height;
                yield return null;
            }
            projectile.position = destination;
        }
    }
}