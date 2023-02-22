using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateNoop : PlayerBaseState {

    public PlayerStateNoop(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base (currentContext, playerStateFactory) {
        _isRootState = false;
        name = "noop";
    }

    public override void EnterState()
    {

    }

    public override void UpdateState()
    {

    }

    public override void ExitState()
    {

    }

    public override void InitializeSubStates()
    {

    }

    public override void CheckSwitchStates()
    {

    }


}