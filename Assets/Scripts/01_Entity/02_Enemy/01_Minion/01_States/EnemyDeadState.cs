using UnityEngine;

public class EnemyDeadState : EnemyAnimationStateBase
{
    private float respawnTime = 0f;

    public EnemyDeadState(EnemyController owner) : base(owner) { }

    public override void OnEnter()
    {
        //간혈적으로 특정 미이이세한 타이밍에 맞아 죽으면
        //공격 상태가 켜진 상태로 죽어버림
        //그래서 부활 후 곧바로 공격 모션이 나오는데 이를 방지하기 위함
        Owner.ResetAttackState();
    }

    public override void OnUpdateState(AnimatorStateInfo stateInfo)
    {
        if (stateInfo.normalizedTime < 1f) return;

        respawnTime += Time.deltaTime;
        Owner.UpdateDeadState(respawnTime);
    }

    public override void OnExit()
    {
        respawnTime = 0f;
    }
}
