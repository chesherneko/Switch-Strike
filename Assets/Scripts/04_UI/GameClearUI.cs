using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameClearUI : MonoSingleton<GameClearUI>
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private CanvasGroup canvasGroup;

    private bool isClear = false;

    private void Update()
    {
        if (isClear == false) return;

        if (InputHelper.WasPressedAnyKey())
            Application.Quit();
    }

    public void GameClear()
    {
        Player.Local.LockControlHard();
        StartCoroutine(ActiveUICoroutine());
    }

    private IEnumerator ActiveUICoroutine()
    {
        canvas.enabled = true;

        yield return canvasGroup
            .DOFade(1f, 2.5f)
            .WaitForCompletion();

        isClear = true;
    }
}
