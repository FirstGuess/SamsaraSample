using UnityEngine;

public class MoveConfig: MonoBehaviour {

    [Header ("Jumping and gravity")]
    public float gravity = 15f;
    public float jumpForce = 10f;
    
    [Header ("Velocity Clamp")]
    public float maxVelocity = 100f;
    public float terminalVelocity = 50f;

    [Header ("Dev Tools")]
    public bool flyMode = false;

    [Header ("Ground movement")]
    public float walkSpeed = 10f;
    public float runSpeed = 25f;
    public float maxCharge = 2f;

    [Header ("Grappling")]

    public float maxDistance = 100f;
    public float minDistance = 15f;
    [ColorUsage(true, true)]
    public Color grappleColor;
    [ColorUsage(true, true)]
    public Color accelColor;
    [ColorUsage(true, true)]
    public Color normalColor;
    public float castRadius = 1f;

    [Header ("Aiming")]
    public float sensitivityMultiplier = 0.5f;
    public float horizontalSensitivity = 1f;
    public float verticalSensitivity = 1f;
    public float minYRotation = -75f;
    public float maxYRotation = 75f;
    
}

