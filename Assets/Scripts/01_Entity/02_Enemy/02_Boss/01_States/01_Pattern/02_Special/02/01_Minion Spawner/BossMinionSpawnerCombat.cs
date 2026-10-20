using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class BossMinionSpawnerCombat : EnemyCombat
{
    [SerializeField, TitleGroup("Components")] private new SpriteRenderer renderer;
    [SerializeField, TitleGroup("Settings")] private float fadeOutDuration = 1.5f;

    private void OnEnable()
    {
        ResetHP();
    }

    protected override void OnDead()
    {
        StartCoroutine(OnDeadCoroutine());
    }

    private IEnumerator OnDeadCoroutine()
    {
        yield return renderer
            .FadeOut(fadeOutDuration)
            .WaitForCompletion();

        gameObject.SetActive(false);
    }
}
