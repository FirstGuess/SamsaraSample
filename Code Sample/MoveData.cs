using UnityEngine;

[System.Serializable]
public class MoveData : MonoBehaviour {

    ///// Fields /////

    // Core Data
    
    public Vector3 origin;
    public Vector3 velocity;
    public Vector3 flatVelocity;
    public Vector3 inputDir;
    public Quaternion velocityRot;
    public Vector2 mouseDelta;
    public Vector2 mousePosition;

    // Input
    
    public float verticalAxis = 0f;
    public float horizontalAxis = 0f;
    public Vector3 wishMove = Vector3.zero;
    public Vector3 flatWishMove = Vector3.zero;
    public bool wishJumpDown = false;
    public bool wishJumpUp = false;
    public bool wishCtrl = false; 
    public bool wishShiftDown = false;
    public bool wishShiftUp = false;
    public bool wishCrouchDown = false;
    public bool wishCrouchUp = false;
    public bool wishQDown = false;
    public bool wishEscapeDown = false;
    public bool wishFireDown = false;
    public bool wishFireUp = false;
    public bool wishFirePress = false;
    public bool wishFire2Down = false;
    public bool wishFire2Up = false;
    public bool wishFire2Press = false;
    
    // Player State
    
    public bool grappling = false; // are we currently
    public bool attacking = false;
    public bool grounded = false;
    public bool detectWall = false;


    // Grapple

    public Vector3 grapplePoint;
    public SpringJoint joint;
    public float distanceFromGrapple = 0f;
    public Vector3 grappleNormal;
    public Vector3 grappleDir;
    public float lookingAtPoint;

    // Focus

    public Vector3 focusPoint;
    public Vector3 focusNormal;
    public Vector3 focusDir;
    public float distanceFromFocus = 0f;
    public Vector3 screenFocusPoint;
    public Vector3 screenLookAtPoint;

    // Other
    public Vector3 targetNormal;
    public Vector3 targetPoint;
    public Vector3 targetDir;
    public float distanceFromTarget = 0f;
    public Vector3 wallNormal;
    public float vCharge;
    public float xAimDamp;
    public float yAimDamp;

    public Collider[] targets;

}
