using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections;

public class BossBeam : MonoBehaviour
{
    [Title("Components")]
    [SerializeField] private new SpriteRenderer renderer;
    [SerializeField] private Transform hitBoxTrans;

    [Title("Settings")]
    [SerializeField] private float positionOffset = 0.25f;
    [SerializeField] private float duration = 1f;

    [Space]
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float lengthOffset = 10f;

    public IEnumerator Shoot(Vector2 pupilDir, Vector2 pupilPos)
    {
        Vector3 offset = pupilDir * positionOffset;
        Vector2 pos = transform.position + offset;

        transform.position = pos;
        transform.up = pupilDir * -1f;

        ResizeRenderingSize(pupilDir);

        gameObject.SetActive(true);

        yield return new WaitForSeconds(duration);

        gameObject.SetActive(false);
    }

    private void ResizeRenderingSize(Vector2 dir)
    {
        Vector2 origin = transform.position;

        RaycastHit2D hit = Physics2D.Raycast(origin, dir, 100f, layerMask);

        float dist = hit.collider == null ? 100f : hit.distance + lengthOffset;

        Vector2 spriteSize = renderer.size;
        spriteSize.y = dist;
        renderer.size = spriteSize;

        hitBoxTrans.localScale = new Vector3(1f, dist, 1f);
    }
}
