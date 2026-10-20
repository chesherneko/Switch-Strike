using UnityEngine;

public class ReaperController : PlayerController
{
    [field: SerializeField] public float SkillKeepDuration { get; private set; } = 2f;

    protected override PlayerAnimationStateBase CreateSkillState()
        => new ReaperSkillState(this);

    protected override PlayerAnimationStateBase CreateSkillEndState()
        => new ReaperSkillEndState(this);
}
