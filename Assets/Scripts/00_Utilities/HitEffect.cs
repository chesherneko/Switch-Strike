using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class HitEffect : MonoBehaviour
{
    private static readonly int HIT_EFFECT_BLEND = Shader.PropertyToID("_HitEffectBlend");

    private static YieldInstruction WAIT_FOR_SECONDS;

    [Title("Components")]
    [SerializeField] private new SpriteRenderer renderer;
    [SerializeField] private Animator animator;

    [Title("Settings")]
    [SerializeField] private float flickInterval = 0.083f;
    [SerializeField] private int repeat = 2;
    [SerializeField] private bool hasHitStun = true;

    private MaterialPropertyBlock propertyBlock;

    private void Awake()
    {
        WAIT_FOR_SECONDS ??= new WaitForSeconds(flickInterval);
        propertyBlock = new();
    }

    public void Play() => StartCoroutine(PlayCoroutine());

    private IEnumerator PlayCoroutine()
    {
        if (hasHitStun) animator.speed = 0.5f;

        for (int i = 0; i < repeat; i++)
        {
            yield return WAIT_FOR_SECONDS;
            SetHitEffectBlend(true);
            yield return WAIT_FOR_SECONDS;
            SetHitEffectBlend(false);
        }

        animator.speed = 1f;
    }

    private void SetHitEffectBlend(bool isOn)
    {
        renderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetFloat(HIT_EFFECT_BLEND, isOn ? 1f : 0f);
        renderer.SetPropertyBlock(propertyBlock);
    }
}
