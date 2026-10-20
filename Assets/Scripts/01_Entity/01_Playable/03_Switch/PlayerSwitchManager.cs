using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using DG.Tweening;

public class PlayerSwitchManager : MonoSingleton<PlayerSwitchManager>
{
    #region FIELDS
    [Title("Components")]
    [SerializeField] private new CinemachineCamera camera;

    [Title("References")]
    [SerializeField] private Player currentPlayer;
    [SerializeField] private Player[] sparePlayers;

    [Title("Settings")]
    [SerializeField] private float switchJumpHeight = 3f;
    [SerializeField] private float switchJumpDuration = 0.5f;
    [SerializeField] private float screenOutDistance = 10f;

    [field: Space]
    [field: SerializeField] public float SwitchCooldown { get; private set; } = 10f;

    [Space]
    [SerializeField] private float qteGaugeMax = 100f;
    [SerializeField] private float qteGaugeGainUnit = 5f;

    [Title("Runtime States")]
    [ShowInInspector, ReadOnly] public bool IsSwitching { get; private set; } = false;

    [Space]
    [ShowInInspector, ReadOnly] private bool isSwitchOnCooldown = false;
    [ShowInInspector, ReadOnly] private float switchCooldownTime = 0f;

    [Space]
    [ShowInInspector, ReadOnly] private float qteGauge = 0f;
    #endregion

    #region UNITY METHODS
    private void Update()
    {
        if (Key.F5.WasPressed())
            qteGauge = qteGaugeMax;

        if (IsSwitching) return;

        if (isSwitchOnCooldown)
        {
            UpdateSwitchCooldownTime();
            return;
        }

        if (Player.Local.IsLockControl) return;

        if (Key.Digit1.WasPressed())
            StartCoroutine(SwitchCoroutine(0));
    }
    #endregion

    #region PRIVATE METHODS
    private void UpdateSwitchCooldownTime()
    {
        switchCooldownTime += Time.deltaTime;

        if (qteGauge >= qteGaugeMax ||
            switchCooldownTime > SwitchCooldown)
        {
            isSwitchOnCooldown = false;
            switchCooldownTime = SwitchCooldown;
        }

        PlayerUI.Instance.UpdateSwitchCooldown(switchCooldownTime);
    }

    private IEnumerator SwitchCoroutine(int nextIdx)
    {
        bool canUseQTE = qteGauge >= qteGaugeMax;
        qteGauge = 0f;

        IsSwitching = true;
        isSwitchOnCooldown = true;
        switchCooldownTime = 0f;
        PlayerUI.Instance.SwitchPortrait();
        PlayerUI.Instance.SetSwitchGlow(false);

        Player curr = currentPlayer;
        Player next = sparePlayers[nextIdx];

        curr.LockControl();
        curr.EnableInvincibility();

        Transform currTrans = curr.gameObject.transform;

        Vector2 switchPoint = currTrans.position;
        float dir = currTrans.localScale.x > 0f ? 1f : -1f;

        Vector2 goalPoint = switchPoint + dir * screenOutDistance * Vector2.left;

        camera.Follow = null;

        next.transform.localScale = currTrans.localScale;
        next.gameObject.SetActive(true);
        next.LockControl();
        next.EnableInvincibility();

        bool switchOutDone = false;
        bool switchInDone = false;

        StartCoroutine(SwitchOutCoroutine(curr, switchPoint, goalPoint, () => switchOutDone = true));
        StartCoroutine(SwitchInCoroutine(curr, next, goalPoint, switchPoint, () => switchInDone = true, canUseQTE));

        yield return new WaitUntil(() => switchOutDone && switchInDone);

        currentPlayer = next;
        sparePlayers[nextIdx] = curr;

        IsSwitching = false;
    }

    private IEnumerator SwitchOutCoroutine(Player curr,
        Vector2 start, Vector2 end, Action onDone)
    {
        curr.Movement.ResetState();

        yield return WaitUntilSwitchOutReady(curr);

        curr.transform.position = start;

        yield return curr.transform.DOJump(
            end, 
            switchJumpHeight, 
            1, 
            switchJumpDuration
        ).WaitForCompletion();

        curr.gameObject.SetActive(false);

        onDone?.Invoke();
    }

    private IEnumerator SwitchInCoroutine(Player curr, Player next,
        Vector2 start, Vector2 end, Action onDone, bool canUseQTE)
    {
        next.Movement.ResetState();

        next.transform.position = end;
        next.RootTrans.position = start;

        Player.SwitchLocalPlayer(curr, next);

        yield return next.RootTrans.DOLocalJump(
            Vector2.zero, 
            switchJumpHeight, 
            1, 
            switchJumpDuration
        ).WaitForCompletion();

        onDone?.Invoke();

        camera.Follow = next.gameObject.transform;

        next.UnlockControl();
        next.DisableInvincibility();

        if (canUseQTE) next.UseQTE();
    }

    private IEnumerator WaitUntilSwitchOutReady(Player player)
    {
        player.LockControl();

        while (true)
        {
            if (player.IsBusyForSwitch() == false) break;
            if (player.IsCurrentAnimationFinished()) break;

            yield return null;
        }
    }
    #endregion

    #region PUBLIC METHODS
    public void GainQTEGauge(float mult)
    {
        qteGauge += qteGaugeGainUnit * mult;
        qteGauge = Mathf.Clamp(qteGauge, 0f, qteGaugeMax);

        if (qteGauge >= qteGaugeMax)
            PlayerUI.Instance.SetSwitchGlow(true);
    }
    #endregion
}
