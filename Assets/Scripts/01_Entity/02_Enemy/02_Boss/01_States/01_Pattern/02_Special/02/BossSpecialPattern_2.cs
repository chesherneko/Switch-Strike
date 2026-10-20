using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;

public class BossSpecialPattern_2 : BossPatternStateBase
{
    [Title("Components")]
    [SerializeField] private BossMinionSpawner minionSpawner;
    [SerializeField] private GameObject successAttack;
    [SerializeField] private GameObject failAttack;
    [SerializeField] private GameObject[] hurtBoxes;

    [Title("Settings")]
    [SerializeField] private float maxPatternTime;

    [SerializeField, TitleGroup("Lag")] private float successLag;
    [SerializeField, TitleGroup("Lag")] private float failLag;

    private WaitForSeconds waitForSuccess;
    private WaitForSeconds waitForFail;

    protected override int PatternStartTrigger => Animator.StringToHash("SpecialAttack2");
    private readonly int patternEndTrigger = Animator.StringToHash("SpecialAttack2_End");

    protected override void OnDisable()
    {
        successAttack.SetActive(false);
        failAttack.SetActive(false);
    }

    protected override void InitYieldInstructions()
    {
        base.InitYieldInstructions();
        waitForSuccess = new(successLag);
        waitForFail = new(failLag);
    }

    protected override IEnumerator PatternCoroutine()
    {
        yield return base.PatternCoroutine();
        SetActiveHurtBoxes(false);

        yield return minionSpawner.StartPattern();

        bool isSuccess = false;

        float time = 0f;

        while (true)
        {
            if (minionSpawner.IsCleared)
            {
                isSuccess = true;
                break;
            }
            if (time > maxPatternTime) break;

            time += Time.deltaTime;
            yield return null;
        }

        if (isSuccess == false)
            yield return minionSpawner.FailPattern();

        yield return new WaitUntil(() => minionSpawner.IsFinished);

        Animator.SetTrigger(patternEndTrigger);
        yield return waitForEndLag;

        GameObject attack = isSuccess ? successAttack : failAttack;
        WaitForSeconds waitForSeconds = isSuccess ? waitForSuccess : waitForFail;

        attack.SetActive(true);
        yield return waitForSeconds;
        attack.SetActive(false);

        SetActiveHurtBoxes(true);
        owner.ChangeState(typeof(BossIdleState));
    }

    private void SetActiveHurtBoxes(bool isActive)
    {
        for (int i = 0; i < hurtBoxes.Length; i++)
            hurtBoxes[i].SetActive(isActive);
    }
}
