using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;

public class AfterimageItem : MonoBehaviour
{
    [Title("Components")]
    [SerializeField] private new SpriteRenderer renderer;

    [Title("Settings")]
    [SerializeField] private AfterimageItemSettings settings;

    private Afterimage owner;

    private void Awake()
    {
        owner = GetComponentInParent<Afterimage>();
        ResetColor();
    }

    private void OnDisable()
    {
        ResetColor();
    }

    public void Execute(Vector2 pos, Sprite sprite, bool isFlip)
    {
        gameObject.SetActive(true);

        transform.parent = null;
        transform.position = pos;
        transform.localScale = Vector3.one;

        renderer.sprite = sprite;
        renderer.flipX = isFlip;

        StartCoroutine(FadeAsync());
    }

    private IEnumerator FadeAsync()
    {
        float time = 0f;
        float duration = settings.FadeDuration;

        float startAlpha = renderer.color.a;

        while (true)
        {
            if (time > duration) break;

            time += Time.deltaTime;
            float progress = time / duration;

            Color color = renderer.color;
            color.a = Mathf.Lerp(startAlpha, 0f, progress);
            renderer.color = color;

            yield return null;
        }

        owner.ReturnItem(this);
    }

    private void ResetColor()
    {
        Color rgb = renderer.color;
        renderer.color = new Color(rgb.r, rgb.g, rgb.b, settings.StartAlpha);
    }
}
