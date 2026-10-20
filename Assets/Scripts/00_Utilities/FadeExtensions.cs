using DG.Tweening;
using System.Collections;
using UnityEngine;

public static class FadeExtensions
{
    public static Tween FadeIn(this SpriteRenderer renderer, float duration)
    {
        renderer.gameObject.SetActive(true);
        return renderer.DOFade(1f, duration);
    }

    public static Tween FadeOut(this SpriteRenderer renderer, float duration)
    {
        return renderer
            .DOFade(0f, duration)
            .OnComplete(() => renderer.gameObject.SetActive(false));
    }
}
