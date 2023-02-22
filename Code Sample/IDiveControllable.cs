using UnityEngine;
using UnityEngine.VFX;


public interface IDiveControllable {

    MoveData moveData { get; }
    MoveConfig moveConfig { get; }
    Camera cam { get; set; }
    CapsuleCollider playerCollider { get; }
    GameObject groundObject { get; set; }
    Vector3 bodyForward { get; }
    Vector3 bodyRight { get; }
    Vector3 bodyUp { get; }
    Vector3 avatarLookForward { get; set; }
    Vector3 avatarLookRight { get; }
    Vector3 avatarLookUp { get; }
    Vector3 viewForward { get; set; }
    Vector3 viewRight { get; }
    Vector3 viewUp { get; }
    Vector3 leftSide { get; }
    Vector3 rightSide { get; }
    Vector3 backSide { get; }
    Vector3 frontSide { get; }
    Vector3 velocityForward { get; }

    Quaternion FlatLookRotation(Vector3 forward, Vector3 normal);
    Vector3 CenteredSlerp(Vector3 start, Vector3 end, Vector3 centerPivot, float t);

    void StopGrapple();

    Vector3 groundNormal { get; }
    VisualEffect _grappleArc { get; }
}
