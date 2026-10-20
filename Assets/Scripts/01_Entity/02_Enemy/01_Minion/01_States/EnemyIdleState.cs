using UnityEngine;

public class EnemyIdleState : EnemyAnimationStateBase
{
    private float timer = 0f;
    private float waitingTime = 0f;

    public EnemyIdleState(EnemyController owner) : base(owner) { }

    public override void OnEnter()
    {
        waitingTime = Owner.GetWaitingTime();

        Animator.SetBool(AnimationParam.IS_MOVING, false);
        Animator.SetBool(AnimationParam.IS_CHASING, false);
    }

    public override void OnUpdateState(AnimatorStateInfo stateInfo)
    {
        if (Owner.TryAttackTarget()) return;
        if (Owner.TryChaseTarget()) return;

        timer += Time.deltaTime;

        if (timer >= waitingTime)
        {
            waitingTime = 0f;
            Animator.SetBool(AnimationParam.IS_MOVING, true);
        }
    }

    public override void OnExit()
    {
        timer = 0f;
    }
}
