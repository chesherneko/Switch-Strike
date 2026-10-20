using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class BossNormalPattern_3 : BossPatternStateBase
{
    private WaitForSeconds waitForActiveLag;

    [SerializeField, TitleGroup("Lag")] private float activeLag = 0.2f;

    [Title("Runtime States")]
    [ShowInInspector, ReadOnly] private GameObject[] sparks;

    protected override void Awake()
    {
        base.Awake();
        InitSparkObjects();
    }

    protected override void OnEnable()
    {
        sparks.Shuffle();
        base.OnEnable();
    }

    protected override void OnDisable() { }

    protected override void InitYieldInstructions()
    {
        base.InitYieldInstructions();
        waitForActiveLag = new WaitForSeconds(activeLag);
    }

    private void InitSparkObjects()
    {
        int count = transform.childCount;

        sparks = new GameObject[count];

        for (int i = 0; i < count; i++)
        {
            Transform child = transform.GetChild(i);
            sparks[i] = child.gameObject;
        }
    }

    protected override IEnumerator PatternCoroutine()
    {
        yield return base.PatternCoroutine();

        for (int i = 0; i < sparks.Length; i++)
        {
            yield return waitForStartLag;
            sparks[i].SetActive(true);

            yield return waitForActiveLag;
            sparks[i].SetActive(false);
        }

        yield return waitForEndLag;
        owner.ChangeState(typeof(BossIdleState));
    }
}
