using UnityEngine;

public class MooniController : PlayerController
{
    [SerializeField] private float healRatio = 0.3f;
    [field: SerializeField] public float SkillApplyLag { get; private set; } = 0.5f;

    protected override PlayerAnimationStateBase CreateSkillState() => null;
    protected override PlayerAnimationStateBase CreateSkillEndState()
        => new MooniSkillEndState(this);

    protected override void RequestSkill()
    {
        Animator.SetTrigger(AnimationParam.SKILL_END);
    }

    public void ApplySkill()
    {
        Combat.TakeHealByMaxHPRatio(healRatio);
    }
}
