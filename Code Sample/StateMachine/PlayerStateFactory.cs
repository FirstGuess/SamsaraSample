using System.Collections.Generic;

public class PlayerStateFactory {

    enum State {
        neutral,
        dash,
        air,
        fall,
        dive,
        grounded,
        grapple,
        melee,
        lunge,
        lungecooldown,
        noop
    }

    PlayerStateMachine _context;
    Dictionary<State, PlayerBaseState> _states = new Dictionary<State, PlayerBaseState>();

    public PlayerStateFactory(PlayerStateMachine currentContext) {
        _context = currentContext;

        // root states

        _states[State.air] = new PlayerStateAir(_context, this);
        _states[State.grounded] = new PlayerStateGrounded(_context, this);
        _states[State.grapple] = new PlayerStateGrapple(_context, this);

        // sub states

        _states[State.neutral] = new PlayerStateNeutral(_context, this);
        _states[State.dash] = new PlayerStateDash(_context, this);
        _states[State.fall] = new PlayerStateFall(_context, this);
        _states[State.dive] = new PlayerStateDive(_context, this);
        
        _states[State.melee] = new PlayerStateMelee(_context, this);
        _states[State.lunge] = new PlayerStateLunge(_context, this);
        _states[State.lungecooldown] = new PlayerStateLungeCooldown(_context, this);
        _states[State.noop] = new PlayerStateNoop(_context, this);

    }

    public PlayerBaseState Neutral() {
        return _states[State.neutral];
    }

    public PlayerBaseState Dash() {
        return _states[State.dash];
    }

    public PlayerBaseState Dive() {
        return _states[State.dive];
    }

    public PlayerBaseState Fall() {
        return _states[State.fall];
    }

    public PlayerBaseState Air() {
        return _states[State.air];
    }

    public PlayerBaseState Grounded() {
        return _states[State.grounded];
    }

    public PlayerBaseState Grapple() {
        return _states[State.grapple];
    }

    public PlayerBaseState Melee() {
        return _states[State.melee];
    }

    public PlayerBaseState Lunge() {
        return _states[State.lunge];
    }

    public PlayerBaseState LungeCooldown() {
        return _states[State.lungecooldown];
    }

    public PlayerBaseState Noop() {
        return _states[State.noop];
    }
}