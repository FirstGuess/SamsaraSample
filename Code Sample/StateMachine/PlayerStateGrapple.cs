using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateGrapple : PlayerBaseState {

    Vector3 grapplePointDir = Vector3.zero;
    float initialDistance = 0f;
    Vector3 pullTarget = Vector3.zero;
    Vector3 pullDir = Vector3.zero;

    public PlayerStateGrapple(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base (currentContext, playerStateFactory) {
        _isRootState = true;
        name = "grapple";
    }

    public override void EnterState()
    {
        // Debug.Log("ENTER GRAPPLE");
        InitializeSubStates();
        initialDistance = ctx.moveData.distanceFromGrapple;
        t = 0f;
    }

    public override void UpdateState()
    {
        GrappleMove();

        CheckSwitchStates();
    }

    public override void ExitState()
    {
        // Debug.Log("EXIT GRAPPLE");
        // ctx.jumpTimer = 0f;
        ctx.reduceGravityTimer = 0f;
    }

    public override void InitializeSubStates()
    {
        SetSubState(factory.Noop());
    }

    public override void CheckSwitchStates()
    {

        if (!ctx.moveData.grappling && ctx.moveData.grounded ) {
            ctx.StopGrapple();
            SwitchState(_factory.Grounded());
        } 

        else if (!ctx.moveData.grappling && !ctx.moveData.grounded) {
            ctx.StopGrapple();
            SwitchState(_factory.Air());
        }
    }

    private void GrappleMoveTargeting() {

        Vector3 grappleDir = (ctx.moveData.grapplePoint - ctx.moveData.origin).normalized;

        float lookCurve = Mathf.Clamp(Vector3.Dot(ctx.avatarLookForward, grappleDir), 0f, 1f);

        // ctx.bezierCurve.GetSpiralCurve(0.7f, t);
        // ctx.bezierCurve.SpiralCurvePoint();
        // ctx.bezierCurve.CreateSpiralFromVelocity();
        // ctx.bezierCurve.CreateArcFromVelocity();
        // ctx.bezierCurve.GrappleArc(ctx.moveData.grappleNormal, ctx.moveData.grapplePoint);
        

    }

    private void GrappleMove() {

            CancelVelocityAgainst(ctx.moveData.grappleDir, 7.5f);
            
            ctx.moveData.velocity.y -= (ctx.moveConfig.gravity * Time.deltaTime * .5f);

            if (ctx.moveData.wishJumpUp) {
                BoostJump((ctx.avatarLookForward).normalized, Mathf.Max(ctx.moveData.velocity.magnitude, 30f));
                ctx.moveData.grappling = false;
            } 
            
            if (ctx.moveData.wishShiftDown) {
                
                // ctx.moveData.velocity = Vector3.Slerp(ctx.moveData.velocity, ctx.moveData.grappleDir * (ctx.moveData.distanceFromGrapple), Time.deltaTime * (1f + t));
                
                ctx.moveData.velocity += ctx.moveData.grappleDir * (initialDistance/2f) * Time.deltaTime * (Mathf.Sqrt(3f)) + ctx.avatarLookForward * (initialDistance/2f) * Time.deltaTime;
                // ctx.moveData.velocity = Vector3.ClampMagnitude(ctx.moveData.velocity, initialDistance * 2f);

                ctx.moveConfig.grappleColor = Color.Lerp(ctx.moveConfig.grappleColor, ctx.moveConfig.accelColor, Time.deltaTime * 2f);
                
                t += Mathf.Min(1f, Time.deltaTime * 1f);
                // Debug.Log(t);

            }
            else {
                t = 0f;
                initialDistance = Mathf.Min(ctx.moveData.distanceFromGrapple, 30f);
            }

            if (ctx.moveData.wishCrouchUp || ctx.moveData.distanceFromGrapple < ctx.moveConfig.minDistance / 2f) {
                ctx.moveData.grappling = false;
                releaseVelocity = ctx.moveData.velocity;
                ctx.releaseTimer = 5f;

                releasedPoints[0] = ctx._grappleArc.GetVector3("Pos0");
                releasedPoints[1] = ctx._grappleArc.GetVector3("Pos1");
                releasedPoints[2] = ctx._grappleArc.GetVector3("Pos2");
                releasedPoints[3] = ctx._grappleArc.GetVector3("Pos3");
            }

    }

}