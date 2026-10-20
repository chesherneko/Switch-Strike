using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class BossMinionSpawner : MonoBehaviour
{
    private static readonly int OUTLINE_ALPHA = Shader.PropertyToID("_OutlineAlpha");

    [Title("Components")]
    [SerializeField] private new SpriteRenderer renderer;
    [SerializeField] private GameObject hurtBox;
    [SerializeField] private EnemyCombat myCombat;

    [Title("Settings")]
    [SerializeField] private int spawnCount;
    [SerializeField] private float spawnHalfRange;
    [SerializeField] private float spawnOffsetY;

    [Space]
    [SerializeField] private float fadeInDuration;

    [Title("References")]
    [SerializeField] private BossMinion[] prefabs;

    private readonly List<BossMinion> remainMinions = new();

    public bool IsCleared => myCombat.IsDead;
    public bool IsFinished => gameObject.activeSelf == false;

    private MaterialPropertyBlock propertyBlock;

    private void Awake()
    {
        propertyBlock = new();
    }

    private void OnDisable()
    {
        remainMinions.Clear();

        hurtBox.SetActive(false);
        SetOutlineAlpha(isActive: false);
    }

    public IEnumerator StartPattern()
    {
        gameObject.SetActive(true);

        yield return renderer
            .FadeIn(fadeInDuration)
            .WaitForCompletion();

        for (int i = 0; i < spawnCount; i++)
            SpawnRandomMinion();
    }

    public IEnumerator FailPattern()
    {
        for (int i = remainMinions.Count - 1; i >= 0; i--)
            remainMinions[i].Kill();

        yield return new WaitUntil(() => remainMinions.Count == 0);

        myCombat.TakeDamage(myCombat.CurrentHP);
    }

    public void NotifyMinionDestroyed(BossMinion minion)
    {
        if (remainMinions.Contains(minion) == false)
        {
            Debug.LogWarning($"해당 Minion({minion.name}은 " +
                $"정상적으로 생성된 객체가 아닙니다.");
            return;
        }

        remainMinions.Remove(minion);

        if (remainMinions.Count == 0)
        {
            hurtBox.SetActive(true);
            SetOutlineAlpha(isActive: true);
        }
    }

    private void SpawnRandomMinion()
    {
        int randIdx = Random.Range(0, prefabs.Length);
        BossMinion prefab = prefabs[randIdx];

        BossMinion minion = Instantiate(prefab, transform);
        Transform minionTrans = minion.transform;

        float randX = Random.Range(-spawnHalfRange, spawnHalfRange);
        minionTrans.localPosition = new Vector2(randX, spawnOffsetY);

        float lookDir = randX < 0f ? 1f : -1f;
        Vector3 tempScale = minionTrans.localScale;
        tempScale.x = lookDir * Mathf.Abs(tempScale.x);
        minionTrans.localScale = tempScale;

        remainMinions.Add(minion);

        minion.gameObject.SetActive(true);
    }

    private void SetOutlineAlpha(bool isActive)
    {
        renderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetFloat(OUTLINE_ALPHA, isActive ? 1f : 0f);
        renderer.SetPropertyBlock(propertyBlock);
    }
}
