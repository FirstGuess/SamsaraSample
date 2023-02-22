using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateAir : PlayerBaseState {

    public PlayerStateAir(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base (currentContext, playerStateFactory) {
        _isRootState = true;
        name = "air";
    }

    public override void EnterState()
    {
        Debug.Log("ENTER AIR");
        InitializeSubStates();
    }

    public override void UpdateState()
    {

        if (ctx.ignoreGravityTimer > 0f) { } 
        else if (ctx.reduceGravityTimer > 0f) {
            ctx.moveData.velocity.y -= (ctx.moveConfig.gravity * Time.deltaTime * (1f - ctx.reduceGravityTimer));
        } else if (!ctx.moveData.grappling) {
            ctx.moveData.velocity.y -= (ctx.moveConfig.gravity * Time.deltaTime);
        }

        AirMovement();

        if (ctx.moveData.velocity.magnitude > ctx.moveConfig.runSpeed) {
            SubtractVelocityAgainst(ctx.moveData.velocity, ctx.moveData.velocity.magnitude / 2f);
        }

        // Debug.Log(ctx.moveData.velocity.magnitude);
        // ctx.moveData.velocity = Vector3.ClampMagnitude(ctx.moveData.velocity, 30f);
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        // Debug.Log("EXIT AIR");
    }

    public override void InitializeSubStates()
    {
        if (ctx.moveData.attacking) {
            SetSubState(factory.Lunge());
        } else if (!ctx.moveData.wishShiftDown) {
            SetSubState(factory.Neutral());
        } 
    }

    public override void CheckSwitchStates()
    {

        // if (ctx.moveData.wishCrouch) {
        //     SwitchState(factory.Charge());
        // } else 
        if (ctx.moveData.grounded) {
            SwitchState(factory.Grounded());
        } 
        else if (ctx.moveData.grappling) {
            SwitchState(factory.Grapple());
        } 

    }

    

    private void AirMovement() {
            
        if (ctx.moveData.detectWall && !ctx.moveData.grappling) { // wall run state

            if (ctx.doubleJump) {
                ctx.airHike.SetVector3("origin", ctx.moveData.origin);
                ctx.airHike.SetVector3("lookAt", ctx.moveData.wallNormal / 2f);
                ctx.airHike.SetFloat("size", 4f);
                ctx.airHike.Play();
            }

            ctx.doubleJump = false;

            if (Vector3.Dot(ctx.moveData.velocity, ctx.moveData.wallNormal) <= -7.5f) {
                
                ctx.smokeLand.SetVector3("velocity", Vector3.ProjectOnPlane(ctx.moveData.velocity / 2f, ctx.moveData.wallNormal));
                ctx.smokeLand.SetVector3("position", ctx.moveData.origin - ctx.moveData.wallNormal / 2f);
                ctx.smokeLand.SetVector3("eulerAngles", Quaternion.LookRotation(ctx.moveData.wallNormal, Vector3.ProjectOnPlane(-ctx.velocityForward, ctx.moveData.wallNormal)).eulerAngles);
                ctx.smokeLand.Play();
            }
            
            // ctx.moveData.velocity.y = Mathf.Lerp(ctx.moveData.velocity.y, 0f, Time.deltaTime / 4f);

            // if (ctx.moveData.velocity.magnitude > ctx.moveConfig.runSpeed + 10f) {
            //     SubtractVelocityAgainst(Vector3.ProjectOnPlane(ctx.moveData.velocity.normalized, ctx.moveData.wallNormal), ctx.moveData.velocity.magnitude / 2f);
            // } 
            // else 
            // if (ctx.moveData.velocity.magnitude < ctx.moveConfig.walkSpeed && ctx.moveData.wishShiftDown) {
            //     AddVelocityTo(Vector3.ProjectOnPlane(ctx.moveData.velocity.normalized, ctx.moveData.wallNormal), ctx.moveConfig.walkSpeed + 5f);
            // }


            // if (ctx.jumpTimer <= 0f) ctx.moveData.velocity += -ctx.moveData.wallNormal;

            ctx.smoke.SetVector3("position", ctx.moveData.origin - ctx.moveData.wallNormal / 2f);
            ctx.smoke.SetVector3("direction", -ctx.moveData.velocity.normalized);

            // if (ctx.jumpTimer <= 0f) {

            //     ctx.moveData.velocity += -ctx.moveData.wallNormal;
            //     ctx.moveData.velocity.y = Mathf.Lerp(ctx.moveData.velocity.y, 0f, Time.deltaTime / 2f);

            // }


            if (ctx.moveData.wishJumpDown) {
                // ctx.framingCam.m_CameraDistance = Mathf.Lerp(ctx.framingCam.m_CameraDistance, 3f, Time.deltaTime * 4f);
                ctx.sphereLines.SetFloat("Speed", -ctx.moveData.vCharge);
                ctx.sphereLines.Play();
                
                SubtractVelocityAgainst(ref ctx.moveData.velocity, -ctx.moveData.velocity.normalized, ctx.moveData.velocity.magnitude / 2f);

                BrakeCharge(ctx.avatarLookForward);
            }

            if (ctx.moveData.wishJumpUp) {
                BoostJump((ctx.moveData.wallNormal + ctx.avatarLookForward).normalized, Mathf.Max(ctx.moveData.velocity.magnitude, 20f));
                ctx.sphereLines.Stop();
            }

        } else { // falling state

            // ctx.bezierCurve.PredictGravityArc(ctx.moveData.origin, ctx.moveConfig.gravity, ctx.moveData.velocity);
            // ctx.bezierCurve.DrawProjection();



            // if (ctx.moveData.wishJumpDown && !ctx.doubleJump) {
            //     SubtractVelocityAgainst(ref ctx.moveData.velocity, -ctx.moveData.velocity.normalized, ctx.moveData.velocity.magnitude / 2f);
            //     ctx.vcam.m_CameraDistance = Mathf.Lerp(ctx.vcam.m_CameraDistance, 3f, Time.deltaTime * 4f);

            //     BrakeCharge();

            //     ctx.sphereLines.SetFloat("Speed", -ctx.moveData.vCharge);

            //     ctx.sphereLines.Play();
            // }

            // if (ctx.moveData.wishJumpUp) {
            //     AirJump(Vector3.up);
            //     ctx.sphereLines.Stop();
            // }

        }
    }

}