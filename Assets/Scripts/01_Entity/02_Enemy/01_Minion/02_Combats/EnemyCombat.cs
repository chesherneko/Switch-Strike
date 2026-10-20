using Sirenix.OdinInspector;
using UnityEngine;

public enum HitReactionType
{
    Animation,
    Shader,
    Both
}

public class EnemyCombat : CombatEntity
{
    [SerializeField, TitleGroup("Components")] private Animator animator;
    [SerializeField, TitleGroup("Components")] private HitEffect hitEffect;

    [SerializeField, TitleGroup("Settings")] private HitReactionType hitReactionType;

    public override void TakeDamage(int damage)
    {
        bool isDead = ApplyDamage(damage);

        if (isDead)
        {
            OnDead();
            return;
        }

        switch (hitReactionType)
        {
            case HitReactionType.Animation:
                animator.SetTrigger(AnimationParam.HIT);
                break;
            case HitReactionType.Shader:
                hitEffect.Play();
                break;
            case HitReactionType.Both:
                animator.SetTrigger(AnimationParam.HIT);
                hitEffect.Play();
                break;
            default:
                throw new System.NotImplementedException();
        }
    }

    public override void TakeHeal(int heal)
    {
        
    }

    protected virtual void OnDead()
    {
        animator.SetTrigger(AnimationParam.DEAD);
    }
}
