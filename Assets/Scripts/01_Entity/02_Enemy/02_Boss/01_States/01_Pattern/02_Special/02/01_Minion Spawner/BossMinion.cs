using Sirenix.OdinInspector;
using UnityEngine;

public class BossMinion : MonoBehaviour
{
    [Title("Components")]
    [SerializeField] private Animator animator;

    [Title("Runtime States")]
    [ShowInInspector, ReadOnly] private BossMinionSpawner owner;

    private void Awake()
    {
        owner = GetComponentInParent<BossMinionSpawner>();

        if (owner == null)
            Debug.LogError("부모에게서 BossMinionSpawner를 찾지 못했습니다.");
    }

    private void OnDestroy()
    {
        owner.NotifyMinionDestroyed(this);
    }

    public void Kill()
    {
        animator.SetTrigger(AnimationParam.DEAD);
    }
}
