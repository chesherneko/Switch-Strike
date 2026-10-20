using Sirenix.OdinInspector;
using UnityEngine;

public class Projectile : HitBox
{
    [Title("Components")]
    [SerializeField] private Rigidbody2D body;

    [Title("Projectile Settings")]
    [SerializeField] private float shootSpeed = 3f;
    [SerializeField] private float shootDuration = 2f;

    [Space]
    [SerializeField] private float checkRadius = 0.125f;
    [SerializeField] private Vector3 checkOffset;

    [Title("Runtime State")]
    [ShowInInspector, ReadOnly] private bool isShooted = false;
    [ShowInInspector, ReadOnly] private Vector2 shootDirection;

    [Space]
    [ShowInInspector, ReadOnly] private float shootTime = 0f;

    private ProjectileShooter shooter;

    private void Update()
    {
        if (isShooted == false) return;
        UpdateShootTime();
    }

    private void FixedUpdate()
    {
        if (isShooted == false) return;

        UpdatePosition();

        Vector2 origin = transform.position;
        RaycastHit2D hit = Physics2D.CircleCast(origin, checkRadius, shootDirection, checkOffset.x, hitLayerMask);

        if (hit.collider == null) return;

        bool isHit = TryHit(hit.collider);
        if (isHit) shooter.ReturnProjectile(this);
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        isShooted = false;
        shootTime = 0f;
    }

    public void InitShooter(ProjectileShooter shooter)
        => this.shooter = shooter;

    public void Shoot(Vector2 dir)
    {
        isShooted = true;
        shootDirection = dir;

        gameObject.SetActive(true);

        transform.SetParent(null);
        transform.localScale = Vector3.one;
        transform.right = shootDirection;
    }

    private void UpdateShootTime()
    {
        shootTime += Time.deltaTime;

        if (shootTime > shootDuration)
            shooter.ReturnProjectile(this);
    }

    private void UpdatePosition()
    {
        float dist = shootSpeed * Time.fixedDeltaTime;
        Vector2 delta = dist * shootDirection;

        body.MovePosition(body.position + delta);
    }
}
