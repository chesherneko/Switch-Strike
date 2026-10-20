using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

public enum AirborneState
{
    None,
    Jumping,
    Falling
}

public abstract class PlayerController : AnimationStateController
{
    #region FIELDS
    [TitleGroup("Components")]
    [SerializeField] private Player owner;

    [Title("Settings")]
    [SerializeField] private float fallTransitionThreshold = -1f;

    private AirborneState airborneState = AirborneState.None;

    private bool wasClingToWall = false;

    private static bool isSkillCooldown = false;
    public static float SkillCooldown { get; } = 5f;
    public static float SkillCooldownTime { get; private set; } = SkillCooldown;

    public bool IsLockMoveDirection { get; set; } = false;

    public PlayerMove Movement => owner.Movement;
    public PlayerCombat Combat => owner.Combat;
    #endregion

    #region UNITY METHOD
    protected override void Update()
    {
        base.Update();

        UpdateGroundedState();
        UpdateWallClingState();
        UpdateSkillCooldownTime();
    }
    #endregion

    #region ABSTRACT METHOD
    protected abstract PlayerAnimationStateBase CreateSkillState();
    protected abstract PlayerAnimationStateBase CreateSkillEndState();

    protected override void InitStateMap()
    {
        stateMap = new()
        {
            { Animator.StringToHash(AnimationTag.ANIM_TAG_MOVEMENT), new PlayerMovementState(this) },
            { Animator.StringToHash(AnimationTag.ANIM_TAG_JUMP), new PlayerJumpState(this) },
            { Animator.StringToHash(AnimationTag.ANIM_TAG_DASH), new PlayerDashState(this) },
            { Animator.StringToHash(AnimationTag.ANIM_TAG_WALL_CLING), new PlayerWallClingState(this) },
            { Animator.StringToHash(AnimationTag.ANIM_TAG_ATTACK), new PlayerAttackState(this) },
            { Animator.StringToHash(AnimationTag.ANIM_TAG_ATTACK_END), new PlayerAttackEndState(this) },
            { Animator.StringToHash(AnimationTag.ANIM_TAG_SKILL), CreateSkillState() },
            { Animator.StringToHash(AnimationTag.ANIM_TAG_SKILL_END), CreateSkillEndState() },
            { Animator.StringToHash(AnimationTag.ANIM_TAG_QTE), new PlayerQTEState(this) },
            { Animator.StringToHash(AnimationTag.ANIM_TAG_DEAD), new PlayerDeadState(this) }
        };
    }

    protected override void InitDefaultState()
    {
        int key = Animator.StringToHash(AnimationTag.ANIM_TAG_MOVEMENT);
        defaultState = stateMap[key];
    }
    #endregion

    #region STATE UPDATE METHOD
    private void UpdateGroundedState()
    {
        //땅인지 여부는 계속해서 갱신
        bool isGrounded = Movement.IsGrounded;
        Animator.SetBool(AnimationParam.IS_GROUNDED, isGrounded);

        if (isGrounded)
        {
            airborneState = AirborneState.None;
            return;
        }

        if (Movement.IsClingToWall) return;
        if (Animator.GetBool(AnimationParam.CAN_JUMP_OR_FALL) == false) return;

        //airborneState에 따라 Jump 및 Fall 트리거 실행
        AirborneState nextState =
            Movement.MoveY > fallTransitionThreshold ?
            AirborneState.Jumping : AirborneState.Falling;

        if (airborneState == nextState) return;

        airborneState = nextState;

        int triggerHash = nextState switch
        {
            AirborneState.Jumping => AnimationParam.JUMP,
            AirborneState.Falling => AnimationParam.FALL,
            _ => throw new InvalidOperationException()
        };

        Animator.SetTrigger(triggerHash);
    }

    private void UpdateWallClingState()
    {
        bool isClingToWall = Movement.IsClingToWall;

        if (isClingToWall == wasClingToWall) return;

        Animator.SetBool(AnimationParam.IS_WALL_CLING, isClingToWall);

        float lastDir = Movement.LastMoveDirection;
        transform.localScale = new Vector2(lastDir * -1f, 1f);

        if (isClingToWall) airborneState = AirborneState.None;
        else Movement.SwitchMoveDirection();

        wasClingToWall = isClingToWall;
    }

    private void UpdateSkillCooldownTime()
    {
        if (isSkillCooldown == false) return;
        if (PlayerSwitchManager.Instance.IsSwitching) return;

        SkillCooldownTime += Time.deltaTime;

        if (SkillCooldownTime > SkillCooldown)
        {
            SkillCooldownTime = SkillCooldown;
            isSkillCooldown = false;
        }

        PlayerUI.Instance.UpdateSkillCooldown(SkillCooldownTime);
    }
    #endregion

    #region INPUT HANDLE METHOD
    public void HandleMoveInput()
    {
        if (owner.IsLockControl) return;

        Vector2 input = InputHelper.GetMoveAxisRaw();

        if (IsLockMoveDirection == false)
        {
            float lastDir = Movement.LastMoveDirection;
            float currentDir = transform.localScale.x;

            bool shouldChangeDir =
                lastDir != currentDir &&
                wasClingToWall == false;

            if (shouldChangeDir)
                transform.localScale = new Vector2(lastDir, 1f);
        }
        
        float inputXAbs = Mathf.Abs(input.x);
        Animator.SetFloat(AnimationParam.MOVEMENT, inputXAbs);

        Movement.SetMoveInput(input);
    }

    /// <summary>
    /// 공중에서 뚝 떨어지는 느낌을 방지하기 위함
    /// </summary>
    public void HandleAirMoveInput()
    {
        if (Movement.IsGrounded && Movement.MoveY == 0f)
        {
            StopMovementInput();
            return;
        }
        HandleMoveInput();
    }

    public void StopMovementInput()
    {
        Movement.SetMoveInput(Vector2.zero);
    }

    public void HandleJumpInput()
    {
        if (owner.IsLockControl) return;

        if (Key.Space.WasPressed())
            Movement.RequestJump();

        if (Key.Space.WasReleased())
            Movement.ReleaseJump();
    }

    public void HandleDashInput()
    {
        if (owner.IsLockControl) return;

        if (Key.LeftShift.WasPressed())
        {
            bool canDash = Movement.RequestDash();
            if (canDash) Animator.SetTrigger(AnimationParam.DASH);
        }
    }

    public void HandleAttackInput()
    {
        if (owner.IsLockControlHard) return;

        if (Key.X.WasPressed())
            Animator.SetBool(AnimationParam.IS_ATTACK, true);
    }

    public void HandleComboAttackInput()
    {
        if (owner.IsLockControlHard) return;

        if (Animator.GetBool(AnimationParam.IS_ATTACK) == false && Key.X.WasPressed())
            Animator.SetBool(AnimationParam.IS_ATTACK, true);
    }

    public void HandleSkillInput()
    {
        if (owner.IsLockControl) return;
        if (isSkillCooldown) return;
        if (WasPressedSkillKey() == false) return;

        RequestSkill();
        Animator.SetBool(AnimationParam.CAN_JUMP_OR_FALL, false);
    }

    public void RequestStartSkillCooldown()
    {
        isSkillCooldown = true;
        SkillCooldownTime = 0f;
    }

    public bool WasPressedSkillKey() => Key.Z.WasPressed();
    #endregion

    #region PUBLIC METHOD
    public bool IsBusyForSwitch()
    {
        AnimatorStateInfo stateInfo = Animator.GetCurrentAnimatorStateInfo(0);

        int tagHash = stateInfo.tagHash;

        bool isBusy =
            tagHash == Animator.StringToHash(AnimationTag.ANIM_TAG_ATTACK) ||
            tagHash == Animator.StringToHash(AnimationTag.ANIM_TAG_ATTACK_END) ||
            tagHash == Animator.StringToHash(AnimationTag.ANIM_TAG_SKILL);

        return isBusy;
    }

    public bool IsCurrentAnimationFinished() => Animator.IsCurrentAnimationFinished();
    
    public void UseQTE()
    {
        Animator.SetTrigger(AnimationParam.QTE);
        Animator.SetBool(AnimationParam.CAN_JUMP_OR_FALL, false);
    }

    public void Dead()
    {
        Animator.SetTrigger(AnimationParam.DEAD);
        Animator.SetBool(AnimationParam.CAN_JUMP_OR_FALL, false);
    }
    #endregion

    protected virtual void RequestSkill()
    {
        Animator.SetTrigger(AnimationParam.SKILL);
    }
}
