using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateNeutral : PlayerBaseState {

    public PlayerStateNeutral(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base (currentContext, playerStateFactory) {
        _isRootState = false;
        name = "neutral";
    }

    public override void EnterState()
    {
        // oldMomentum = Vector3.Scale(ctx.moveData.velocity, new Vector3(1f, 0f, 1f));
        // Debug.Log("ENTER NEUTRAL");
    }

    public override void UpdateState()
    {
        OnlyInfluence();

        if (ctx.moveData.wishJumpUp && ctx.moveData.grounded) {
            BoostJump(ctx.avatarLookForward, Mathf.Max(ctx.moveData.velocity.magnitude, 20f));
            ctx.sphereLines.Stop();
        }

        CheckSwitchStates();
    }

    public override void ExitState()
    {

    }

    public override void InitializeSubStates()
    {

    }

    public override void CheckSwitchStates()
    {
        if (ctx.moveData.wishFireDown) {
            SwitchState(factory.Lunge());
        }

        // if (ctx.moveData.wishShiftDown) {
        //     // oldMomentum = Vector3.zero;
        //     SwitchState(_factory.Dash());
        // } 
        // else if (!ctx.moveData.grounded) {
        //     SwitchState(factory.Fall());
        // }
        // else if (ctx.moveData.grappling) {
        //     SwitchState(_factory.Grapple());
        // }

    }

}