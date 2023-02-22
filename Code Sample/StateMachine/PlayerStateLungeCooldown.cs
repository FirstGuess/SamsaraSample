using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateLungeCooldown : PlayerBaseState {

    Vector3 toTarget = Vector3.zero;

    public PlayerStateLungeCooldown(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base (currentContext, playerStateFactory) {
        _isRootState = false;
        name = "lungecooldown";
    }

    public override void EnterState()
    {
        // Debug.Log("ENTER LUNGECOOLDOWN");
        t = 0f;
        ctx.moveData.velocity /= 2f;

    }

    public override void UpdateState()
    {

        // ctx.moveData.velocity = Vector3.Lerp(ctx.moveData.velocity, Vector3.zero, Time.deltaTime * 8f);
        // SubtractVelocityAgainst(ctx.moveData.velocity, ctx.moveData.velocity.magnitude * 2f);

        t += Time.deltaTime;
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        ctx.moveData.attacking = false;
        ctx.slashObj.SetActive(false);
        ctx.slash.Stop();
    }

    public override void InitializeSubStates()
    {

    }

    public override void CheckSwitchStates()
    {

        if (t > .5f) {
            SwitchState(factory.Neutral());
        }
    }

}