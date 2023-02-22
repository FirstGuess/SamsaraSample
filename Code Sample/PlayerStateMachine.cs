using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.Rendering;
using UnityEngine.UI;
using DynamicMeshCutter;
using UnityEngine.VFX;
using Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Events;
using MagicaCloth;
using QFSW.QC;

// BanannaRepublic
// user
// wTTg3nspLoYM
// qGWQtm1FcHUB

public enum ColliderType {
    Capsule,
    Box,
    Sphere
}

public class PlayerStateMachine : MonoBehaviour, IDiveControllable {

    [SerializeField] GameObject grappleGun;
    [SerializeField] GameObject smokeObj;
    [SerializeField] GameObject smokeLandObj;
    [SerializeField] GameObject sonicBoomObj;
    [SerializeField] GameObject airHikeObj;
    [SerializeField] GameObject sphereLinesObj;
    [SerializeField] public GameObject ballObj;
    [SerializeField] MagicaAreaWind wind;
    public Volume globalVolume;
    [HideInInspector] public VisualEffect grappleArc;
    [HideInInspector] public VisualEffect slash;
    [HideInInspector] public VisualEffect smoke;
    [HideInInspector] public VisualEffect smokeLand;
    [HideInInspector] public VisualEffect sonicBoom;
    [HideInInspector] public VisualEffect airHike;
    [HideInInspector] public VisualEffect sphereLines;
    public GameObject _vcam;
    public GameObject _groupCam;
    public GameObject _targetGroup;
    public ParticleSystem speedTrails;
    [HideInInspector] public CinemachineVirtualCamera virtualCam;
    [HideInInspector] public CinemachineFramingTransposer framingCam;
    [HideInInspector] public CinemachineSameAsFollowTarget aimCam;
    [HideInInspector] public CinemachineFramingTransposer groupcam;
    [HideInInspector] public CinemachineTargetGroup targetGroup;
    [HideInInspector] public CinemachineBrain brain;
    
    [SerializeField] LayerMask _groundMask;

    public Grapple _grapple;
    protected Grapple grapple;

    public Text debug;

    public BezierCurve bezierCurve;
    public GameObject bezierObj;

    public Camera _cam;
    GameObject _groundObject;

    ///// Fields /////

    public Transform avatarLookTransform;




    
    
    private CapsuleCollider _playerCollider;
    private SphereCollider _sphereCollider;
    private Vector3 _frontSide;
    private Vector3 _leftSide;
    private Vector3 _rightSide;
    private Vector3 _backSide;

    PlayerBaseState _currentState;
    PlayerStateFactory _states;
    public PlayerControls PlayerControls;

    public MoveData _moveData;
    public MoveConfig _moveConfig;

    ///// Properties /////

    public PlayerBaseState currentState { get {return _currentState; } set { _currentState = value; } }
    public MoveConfig moveConfig { get { return _moveConfig; } }
    public MoveData moveData { get { return _moveData; } }

    public Camera cam { get { return _cam; } set { _cam = value; } }
    [HideInInspector] public Vector3 viewForward { get { return cam.transform.forward; } set { cam.transform.forward = value; } }
    [HideInInspector] public Vector3 viewRight { get { return cam.transform.right; } }
    [HideInInspector] public Vector3 viewUp { get { return cam.transform.up; } }
    [HideInInspector] public Vector3 avatarLookForward { get { return avatarLookTransform.forward; } set { avatarLookTransform.forward = value; } }
    [HideInInspector] public Vector3 avatarLookRight { get { return avatarLookTransform.right; } }
    [HideInInspector] public Vector3 avatarLookUp { get { return avatarLookTransform.up; } }
    [HideInInspector] public Vector3 bodyForward { get { return transform.forward; } }
    [HideInInspector] public Vector3 bodyRight { get { return transform.right; } }
    [HideInInspector] public Vector3 bodyUp { get { return transform.up; } }
    [HideInInspector] public Vector3 velocityForward { get { return velocityRotation * Vector3.forward; } }
    [HideInInspector] public Vector3 velocityRight { get { return velocityRotation * Vector3.right; } }
    [HideInInspector] public Vector3 velocityUp { get { return velocityRotation * Vector3.up; } }
    [HideInInspector] public Vector3 leftSide { get { return _leftSide; } set { _leftSide = value; } }
    [HideInInspector] public Vector3 rightSide { get { return _rightSide; } set { _rightSide = value; } }
    [HideInInspector] public Vector3 backSide { get { return _backSide; } set { _backSide = value; } }
    [HideInInspector] public Vector3 frontSide { get { return _frontSide; } set { _frontSide = value; } }
    [HideInInspector] public LayerMask groundMask { get { return _groundMask; } set { _groundMask = value; } }
    [HideInInspector] public Vector3 groundNormal { get { return _groundNormal; } set { _groundNormal = value; } }
    [HideInInspector] public CapsuleCollider playerCollider { get { return _playerCollider; } set { _playerCollider = value; } }
    [HideInInspector] public GameObject groundObject { get { return _groundObject; } set { _groundObject = value; } }
    [HideInInspector] public Quaternion viewRotation { get { return cam.transform.rotation; } set { cam.transform.rotation = value; } }
    [HideInInspector] public Quaternion avatarLookRotation { get { return avatarLookTransform.rotation; } set { avatarLookTransform.rotation = value; } }
    [HideInInspector] public Quaternion bodyRotation { get { return transform.rotation; } set { transform.rotation = value; } }
    [HideInInspector] public Quaternion focusRotation { get { return Quaternion.LookRotation((focusOnThis.transform.position - avatarLookTransform.position).normalized, groundNormal); } }
    public VisualEffect _grappleArc { get { return grappleArc; } }

    Vector3 prevPosition;
    Animator animator;
    Vector3 _groundNormal = Vector3.up;
    Vector3 lastContact = Vector3.zero;
    [HideInInspector] public float wallTouchTimer = 0f;
    [HideInInspector] public float jumpTimer = 0f;
    [HideInInspector] public float groundInputTimer = 0f;
    [HideInInspector] public float boostInputTimer = 0f;
    [HideInInspector] public float grappleShootTimer = 0f;
    [HideInInspector] public float grappleZipTimer = 0f;
    [HideInInspector] public float reduceGravityTimer = 0f;
    [HideInInspector] public float ignoreGravityTimer = 0f;
    [HideInInspector] public float inputBufferTimer = 0f;
    [HideInInspector] public float runTimer = 2f;
    [HideInInspector] public float lungeCooldownTimer = 0f;
    [HideInInspector] public float releaseTimer = 0f;
    [HideInInspector] public bool doubleJump = false;
    [HideInInspector] public Quaternion velocityRotation;
	[HideInInspector] public float xMovement;
	[HideInInspector] public float yMovement;
	[HideInInspector] public LayerMask CamOcclusion;
	[HideInInspector] public Vector3 displacement;
	[HideInInspector] private Vector3 camMask = new Vector3(0, 1, -2);
    [HideInInspector] public Vector3 viewTransformLookAt;
    Vector3 tmpLookAt = Vector3.zero;
    Quaternion tmpLookAtRot;

    // KIK: Marsh_Mellow9


    public Transform lookAtThis;
    public Transform focusOnThis;

    [HideInInspector] public GameObject slashObj;
    [HideInInspector] public GameObject trajectory;
    // public GameObject cloak;
    // private Material cloakMat;
    private Material circleMat;
    private Material lineMat;
    private Material dotMat;

    

    [HideInInspector] public int targetLength = 0;
    [HideInInspector] public Collider currentTarget = null;
    private Vector3 centeredPoint;
    private float stability;
    [HideInInspector] public float focusAimBlend;
    [HideInInspector] public float elasticTime = .5f;
    [HideInInspector] public float centripetalForce = 2f;
    

    
    

    private void Awake() {

        playerCollider = transform.GetComponent<CapsuleCollider>();
        animator = transform.GetChild(0).GetComponent<Animator>();
        slashObj = transform.GetChild(1).transform.GetChild(1).gameObject;




        // cloakMat = cloak.GetComponent<Renderer>().material;
        circleMat = Resources.Load("Materials/blueCircle") as Material;
        lineMat = Resources.Load("Materials/redLine") as Material;
        dotMat = Resources.Load("Materials/greenDot") as Material;

        moveConfig.grappleColor = moveConfig.normalColor;

        

        virtualCam =  _vcam.GetComponent<CinemachineVirtualCamera>();
        framingCam = _vcam.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>();
        groupcam = _groupCam.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>();
        targetGroup = _targetGroup.GetComponent<CinemachineTargetGroup>();
        brain = cam.GetComponent<CinemachineBrain>();
        aimCam = _vcam.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineSameAsFollowTarget>();
        
        circleMat.SetFloat("_alpha", 0f);

        moveData.origin = transform.position;
        prevPosition = transform.position;

        PlayerControls = new PlayerControls();

        Cursor.lockState = CursorLockMode.Locked;

        // lr = grappleGun.GetComponent<LineRenderer>();
        grappleArc = grappleGun.GetComponent<VisualEffect>();
        smoke = smokeObj.GetComponent<VisualEffect>();
        smokeLand = smokeLandObj.GetComponent<VisualEffect>();
        sonicBoom = sonicBoomObj.GetComponent<VisualEffect>();
        airHike = airHikeObj.GetComponent<VisualEffect>();
        sphereLines = sphereLinesObj.GetComponent<VisualEffect>();

        sonicBoomObj.SetActive(true);

        sonicBoom.Stop();
        sphereLines.Stop();

        grapple = null;
        viewTransformLookAt = cam.transform.forward;
        lookAtThis.position = Vector3.zero;
        lookAtThis.localPosition = Vector3.zero;
        lookAtThis.localScale = new Vector3(moveConfig.castRadius/2f, moveConfig.castRadius/2f, moveConfig.castRadius/2f);

        focusOnThis.position = Vector3.zero;
        focusOnThis.localPosition = Vector3.zero;
        focusOnThis.localScale = new Vector3(2f, 2f, 2f);

        avatarLookForward = bodyForward;
        focusAimBlend = .5f;

        moveData.targets = new Collider[5];



    }

    private void OnEnable() {
        PlayerControls.Enable();
    }

    private void OnDisable() {
        PlayerControls.Disable();
    }

    
    private void Start() {

        PlayerControls.Player.Param1.started += context => {
            elasticTime -= .5f;
        };

        PlayerControls.Player.Param2.started += context => {
            elasticTime += .5f;
        };

        PlayerControls.Player.Param3.started += context => {
            centripetalForce -= .5f;
        };

        PlayerControls.Player.Param4.started += context => {
            centripetalForce += .5f;
        };

        PlayerControls.Player.Dive.started += context => {
            moveData.wishCtrl = true;
        };

        PlayerControls.Player.Dive.canceled += context => {
            moveData.wishCtrl = false;
        };

        PlayerControls.Player.Q.started += context => {
            moveData.wishQDown = true;
        };

        PlayerControls.Player.Q.canceled += context => {
            moveData.wishQDown = false;
        };

        PlayerControls.Player.Escape.started += context => {
            moveData.wishEscapeDown = true;
        };

        PlayerControls.Player.Escape.canceled += context => {
            moveData.wishEscapeDown = false;
        };

        PlayerControls.Player.Fire.started += context => {
            moveData.wishFireDown = true;
            moveData.wishFirePress = true;
        };

        PlayerControls.Player.Fire.canceled += context => {
            moveData.wishFireDown = false;
            moveData.wishFireUp = true;
        };

        PlayerControls.Player.Fire2.started += context => {
            moveData.wishFire2Down = true;
            moveData.wishFire2Press = true;
        };

        PlayerControls.Player.Fire2.canceled += context => {
            moveData.wishFire2Down = false;
            moveData.wishFire2Up = true;
        };

        PlayerControls.Player.Move.started += context => {
            moveData.horizontalAxis = context.ReadValue<Vector2>().x;
            moveData.verticalAxis = context.ReadValue<Vector2>().y;
        };

        PlayerControls.Player.Move.performed += context => {
            moveData.horizontalAxis = context.ReadValue<Vector2>().x;
            moveData.verticalAxis = context.ReadValue<Vector2>().y;
        };

        PlayerControls.Player.Move.canceled += context => {
            moveData.horizontalAxis = context.ReadValue<Vector2>().x;
            moveData.verticalAxis = context.ReadValue<Vector2>().y;
        };

        PlayerControls.Player.Crouch.started += context => {
            moveData.wishCrouchDown = true;
        };

        PlayerControls.Player.Crouch.canceled += context => {
            moveData.wishCrouchDown = false;
            moveData.wishCrouchUp = true;
        };

        PlayerControls.Player.Shift.started += context => {
            moveData.wishShiftDown = true;
        };

        PlayerControls.Player.Shift.canceled += context => {
            moveData.wishShiftDown = false;
            moveData.wishShiftUp = true;
        };


        PlayerControls.Player.Jump.started += context => {
            moveData.wishJumpDown = true;
        };

        PlayerControls.Player.Jump.canceled += context => {
            moveData.wishJumpDown = false;
            moveData.wishJumpUp = true;
        };

        // 1920 x 1200 res

        PlayerControls.Player.Look.performed += context => {
            moveData.mouseDelta = context.ReadValue<Vector2>();
            moveData.mousePosition += moveData.mouseDelta;

            moveData.mousePosition.x = moveData.mousePosition.x % 1920f;
            moveData.mousePosition.y = moveData.mousePosition.y % 1200f;

            
        };

        // PlayerControls.Player.Look2.performed += context => {
        //     moveData.mousePosition = context.ReadValue<Vector2>();
        // };

        // PlayerControls.Player.Dive.performed += context => {
        //     if (context.interaction is HoldInteraction) {
        //         moveType = MoveType.Charge;
        //     }

        // };

        currentState = new PlayerStateAir(this, new PlayerStateFactory(this));
        currentState.InitializeSubStates();
        bezierCurve = new BezierCurve(this);

    }

    private void Update () {

        debug.text = String.Format("Aim Assist Blend: {0}\nCurrent Super State: {1}\nCurrent Sub State: {2}\nSpeed: {3}", focusAimBlend, currentState?.name, currentState?.currentSubState?.name, Mathf.Floor(moveData.velocity.magnitude));

        // CollisionCheck >> Player Input >> Update position from last >> Resolve Collisions >> Update States >> Update Rotations

        DecrementTimers();
        CollisionCheck();
        UpdateInputData();
        FindTargets();

        Vector3 positionalMovement = transform.position - prevPosition; // TODO: 
        transform.position = prevPosition;
        moveData.origin += positionalMovement;

        ClampVelocity();
        ResolveCollisions();
        DoVelocityAnimations();
        CheckGrapplePress();
        CameraStuff();
        
        currentState.UpdateStates();

        transform.position = moveData.origin;
        prevPosition = transform.position;

        moveData.flatVelocity = Vector3.ProjectOnPlane(moveData.velocity, groundNormal);

        TransformRotation();

        leftSide = Vector3.zero;
        rightSide = Vector3.zero;
        backSide = Vector3.zero;
        frontSide = Vector3.zero;

        moveData.wishJumpUp = false;
        moveData.wishShiftUp = false;
        moveData.wishCrouchUp = false;
        moveData.wishFireUp = false;
        moveData.wishFirePress = false;
        moveData.wishFire2Up = false;
        moveData.wishFire2Press = false;

        if (moveData.wishEscapeDown) {
            Application.Quit();
        }

    }

    private void FindTargets() {

        targetLength = Physics.OverlapSphereNonAlloc(moveData.origin, 5f, moveData.targets, LayerMask.GetMask (new string[] { "Enemy" }));

    }

    // [Command("float-prop")] // 
    // protected static float SetAimAssist { get; private set;}

    

    public Quaternion FlatLookRotation(Vector3 forward) {
        return Quaternion.LookRotation(Vector3.ProjectOnPlane(forward, groundNormal).normalized, groundNormal);
    }

    public Quaternion FlatLookRotation(Vector3 forward, Vector3 normal) {
        return Quaternion.LookRotation(Vector3.ProjectOnPlane(forward, normal).normalized, normal);
    }

    private void DoVelocityAnimations() {

        // Vector3.ProjectOnPlane(moveData.velocity, groundNormal);

        var yVel = Vector3.Dot(Vector3.ProjectOnPlane(moveData.velocity, groundNormal), bodyForward);

        var xVel = Vector3.Dot(Vector3.ProjectOnPlane(moveData.velocity, groundNormal), bodyRight);

        var isDiveInput = new Vector2(moveData.horizontalAxis, moveData.verticalAxis) != Vector2.zero;

        smoke.SetVector3("position", moveData.origin - groundNormal);
        smoke.SetFloat("force", moveData.velocity.magnitude / 10f);
        smoke.SetFloat("spawnRate", 32f + moveData.velocity.magnitude);


        if (moveData.velocity.magnitude > moveConfig.walkSpeed) {

            if (moveData.grounded) {
                smoke.SetVector3("direction", -moveData.flatWishMove);
            } else {
                smoke.SetVector3("direction", -moveData.wishMove);
            }

        } else {
            smoke.SetVector3("direction", Vector3.zero);
            smoke.SetFloat("force", 0f);
        }

        if (moveData.wishCrouchDown && moveData.grounded && moveData.velocity.magnitude > moveConfig.walkSpeed) {
            smoke.SetVector3("direction", moveData.velocity.normalized);
            smoke.SetFloat("force", moveData.velocity.magnitude / 10f);

            // xVel = Mathf.Lerp(xVel, -xVel, .3f);
            // yVel = Mathf.Lerp(yVel, -yVel, .3f);
        }

        // if (!moveData.wishFire2) {
            animator.SetFloat("xVel", xVel);
            animator.SetFloat("yVel", yVel);


            animator.SetBool("ChargePress", moveData.wishCrouchDown);
            animator.SetBool("onGround", moveData.grounded);
            
            if (xVel >= 2f) {
                // cloakMat.SetFloat("_WaveFrequency", -.25f);
            } else {
                // cloakMat.SetFloat("_WaveFrequency", .25f);
            }

            // Mathf.Lerp(-.1f, .3f, moveData.velocity.magnitude/moveConfig.runSpeed)

            // cloakMat.SetVector("_WindDirection2", new Vector3(-xVel/moveConfig.runSpeed, -moveData.velocity.y/moveConfig.runSpeed + .25f, -yVel/moveConfig.runSpeed));
            // cloakMat.SetFloat("_Amplitude", .15f);
            // cloakMat.SetFloat("_AmplitudeFloor", Mathf.Lerp(.15f, .35f, moveData.velocity.magnitude/moveConfig.runSpeed));
        // }

    }

    private void CheckGrapplePress() {

        float distance = 150f * grappleShootTimer * 2f;

        if (moveData.wishFire2Press && !moveData.grappling) {
            ConnectGrapple(moveData.focusPoint);

            // moveData.focusDir = -differenceInPixels.normalized;
        }

        // if (moveData.wishFire2Down && !moveData.grappling) {

        //     grappleShootTimer = Mathf.Min((Time.deltaTime) + grappleShootTimer, .5f);
        //     circleMat.SetFloat("_size", .5f - grappleShootTimer + .001f);
        //     circleMat.SetFloat("_alpha", grappleShootTimer*2f);

        //     Ray ray = new Ray(avatarLookTransform.position + avatarLookForward * moveConfig.castRadius * 2f, avatarLookForward);
        //     RaycastHit hit;

        //     if (Physics.SphereCast(ray, moveConfig.castRadius * 2f, out hit, distance, LayerMask.GetMask (new string[] { "Focus", "Ground" }))) {
        //         circleMat.SetColor("_color", moveConfig.grappleColor);
        //     } else {
        //         circleMat.SetColor("_color", Color.grey);
        //     }

        // } else {

        //     if (grappleShootTimer > 0f) {
                // ShootGrapple(distance);
        //     }


        //     grappleShootTimer = 0f;
        //     circleMat.SetFloat("_alpha", 0f);
        // }

        // if (moveData.grappling) {
        //     if (moveData.wishCrouchDown || moveData.distanceFromPoint > 150f || moveData.distanceFromPoint < moveConfig.minDistance) {
        //         StopGrapple();
        //     }
        // }


    }

    //     private void CheckGrapplePress() {

    //     float distance = moveConfig.maxDistance * grappleShootTimer * 2f;

    //     if (moveData.wishGrapple) {

    //         grappleShootTimer = Mathf.Min((Time.deltaTime) + grappleShootTimer, .5f);
    //         circleMat.SetFloat("_size", .5f - grappleShootTimer + .001f);
    //         circleMat.SetFloat("_alpha", grappleShootTimer*2f);

    //         Ray ray = new Ray(viewTransform.position + viewForward * 10f, viewForward);
    //         RaycastHit hit;

    //         if (Physics.SphereCast(ray, 2f, out hit, distance, LayerMask.GetMask (new string[] { "Focus", "Ground" }))) {
    //             circleMat.SetColor("_color", grappleColor);
    //         } else {
    //             circleMat.SetColor("_color", Color.grey);
    //         }

    //     } else {

    //         if (grappleShootTimer > 0f) {
    //             ShootGrapple(distance);
    //         }


    //         grappleShootTimer = 0f;
    //         circleMat.SetFloat("_alpha", 0f);
    //     }

    //     if (moveData.grappling) {
    //         if (moveData.wishCrouch || moveData.distanceFromPoint > moveConfig.maxDistance || moveData.distanceFromPoint < moveConfig.minDistance) {
    //             StopGrapple();
    //         }
    //     }


    // }

    private void LateUpdate() {
        DrawRope();
    }

    public Vector3 CenteredSlerp(Vector3 start, Vector3 end, Vector3 centerPivot, float t) {
        // Vector3 centerPivot = (start + end) * .5f;


        Vector3 startRelativeCenter = start - centerPivot;
        Vector3 endRelativeCenter = end - centerPivot;

        return Vector3.Slerp(startRelativeCenter, endRelativeCenter, t) + centerPivot;
    }

    private void TransformRotation() {


        string[] mask = new string[] { "Ground", "Ball", "Enemy" };

        Vector3 castPos = avatarLookTransform.position;
        // bool aimAssistOn = false;
        
        RaycastHit hit;

        if (!moveData.attacking && !moveData.wishFireDown) { // aim assist stuff

            for (float i = 1f; i <= moveConfig.maxDistance; i++) {

                castPos = avatarLookTransform.position + avatarLookForward * i / 2f;

                Ray r = new Ray(castPos, avatarLookForward);

                if (Physics.SphereCast (
                    ray: r,
                    radius: i / 2f,
                    hitInfo: out hit,
                    maxDistance: moveConfig.maxDistance - i,
                    layerMask: LayerMask.GetMask (mask),
                    queryTriggerInteraction: QueryTriggerInteraction.Ignore))
                {
                    
                    moveData.focusPoint = hit.point;
                    moveData.focusDir = (moveData.focusPoint - moveData.origin).normalized;
                    moveData.distanceFromFocus = (moveData.focusPoint - moveData.origin).magnitude;
                    moveData.focusNormal = hit.normal;
                    // aimAssistOn = true;

                    

                    if (LayerMask.LayerToName(hit.collider.gameObject.layer) == "Enemy") {
                        moveData.targetPoint = hit.collider.transform.position;
                        moveData.targetDir = (moveData.targetPoint - moveData.origin).normalized;
                        moveData.distanceFromTarget = (moveData.targetPoint - moveData.origin).magnitude;
                        currentTarget = hit.collider;
                    } else {
                        currentTarget = null;
                        moveData.targetPoint = Vector3.zero;
                        moveData.targetDir = Vector3.zero;
                        moveData.distanceFromTarget = 0f;
                    }

                    break;
                } else {
                    moveData.focusPoint = avatarLookTransform.position + avatarLookForward * moveConfig.maxDistance / 2f;
                    // moveData.focusPoint = lookAtThis.position;
                }

            }

        }

        focusOnThis.position = Vector3.Lerp(focusOnThis.position, moveData.focusPoint, Time.deltaTime * 10f);
        
        Vector3 combinedLookPosition = Vector3.Lerp(lookAtThis.position, focusOnThis.position, focusAimBlend);
        Quaternion combinedLookRotation = Quaternion.LookRotation((combinedLookPosition - avatarLookTransform.position).normalized, groundNormal);

        if (moveData.velocity.magnitude > moveConfig.walkSpeed) { // TODO: make bodyTransform not this transform
            avatarLookRotation = Quaternion.Slerp(avatarLookRotation, combinedLookRotation, Time.deltaTime * 20f);
            velocityRotation = Quaternion.LookRotation(moveData.velocity, groundNormal);
            bodyRotation = Quaternion.Slerp(bodyRotation, FlatLookRotation(viewForward), Time.deltaTime * 5f);
        } else {
            avatarLookRotation = Quaternion.Slerp(avatarLookRotation, combinedLookRotation, Time.deltaTime * 20f);
            bodyRotation = Quaternion.Slerp(bodyRotation, FlatLookRotation(viewForward), Time.deltaTime * 5f);
            velocityRotation = bodyRotation;
        }

        lineMat.SetFloat("_lineAlpha", 0f);

        // Vector2 differenceInPixels = cam.WorldToScreenPoint(lookAtThis.position) - cam.WorldToScreenPoint(moveData.focusPoint);

        moveData.xAimDamp = Mathf.Lerp(moveData.xAimDamp, 1f, Time.deltaTime);
        moveData.yAimDamp = Mathf.Lerp(moveData.yAimDamp, 1f, Time.deltaTime);

        xMovement = moveData.mouseDelta.x * moveConfig.horizontalSensitivity * moveConfig.sensitivityMultiplier * moveData.xAimDamp;
		yMovement = -moveData.mouseDelta.y * moveConfig.verticalSensitivity  * moveConfig.sensitivityMultiplier * moveData.yAimDamp;

        viewTransformLookAt.x = Mathf.Clamp(viewTransformLookAt.x + yMovement, moveConfig.minYRotation, moveConfig.maxYRotation) % 360f;
        viewTransformLookAt.y = viewTransformLookAt.y + xMovement % 360f;
        
        avatarLookTransform.localPosition = groundNormal; // TODO: reverse this

        viewRotation = // this is what controls the cinemachine camera, in case someone other than me is trying to figure it out
        Quaternion.AngleAxis(viewTransformLookAt.y, Vector3.up) *
        Quaternion.AngleAxis(viewTransformLookAt.z, Vector3.forward) *
        Quaternion.AngleAxis(viewTransformLookAt.x, Vector3.right);

        // _targetGroup.transform.rotation = viewRotation;

        _vcam.SetActive(true);
        _groupCam.SetActive(false);

        // _vcam.SetActive(false);
        // _groupCam.SetActive(true);

        // The player input is actually bound in polar coordinates around the lookAtThis transform, but you can't tell when it is
        // far away. In theory, a lock on system will make this more obvious in the future, if I decide that is even a good idea
        
        Vector3 vanishingPoint = cam.transform.position + cam.transform.forward * moveConfig.maxDistance;
        lookAtThis.position = Vector3.Lerp(lookAtThis.position, vanishingPoint, Time.deltaTime * 15f);
    }

    private void CameraStuff() { // TODO:

        framingCam.m_UnlimitedSoftZone = false;

        if (moveData.detectWall) {
            framingCam.m_ScreenX = Mathf.Lerp(framingCam.m_ScreenX, 0.5f + Vector3.Dot(moveData.wallNormal, -viewRight) / 3f, Time.deltaTime * 2f);
        } else {
            framingCam.m_ScreenX = Mathf.Lerp(framingCam.m_ScreenX, 0.5f, Time.deltaTime);
        }

        if (moveData.wishJumpDown) {
            virtualCam.m_Lens.FieldOfView = Mathf.Lerp(virtualCam.m_Lens.FieldOfView, 75f, Time.deltaTime * 4f);
        } else {
            virtualCam.m_Lens.FieldOfView = Mathf.Lerp(virtualCam.m_Lens.FieldOfView, 90f, Time.deltaTime * 2f);
        }

        framingCam.m_DeadZoneDepth = 0f;
        // framingCam.m_TrackedObjectOffset = Vector3.up * framingCam.m_CameraDistance / 10f;


        framingCam.m_LookaheadTime = 0f;

        // if (!moveData.wishShiftDown) {
        //     framingCam.m_CameraDistance = Mathf.Lerp(framingCam.m_CameraDistance, 3f, Time.deltaTime);
        // } else {
        //     framingCam.m_CameraDistance = Mathf.Lerp(framingCam.m_CameraDistance, 9f, Time.deltaTime);
        // }

        framingCam.m_CameraDistance = Mathf.Lerp(framingCam.m_CameraDistance, Mathf.Max(Vector3.Dot(moveData.velocity, viewForward) / 4f + 2f, 5f), Time.deltaTime * 2f);
        
        framingCam.m_SoftZoneHeight = Mathf.Lerp(framingCam.m_SoftZoneHeight, .5f, Time.deltaTime * 4f);
        framingCam.m_SoftZoneWidth = Mathf.Lerp(framingCam.m_SoftZoneWidth, .5f, Time.deltaTime * 4f);
        framingCam.m_DeadZoneHeight = 0f;
        framingCam.m_DeadZoneWidth = .333f;
        framingCam.m_XDamping = Mathf.Lerp(framingCam.m_XDamping, 1f, Time.deltaTime * 4f);
        framingCam.m_YDamping = Mathf.Lerp(framingCam.m_YDamping, 1f, Time.deltaTime * 4f);
        framingCam.m_ZDamping = Mathf.Lerp(framingCam.m_ZDamping, 0f, Time.deltaTime * 4f);
        aimCam.m_Damping = Mathf.Lerp(aimCam.m_Damping, 0f, Time.deltaTime * 2f);

        // aimCam.m_Damping = Mathf.Lerp(aimCam.m_Damping, Mathf.Clamp01(moveData.velocity.magnitude / moveConfig.runSpeed) * stability, Time.deltaTime * 2f);

        // framingCam.m_DeadZoneDepth = Mathf.Lerp(framingCam.m_DeadZoneDepth, 0f, Time.deltaTime * 4f);

    }

    private void ConnectGrapple(Vector3 grapplePosition) {

        if (Vector3.Distance(moveData.origin, grapplePosition) < moveConfig.minDistance) {
            return;
        }

        moveData.grapplePoint = grapplePosition;
        moveData.joint = gameObject.AddComponent<SpringJoint>();
        moveData.joint.autoConfigureConnectedAnchor = false;
        moveData.joint.connectedAnchor = moveData.grapplePoint;

        moveData.distanceFromGrapple = Vector3.Distance(moveData.origin, moveData.grapplePoint);
        moveData.grappling = true;

    }

    public void StopGrapple() {
        moveData.grapplePoint = focusOnThis.position;
        Destroy(moveData.joint);

        // bezierCurve.Clear();
        grappleArc.enabled = false;
        
    }

    void DrawRope() {

        if (!moveData.grappling) return;

        var _lr = grappleGun.GetComponent<LineRenderer>();

        _lr.positionCount = 2;

        _lr.useWorldSpace = true;

        _lr.SetPosition(0, grappleGun.transform.position);
        _lr.SetPosition(1, moveData.grapplePoint);

        _lr.materials[0].mainTextureOffset += new Vector2(-Time.deltaTime, 0f);
        
        grappleArc.enabled = true;
    }

    public void EraseRope() {
        grappleArc.SetVector3("Pos0", grappleGun.transform.position);
        grappleArc.SetVector3("Pos1", grappleGun.transform.position);
        grappleArc.SetVector3("Pos2", grappleGun.transform.position);
        grappleArc.SetVector3("Pos3", grappleGun.transform.position);
        grappleArc.enabled = false;

    }

    // private void ShootGrapple() {
    //     Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
    //     RaycastHit hit;

    //     Vector3 targetPoint;
    //     if (Physics.Raycast(ray, out hit))
    //         targetPoint = hit.point;
    //     else
    //         targetPoint = ray.GetPoint(75);

    //     if (!grapple) {

    //         grapple = Instantiate(_grapple, viewTransform.position + viewForward*2f, Quaternion.identity);

    //     }

    //     grapple.playerCollider = innerCollider;
    //     grapple.rb.useGravity = false;
    //     Physics.IgnoreCollision(grapple.gameObject.GetComponent<SphereCollider>(), grapple.playerCollider, true);
            
    //     grapple.gameObject.SetActive(true);

    //     float launchSpeed = moveConfig.maxDistance * 4f;

    //     // Debug.Log(Vector3.Dot(moveData.velocity, viewForward) * viewForward);
        
    //     grapple.transform.forward = viewForward;
    //     grapple.GetComponent<Rigidbody>().velocity = viewForward * launchSpeed + Mathf.Max(Vector3.Dot(moveData.velocity, viewForward), 0f) * viewForward;
        // StartCoroutine(GrappleRoutine());
    // }

    private void ShootGrapple(float distance) {
        Ray ray = new Ray(avatarLookTransform.position + avatarLookForward * moveConfig.castRadius * 2f, avatarLookForward);
        RaycastHit hit;

        if (Physics.SphereCast(ray, moveConfig.castRadius, out hit, distance, LayerMask.GetMask (new string[] { "Ground" })))
            ConnectGrapple(hit.point); // TODO: Startup animation
            
        moveData.grappleNormal = hit.normal;
    }

    // IEnumerator GrappleRoutine() {

    //     float count = 0f;

    //     while (grapple) {

    //         Vector3 towardsPlayer = transform.position - grapple.transform.position;

    //         count+=Time.deltaTime;

    //         // if (count == Time.deltaTime) {
    //         //     yield return null;
    //         // }

    //         // v*t = 1/2*a*t^2 = d distance of average velocity / acceleration
    //         // d = 1/2*a*t^2 
    //         // 50 = 1/2*a*t^2 
    //         // 100 = a*t^2


    //         float launchSpeed = moveConfig.maxDistance * 4f;
            
    //         grapple.GetComponent<Rigidbody>().velocity += towardsPlayer.normalized * launchSpeed * Time.deltaTime * 2f;
    //         Vector3 tmpVel = grapple.GetComponent<Rigidbody>().velocity;
    //         float speed = grapple.GetComponent<Rigidbody>().velocity.magnitude;

    //         // Debug.Log(Vector3.Dot(tmpVel, towardsPlayer));

    //         if (Vector3.Dot(tmpVel, towardsPlayer) >= 0f && count >= 0.5f) {
    //             grapple.returning = true;
    //             grapple.GetComponent<Rigidbody>().velocity = Vector3.Project(tmpVel, towardsPlayer.normalized * speed);
    //         } 
    //         // else {
    //         //     grapple.GetComponent<Rigidbody>().velocity = Vector3.Project(tmpVel, -towardsPlayer.normalized * speed);
    //         // }

    //         if (count > 1.5f) {
    //             GrappleReturn();
    //         }

    //         yield return null;

    //     }
    // }

    private void CheckGrounded() {

        RaycastHit hit;
        if (Physics.Raycast (
            origin: moveData.origin,
            direction: -groundNormal,
            hitInfo: out hit,
            maxDistance: 1.5f,
            layerMask: LayerMask.GetMask (new string[] { "Focus", "Ground" }),
            queryTriggerInteraction: QueryTriggerInteraction.Ignore)) {
            
        }

        if (hit.collider == null || jumpTimer > 0f) {

            SetGround(null);
            groundNormal = Vector3.Lerp(groundNormal, Vector3.up, Time.deltaTime / 2f);
            moveData.grounded = false;

        } else {

            lastContact = hit.point;
            groundNormal = hit.normal.normalized;
            SetGround(hit.collider.gameObject);

            if (Vector3.Distance(moveData.origin - groundNormal, lastContact) < .49f) {
                moveData.origin += groundNormal * Mathf.Min(Time.deltaTime, .01f); // soft collision resolution?
            }

            if (!moveData.grounded) {

                if (Vector3.Dot(moveData.velocity, groundNormal) <= -7.5f) {
                    smokeLand.SetVector3("velocity", Vector3.ProjectOnPlane(moveData.velocity / 2f, groundNormal));
                    smokeLand.SetVector3("position", moveData.origin - groundNormal / 2f);
                    smokeLand.SetVector3("eulerAngles", Quaternion.LookRotation(groundNormal, Vector3.ProjectOnPlane(-velocityForward, groundNormal)).eulerAngles);
                    smokeLand.Play();
                }
                
            }

            doubleJump = true;

            moveData.grounded = true;

        }
    }

    private void SetGround (GameObject obj) {

        if (obj != null) {

            groundObject = obj;

        } else
            groundObject = null;

    }
    

    private void ClampVelocity() {

        float yVel = moveData.velocity.y;
        moveData.velocity.y = 0f;

        moveData.velocity = Vector3.ClampMagnitude(moveData.velocity, moveConfig.maxVelocity);
        moveData.velocity.y = Mathf.Max(yVel, -moveConfig.terminalVelocity);
        moveData.velocity.y = Mathf.Min(moveData.velocity.y, moveConfig.terminalVelocity);

    }

    private void ResolveCollisions() {

        if ((moveData.velocity.magnitude) == 0f) {

            DivePhysics.ResolveCollisions(playerCollider, ref moveData.origin, ref moveData.velocity, LayerMask.GetMask (new string[] { "Ground" }));

        } else {

            moveData.origin += moveData.velocity * Time.deltaTime; // p = v * dt

            DivePhysics.ResolveCollisions(playerCollider, ref moveData.origin, ref moveData.velocity, LayerMask.GetMask (new string[] { "Ground" }));

        }

    }

    private void CollisionCheck() {

        CheckGrounded();
        
        if (jumpTimer > 0f) return;

        RaycastHit hit;
        if (Physics.Raycast(moveData.origin, bodyForward, out hit, 1.1f, groundMask)) {
            frontSide = hit.normal;
        }

        if (Physics.Raycast(moveData.origin, bodyForward + bodyRight, out hit, 1.1f, groundMask)) {
            frontSide = hit.normal;
            if (frontSide == Vector3.zero) rightSide = hit.normal;
        }

        if (Physics.Raycast(moveData.origin, bodyRight, out hit, 1.1f, groundMask)) {
            rightSide = hit.normal;
        }

        if (Physics.Raycast(moveData.origin, bodyRight - bodyForward, out hit, 1.1f, groundMask)) {
            rightSide = hit.normal;
            if (rightSide == Vector3.zero) backSide = hit.normal;
        }

        if (Physics.Raycast(moveData.origin, -bodyForward, out hit, 1.1f, groundMask)) {
            backSide = hit.normal;
        }

        if (Physics.Raycast(moveData.origin, -bodyForward - bodyRight, out hit, 1.1f, groundMask)) {
            backSide = hit.normal;
            if (backSide == Vector3.zero) leftSide = hit.normal;
        }

        if (Physics.Raycast(moveData.origin, -bodyRight, out hit, 1.1f, groundMask)) {
            leftSide = hit.normal;
        }

        if (Physics.Raycast(moveData.origin, -bodyRight + bodyForward, out hit, 1.1f, groundMask)) {
            leftSide = hit.normal;
            if (leftSide == Vector3.zero) frontSide = hit.normal;
        }

        moveData.detectWall = leftSide != Vector3.zero || rightSide != Vector3.zero || backSide != Vector3.zero || frontSide != Vector3.zero;
        moveData.detectWall = moveData.detectWall && wallTouchTimer <= 0f;

        if (moveData.detectWall) {
            moveData.wallNormal = (leftSide + rightSide + backSide + frontSide).normalized;
        } else {
            moveData.wallNormal = Vector3.zero;
        }

    }

    private void DecrementTimers() {
        if (wallTouchTimer > 0f) {
            wallTouchTimer -= Time.deltaTime;
        }

        if (jumpTimer > 0f) {
            jumpTimer -= Time.deltaTime;
        }

        if (groundInputTimer > 0f) {
            groundInputTimer -= Time.deltaTime;
        }

        if (boostInputTimer > 0f) {
            boostInputTimer -= Time.deltaTime;
        }

        // if (grappleShootTimer > 0f) {
        //     grappleShootTimer -= Time.deltaTime;
        // }

        if (grappleZipTimer > 0f) {
            grappleZipTimer -= Time.deltaTime;
        }

        if (reduceGravityTimer > 0f) {
            reduceGravityTimer -= Time.deltaTime;
        }

        if (ignoreGravityTimer > 0f) {
            ignoreGravityTimer -= Time.deltaTime;
        }

        if (inputBufferTimer > 0f) {
            inputBufferTimer -= Time.deltaTime;
        }

        if (lungeCooldownTimer > 0f) {
            lungeCooldownTimer -= Time.deltaTime;
        }

        if (releaseTimer > 0f) {
            releaseTimer -= Time.deltaTime;
        }
        
    }

    private void UpdateInputData () {

        moveData.distanceFromGrapple = Vector3.Distance(moveData.origin, moveData.grapplePoint);
        moveData.grappleDir = (moveData.grapplePoint - avatarLookTransform.position).normalized;
        moveData.lookingAtPoint = Mathf.Clamp01(Vector3.Dot(avatarLookForward, moveData.grappleDir));

        sonicBoomObj.transform.rotation = avatarLookRotation;
        
    }

    

    private void OnTriggerEnter (Collider other) {

    }

    private void OnTriggerExit (Collider other) {

    }
  
}