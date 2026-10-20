using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "AfterimageItemSettings", menuName = "Scriptable Objects/Afterimage/Item/Settings")]
public class AfterimageItemSettings : ScriptableObject
{
    [field: Title("Settings")]
    [field: SerializeField] public float StartAlpha { get; private set; } = 0.75f;
    [field: SerializeField] public float FadeDuration { get; private set; } = 0.1f;
}
