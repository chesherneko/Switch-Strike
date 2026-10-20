using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

[DefaultExecutionOrder(100)]
public class BossroomCutscene : MonoBehaviour
{
    [Title("Components")]
    [SerializeField] private new BoxCollider2D collider;
    [SerializeField] private new CinemachineCamera camera;
    [SerializeField] private CinemachinePixelPerfect cameraPixelPerfect;
    [SerializeField] private CinemachineConfiner2D cameraConfiner;
    [SerializeField] private CinemachineFollow cameraFollow;
    [SerializeField] private CinemachineBasicMultiChannelPerlin cameraNoise;

    [Title("Settings")]
    [SerializeField] private LayerMask collisionMask;

    [Space]
    [SerializeField] private GameObject entranceBlocker;
    [SerializeField] private float entranceShakeDuration = 0.5f;
    [SerializeField] private float entranceShakeAmplitude = 2f;
    [SerializeField] private float entranceShakeFrequency = 8f;

    [Space]
    [SerializeField] private Transform centerPoint;
    [SerializeField] private float moveToCenterDuration = 2f;

    [Space]
    [SerializeField] private GameObject bossGameObject;
    [SerializeField] private float bossAppearShakeDuration = 0.5f;
    [SerializeField] private float bossAppearShakeAmplitude = 2f;
    [SerializeField] private float bossAppearShakeFrequency = 8f;

    [Space]
    [SerializeField] private BoxCollider2D newBoundary;
    [SerializeField] private float bossCameraSize = 11.25f;
    [SerializeField] private float bossCameraOffsetY = 8f;
    [SerializeField] private float bossCameraAdjustDuration = 2f;
    [SerializeField] private float bossAppearTargetY = 5.75f;
    [SerializeField] private float bossAppearScale = 2f;

    private IEnumerator PlayCoroutine()
    {
        Player player = Player.Local;

        player.LockControlHard();
        player.ResetMovementState();

        yield return BlockEntrance(player);
        yield return MovePlayerToCenter(player);
        yield return LookAround(player);
        yield return BossAppear(player);
        yield return AdjustCameraForBoss();
        yield return BossSpawn();

        player.UnlockControlHard();
    }

    private IEnumerator BlockEntrance(Player player)
    {
        entranceBlocker.SetActive(true);

        Transform trans = player.transform;

        float dir = Mathf.Sign(entranceBlocker.transform.position.x - trans.position.x);
        trans.localScale = new Vector3(dir, 1f, 1f);

        player.Emotion.Play(EmotionType.Sigh);

        float amplitude = entranceShakeAmplitude;
        float frequency = entranceShakeFrequency;
        float duration = entranceShakeDuration;
        yield return ShakeCamera(amplitude, frequency, duration);
        yield return new WaitForSeconds(2f);

        yield return player.Emotion.PlayCoroutine(EmotionType.Whisper);
        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator MovePlayerToCenter(Player player)
    {
        Transform trans = player.transform;
        Animator animator = player.Controller.Animator;

        Vector3 targetPos = trans.position;
        targetPos.x = centerPoint.position.x;

        float dir = Mathf.Sign(targetPos.x - trans.position.x);

        trans.localScale = new Vector3(dir, 1f, 1f);
        animator.SetFloat(AnimationParam.MOVEMENT, 1f);

        yield return trans
            .DOMoveX(targetPos.x, moveToCenterDuration)
            .SetEase(Ease.Linear)
            .WaitForCompletion();

        animator.SetFloat(AnimationParam.MOVEMENT, 0f);
    }

    private IEnumerator LookAround(Player player)
    {
        yield return new WaitForSeconds(1f);

        yield return player.Emotion.PlayCoroutine(EmotionType.Whisper);
        yield return new WaitForSeconds(0.5f);

        Transform trans = player.transform;

        float startDir = trans.localScale.x;
        float dir = -startDir;

        for (int i = 0; i < 4; i++)
        {
            trans.localScale = new Vector3(dir, 1f, 1f);
            yield return new WaitForSeconds(0.5f);
            dir *= -1f;
        }

        yield return player.Emotion.PlayCoroutine(EmotionType.Question);
        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator BossAppear(Player player)
    {
        bossGameObject.SetActive(true);
        player.Emotion.Play(EmotionType.Sigh);

        float amplitude = bossAppearShakeAmplitude;
        float frequency = bossAppearShakeFrequency;
        float duration = bossAppearShakeDuration;
        yield return ShakeCamera(amplitude, frequency, duration);
        yield return new WaitForSeconds(1.5f);
    }

    private IEnumerator AdjustCameraForBoss()
    {
        Transform bossTrans = bossGameObject.transform;

        cameraPixelPerfect.enabled = false;

        cameraConfiner.BoundingShape2D = newBoundary;
        cameraConfiner.InvalidateLensCache();
        cameraConfiner.InvalidateBoundingShapeCache();

        Tween offsetTween = DOTween.To(
            () => cameraFollow.FollowOffset.y,
            value =>
            {
                Vector3 offset = cameraFollow.FollowOffset;
                offset.y = value;
                cameraFollow.FollowOffset = offset;
            },
            bossCameraOffsetY,
            bossCameraAdjustDuration
        );

        Tween bossPositionTween = bossTrans
            .DOMoveY(bossAppearTargetY, bossCameraAdjustDuration);

        Sequence seq_1 = DOTween.Sequence()
            .Join(bossPositionTween)
            .Join(offsetTween)
            .SetEase(Ease.InOutQuad);

        yield return seq_1.WaitForCompletion();
        yield return new WaitForSeconds(0.5f);

        Tween sizeTween = DOTween.To(
            () => camera.Lens.OrthographicSize,
            value =>
            {
                LensSettings lens = camera.Lens;
                lens.OrthographicSize = value;
                camera.Lens = lens;
            },
            bossCameraSize,
            bossCameraAdjustDuration
        );

        Tween bossScaleTween = bossGameObject.transform
            .DOScale(bossAppearScale, bossCameraAdjustDuration);

        Sequence seq_2 = DOTween.Sequence()
            .Join(sizeTween)
            .Join(bossScaleTween)
            .SetEase(Ease.InOutQuad);

        yield return seq_2.WaitForCompletion();
        yield return new WaitForSeconds(1f);

        cameraPixelPerfect.enabled = true;
    }

    private IEnumerator BossSpawn()
    {
        Animator animator = bossGameObject.GetComponent<Animator>();
        animator.enabled = true;

        animator.SetTrigger("Start");

        yield return new WaitUntil(() => animator.IsCurrentAnimationFinished());
        yield return new WaitForSeconds(2f);

        Transform bossTrans = bossGameObject.transform;

        GameObject hurtBox = bossTrans.Find("HurtBox").gameObject;
        GameObject specialHurtBox = bossTrans.Find("HurtBox_Special").gameObject;

        hurtBox.SetActive(true);
        specialHurtBox.SetActive(true);

        BossController controller = bossGameObject.GetComponent<BossController>();
        controller.enabled = true;

        BossUI.Instance.SetActiveUI(true);
    }

    private IEnumerator ShakeCamera(float amplitude, float frequency, float duration)
    {
        cameraNoise.AmplitudeGain = amplitude;
        cameraNoise.FrequencyGain = frequency;

        yield return new WaitForSeconds(duration);

        cameraNoise.AmplitudeGain = 0f;
        cameraNoise.FrequencyGain = 0F;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collisionMask.Contains(collision) == false) return;

        collider.enabled = false;
        StartCoroutine(PlayCoroutine());
    }
}
