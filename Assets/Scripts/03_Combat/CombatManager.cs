using System.Collections.Generic;
using UnityEngine;

public enum CombatEventType
{
    Damage,
    Heal
}

public struct CombatEvent
{
    public CombatEventType EventType;
    public int Amount;
    public Vector3 Position;

    public float HitStopDuration;
}

public class CombatManager : MonoSingleton<CombatManager>
{
    private readonly Dictionary<Collider2D, HurtBox> entityCaches = new();

    public void RemoveHurtBox(Collider2D col)
    {
        if (entityCaches.ContainsKey(col) == false) return;
        entityCaches.Remove(col);
    }

    public bool TryGetHurtBox(Collider2D col, out HurtBox hurtBox)
    {
        hurtBox = null;

        if (entityCaches.TryGetValue(col, out HurtBox result))
        {
            hurtBox = result;
            return true;
        }

        if (col.TryGetComponent(out result) == false)
            return false;

        hurtBox = result;
        entityCaches[col] = hurtBox;

        return true;
    }

    public void To(CombatEntity sender, CombatEntity receiver, CombatEvent @event)
    {
        if (sender == null || receiver == null)
        {
            Debug.LogWarning(
                $"sender: {sender == null}\n" +
                $"receiver: {receiver == null}"
            );
            return;
        }

        switch (@event.EventType)
        {
            case CombatEventType.Damage:
                ApplyDamage(sender, receiver, @event);
                break;
            case CombatEventType.Heal:
                receiver.TakeHeal(@event.Amount);
                break;
            default:
                throw new System.NotImplementedException();
        }
    }

    private void ApplyDamage(CombatEntity sender, CombatEntity receiver, CombatEvent @event)
    {
        if (receiver.CanTakeDamage(sender, @event) == false)
        {
            return;
        }

        HitStopManager.Instance.Play(@event.HitStopDuration);
        receiver.TakeDamage(@event.Amount);
    }
}
