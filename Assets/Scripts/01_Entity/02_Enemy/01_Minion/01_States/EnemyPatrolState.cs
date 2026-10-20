using UnityEngine;

public class EnemyPatrolState : EnemyAnimationStateBase
{
    private Vector3 goalPoint;

    public EnemyPatrolState(EnemyController owner) : base(owner) { }

    public override void OnEnter()
    {
        goalPoint = Owner.GetPatrolPoint();
        Owner.FaceTowards(goalPoint.x);
    }

    public override void OnUpdateState(AnimatorStateInfo stateInfo)
    {
        if (Owner.TryAttackTarget()) return;
        if (Owner.TryChaseTarget()) return;

        Transform trans = Owner.transform;

        Vector3 currentPoint = trans.position;
        float maxDistDelta = Owner.PatrolSpeed * Time.deltaTime;

        trans.position = Vector3.MoveTowards(currentPoint, goalPoint, maxDistDelta);

        if (trans.position == goalPoint)
            Animator.SetBool(AnimationParam.IS_MOVING, false);
    }
}
