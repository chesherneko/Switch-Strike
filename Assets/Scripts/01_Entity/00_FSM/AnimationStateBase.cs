using UnityEngine;

public class AnimationStateBase
{
    protected Animator Animator { get; }

    protected AnimationStateBase(AnimationStateController owner)
    {
        Animator = owner.Animator;
    }

    public virtual void OnEnter() { }
    public virtual void OnUpdateState(AnimatorStateInfo stateInfo) { }
    public virtual void OnExit() { }
}
