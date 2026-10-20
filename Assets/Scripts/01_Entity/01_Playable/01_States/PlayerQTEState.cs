using UnityEngine;

public class PlayerQTEState : PlayerAnimationStateBase
{
    public PlayerQTEState(PlayerController owner) : base(owner) { }

    public override void OnEnter()
    {
        Owner.StopMovementInput();
        Animator.SetBool(AnimationParam.CAN_JUMP_OR_FALL, false);
    }

    public override void OnExit()
    {
        Animator.SetBool(AnimationParam.CAN_JUMP_OR_FALL, true);

        if (Animator.GetBool(AnimationParam.IS_GROUNDED))
            Animator.ResetTrigger(AnimationParam.FALL);
    }
}
