using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class BossEye : MonoBehaviour
{
    [Title("Components")]
    [SerializeField] private new SpriteRenderer renderer;
    [SerializeField] private BossPupil pupil;
    [SerializeField] private BossBeam beam;

    [Title("Settings")]
    [SerializeField] private float fadeInDuration = 1f;
    [SerializeField] private float fadeOutDuration = 2f;

    [Space]
    [SerializeField] private float startLag = 1.5f;
    [SerializeField] private float endLag = 1f;

    public void Spawn(Vector2 spawnPos)
    {
        transform.localPosition = spawnPos;
        gameObject.SetActive(true);
    }

    public void ExecuteLogic()
    {
        StartCoroutine(ExecuteLogicCoroutine());
    }

    private IEnumerator ExecuteLogicCoroutine()
    {
        Transform target = Player.Local.transform;
        pupil.RequestFollowTarget(target);

        yield return FadeCoroutine(true, fadeInDuration);
        yield return new WaitForSeconds(startLag);

        pupil.StopFollowTarget();
        yield return new WaitForSeconds(0.25f);

        Vector2 pupilDir = pupil.LookDirection;
        Vector2 pupilPos = pupil.transform.position;
        yield return beam.Shoot(pupilDir, pupilPos);

        yield return FadeCoroutine(false, fadeOutDuration);
        yield return new WaitForSeconds(endLag);

        gameObject.SetActive(false);
    }

    private IEnumerator FadeCoroutine(bool isFadeIn, float duration)
    {
        Sequence sequence = DOTween.Sequence();

        if (isFadeIn)
        {
            sequence.Join(renderer.FadeIn(duration));
            sequence.Join(pupil.Renderer.FadeIn(duration));
        }
        else
        {
            sequence.Join(renderer.FadeOut(duration));
            sequence.Join(pupil.Renderer.FadeOut(duration));
        }

        yield return sequence.WaitForCompletion();
    }
}
