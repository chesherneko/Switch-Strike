using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

using AnimState = AnimationStateBase;

public abstract class AnimationStateController : MonoBehaviour
{
    [field: TitleGroup("Components")]
    [field: SerializeField] public Animator Animator { get; private set; }

    protected Dictionary<int, AnimState> stateMap;
    protected AnimState defaultState;
    protected AnimState currentState;

    protected virtual void Awake()
    {
        InitStateMap();
        InitDefaultState();
    }

    protected virtual void Update()
    {
        UpdateAnimationState();
    }

    protected abstract void InitStateMap();
    protected abstract void InitDefaultState();

    protected virtual void UpdateAnimationState()
    {
        AnimatorStateInfo stateInfo = Animator.GetCurrentAnimatorStateInfo(0);

        if (stateMap.TryGetValue(stateInfo.tagHash, out AnimState nextState) == false)
            nextState = defaultState;

        if (currentState != nextState)
        {
            currentState?.OnExit();

            currentState = nextState;
            nextState.OnEnter();
        }

        currentState.OnUpdateState(stateInfo);
    }
}
