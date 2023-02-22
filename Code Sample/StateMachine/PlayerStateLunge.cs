using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateLunge : PlayerBaseState {

    Vector3 toTarget = Vector3.zero;
    Vector3 startPos = Vector3.zero;
    Vector3 di = Vector3.zero;
    Vector3 centerPivot = Vector3.zero;

    public PlayerStateLunge(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base (currentContext, playerStateFactory) {
        _isRootState = false;
        name = "lunge";
    }

    public override void EnterState()
    {
        // Debug.Log("ENTER LUNGE");
        t = 0f;

    }

    public override void UpdateState()
    {

        if (ctx.moveData.wishFireDown && !ctx.moveData.attacking) {

            di = (Vector3.ProjectOnPlane(ctx.avatarLookForward, -ctx.moveData.focusDir).normalized * 5f);

            toTarget = (ctx.moveData.focusPoint - ctx.moveData.origin);

            startPos = ctx.moveData.origin;

            centerPivot = startPos + toTarget * .5f - di;

            // ctx.bezierCurve.DrawCircle(centerPivot);



        }

        else {
            ctx.moveData.attacking = true;

            // if (ctx.moveData.wishFireUp) {
            //     ctx.moveData.velocity /= 2f;
            // }

            // ctx.moveData.origin = Vector3.Slerp(startPos, ctx.moveData.focusPoint, t * 2f);
            // ctx.moveData.origin = ctx.CenteredSlerp(startPos, ctx.moveData.focusPoint, centerPivot, t);

            if (ctx.moveData.grappling) {
                di = (Vector3.ProjectOnPlane(ctx.avatarLookForward, -ctx.moveData.grappleDir).normalized * 2f);
                toTarget = (ctx.moveData.grapplePoint - ctx.moveData.origin);
                ctx.moveData.velocity = Vector3.Slerp(ctx.moveData.velocity, toTarget, Time.deltaTime * (2f));
            }
            else if (ctx.moveData.wishFireUp) {
                BoostJump((toTarget + di).normalized, toTarget.magnitude * 2f);
                ctx.ignoreGravityTimer = .9f;
            }

            // ctx.bezierCurve.InterpolateAcrossCircleC1(startPos, ctx.moveData.focusPoint, centerPivot, t);

            // float length = (end - start).magnitude * Mathf.PI / 2f;
            // float estimatedTime =  length / Mathf.Max(ctx.moveConfig.runSpeed, ctx.moveData.velocity.magnitude);


            t += Time.deltaTime;
        }

        CheckSwitchStates();
    }

    public override void ExitState()
    {
        ctx.ignoreGravityTimer = 1f;
        // ctx.slashObj.SetActive(true);
        // ctx.slash.Play();
    }

    public override void InitializeSubStates()
    {

    }

    public override void CheckSwitchStates()
    {

        if (t > 2f || ctx.targetLength > 0) {
            SwitchState(factory.LungeCooldown());
        }
    }

}