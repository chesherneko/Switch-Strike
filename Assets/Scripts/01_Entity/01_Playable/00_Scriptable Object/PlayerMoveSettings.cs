using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerMoveSettings", menuName = "Scriptable Objects/Entity/Player/Move/Settings")]
public class PlayerMoveSettings : ScriptableObject
{
    [field: Title("Move")]
    [field: SerializeField] public float MaxMoveSpeed { get; private set; } = 6.5f;
    [field: SerializeField] public float MaxFallSpeed { get; private set; } = -20f;
    [field: SerializeField] public float Acceleration { get; private set; } = 20f;
    [field: SerializeField] public float Deceleration { get; private set; } = 30f;

    [field: Title("Gravity")]
    [field: SerializeField] public float Gravity { get; private set; } = -9.81f;
    [field: SerializeField] public float GroundStickGravity { get; private set; } = -2f;

    [field: PropertySpace]
    [field: SerializeField] public float FallGravityMult { get; private set; } = 4.25f;
    [field: SerializeField] public float LastAscentGravityMult { get; private set; } = 5.25f;

    [field: Title("Jump")]
    [field: SerializeField] public float JumpPower { get; private set; } = 13f;
    [field: SerializeField] public int MaxJumpCount { get; private set; } = 2;

    [field: PropertySpace]
    [field: SerializeField] public float MaxJumpHoldTime { get; private set; } = 0.15f;

    [field: PropertySpace]
    [field: SerializeField] public float JumpPowerMultInAir { get; private set; } = 1f;
    [field: SerializeField] public float JumpStartMoveSpeedMult { get; private set; } = 0.75f;
    [field: SerializeField] public float MinJumpHoldGravityMult { get; private set; } = 2.5f;

    [field: PropertySpace]
    [field: SerializeField] public float CoyoteTimeLimit { get; private set; } = 0.15f;
    [field: SerializeField] public float JumpBufferTimeLimit { get; private set; } = 0.15f;

    [field: PropertySpace]
    [field: SerializeField] public float JumpHangThreshold { get; private set; } = 0.5f;
    [field: SerializeField] public float JumpHangGravityMult { get; private set; } = 0.5f;
    [field: SerializeField] public float JumpHangSpeedMult { get; private set; } = 1.25f;

    [field: Title("Wall Cling")]
    [field: SerializeField] public LayerMask WallClingLayerMask { get; private set; }
    [field: SerializeField] public float WallCheckDistance { get; private set; } = 0.05f;
    [field: SerializeField] public float WallDetachInputTimeLimit { get; private set; } = 0.5f;

    [field: PropertySpace]
    [field: SerializeField] public float WallClingGravityMult { get; private set; } = 0.75f;
    [field: SerializeField] public float WallJumpMoveSpeedMult { get; private set; } = 0.5f;

    [field: Title("Dash")]
    [field: SerializeField] public float DashSpeed { get; private set; } = 30f;
    [field: SerializeField] public float DashDuration { get; private set; } = 0.15f;
    [field: SerializeField] public float DashCooldown { get; private set; } = 0.5f;

    [field: PropertySpace]
    [field: SerializeField] public float DashEndSpeedMult { get; private set; } = 0.75f;

    [field: Title("Collision")]
    [field: SerializeField] public float SkinWidth { get; private set; } = 0.02f;
    [field: SerializeField] public float GroundCheckDistance { get; private set; } = 0.2f;
    [field: SerializeField] public LayerMask GroundLayerMask { get; private set; }
}
