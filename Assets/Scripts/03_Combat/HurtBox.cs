using Sirenix.OdinInspector;
using UnityEngine;

public class HurtBox : MonoBehaviour
{
    [Title("Components")]
    [SerializeField] private new Collider2D collider;

    [Title("Settings")]
    [field: SerializeField] public float HitStopDuration { get; private set; } = 0.1f;

    [Title("Runtime States")]
    [ShowInInspector, ReadOnly] public CombatEntity Owner { get; private set; }

    private void Awake()
    {
        Owner = GetComponentInParent<CombatEntity>();
        if (Owner == null) Destroy(gameObject);
    }

    private void OnDestroy()
    {
        CombatManager instance = CombatManager.Instance;

        if (instance == null) return;
        instance.RemoveHurtBox(collider);
    }
}
