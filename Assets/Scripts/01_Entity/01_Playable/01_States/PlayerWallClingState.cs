using Unity.VisualScripting;
using UnityEngine;

public class PlayerWallClingState : PlayerAnimationStateBase
{
    public PlayerWallClingState(PlayerController owner) : base(owner) { }

    public override void OnUpdateState(AnimatorStateInfo stateInfo)
    {
        Owner.HandleMoveInput();
        Owner.HandleJumpInput();
    }
}
