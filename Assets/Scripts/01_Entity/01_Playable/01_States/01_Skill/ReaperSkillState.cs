using UnityEngine;

public class ReaperSkillState : PlayerAnimationStateBase
{
    private float skillKeepTime = 0f;

    private bool isTriggered = false;

    private new ReaperController Owner { get; }

    public ReaperSkillState(ReaperController owner) : base(owner)
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
        if (stateInfo.normalizedTime < 0.95f) return;
        if (isTriggered) return;

        skillKeepTime += Time.deltaTime;

        Owner.HandleMoveInput();

        if (Owner.WasPressedSkillKey() || skillKeepTime > Owner.SkillKeepDuration)
        {
            Owner.StopMovementInput();

            isTriggered = true;
            Animator.SetTrigger(AnimationParam.SKILL_END);
        }
    }
    
    public override void OnExit()
    {
        skillKeepTime = 0f;
        isTriggered = false;
    }
}
