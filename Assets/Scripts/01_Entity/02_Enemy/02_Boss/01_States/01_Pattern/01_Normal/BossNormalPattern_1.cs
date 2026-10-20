using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class BossNormalPattern_1 : BossPatternStateBase
{
    [Title("Components")]
    [SerializeField] private Animator animator;

    [Title("Settings")]
    [SerializeField] private float followDuration = 2f;
    [SerializeField] private Vector3 followOffset;

    protected override void OnDisable()
    {
        animator.gameObject.SetActive(false);
    }

    protected override IEnumerator PatternCoroutine()
    {
        yield return base.PatternCoroutine();

        animator.gameObject.SetActive(true);

        float time = 0f;
        while (true)
        {
            if (time > followDuration) break;

            time += Time.deltaTime;

            Transform playerTrans = Player.Local.transform;
            Vector3 playerPos = playerTrans.position;

            transform.position = playerPos + followOffset;

            yield return null;
        }

        yield return waitForStartLag;
        animator.SetTrigger(AnimationParam.HIT);

        yield return waitForEndLag;
        owner.ChangeState(typeof(BossIdleState));
    }
}
