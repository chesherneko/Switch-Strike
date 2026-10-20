using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "EntityStat", menuName = "Scriptable Objects/Entity/Stat")]
public class EntityStat : ScriptableObject
{
    [field: Title("Stat")]
    [field: SerializeField] public int MaxHP { get; private set; }
    [field: SerializeField] public int ATK { get; private set; }
}
