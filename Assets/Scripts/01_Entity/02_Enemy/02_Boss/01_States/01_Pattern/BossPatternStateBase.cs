using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public abstract class BossPatternStateBase : BossStateBase
{
    protected WaitForSeconds waitForStartLag;
    protected WaitForSeconds waitForEndLag;

    [SerializeField, TitleGroup("Lag")] protected float startLag;
    [SerializeField, TitleGroup("Lag")] protected float endLag;

    protected virtual int PatternStartTrigger { get; } = Animator.StringToHash("NormalAttack");

    protected virtual void Awake()
    {
        InitYieldInstructions();
    }

    protected override void OnEnable()
    {
        StartCoroutine(PatternCoroutine());
    }

    [PropertySpace, Button(ButtonHeight = 40)]
    protected virtual void InitYieldInstructions()
    {
        waitForStartLag = new WaitForSeconds(startLag);
        waitForEndLag = new WaitForSeconds(endLag);
    }

    protected virtual IEnumerator PatternCoroutine()
    {
        Animator.SetTrigger(PatternStartTrigger);
        yield return waitForStartLag;
    }
}
