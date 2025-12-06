using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Option : MonoBehaviour
{
    //clsoe
    [SerializeField]
    public Button b_close;

    //sections
    [SerializeField]
    private Button b_general;
    [SerializeField]
    private Button b_graphics;
    [SerializeField]
    private Button b_sound;

    [SerializeField]
    private GameObject p_general;
    [SerializeField]
    private GameObject p_graphics;
    [SerializeField]
    private GameObject p_sound;


    //general
    [SerializeField]
    private TMP_Dropdown d_language;


    //graphics
    [SerializeField]
    private TMP_Dropdown d_resolutions;
    [SerializeField]
    private Toggle t_fullScreen;
    List<Resolution> resolutions = new();

    //sounds
    [SerializeField]
    private Slider s_master;
    [SerializeField]
    private Slider s_bgm;
    [SerializeField]
    private Slider s_effect;

    private void OnEnable()
    {
        CloseInit();
        SectionsInit();
        GeneralInit();
        GraphicInit();
        SoundInit();
    }

    private void OnDisable()
    {
        b_close.onClick.RemoveAllListeners();
        b_general.onClick.RemoveAllListeners();
        b_graphics.onClick.RemoveAllListeners();
        b_sound.onClick.RemoveAllListeners();
        d_language.onValueChanged.RemoveAllListeners();
        d_resolutions.onValueChanged.RemoveAllListeners();
        SoundDeinit();
    }


    private void CloseInit()
    {
        void Close()
        {
            GameManager.Instance.OnOptionClose();
            SoundManager.Instance.Play(SoundType.UIClick);
        }
        b_close.onClick.AddListener(Close);
    }

    private void SectionsInit()
    {
        void General()
        {
            p_general.SetActive(true);
            p_graphics.SetActive(false);
            p_sound.SetActive(false);
            SoundManager.Instance.Play(SoundType.UIApply);
        }
        void Graphics()
        {
            p_general.SetActive(false);
            p_graphics.SetActive(true);
            p_sound.SetActive(false);
            SoundManager.Instance.Play(SoundType.UIApply);
        }
        void Sound()
        {
            p_general.SetActive(false);
            p_graphics.SetActive(false);
            p_sound.SetActive(true);
            SoundManager.Instance.Play(SoundType.UIApply);
        }
        b_general.onClick.AddListener(General);
        b_graphics.onClick.AddListener(Graphics);
        b_sound.onClick.AddListener(Sound);
    }

    private void GeneralInit()
    {
        void OnChangeLanguage(int value)
        {
            LocalizationManager.Instance.Current = (Language)value;
            SoundManager.Instance.Play(SoundType.UIApply);
        }

        d_language.ClearOptions();
        List<string> values = Enum.GetNames(typeof(Language)).ToList();
        d_language.AddOptions(values);
        d_language.value = (int)LocalizationManager.Instance.Current;
        d_language.onValueChanged.AddListener(OnChangeLanguage);
    }

    private void GraphicInit()
    {
        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            if (Screen.resolutions[i].refreshRateRatio.value >= 60)
            {
                resolutions.Add(Screen.resolutions[i]);
            }
        }

        d_resolutions.options.Clear();
        List<string> options = new();
        int value = 0;
        for (int i = 0; i < resolutions.Count; i++)
        {
            options.Add(resolutions[i].width + " x " + resolutions[i].height);
            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
                value = i;
        }
        d_resolutions.AddOptions(options);
        d_resolutions.value = value;
        d_resolutions.RefreshShownValue();

        t_fullScreen.isOn = Screen.fullScreenMode.Equals(FullScreenMode.FullScreenWindow) ? true : false;

        d_resolutions.onValueChanged.AddListener(DropdownValueChange);
        t_fullScreen.onValueChanged.AddListener(Apply);
    }

    private void DropdownValueChange(int _value)
    {
        Apply(_value);
    }

    private void Apply(int value)
    {
        Screen.SetResolution(resolutions[value].width, resolutions[value].height, 
            t_fullScreen.isOn ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);
        SoundManager.Instance.Play(SoundType.UIApply);
    }
    private void Apply(bool on)
    {
        Screen.SetResolution(Screen.width, Screen.height,
            on ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);
        SoundManager.Instance.Play(SoundType.UIApply);
    }

    private void SoundInit()
    {
        s_master.value = SoundManager.Instance.GetVolum("Master");
        void Master(float value)
        {
            SoundManager.Instance.SetMasterVolum(value);
        }
        s_master.onValueChanged.AddListener(Master);

        s_bgm.value = SoundManager.Instance.GetVolum("BGM");
        void BGM(float value)
        {
            SoundManager.Instance.SetBGMVolum(value);
        }
        s_bgm.onValueChanged.AddListener(BGM);

        s_effect.value = SoundManager.Instance.GetVolum("Effect");
        void Effect(float value)
        {
            SoundManager.Instance.SetEffectVolum(value);
        }
        s_effect.onValueChanged.AddListener(Effect);
    }
    private void SoundDeinit()
    {
        s_master.onValueChanged.RemoveAllListeners();
        s_bgm.onValueChanged.RemoveAllListeners();
        s_effect.onValueChanged.RemoveAllListeners();
    }
}
