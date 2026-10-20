using Sirenix.OdinInspector;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    [Title("Components")]
    [SerializeField] private Transform spawnPoint;

    [Title("Settings")]
    [SerializeField] private LayerMask collisionMask;

    public Vector3 SpawnPoint => spawnPoint.position;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collisionMask.Contains(collision) == false) 
            return;

        RespawnManager.Instance.SetCurrentSavePoint(this);
    }
}
