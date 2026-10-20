using UnityEngine;

public class EnemySpawnState : EnemyAnimationStateBase
{
    public EnemySpawnState(EnemyController owner) : base(owner) { }

    public override void OnEnter()
    {
        Owner.Spawn();
    }
}
