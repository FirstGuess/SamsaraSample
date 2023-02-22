using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateDash : PlayerBaseState {

    float dashSpeed = 0f;
    Vector3 dashDir = Vector3.zero;

    public PlayerStateDash(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base (currentContext, playerStateFactory) {
        _isRootState = false;
        name = "dash";
    }

    public override void EnterState()
    {
        // ctx.sonicBoom.Play();
        t = 0f;
        dashSpeed = ctx.moveData.velocity.magnitude + 10f;
        dashDir = ctx.avatarLookForward;
    }

    public override void UpdateState()
    {

        Dash(dashDir, dashSpeed);

        t += Time.deltaTime;

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
        if (t > .5f) {
            SwitchState(_factory.Neutral());
        }
    }

    

}