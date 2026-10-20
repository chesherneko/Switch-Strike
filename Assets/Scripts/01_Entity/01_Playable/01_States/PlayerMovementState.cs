using UnityEngine;

public class PlayerMovementState : PlayerAnimationStateBase
{
    public PlayerMovementState(PlayerController owner) : base(owner) { }

    public override void OnUpdateState(AnimatorStateInfo stateInfo)
    {
        Owner.HandleMoveInput();
        Owner.HandleJumpInput();
        Owner.HandleDashInput();
        Owner.HandleAttackInput();
        Owner.HandleSkillInput();
    }
}
