using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum SoundType
{
    BGM,
    Run,
    CoinGet,
    CoinPut,
    ChestOpen,
    ChestClose,
    UIItemPick,
    UIItemPut,
    UIItemUp,
    OutfitGet,
    OutfitPut,
    DropCoin,
    UIUp,
    UIClick,
    UIApply,
    Sword,

    NULL,
}


public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    [SerializeField]
    private AudioMixer mixer;

    [SerializeField]
    private AudioSource[] audioClips;

    private Dictionary<SoundType, AudioSource> dictionary = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int count = 0;
        foreach (SoundType type in Enum.GetValues(typeof(SoundType)))
        {
            if (type == SoundType.NULL) continue;
            dictionary[type] = audioClips[count++];
        }
    }

    //TooltipService 에서 UIItem 호출
    public void Play(SoundType clip)
    {
        if (clip == SoundType.NULL) return;
        dictionary[clip].Play();
    }
    //Run만 사용할듯
    public void Stop(SoundType clip)
    {
        if (clip == SoundType.NULL) return;
        dictionary[clip].Stop();
    }

    public void SetMasterVolum(float value)
    {
        float dB = Mathf.Lerp(-80f, 20f, value);
        mixer.SetFloat("Master", dB);
    }
    public void SetBGMVolum(float value)
    {
        float dB = Mathf.Lerp(-80f, 20f, value);
        mixer.SetFloat("BGM", dB);
    }
    public void SetEffectVolum(float value)
    {
        float dB = Mathf.Lerp(-80f, 20f, value);
        mixer.SetFloat("Effect", dB);
    }
    public float GetVolum(string str)
    {
        mixer.GetFloat(str, out float value);
        return Mathf.InverseLerp(-80f, 20f, value);
    }
}
