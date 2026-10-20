using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerCombat : CombatEntity
{
    [Title("Components")]
    [SerializeField] private Player owner;
    [SerializeField] private GameObject hurtBox;
    [SerializeField] private HitEffect hitEffect;

    [Title("Settings")]
    [SerializeField] private float dashInvincibleDuration = 0.05f;
    [SerializeField] private float qteGaugeGainMult = 4f;

    [Title("Runtime State")]
    [ShowInInspector, ReadOnly] private bool isDashInvincible = false;
    [ShowInInspector, ReadOnly] private float dashTime = 0f;

    private void Update()
    {
        if (isDashInvincible == false) return;

        dashTime += Time.deltaTime;

        if (dashTime > dashInvincibleDuration)
        {
            dashTime = 0f;
            isDashInvincible = false;
        }
    }

    public override bool CanTakeDamage(CombatEntity sender, CombatEvent @event)
    {
        if (isDashInvincible)
        {
            PlayerSwitchManager.Instance.GainQTEGauge(qteGaugeGainMult);
            return false;
        }

        return true;
    }

    public override void TakeDamage(int damage)
    {
        Player.Local.Combat.ApplyDamage(damage);
        PlayerUI.Instance.UpdateRemainHP(Player.Local.Combat.HPRatio);

        if (IsDead == false) hitEffect.Play();
        else owner.Controller.Dead();
    }

    public override void TakeHeal(int heal)
    {
        Player.Local.Combat.ApplyHeal(heal);
        PlayerUI.Instance.UpdateRemainHP(Player.Local.Combat.HPRatio);
    }

    public void TakeHealByMaxHPRatio(float ratio)
    {
        int healAmount = Mathf.RoundToInt(stat.MaxHP * ratio);
        TakeHeal(healAmount);
    }

    public void SyncPrevPlayerState(PlayerCombatSyncData syncData)
    {
        CurrentHP = syncData.CurrentHP;
    }

    public void OnDashStarted() => isDashInvincible = true;

    public void OnDashFinished()
    {
        dashTime = 0f;
        isDashInvincible = false;
    }

    public void SetHurtBoxActive(bool isActive)
        => hurtBox.SetActive(isActive);
}
