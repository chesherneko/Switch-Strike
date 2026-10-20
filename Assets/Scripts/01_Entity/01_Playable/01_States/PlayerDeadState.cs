using UnityEngine;

public class PlayerDeadState : PlayerAnimationStateBase
{
    private bool isRespawnRequested = false;

    public PlayerDeadState(PlayerController owner) : base(owner) { }

    public override void OnUpdateState(AnimatorStateInfo stateInfo)
    {
        if (isRespawnRequested) return;
        if (stateInfo.normalizedTime < 1f) return;

        isRespawnRequested = true;
        RespawnManager.Instance.Respawn();
    }

    public override void OnExit()
    {
        isRespawnRequested = false;
    }
}
