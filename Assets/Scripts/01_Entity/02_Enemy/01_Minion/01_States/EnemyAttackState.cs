using UnityEngine;

public class EnemyAttackState : EnemyAnimationStateBase
{
    public EnemyAttackState(EnemyController owner) : base(owner) { }

    public override void OnEnter()
    {
        Owner.FaceTowards(Owner.Target.position.x);
    }

    public override void OnExit()
    {
        Owner.OnAttackEnd();
    }
}
