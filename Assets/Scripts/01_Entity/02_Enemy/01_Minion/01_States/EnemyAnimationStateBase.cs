using UnityEngine;

public class EnemyAnimationStateBase : AnimationStateBase
{
    protected EnemyController Owner { get; }

    public EnemyAnimationStateBase(EnemyController owner) : base(owner)
    {
        Owner = owner;
    }
}
