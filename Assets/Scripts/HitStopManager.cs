using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HitStopManager : MonoSingleton<HitStopManager>
{
    [SerializeField] private CinemachineBasicMultiChannelPerlin cameraNoise;
    [SerializeField] private float shakeAmplitude = 0.5f;
    [SerializeField] private float shakeFrequency = 12f;

    //private float defaultFixedDeltaTime;
    private bool isPlaying = false;

    protected override void Awake()
    {
        base.Awake();
        //defaultFixedDeltaTime = Time.fixedDeltaTime;
    }

    public void Play(float duration)
    {
        if (isPlaying) return;
        if (duration <= 0f) return;

        StartCoroutine(PlayCoroutine(duration));
    }

    private IEnumerator PlayCoroutine(float duration)
    {
        isPlaying = true;

        Time.timeScale = 0f;
        //Time.fixedDeltaTime = 0f;

        SetCameraShake(true);

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1f;
        //Time.fixedDeltaTime = defaultFixedDeltaTime;

        SetCameraShake(false);

        isPlaying = false;
    }

    private void SetCameraShake(bool isActive)
    {
        cameraNoise.AmplitudeGain = isActive ? shakeAmplitude : 0f;
        cameraNoise.FrequencyGain = isActive ? shakeFrequency : 0f;
    }
}
