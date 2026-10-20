using Sirenix.OdinInspector;
using UnityEngine;

public class SpearmanController : EnemyController
{
    [PropertyOrder(1)]
    [ShowInInspector, ReadOnly, TitleGroup("Attack")] private bool isCharged = false;
    [ShowInInspector, ReadOnly, TitleGroup("Attack")] private int lastAttackID;

    protected override int Attack()
    {
        int id;

        if (isCharged)
        {
            id = AnimationParam.ATTACK_1;
            Animator.SetTrigger(id);
        }
        else id = base.Attack();

        if (id == AnimationParam.ATTACK_2)
        {
            isCharged = true;
            combat.SetATKMultiplier(2f);
        }

        lastAttackID = id;
        return id;
    }
    
    public override void OnAttackEnd()
    {
        base.OnAttackEnd();

        if (lastAttackID != AnimationParam.ATTACK_1) return;
        if (isCharged == false) return;

        isCharged = false;
        combat.ResetATKMultiplier();
    }
}
