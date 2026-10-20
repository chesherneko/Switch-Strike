using UnityEngine;

public class ReaperSkillEndState : PlayerAnimationStateBase
{
    private bool isRequest = false;

    public ReaperSkillEndState(PlayerController owner) : base(owner) { }

    public override void OnUpdateState(AnimatorStateInfo stateInfo)
    {
        if (isRequest) return;
        if (stateInfo.normalizedTime < 0.75f) return;

        isRequest = true;
        Owner.RequestStartSkillCooldown();
    }

    public override void OnExit()
    {
        isRequest = false;
        Animator.SetBool(AnimationParam.CAN_JUMP_OR_FALL, true);
    }
}
