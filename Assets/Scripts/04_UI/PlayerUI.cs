using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(10)]
public class PlayerUI : MonoSingleton<PlayerUI>
{
    [Title("Components")]
    [SerializeField] private Image remainHP;
    [SerializeField] private Image skillCooldown;
    [SerializeField] private Image switchCooldown;
    [SerializeField] private Image switchGlow;

    [Space]
    [SerializeField] private Image mainPortrait;
    [SerializeField] private Image subPortrait;

    [Title("Settings")]
    [SerializeField] private Transform mainSwitchY;
    [SerializeField] private Transform subSwitchY;
    [SerializeField] private float switchDuration = 0.25f;

    private readonly float skillCooldownUnit = 1 / PlayerController.SkillCooldown;
    private float switchCooldownUnit;

    protected override void Awake()
    {
        base.Awake();
        switchCooldownUnit = 1 / PlayerSwitchManager.Instance.SwitchCooldown;
    }

    public void UpdateRemainHP(float hpRatio)
    {
        remainHP.fillAmount = hpRatio;
    }

    public void UpdateSkillCooldown(float cooldown)
    {
        skillCooldown.fillAmount = Mathf.Clamp01(cooldown * skillCooldownUnit);
    }


    public void SwitchPortrait()
    {
        StartCoroutine(SwitchPortraitCoroutine());
    }

    public void SetSwitchGlow(bool isActive)
        => switchGlow.enabled = isActive;

    public IEnumerator SwitchPortraitCoroutine()
    {
        Transform mainPortTrans = mainPortrait.transform;
        Transform subPortTrans = subPortrait.transform;

        Sprite mainSprite = mainPortrait.sprite;
        Sprite subSprite = subPortrait.sprite;

        float mainY = mainPortTrans.position.y;
        float subY = subPortTrans.position.y;

        Tween mainDownTween = mainPortTrans
            .DOMoveY(mainSwitchY.position.y, switchDuration)
            .OnComplete(() => mainPortrait.sprite = subSprite);

        Tween subUpTween = subPortTrans
            .DOMoveY(subSwitchY.position.y, switchDuration)
            .OnComplete(() => subPortrait.sprite = mainSprite);

        Sequence seq_1 = DOTween.Sequence()
            .Join(mainDownTween)
            .Join(subUpTween)
            .SetEase(Ease.InOutQuad);

        yield return seq_1.WaitForCompletion();

        Tween mainUpTween = mainPortTrans.DOMoveY(mainY, switchDuration);
        Tween subDownTween = subPortTrans.DOMoveY(subY, switchDuration);
        Sequence seq_2 = DOTween.Sequence()
            .Join(mainUpTween)
            .Join(subDownTween)
            .SetEase(Ease.InOutQuad);

        yield return seq_2.WaitForCompletion();
    }

    public void UpdateSwitchCooldown(float cooldown)
    {
        float fillAmount = 1f - Mathf.Clamp01(cooldown * switchCooldownUnit);
        switchCooldown.fillAmount = fillAmount;
    }
}
