using UnityEngine;

public class DivePhysics {

    ///// Fields /////

    /// <summary>
    /// Change this if your ground is on a different layer
    /// </summary>
    public static int groundLayerMask = LayerMask.GetMask (new string[] { "Ground", "Player clip" }); //(1 << 0);

    private static Collider[] _colliders = new Collider [maxCollisions];
    private const int maxCollisions = 128;

    ///// Methods /////
    public static void ResolveCollisions (Collider collider, ref Vector3 origin, ref Vector3 velocity, int layerMask) {
        
        // manual collision resolving
        int numOverlaps = 0;
        if (collider is CapsuleCollider) {

            var capc = collider as CapsuleCollider;

            Vector3 point1, point2;
            GetCapsulePoints (capc, origin, out point1, out point2);

            numOverlaps = Physics.OverlapCapsuleNonAlloc (point1, point2, capc.radius,
                _colliders, layerMask, QueryTriggerInteraction.Ignore);

        } else if (collider is BoxCollider) {

            numOverlaps = Physics.OverlapBoxNonAlloc (origin, collider.bounds.extents, _colliders,
                Quaternion.identity, layerMask, QueryTriggerInteraction.Ignore);

        } else if (collider is SphereCollider) {

            var caps = collider as SphereCollider;

            numOverlaps = Physics.OverlapSphereNonAlloc (origin, caps.radius, _colliders,
                layerMask, QueryTriggerInteraction.Ignore);

        }

        for (int i = 0; i < numOverlaps; i++) {

            Vector3 direction;
            float distance;

            if (Physics.ComputePenetration (collider, origin,
                Quaternion.identity, _colliders [i], _colliders [i].transform.position,
                _colliders [i].transform.rotation, out direction, out distance)) {

                if (distance == 0f) {
                    Debug.Log("zero");
                    return;
                }

                velocity += Vector3.Dot(velocity, -direction) * direction;

                float isSide = Mathf.Abs(Vector3.Dot(direction, Vector3.up));

                // Handle collision
                direction.Normalize();
                Vector3 penetrationVector = direction * (distance + .5f * isSide);
                origin += penetrationVector;

                // velocity += planeForward;


            }
        }
    }

    public static void ResolveBallCollisions (Collider collider, ref Vector3 origin, ref Vector3 totalVelocity, int layerMask, float elasticity = 1f) {
        
        // manual collision resolving
        int numOverlaps = 0;
        if (collider is CapsuleCollider) {

            var capc = collider as CapsuleCollider;

            Vector3 point1, point2;
            GetCapsulePoints (capc, origin, out point1, out point2);

            numOverlaps = Physics.OverlapCapsuleNonAlloc (point1, point2, capc.radius,
                _colliders, layerMask, QueryTriggerInteraction.Ignore);

        } else if (collider is BoxCollider) {

            numOverlaps = Physics.OverlapBoxNonAlloc (origin, collider.bounds.extents, _colliders,
                Quaternion.identity, layerMask, QueryTriggerInteraction.Ignore);

        } else if (collider is SphereCollider) {

            var caps = collider as SphereCollider;

            numOverlaps = Physics.OverlapSphereNonAlloc (origin, caps.radius * caps.transform.localScale.x, _colliders,
                layerMask, QueryTriggerInteraction.Ignore);

        }

        for (int i = 0; i < numOverlaps; i++) {

            Vector3 direction;
            float distance;

            if (Physics.ComputePenetration (collider, origin,
                Quaternion.identity, _colliders [i], _colliders [i].transform.position,
                _colliders [i].transform.rotation, out direction, out distance)) {

                if (distance == 0f) {
                    return;
                }

                // _colliders[i].gameObject

                totalVelocity += Vector3.Dot(totalVelocity, -direction) * direction * elasticity;

                // Handle collision
                direction.Normalize();
                Vector3 penetrationVector = direction * (distance);
                origin += penetrationVector;


                // totalVelocity += planeForward;


            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    public static void GetCapsulePoints(CapsuleCollider capc, Vector3 origin, out Vector3 p1, out Vector3 p2) {

        var distanceToPoints = capc.height / 2f - capc.radius;
        p1 = origin + capc.center + Vector3.up * distanceToPoints;
        p2 = origin + capc.center - Vector3.up * distanceToPoints;

    }

}
