using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections;
using DG.Tweening;

public class PlayerMove : MonoBehaviour
{
    #region FIELDS
    [Title("Components")]
    [SerializeField] private Player owner;
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private new Collider2D collider;
    [SerializeField] private Afterimage afterImage;

    [Title("Settings")]
    [SerializeField] private PlayerMoveSettings settings;

    public PlayerCombat Combat => owner.Combat;
    #endregion  

    #region RUNTIME STATE FIELDS
    [field: Title("Runtime State")]
    [field: ShowInInspector, ReadOnly] public bool IsGrounded { get; private set; } = false;

    [Space] //Move States
    [ShowInInspector, ReadOnly] private Vector2 moveInput;
    [field: ShowInInspector, ReadOnly] public float MoveX { get; private set; } = 0f;
    [field: ShowInInspector, ReadOnly] public float MoveY { get; private set; } = 0f;
    [field: ShowInInspector, ReadOnly] public float LastMoveDirection { get; private set; } = 1f;

    [Space] //Jump States
    [ShowInInspector, ReadOnly] private bool isJumpRequested = false;
    [ShowInInspector, ReadOnly] private bool isJumping = false;
    [ShowInInspector, ReadOnly] private int jumpCount = 0;
    [ShowInInspector, ReadOnly] private bool isJumpHold = false;
    [ShowInInspector, ReadOnly] private float jumpHoldTime = 0f;
    [ShowInInspector, ReadOnly] private float coyoteTime = float.PositiveInfinity;
    [ShowInInspector, ReadOnly] private float jumpBufferTime = float.PositiveInfinity;

    [field: PropertySpace] //Wall Cling States
    [field: ShowInInspector, ReadOnly] public bool IsClingToWall { get; private set; } = false;
    [ShowInInspector, ReadOnly] private float wallDetachInputTime = 0f;

    [Space] //Dash States
    [ShowInInspector, ReadOnly] private bool isDashRequested = false;
    [ShowInInspector, ReadOnly] private bool isDashOnCooldown = false;
    [ShowInInspector, ReadOnly] private float dashCooldownTime = 0f;

    [Space] //Dashing States
    [ShowInInspector, ReadOnly] private bool isDashing = false;
    [ShowInInspector, ReadOnly] private float dashTime = 0f;
    [ShowInInspector, ReadOnly] private float dashStartX = 0f;
    [ShowInInspector, ReadOnly] private float dashEndX = 0f;
    [ShowInInspector, ReadOnly] private float dashPreviousX = 0f;

    private bool IsBonusPeakSection => Mathf.Abs(MoveY) < settings.JumpHangThreshold;
    #endregion

    #region UNITY METHOD
    private void Update()
    {
        if (owner.IsLockControl) return;

        UpdateJumpBufferTime();
        UpdateCoyoteTime();

        UpdateJumpHoldTime();

        UpdateDashCooltime();

        UpdateWallDetachInputTime();
    }

    private void FixedUpdate()
    {
        if (owner.IsLockControl) return;

        IsGrounded = CheckGrounded();
        UpdateGroundedState();

        if (isDashRequested)
        {
            isDashRequested = false;
            ReadyDash();
        }

        if (isDashing)
        {
            ApplyDash();
            return;
        }

        if (IsClingToWall) ApplyWallCling();
        else ApplyMove();

        var (desiredMoveX, canMoveXAsDesired) = ApplyActualMove();

        if (IsClingToWall == false)
            TryEnterWallCling(desiredMoveX, canMoveXAsDesired);
    }
    #endregion

    #region INPUT/TIME UPDATE METHOD
    private void UpdateJumpBufferTime()
    {
        if (isJumpRequested == false) return;

        jumpBufferTime += Time.deltaTime;

        if (jumpBufferTime >= settings.JumpBufferTimeLimit)
        {
            isJumpRequested = false;
            jumpBufferTime = float.PositiveInfinity;
        }
    }

    private void UpdateCoyoteTime()
    {
        if (isJumping) return;
        if (IsGrounded) return;

        coyoteTime += Time.deltaTime;
    }

    private void UpdateJumpHoldTime()
    {
        if (isJumping == false) return;
        if (isJumpHold == false) return;

        jumpHoldTime += Time.deltaTime;

        if (jumpHoldTime >= settings.MaxJumpHoldTime)
        {
            isJumpHold = false;
            jumpHoldTime = settings.MaxJumpHoldTime;
        }
    }

    private void UpdateDashCooltime()
    {
        if (isDashOnCooldown == false) return;
        if (isDashing) return;
        if (IsGrounded == false) return;

        dashCooldownTime += Time.deltaTime;

        if (dashCooldownTime >= settings.DashCooldown)
        {
            dashCooldownTime = 0f;
            isDashOnCooldown = false;
        }
    }

    private void UpdateWallDetachInputTime()
    {
        if (IsClingToWall == false) return;

        float input = moveInput.x;
        bool isHoldingAwayFromWall = input == LastMoveDirection * -1f;

        if (isHoldingAwayFromWall == false)
        {
            wallDetachInputTime = 0f;
            return;
        }

        wallDetachInputTime += Time.deltaTime;

        if (wallDetachInputTime >= settings.WallDetachInputTimeLimit)
            IsClingToWall = false;
    }
    #endregion

    #region STATE UPDATE METHOD
    private void UpdateGroundedState()
    {
        if (IsGrounded == false) return;
        if (MoveY > 0f) return;

        IsClingToWall = false;

        jumpCount = 0;
        isJumping = false;

        isJumpHold = false;
        jumpHoldTime = 0f;

        coyoteTime = 0f;
        jumpBufferTime = float.PositiveInfinity;

        if (isDashOnCooldown == false && dashCooldownTime > 0f)
            dashCooldownTime = 0f;
    }

    public void ResetState(bool includeMoveX = false)
    {
        moveInput = Vector2.zero;
        MoveY = 0f;
        if (includeMoveX) MoveX = 0f;

        isJumpRequested = false;

        isJumping = false;
        jumpCount = 0;

        isJumpHold = false;
        jumpHoldTime = 0f;

        jumpBufferTime = float.PositiveInfinity;
        coyoteTime = float.PositiveInfinity;

        IsClingToWall = false;
        wallDetachInputTime = 0f;

        isDashRequested = false;
        isDashing = false;
        dashTime = 0f;

        isDashOnCooldown = false;
        dashCooldownTime = 0f;

        afterImage.Stop();
        Combat.OnDashFinished();
    }
    #endregion

    #region CORE MOVE METHOD
    private void ApplyMove()
    {
        ApplyHorizontalMove();

        float gravityMult = GetGravityMultiplier();
        ApplyGravity(gravityMult);

        TryApplyJump();
    }

    private (float desiredMoveX, bool canMoveXAsDesired) ApplyActualMove()
    {
        Vector2 desireMove = new Vector2(MoveX, MoveY) * Time.fixedDeltaTime;

        Bounds box = GetCastBox();

        float moveX = ResolveAxisMovement(desireMove.x, Vector2.right, box, out bool canMoveXAsDesired);
        box.center += Vector3.right * moveX;

        float moveY = ResolveAxisMovement(desireMove.y, Vector2.up, box, out bool canMoveYAsDesired);

        Vector2 move = new(moveX, moveY);

        if (canMoveYAsDesired == false) MoveY = 0f;

        body.MovePosition(body.position + move);

        return (desireMove.x, canMoveXAsDesired);
    }

    private float ResolveAxisMovement(float move, Vector2 axis, Bounds box, out bool canMoveAsDesired)
    {
        canMoveAsDesired = true;

        if (move == 0f) return 0f;

        Vector3 dir = move > 0f ? axis : -axis;
        float dist = Mathf.Abs(move);

        RaycastHit2D hit = Physics2D.BoxCast(box.center, box.size, 0f, dir, dist, settings.GroundLayerMask);

        if (hit.collider == null) return move;

        canMoveAsDesired = false;
        float resolvedMove = Mathf.Max(0f, hit.distance - settings.SkinWidth);

        return Mathf.Sign(move) * resolvedMove;
    }
    #endregion

    #region HORIZONTAL MOVE METHOD
    private void ApplyHorizontalMove()
    {
        float dir = moveInput.x;
        bool isMoving = dir != 0f;

        float targetSpeed;

        float speedChangeRate = isMoving ? settings.Acceleration : settings.Deceleration;
        float maxDelta = speedChangeRate * Time.fixedDeltaTime;

        if (isMoving == false) targetSpeed = 0f;
        else
        {
            LastMoveDirection = dir;

            targetSpeed = settings.MaxMoveSpeed * dir;

            bool isTurnDir = MoveX != 0f && Mathf.Sign(MoveX) != dir;
            if (IsGrounded && isTurnDir) MoveX = 0f;

            if (isJumping && IsBonusPeakSection)
                targetSpeed *= settings.JumpHangSpeedMult;
        }

        MoveX = Mathf.MoveTowards(MoveX, targetSpeed, maxDelta);
    }
    #endregion

    #region GRAVITY METHOD
    private void ApplyGravity(float gravityMult)
    {
        if (IsGrounded && MoveY <= 0f)
        {
            MoveY = settings.GroundStickGravity;
            return;
        }

        float gravity = settings.Gravity * gravityMult;

        MoveY += gravity * Time.fixedDeltaTime;
        MoveY = Mathf.Max(MoveY, settings.MaxFallSpeed);
    }

    private float GetGravityMultiplier()
    {
        if (isJumping == false)
            return MoveY > 0f ? 1f : settings.FallGravityMult;

        float mult;

        if (isJumpHold) mult = 1f;
        else
        {
            if (MoveY > 0f)
            {
                float holdTime = Mathf.Min(jumpHoldTime, settings.MaxJumpHoldTime);

                float t = Mathf.InverseLerp(0f, settings.MaxJumpHoldTime, holdTime);
                float gravityMult = Mathf.Lerp(settings.MinJumpHoldGravityMult, 1f, t);

                mult = settings.LastAscentGravityMult * gravityMult;
            }
            else mult = settings.FallGravityMult;
        }

        if (IsBonusPeakSection)
            mult *= settings.JumpHangGravityMult;

        return mult;
    }
    #endregion

    #region JUMP METHOD
    private void TryApplyJump()
    {
        if (CanJump() == false) return;

        coyoteTime = float.PositiveInfinity;
        if (jumpCount == 0)
        {
            dashCooldownTime = settings.DashCooldown;
            isDashOnCooldown = false;
        }

        float moveSpeed = MoveX * settings.JumpStartMoveSpeedMult;
        PerformJump(moveSpeed, true);
    }

    private void ApplyWallJump()
    {
        IsClingToWall = false;

        float dir = LastMoveDirection * -1f;
        float input = moveInput.x;

        float moveSpeedMult = input == dir ? settings.JumpStartMoveSpeedMult : settings.WallJumpMoveSpeedMult;
        float moveSpeed = settings.MaxMoveSpeed * moveSpeedMult * dir;

        PerformJump(moveSpeed, false);
    }

    private void PerformJump(float moveSpeed, bool canHoldJump)
    {
        isJumpRequested = false;
        isJumping = true;

        isJumpHold = canHoldJump && jumpCount == 0;

        if (isJumpHold == false)
            jumpHoldTime = settings.MaxJumpHoldTime;

        float power = settings.JumpPower;
        float powerMult = jumpCount == 0 ? 1f : settings.JumpPowerMultInAir;

        power *= powerMult;

        MoveX = moveSpeed;
        MoveY = power;

        jumpCount += 1;
    }

    private bool CanJump()
    {
        if (isJumpRequested == false) return false;
        if (IsGrounded) return true;
        if (coyoteTime <= settings.CoyoteTimeLimit) return true;
        if (isJumping == false) return false;

        return jumpCount < settings.MaxJumpCount;
    }
    #endregion

    #region WALL CLING METHOD
    private void TryEnterWallCling(float desiredMoveX, bool canMoveXAsDesired)
    {
        if (IsGrounded) return;
        if (canMoveXAsDesired) return;
        if (desiredMoveX == 0f) return;

        LastMoveDirection = Mathf.Sign(desiredMoveX);

        if (CheckWallClingSurface() == false) return;

        MoveX = 0f;
        MoveY = 0f;

        jumpCount = 0;

        dashCooldownTime = settings.DashCooldown;
        isDashOnCooldown = false;

        IsClingToWall = true;
        wallDetachInputTime = 0f;
    }

    private void ApplyWallCling()
    {
        if (CheckWallClingSurface() == false)
        {
            IsClingToWall = false;
            return;
        }

        MoveX = 0f;

        if (isJumpRequested)
        {
            ApplyWallJump();
            return;
        }

        Vector2 moveAxis = moveInput;

        bool shouldIgnoreGravity =
            moveAxis.y >= 0f &&
            moveAxis != Vector2.zero;

        if (shouldIgnoreGravity) MoveY = 0f;
        else ApplyGravity(settings.WallClingGravityMult);
    }

    private bool CheckWallClingSurface()
    {
        Bounds box = GetCastBox();

        Vector2 origin = Vector2.zero;
        origin.x = LastMoveDirection > 0f ? box.max.x : box.min.x;
        origin.y = box.center.y;

        Vector2 dir = Vector2.right * LastMoveDirection;

        RaycastHit2D hit = Physics2D.Raycast(
            origin,
            dir,
            settings.WallCheckDistance,
            settings.WallClingLayerMask
        );

        return hit.collider != null;
    }
    #endregion

    #region DASH METHOD
    private void ReadyDash()
    {
        isDashing = true;
        isDashOnCooldown = true;

        Combat.OnDashStarted();

        if (IsGrounded) coyoteTime = float.PositiveInfinity;

        float dir = moveInput.x;
        LastMoveDirection = dir != 0f ? dir : LastMoveDirection;

        float distance = settings.DashSpeed * settings.DashDuration;
        float delta = distance * LastMoveDirection;

        dashStartX = body.position.x;
        dashEndX = dashStartX + delta;
        dashPreviousX = dashStartX;

        MoveY = 0f; //대시 중에는 y축 이동량 제거

        afterImage.Execute(distance);
    }

    private void ApplyDash()
    {
        dashTime += Time.fixedDeltaTime;

        float progress = Mathf.Clamp01(dashTime / settings.DashDuration);

        float easedMove = EasingFunction.EaseInOutQuart(dashStartX, dashEndX, progress);
        float desiredMove = easedMove - dashPreviousX;
        float actualMove = ResolveAxisMovement(desiredMove, Vector2.right, GetCastBox(), out bool canMoveAsDesired);

        body.MovePosition(body.position + new Vector2(actualMove, 0f));

        dashPreviousX += desiredMove;

        if (progress >= 1f || canMoveAsDesired == false)
        {
            isDashing = false;
            dashTime = 0f;

            //대쉬한 방향으로 속도값 조정
            MoveX = Mathf.Abs(MoveX) *
                settings.DashEndSpeedMult *
                LastMoveDirection;

            afterImage.Stop();
            Combat.OnDashFinished();
        }
    }
    #endregion

    #region SWITCH METHOD
    public void SyncPrevPlayerState(PlayerMoveSyncData syncData)
    {
        MoveX = syncData.MoveX;
        LastMoveDirection = syncData.LastMoveDirection;

        isJumping = true;
    }
    #endregion

    #region COLLISION METHOD
    private bool CheckGrounded()
    {
        Bounds box = GetCastBox();

        RaycastHit2D hit = Physics2D.BoxCast(box.center, box.size, 0f,
            Vector2.down, settings.GroundCheckDistance,
            settings.GroundLayerMask);

        return hit.collider != null;
    }

    private Bounds GetCastBox()
    {
        Bounds box = collider.bounds;

        Vector3 size = box.size;
        size.x -= settings.SkinWidth * 2;
        size.y -= settings.SkinWidth * 2;
        box.size = size;

        return box;
    }
    #endregion

    #region PUBLIC METHOD
    public void SetMoveInput(Vector2 input)
    {
        float x = Mathf.Clamp(input.x, -1f, 1f);
        float y = Mathf.Clamp(input.y, -1f, 1f);
        moveInput = new Vector2(x, y);
    }

    public void RequestJump()
    {
        if (isDashing) return;

        isJumpRequested = true;
        jumpBufferTime = 0f;
    }

    public void ReleaseJump()
    {
        if (isJumpHold)
            isJumpHold = false;
    }

    public bool RequestDash()
    {
        if (isDashing) return false;
        if (isDashOnCooldown) return false;

        isDashRequested = true;
        return true;
    }

    public void SwitchMoveDirection() 
        => LastMoveDirection *= -1f;
    #endregion
}
