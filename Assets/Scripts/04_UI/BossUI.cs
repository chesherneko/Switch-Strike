using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class BossUI : MonoSingleton<BossUI>
{
    [Title("Components")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private Image remainHP;

    public void SetActiveUI(bool isActive)
    {
        canvas.enabled = isActive;
    }

    public void UpdateHP(float hpRatio)
    {
        remainHP.fillAmount = hpRatio;
    }
}
