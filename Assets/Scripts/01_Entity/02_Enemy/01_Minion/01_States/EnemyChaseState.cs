using UnityEngine;

public class EnemyChaseState : EnemyAnimationStateBase
{
    public EnemyChaseState(EnemyController owner) : base(owner) { }

    public override void OnUpdateState(AnimatorStateInfo stateInfo)
    {
        if (Owner.TryAttackTarget()) return;
        if (Owner.TryChaseTarget() == false) return;

        Owner.FaceTowards(Owner.Target.position.x);

        Transform trans = Owner.transform;

        Vector3 currentPoint = trans.position;
        Vector3 targetPoint = Owner.GetChasePoint();
        float maxDistDelta = Owner.ChaseSpeed * Time.deltaTime;

        trans.position = Vector3.MoveTowards(currentPoint, targetPoint, maxDistDelta);
    }
}
