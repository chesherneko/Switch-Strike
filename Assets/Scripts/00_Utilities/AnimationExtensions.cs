using UnityEngine;

public static class AnimationExtensions
{
    public static bool IsCurrentAnimationFinished(this Animator animator, int layerIndex = 0)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);

        if (animator.IsInTransition(layerIndex))
            return false;

        return stateInfo.normalizedTime >= 1f;
    }
}
