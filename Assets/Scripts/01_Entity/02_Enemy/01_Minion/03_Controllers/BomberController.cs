using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

//기본적으로 공격 1만 실행
//만약 HP가 30% 이하인 상태라면, 무조건 공격 2로 실행
//공격 2를 실행 시, 공격이 끝난 후 HP를 0 및 죽음 판정 실행
public class BomberController : EnemyController
{
    [SerializeField, TitleGroup("Components")] private new SpriteRenderer renderer;

    [Title("Bomber")]
    [SerializeField] private float suicideHPRatioThreshold = 0.3f;
    [Space, ShowInInspector, ReadOnly] private bool isSuicide = false;

    protected override int Attack()
    {
        int id;

        float hpRatio = combat.HPRatio;

        if (hpRatio <= suicideHPRatioThreshold)
        {
            id = AnimationParam.ATTACK_2;
            isSuicide = true;
        }
        else id = AnimationParam.ATTACK_1;

        Animator.SetTrigger(id);

        return id;
    }

    public override void OnAttackEnd()
    {
        base.OnAttackEnd();

        if (isSuicide)
        {
            isSuicide = false;
            Animator.SetTrigger(AnimationParam.DEAD);
        }
    }

    public override void ResetAttackState()
    {
        base.ResetAttackState();
        isSuicide = false;
    }

    public void DisableRenderer() => renderer.enabled = false;

    public void EnableRenderer() => renderer.enabled = true;
}
