using UnityEngine;

public class MooniSkillEndState : PlayerAnimationStateBase
{
    private bool isApplied = false;

    private new MooniController Owner { get; }

    public MooniSkillEndState(MooniController owner) : base(owner)
    {
        Owner = owner;
    }

    public override void OnEnter()
    {
        Owner.StopMovementInput();
        Animator.SetBool(AnimationParam.CAN_JUMP_OR_FALL, false);
    }

    public override void OnUpdateState(AnimatorStateInfo stateInfo)
    {
        Owner.HandleDashInput();

        if (isApplied == false && stateInfo.normalizedTime >= Owner.SkillApplyLag)
        {
            isApplied = true;
            Owner.ApplySkill();
            Owner.RequestStartSkillCooldown();
        }

        if (stateInfo.normalizedTime < 0.75f) return;
    }
    
    public override void OnExit()
    {
        isApplied = false;
        Animator.SetBool(AnimationParam.CAN_JUMP_OR_FALL, true);
    }
}
