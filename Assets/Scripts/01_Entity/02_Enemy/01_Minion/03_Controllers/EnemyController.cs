using UnityEngine;
using Sirenix.OdinInspector;

public class EnemyController : AnimationStateController
{
    private const string PATROL_POINT_NAME = "Patrol Point";

    private const float OFFSET_UNIT = 1f / 4;

    #region COMPONENT FIELDS
    [SerializeField, TitleGroup("Components")] protected EnemyCombat combat;
    #endregion

    #region SETTINGS FIELDS
    [SerializeField, TitleGroup("Idle")] private float minIdleTime = 1f;
    [SerializeField, TitleGroup("Idle")] private float maxIdleTime = 3f;

    [field: SerializeField, TitleGroup("Patrol")] public float PatrolSpeed { get; private set; } = 1f;

    [SerializeField, TitleGroup("Chase")] private float chaseRange = 5f;
    [field: SerializeField, TitleGroup("Chase")] public float ChaseSpeed { get; private set; } = 2f;
    [SerializeField, TitleGroup("Chase")] private int chaseAreaOffsetCount = 0;
    [SerializeField, TitleGroup("Chase")] private LayerMask obstacleLayer;

    [SerializeField, TitleGroup("Attack")] private float attackRange = 3f;
    [SerializeField, Range(1, 9), TitleGroup("Attack")] private int enhancedAttackChance = 3;

    [field: SerializeField, TitleGroup("Spawn")] public bool CanRespawn { get; private set; } = true;
    [field: SerializeField, TitleGroup("Spawn")] public float RespawnDelay { get; private set; } = 30f;
    #endregion

    #region RUNTIME STATE FIELDS
    [Space]
    [ShowInInspector, ReadOnly, TitleGroup("Patrol")] private float patrolMinX;
    [ShowInInspector, ReadOnly, TitleGroup("Patrol")] private float patrolMaxX;
    [ShowInInspector, ReadOnly, TitleGroup("Patrol")] private float patrolY;

    [PropertySpace]
    [ShowInInspector, ReadOnly, TitleGroup("Chase")] public Transform Target
    {
        get
        {
            Player localPlayer = Player.Local;

            if (localPlayer == null) 
                return null;

            return localPlayer.transform;
        }
    }

    [PropertySpace]
    [ShowInInspector, ReadOnly, TitleGroup("Attack")] public bool IsAttackRequested { get; set; } = false;

    [PropertySpace]
    [ShowInInspector, ReadOnly, TitleGroup("Spawn")] public bool HasSpawnedOnce { get; private set; } = false;
    [ShowInInspector, ReadOnly, TitleGroup("Spawn")] private Vector2 respawnPoint;
    [ShowInInspector, ReadOnly, TitleGroup("Spawn")] private Vector3 respawnDirection;
    #endregion

    #region UNITY METHOD
    protected override void Awake()
    {
        base.Awake();

        respawnPoint = transform.position;
        respawnDirection = transform.localScale;

        Transform patrolPoint = transform.Find(PATROL_POINT_NAME);

        if (patrolPoint == null)
        {
            Debug.LogError($"{name}의 순찰 지점이 존재하지 않습니다.");
            patrolPoint = transform;
        }

        float startX = transform.position.x;
        float pointX = patrolPoint.position.x;

        patrolMinX = Mathf.Min(startX, pointX);
        patrolMaxX = Mathf.Max(startX, pointX);
        patrolY = transform.position.y;
    }
    #endregion

    #region ABSTRACT METHOD
    protected override void InitStateMap()
    {
        stateMap = new()
        {
            { Animator.StringToHash(AnimationTag.ANIM_TAG_IDLE), new EnemyIdleState(this) },
            { Animator.StringToHash(AnimationTag.ANIM_TAG_PATROL), new EnemyPatrolState(this) },
            { Animator.StringToHash(AnimationTag.ANIM_TAG_CHASE), new EnemyChaseState(this) },
            { Animator.StringToHash(AnimationTag.ANIM_TAG_ATTACK), new EnemyAttackState(this) },
            { Animator.StringToHash(AnimationTag.ANIM_TAG_HIT), new EnemyHitState(this) },
            { Animator.StringToHash(AnimationTag.ANIM_TAG_DEAD), new EnemyDeadState(this) },
            { Animator.StringToHash(AnimationTag.ANIM_TAG_SPAWN), new EnemySpawnState(this) }
        };
    }

    protected override void InitDefaultState()
    {
        int key = Animator.StringToHash(AnimationTag.ANIM_TAG_SPAWN);
        defaultState = stateMap[key];
    }
    #endregion

    #region PRIVATE METHOD
    private bool IsTargetVisible()
    {
        RaycastHit2D hit = Physics2D.Linecast(
            transform.position, 
            Target.position, 
            obstacleLayer
        );

        return hit.collider == null;
    }

    private bool CanChaseTarget()
    {
        if (Target == null) return false;

        float targetX = Target.position.x;

        float offset = OFFSET_UNIT * chaseAreaOffsetCount;

        bool isTargetInArea =
            targetX >= patrolMinX - offset &&
            targetX <= patrolMaxX + offset;

        if (isTargetInArea == false) return false;
        if (transform.IsInRange(Target, chaseRange) == false) return false;

        return IsTargetVisible();
    }

    protected virtual int Attack()
    {
        int chance = Random.Range(0, 10);

        int id = chance < enhancedAttackChance ?
            AnimationParam.ATTACK_2 : 
            AnimationParam.ATTACK_1;

        Animator.SetTrigger(id);

        return id;
    }
    #endregion

    #region PUBLIC METHOD
    public float GetWaitingTime()
    {
        float waitingTime = Random.Range(minIdleTime, maxIdleTime);
        return waitingTime;
    }

    public void FaceTowards(float targetX)
    {
        float dirRaw = targetX - transform.position.x;

        if (Mathf.Approximately(dirRaw, 0f)) return;

        float dir = Mathf.Sign(dirRaw);

        Vector3 scale = transform.localScale;
        scale.x = dir * Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    public Vector2 GetPatrolPoint()
    {
        float randPatrolX = Random.Range(patrolMinX, patrolMaxX);
        return new Vector2(randPatrolX, patrolY);
    }

    public bool TryChaseTarget()
    {
        bool canChase = CanChaseTarget();
        Animator.SetBool(AnimationParam.IS_CHASING, canChase);

        return canChase;
    }

    public Vector2 GetChasePoint()
    {
        float targetX = Mathf.Clamp(
            Target.position.x,
            patrolMinX,
            patrolMaxX
        );

        return new Vector3(targetX, patrolY);
    }

    public bool TryAttackTarget()
    {
        if (IsAttackRequested) return false;
        if (Target == null) return false;
        if (transform.IsInRange(Target, attackRange) == false) return false;
        if (IsTargetVisible() == false) return false;

        IsAttackRequested = true;
        Attack();

        return true;
    }

    public virtual void OnAttackEnd()
    {
        IsAttackRequested = false;
    }

    public virtual void ResetAttackState()
    {
        IsAttackRequested = false;
        Animator.ResetTrigger(AnimationParam.ATTACK_1);
        Animator.ResetTrigger(AnimationParam.ATTACK_2);
    }

    public void UpdateDeadState(float elapsedTime)
    {
        if (CanRespawn == false)
        {
            Destroy(gameObject);
            return;
        }

        if (elapsedTime > RespawnDelay)
            Animator.SetTrigger(AnimationParam.SPAWN);
    }

    public void Spawn()
    {
        combat.ResetHP();

        if (HasSpawnedOnce == false)
            HasSpawnedOnce = true;
        else
        {
            transform.position = respawnPoint;
            transform.localScale = respawnDirection;
        }
    }
    #endregion
}
