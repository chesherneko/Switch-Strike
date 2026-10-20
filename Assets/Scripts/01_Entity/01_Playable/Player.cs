using UnityEngine;
using Sirenix.OdinInspector;

public struct PlayerMoveSyncData
{
    public float MoveX;
    public float LastMoveDirection;
}

public struct PlayerCombatSyncData
{
    public int CurrentHP;
}

public class Player : MonoBehaviour
{
    #region FIELDS
    public static Player Local { get; private set; }

    [field: Title("Components")]
    [field: SerializeField] public Transform RootTrans { get; private set; }
    [field: SerializeField] public PlayerController Controller { get; private set; }
    [field: SerializeField] public PlayerMove Movement { get; private set; }
    [field: SerializeField] public PlayerCombat Combat { get; private set; }
    [field: SerializeField] public EmotionCom Emotion { get; private set; }

    [Title("Runtime States")]
    [ShowInInspector] public bool IsLockControl { get; private set; } = false;
    [ShowInInspector] public bool IsLockControlHard { get; private set; } = false;
    #endregion

    #region UNITY METHODS
    private void Awake()
    {
        if (Local == null)
            Local = this;
    }
    #endregion

    #region STATIC METHODS
    public static void SwitchLocalPlayer(Player curr, Player next)
    {
        if (Local != null)
        {
            SyncMovementState(curr, next);
            SyncCombatState(curr, next);
        }

        Local = next;
    }

    private static void SyncMovementState(Player curr, Player next)
    {
        PlayerMoveSyncData syncData = new()
        {
            MoveX = curr.Movement.MoveX,
            LastMoveDirection = curr.Movement.LastMoveDirection
        };

        next.Movement.SyncPrevPlayerState(syncData);
    }

    private static void SyncCombatState(Player curr, Player next)
    {
        PlayerCombatSyncData syncData = new()
        {
            CurrentHP = curr.Combat.CurrentHP
        };

        next.Combat.SyncPrevPlayerState(syncData);
    }
    #endregion

    #region PUBLIC METHODS
    public void LockControl() => IsLockControl = true;
    public void UnlockControl() => IsLockControl = false;

    public void LockControlHard()
    {
        LockControl();
        IsLockControlHard = true;
    }
    public void UnlockControlHard()
    {
        UnlockControl();
        IsLockControlHard = false;
    }

    public void EnableInvincibility() => Combat.SetHurtBoxActive(false);
    public void DisableInvincibility() => Combat.SetHurtBoxActive(true);

    public bool IsBusyForSwitch() => Controller.IsBusyForSwitch();
    public bool IsCurrentAnimationFinished() => Controller.IsCurrentAnimationFinished();
    public void UseQTE() => Controller.UseQTE();

    public void ResetMovementState()
    {
        Movement.ResetState(true);
        Controller.Animator.SetFloat(AnimationParam.MOVEMENT, 0f);
    }
    #endregion
}
