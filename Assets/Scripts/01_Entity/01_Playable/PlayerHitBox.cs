using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerHitBox : HitBox
{
    [SerializeField, TitleGroup("Settings")] private float qteGaugeGainMult = 1f;

    protected override bool TryHit(Collider2D collision)
    {
        if (base.TryHit(collision) == false)
            return false;

        PlayerSwitchManager.Instance.GainQTEGauge(qteGaugeGainMult);
        return true;
    }
}
