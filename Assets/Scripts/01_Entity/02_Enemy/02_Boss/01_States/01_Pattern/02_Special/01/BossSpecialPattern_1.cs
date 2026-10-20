using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

using Random = UnityEngine.Random;

public class BossSpecialPattern_1 : BossPatternStateBase
{
    [Title("Settings")]
    [SerializeField] private Vector2 minSpawnPoint;
    [SerializeField] private Vector2 maxSpawnPoint;
    [SerializeField] private int spawnCount = 10;
    [SerializeField] private float spawnInterval = 0.5f;

    [Title("References")]
    [SerializeField] private BossEye prefab;

    [Title("Runtime States")]
    [ShowInInspector, ReadOnly] private BossEye[] eyes;

    protected override int PatternStartTrigger => Animator.StringToHash("SpecialAttack1");

    protected override void Awake()
    {
        base.Awake();
        InitLasers();
    }

    protected override void OnDisable() { }

    private void InitLasers()
    {
        eyes = new BossEye[spawnCount];

        BossEye[] preLoadeds = GetComponentsInChildren<BossEye>(true);

        for (int i = 0; i < preLoadeds.Length; i++)
            eyes[i] = preLoadeds[i];

        for (int i = preLoadeds.Length; i < spawnCount; i++)
        {
            BossEye eye = Instantiate(prefab, transform);
            eyes[i] = eye;

            eye.gameObject.SetActive(false);
        }
    }

    protected override IEnumerator PatternCoroutine()
    {
        yield return base.PatternCoroutine();

        //차례대로 눈을 하나씩 랜덤한 위치에 생성
        for (int i = 0; i < spawnCount; i++)
        {
            BossEye eye = eyes[i];

            float randX = Random.Range(minSpawnPoint.x, maxSpawnPoint.x);
            float randY = Random.Range(minSpawnPoint.y, maxSpawnPoint.y);
            Vector2 spawnPos = new(randX, randY);

            eye.Spawn(spawnPos);
            eye.ExecuteLogic();

            yield return new WaitForSeconds(spawnInterval);
        }

        //모든 눈이 비활성화 상태일 때 까지 대기
        while (true)
        {
            if (HasActiveEye() == false) break;
            yield return null;
        }

        owner.ChangeState(typeof(BossIdleState));
    }

    private bool HasActiveEye()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            BossEye eye = eyes[i];
            if (eye.isActiveAndEnabled) return true;
        }

        return false;
    }
}
