using UnityEngine;

public class PlayerAttackEndState : PlayerAnimationStateBase
{
    public PlayerAttackEndState(PlayerController owner) : base(owner) { }

    public override void OnEnter()
    {
        Owner.IsLockMoveDirection = true;

        Animator.SetBool(AnimationParam.IS_ATTACK, false);
        Animator.SetBool(AnimationParam.CAN_JUMP_OR_FALL, false);
    }

    public override void OnUpdateState(AnimatorStateInfo stateInfo)
    {
        Owner.HandleAirMoveInput();
        Owner.HandleDashInput();
        Owner.HandleSkillInput();
    }

    public override void OnExit()
    {
        Owner.IsLockMoveDirection = false;
        Animator.SetBool(AnimationParam.CAN_JUMP_OR_FALL, true);
    }
}
