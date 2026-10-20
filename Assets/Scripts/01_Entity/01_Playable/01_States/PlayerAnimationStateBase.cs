using UnityEngine;

public class PlayerAnimationStateBase : AnimationStateBase
{
    public const float INPUT_RESET_TIME = 0.3f;
    public const float COMBO_INPUT_END_TIME = 0.9f;

    protected PlayerController Owner { get; }

    protected PlayerAnimationStateBase(PlayerController owner) : base(owner)
    {
        Owner = owner;
    }
}
