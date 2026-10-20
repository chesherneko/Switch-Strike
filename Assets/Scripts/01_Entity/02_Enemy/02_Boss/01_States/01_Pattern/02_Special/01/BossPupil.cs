using Sirenix.OdinInspector;
using UnityEngine;

public class BossPupil : MonoBehaviour
{
    [Title("Components")]
    [field: SerializeField] public SpriteRenderer Renderer { get; private set; }

    [Title("Settings")]
    [SerializeField] private float moveRadius = 0.25f;

    [Title("Runtime States")]
    [ShowInInspector, ReadOnly] private bool isFollowTarget = false;
    [ShowInInspector, ReadOnly] private Transform target;

    [PropertySpace]
    [ShowInInspector, ReadOnly] public Vector2 LookDirection { get; private set; }

    private void Update()
    {
        if (isFollowTarget == false) return;

        LookDirection = FollowTarget();
    }

    private Vector2 FollowTarget()
    {
        Vector2 center = transform.parent.position;
        Vector2 targetPos = target.position;

        Vector2 dir = targetPos - center;
        dir = dir.normalized;

        Vector2 pos = center + dir * moveRadius;

        transform.position = pos;

        return dir;
    }

    public void RequestFollowTarget(Transform target)
    {
        if (target == null)
        {
            Debug.LogError("입력받은 타겟의 값이 비어있습니다.");
            return;
        }

        this.target = target;
        isFollowTarget = true;
    }

    public void StopFollowTarget()
    {
        isFollowTarget = false;
        target = null;
    }
}
