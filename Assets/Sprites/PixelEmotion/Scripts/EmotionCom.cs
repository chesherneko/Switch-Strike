using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmotionCom : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private Animator animator;

    public void Play(EmotionType type)
    {
        StartCoroutine(PlayCoroutine(type));
    }

    public IEnumerator PlayCoroutine(EmotionType type)
    {
        root.SetActive(true);

        string enumName = GetEnumName<EmotionType>((int)type);
        animator.Play(enumName, 0, 0f);

        while (true)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.normalizedTime >= 1f) break;
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        root.SetActive(false);
    }

    public string GetEnumName<T>(int value)
    {
        string name = "";
        name = Enum.Parse(typeof(T), Enum.GetName(typeof(T), value)).ToString();
        return name;
    }
}
