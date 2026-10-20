using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class BossNormalPattern_2 : BossPatternStateBase
{
    [Title("Settings")]
    [SerializeField] private int spawnCount = 10;
    [SerializeField] private float spawnRange = 25f;
    [SerializeField] private float spawnInterval = 1f;

    [Title("References")]
    [SerializeField] private Animator prefab;

    [Title("Runtime States")]
    [ShowInInspector, ReadOnly] private Animator[] attackObjects;
    [ShowInInspector, ReadOnly] private Vector2[] spawnPoints;

    [SerializeField, TitleGroup("Lag")] private float patternDelay;

    private WaitForSeconds waitForPatternDelay;

    protected override void Awake()
    {
        base.Awake();

        InitAttackObjects();
        InitSpawnPoints();
    }

    protected override void OnEnable()
    {
        spawnPoints.Shuffle();
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            GameObject obj = attackObjects[i].gameObject;
            obj.SetActive(false);
        }
    }

    private void InitAttackObjects()
    {
        attackObjects = new Animator[spawnCount];

        Animator[] objs = GetComponentsInChildren<Animator>(true);

        for (int i = 0; i < objs.Length; i++)
            attackObjects[i] = objs[i];

        for (int i = objs.Length; i < spawnCount; i++)
        {
            Animator atkObj = Instantiate(prefab, transform);
            atkObj.gameObject.SetActive(false);

            attackObjects[i] = atkObj;
        }
    }

    private void InitSpawnPoints()
    {
        int count = (int)(spawnRange / spawnInterval) + 1;

        spawnPoints = new Vector2[count];

        for (int i = 0; i < count; i++)
        {
            Vector2 spawnPoint = new(spawnInterval * i, 0f);
            spawnPoints[i] = spawnPoint;
        }
    }

    protected override void InitYieldInstructions()
    {
        base.InitYieldInstructions();
        waitForPatternDelay = new(patternDelay);
    }

    protected override IEnumerator PatternCoroutine()
    {
        yield return base.PatternCoroutine();

        for (int i = 0; i < spawnCount; i++)
        {
            Animator obj = attackObjects[i];

            Vector2 spawnPoint = spawnPoints[i];
            obj.transform.localPosition = spawnPoint;

            obj.gameObject.SetActive(true);
        }

        yield return waitForPatternDelay;

        for (int i = 0; i < spawnCount; i++)
        {
            Animator anim = attackObjects[i];
            anim.SetTrigger(AnimationParam.HIT);
        }

        yield return waitForEndLag;

        owner.ChangeState(typeof(BossIdleState));
    }
}
