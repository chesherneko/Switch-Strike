using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Afterimage : MonoBehaviour
{
    [Title("Components")]
    [SerializeField] private Transform mainTransform;
    [SerializeField] private new SpriteRenderer renderer;

    [Title("Settings")]
    [SerializeField] private int spawnCount = 10;

    [Title("References")]
    [SerializeField] private AfterimageItem prefab;

    private readonly Queue<AfterimageItem> itemQueue = new();

    private Coroutine coroutine;

    private void Awake()
    {
        AfterimageItem[] items = GetComponentsInChildren<AfterimageItem>(true);

        for (int i = 0; i < items.Length; i++)
            itemQueue.Enqueue(items[i]);
    }

    public void Execute(float distance)
        => coroutine = StartCoroutine(ExecuteCoroutine(distance));

    public void Stop()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
    }

    public void ReturnItem(AfterimageItem item)
    {
        item.transform.SetParent(transform);
        item.gameObject.SetActive(false);

        itemQueue.Enqueue(item);
    }

    private IEnumerator ExecuteCoroutine(float distance)
    {
        float startPos = mainTransform.position.x;

        float spawnInterval = distance / spawnCount;
        float prevSpawnPos = startPos;

        while (true)
        {
            float currentPos = mainTransform.position.x;

            if (Mathf.Abs(currentPos - startPos) >= distance) break;

            float moveDist = Mathf.Abs(currentPos - prevSpawnPos);

            while (true)
            {
                if (moveDist < spawnInterval) break;

                float direction = Mathf.Sign(currentPos - prevSpawnPos);

                prevSpawnPos += direction * spawnInterval;

                AfterimageItem item = GetItem();

                bool isFlip = mainTransform.localScale.x < 0f;

                Vector3 spawnPos = transform.position;
                spawnPos.x = prevSpawnPos;

                item.Execute(spawnPos, renderer.sprite, isFlip);

                moveDist = Mathf.Abs(currentPos - prevSpawnPos);
            }

            yield return null;
        }

        coroutine = null;
    }

    private AfterimageItem GetItem()
    {
        if (itemQueue.Count == 0)
        {
            AfterimageItem item = Instantiate(prefab, transform);
            return item;
        }
        return itemQueue.Dequeue();
    }
}
