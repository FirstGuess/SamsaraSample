using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerStateMelee : PlayerBaseState {

    float initialDistance = 0f;
    Vector3 angleDir = Vector3.zero;
    Vector3 angleForward = Vector3.zero;
    Vector3 angleTarget = Vector3.zero;


    public PlayerStateMelee(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base (currentContext, playerStateFactory) {
        _isRootState = false;
        name = "melee";
    }

    public override void EnterState()
    {
        Debug.Log("ENTER MELEE");
        // FindLookingAtTarget();
        t = 0f;
        // ctx.moveData.velocity = Vector3.zero;
        // ctx.moveData.attacking = false;
        
        
    }

    public override void UpdateState()
    {


        // IHittable ball = GameObject.Instantiate(ctx.ballObj, ctx.avatarLookTransform.position + ctx.avatarLookForward * 5f, ctx.avatarLookRotation).GetComponent<IHittable>();
        // ball.GetHit(ctx.avatarLookForward * 10f);


        if (ctx.currentTarget) {
            // if (ctx.currentTarget.TryGetComponent(out IHittable target)) {

                // if (ctx.moveData.wishFireDown) {

                //     // ball.Stop();
                //     t = 0f;


                // } else {
                    
                //     string[] mask = new string[] { "Ball", "Enemy" };

                //     Collider[] hits = Physics.OverlapSphere(ctx.moveData.origin, 3f);

                //     foreach (Collider hit in hits) {
                        
                //         if (hit.TryGetComponent(out IHittable _ball)) {
                //             _ball.GetHit(angleForward * 15f);
                //             ctx.moveData.velocity = Vector3.zero;
                //         }

                //     }

                Vector3 toTarget = (ctx.moveData.targetPoint - ctx.moveData.origin);

                ctx.moveData.velocity = Vector3.Slerp(ctx.moveData.velocity, toTarget, Time.deltaTime * 2f);


                    

                // }


                t += Mathf.Min(3f, Time.deltaTime);
            // }
        } else {
            ctx.moveData.attacking = false;
        }

        if (ctx.moveData.wishFireUp) {
            ctx.moveData.attacking = false;
            ctx.slashObj.SetActive(true);
            ctx.slash.Play();
        }

        CheckSwitchStates();
    }

    public override void ExitState()
    {
        ctx.reduceGravityTimer = 0f;
 
        Debug.Log("EXIT MELEE");
    }

    public override void InitializeSubStates()
    {

    }

    public override void CheckSwitchStates()
    {

        if (ctx.moveData.grounded && !ctx.moveData.attacking) {
            SwitchState(_factory.Neutral());
        } else if (!ctx.moveData.grounded  && !ctx.moveData.attacking) {
            SwitchState(_factory.Fall());
        }
    }

    // private void LungeTargeting() {

    //     if (Vector3.Dot(ctx.avatarLookForward, ctx.moveData.targetDir) < .5f) {
    //         ctx.currentTarget = null;
    //         t = 1f;
    //         return;
    //     }

    //     ctx.bezierCurve.AttackArc(Vector3.ProjectOnPlane(ctx.moveData.targetNormal, Vector3.up).normalized, ctx.currentTarget.transform.position);

    // }

    // private void FindLookingAtTarget() {

    //     if (ctx.targetLength > 0) {
            
    //         Vector3 _targetPos = Vector3.zero;
    //         Vector3 _targetDir = Vector3.zero;

    //         foreach (Collider target in ctx.moveData.targets) {

    //             if (target == null) continue;

    //             _targetPos = target.transform.position;
    //             _targetDir = (target.transform.position - ctx.avatarLookTransform.position).normalized;

    //             if (Vector3.Dot(ctx.avatarLookForward, _targetDir) >= .5f) {
    //                 ctx.moveData.targetPoint = _targetPos;
    //                 ctx.moveData.targetDir = _targetDir;
    //                 ctx.moveData.distanceFromTarget = (ctx.moveData.targetPoint - ctx.avatarLookTransform.position).magnitude;
    //                 ctx.currentTarget = target;
    //             }
    //         }
            
    //     }

    //     if (ctx.moveData.targetDir == Vector3.zero) {
    //         ctx.currentTarget = null;
    //         ctx.moveData.targetPoint = Vector3.zero;
    //         ctx.moveData.targetDir = Vector3.zero;
    //         ctx.moveData.distanceFromTarget = 0f;
    //         ctx.moveData.targetNormal = Vector3.zero;
    //         return;
    //     }

    //     // Ray ray = new Ray(ctx.avatarLookTransform.position, ctx.moveData.targetDir);
    //     // RaycastHit hit;

    //     // if (Physics.SphereCast(ray, 1f, out hit, 300f, LayerMask.GetMask (new string[] { "Enemy" }))) {
    //     //     ctx.moveData.targetNormal = hit.normal;
    //     //     // ctx.bezierCurve = new BezierCurve(ctx);
    //     //     // ctx.bezierCurve.AttackArc(ctx.moveData.targetNormal, ctx.moveData.targetPoint);
    //     // } else {
    //     //     ctx.currentTarget = null;
    //     //     ctx.moveData.targetPoint = Vector3.zero;
    //     //     ctx.moveData.targetDir = Vector3.zero;
    //     //     ctx.moveData.distanceFromTarget = 0f;
    //     //     ctx.moveData.targetNormal = Vector3.zero;
    //     //     Debug.Log("miss");
    //     // }


    //     // Collision closestTarget = null;

    //     // foreach (Collision target in moveData.targets) {

    //     //     if (closestTarget == null) {
    //     //         closestTarget = target;
    //     //     } else {

    //     //         if (Vector3.Distance(target.transform.position, moveData.origin) > Vector3.Distance(closestTarget.transform.position, moveData.origin)) {
    //     //             closestTarget = target;
    //     //         }

    //     //     }

    //     // }

    // }


    

}