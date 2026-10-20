using UnityEngine;

public class PlayerJumpState : PlayerAnimationStateBase
{
    public PlayerJumpState(PlayerController owner) : base(owner) { }

    public override void OnUpdateState(AnimatorStateInfo stateInfo)
    {
        Owner.HandleMoveInput();
        Owner.HandleJumpInput();
        Owner.HandleDashInput();
        Owner.HandleAttackInput();
    }
}
