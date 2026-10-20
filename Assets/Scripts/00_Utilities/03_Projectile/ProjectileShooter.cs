using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileShooter : MonoBehaviour
{
    [Title("References")]
    [SerializeField] private Projectile prefab;

    private readonly Queue<Projectile> projectiles = new();

    private void Awake()
    {
        Projectile[] projs = GetComponentsInChildren<Projectile>(true);

        for (int i = 0; i < projs.Length; i++)
        {
            Projectile proj = projs[i];

            proj.InitShooter(this);
            ReturnProjectile(proj);
        }
    }

    private Projectile GetProjectile()
    {
        if (projectiles.Count > 0)
            return projectiles.Dequeue();

        Projectile proj = Instantiate(prefab, transform);
        proj.InitShooter(this);

        return proj;
    }

    public void ShootProjectile(Vector3 targetPos)
    {
        Projectile proj = GetProjectile();

        Vector2 dir = targetPos - transform.position;
        proj.Shoot(dir.normalized);
    }

    public void ReturnProjectile(Projectile proj)
    {
        proj.transform.SetParent(transform);
        proj.transform.localPosition = Vector3.zero;
        proj.transform.right = Vector3.zero;

        proj.gameObject.SetActive(false);

        projectiles.Enqueue(proj);
    }
}
