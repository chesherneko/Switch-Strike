using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [Title("Components")]
    [field: SerializeField] public Animator Animator { get; private set; }

    [Title("Runtime States")]
    [ShowInInspector, ReadOnly] private BossStateBase currentState;

    private readonly Dictionary<Type, BossStateBase> stateMap = new();

    private void Awake()
    {
        BossStateBase[] states = GetComponentsInChildren<BossStateBase>(true);

        for (int i = 0; i < states.Length; i++)
        {
            BossStateBase state = states[i];
            state.Initialize(this);

            stateMap.Add(state.GetType(), state);

            state.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        Type start = typeof(BossIdleState);
        ChangeState(start);
    }

    public void ChangeState(Type stateType)
    {
        if (stateMap.TryGetValue(stateType, 
            out BossStateBase nextState) == false)
        {
            Debug.LogError($"'{stateType.FullName}'은 등록되지 않은 상태입니다.");
            return;
        }

        if (currentState != null)
            currentState.gameObject.SetActive(false);

        currentState = nextState;
        currentState.gameObject.SetActive(true);
    }
}
