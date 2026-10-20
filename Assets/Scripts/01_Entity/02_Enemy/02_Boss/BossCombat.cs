using UnityEngine;

public class BossCombat : EnemyCombat
{
    [SerializeField] private BossController controller;

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        BossUI.Instance.UpdateHP(HPRatio);
    }

    protected override void OnDead()
    {
        controller.ChangeState(typeof(BossDeadState));
    }
}
