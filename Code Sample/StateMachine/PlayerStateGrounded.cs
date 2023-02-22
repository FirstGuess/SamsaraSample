using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateGrounded : PlayerBaseState
{
    public PlayerStateGrounded(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        _isRootState = true;
        name = "grounded";
    }

    public override void EnterState()
    {
        Debug.Log("ENTER GROUNDED");

        InitializeSubStates();
    }

    public override void UpdateState()
    {

        // Debug.Log(ctx.moveData.velocity);

        if (ctx.moveData.wishJumpDown) {
            // ctx.framingCam.m_CameraDistance = Mathf.Lerp(ctx.framingCam.m_CameraDistance, 3f, Time.deltaTime * 4f);
            ctx.sphereLines.SetFloat("Speed", -ctx.moveData.vCharge);
            ctx.sphereLines.Play();
            
            SubtractVelocityAgainst(ref ctx.moveData.velocity, -ctx.moveData.velocity.normalized, ctx.moveData.velocity.magnitude * 2f);

            BrakeCharge(ctx.avatarLookForward);
        }

        // if (ctx.moveData.wishJumpUp && !ctx.moveData.grappling) {
        //     BoostJump(ctx.avatarLookForward, Mathf.Max(ctx.moveData.velocity.magnitude, 30f));
        // } 

        if (ctx.moveData.wishCrouchDown) {

            SubtractVelocityAgainst(ctx.moveData.velocity, ctx.moveData.velocity.magnitude);
        
        }

        CheckSwitchStates();
    }

    public override void ExitState()
    {
        // ctx.smokeLand.SetVector3("direction", Vector3.ProjectOnPlane(ctx.moveData.velocity, ctx.groundNormal));
        // ctx.smokeLand.SetVector3("position", ctx.moveData.origin);
        // Debug.Log("EXIT GROUNDED");

        ctx.airHike.SetVector3("origin", ctx.moveData.origin);
        ctx.airHike.SetVector3("lookAt", ctx.groundNormal);
        ctx.airHike.SetFloat("size", 4f);
        ctx.airHike.Play();
    }

    public override void InitializeSubStates()
    {
        if (ctx.moveData.attacking) {
            SetSubState(factory.Lunge());
        } else if (ctx.moveData.wishShiftDown) {
            SetSubState(factory.Dash());
        } else {
            SetSubState(factory.Neutral());
            // _currentSubState.oldMomentum = Vector3.Scale(ctx.moveData.velocity, new Vector3(1f, 0f, 1f));
        } 
    }

    public override void CheckSwitchStates()
    {
        // if (ctx.moveData.wishCrouch) {
        //     SwitchState(factory.Charge());
        // } else 

        if (!ctx.moveData.grounded) {
            SwitchState(factory.Air());
        }
        else if (ctx.moveData.grappling) {
            SwitchState(factory.Grapple());
        }
    }

}