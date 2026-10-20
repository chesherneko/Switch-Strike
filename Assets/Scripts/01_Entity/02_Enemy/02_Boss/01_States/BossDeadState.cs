using System.Collections;
using UnityEngine;

public class BossDeadState : BossStateBase
{
    [SerializeField] private GameObject bossGameObject;
    [SerializeField] private GameObject[] hurtBoxes;

    protected override void OnEnable()
    {
        for (int i = 0; i < hurtBoxes.Length; i++)
            hurtBoxes[i].SetActive(false);

        Animator.SetTrigger(AnimationParam.DEAD);

        StartCoroutine(DeadCoroutine());
    }

    protected override void OnDisable() { }

    private IEnumerator DeadCoroutine()
    {
        yield return new WaitUntil(() => Animator.IsCurrentAnimationFinished());
        
        BossUI.Instance.SetActiveUI(false);
        GameClearUI.Instance.GameClear();

        Destroy(bossGameObject);
    }
}
