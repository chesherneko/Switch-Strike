using Sirenix.OdinInspector;
using UnityEngine;

public class ArcherController : EnemyController
{
    [SerializeField, TitleGroup("Components")] private ProjectileShooter normalShooter;
    [SerializeField, TitleGroup("Components")] private ProjectileShooter enhancedShooter;

    private Vector2 targetPosition;
    private bool isReady = false;

    public void ReadyToShoot()
    {
        if (isReady) return;

        isReady = true;
        targetPosition = Target.position;
    }

    public void LockOnTarget()
    {
        if (isReady == false) return;

        Vector2 currentTargetPos = Target.position;

        float readyXDir = Mathf.Sign(targetPosition.x);
        float currentXDir = Mathf.Sign(currentTargetPos.x);

        //준비 단계의 x축 방향이 같다면, 타겟 위치를 쏘기 직전에 다시 초기화함으로서
        //투사체가 나아갈 방향을 고정한 채, 높이를 보정
        if (readyXDir == currentXDir)
            targetPosition.y = Target.position.y;
    }

    public void ShootNormalArrow() => ShootArrow(isEnhanced: false);

    public void ShootEnhancedArrow() => ShootArrow(isEnhanced: true);

    private void ShootArrow(bool isEnhanced)
    {
        ProjectileShooter shooter = isEnhanced switch
        {
            false => normalShooter,
            _ => enhancedShooter
        };

        Vector2 pos = isReady ? targetPosition : Target.position;
        shooter.ShootProjectile(pos);

        isReady = false;
    }
}
