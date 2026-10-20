using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

using PatternState = BossPatternStateBase;

public class BossIdleState : BossStateBase
{
    [Title("Patterns")]
    [HideReferenceObjectPicker]
    [SerializeField] private List<PatternState> normalPatterns;

    [Space]
    [HideReferenceObjectPicker]
    [SerializeField] private List<PatternState> specialPatterns;

    [Title("Settings")]
    [SerializeField] private float specialPatternChance = 0.25f;

    protected override void OnEnable()
    {
        StartCoroutine(ExecuteCoroutine());
    }

    protected override void OnDisable()
    {
        
    }

    private IEnumerator ExecuteCoroutine()
    {
        yield return new WaitForSeconds(2f);

        float randChance = Random.Range(0f, 1f);

        List<PatternState> patterns = randChance <= specialPatternChance ?
            specialPatterns : normalPatterns;

        int randIdx = Random.Range(0, patterns.Count);

        PatternState pattern = patterns[randIdx];
        owner.ChangeState(pattern.GetType());
    }
}
