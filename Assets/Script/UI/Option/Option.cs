using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Option : MonoBehaviour
{
    // Close
    [SerializeField] public Button b_close;

    // Sections
    [SerializeField] private Button b_general;
    [SerializeField] private Button b_graphics;
    [SerializeField] private Button b_sound;

    [SerializeField] private GameObject p_general;
    [SerializeField] private GameObject p_graphics;
    [SerializeField] private GameObject p_sound;

    // General
    [SerializeField] private TMP_Dropdown d_language;

    // Graphics
    [SerializeField] private TMP_Dropdown d_resolutions;
    [SerializeField] private Toggle t_fullScreen;
    private List<Resolution> resolutions = new();

    // Sounds
    [SerializeField] private Slider s_master;
    [SerializeField] private Slider s_bgm;
    [SerializeField] private Slider s_effect;

    private void OnEnable()
    {
        InitUI();          // ① UI 기본 구조 & 리스너 세팅
        ApplyDataToUI();   // ② 저장된 설정값을 UI에 반영
    }

    private void OnDisable()
    {
        RemoveListeners();
    }

    // ------------------------------------------------------------
    #region ① Init UI (옵션 목록 구성 & 리스너 설정)
    // ------------------------------------------------------------
    private void InitUI()
    {
        InitClose();
        InitSections();
        InitGeneral();
        InitGraphics();
        InitSound();
    }

    private void InitClose()
    {
        b_close.onClick.AddListener(() =>
        {
            GameManager.Instance.OnOptionClose();
            SoundManager.Instance.Play(SoundType.UIClick);
        });
    }

    private void InitSections()
    {
        b_general.onClick.AddListener(() =>
        {
            p_general.SetActive(true);
            p_graphics.SetActive(false);
            p_sound.SetActive(false);
            SoundManager.Instance.Play(SoundType.UIApply);
        });

        b_graphics.onClick.AddListener(() =>
        {
            p_general.SetActive(false);
            p_graphics.SetActive(true);
            p_sound.SetActive(false);
            SoundManager.Instance.Play(SoundType.UIApply);
        });

        b_sound.onClick.AddListener(() =>
        {
            p_general.SetActive(false);
            p_graphics.SetActive(false);
            p_sound.SetActive(true);
            SoundManager.Instance.Play(SoundType.UIApply);
        });
    }

    private void InitGeneral()
    {
        d_language.ClearOptions();
        List<string> values = Enum.GetNames(typeof(Language)).ToList();
        d_language.AddOptions(values);

        d_language.onValueChanged.AddListener(value =>
        {
            LocalizationManager.Instance.Current = (Language)value;
            PlayerSetting.Instance.data.language = (Language)value;
            SoundManager.Instance.Play(SoundType.UIApply);
        });
    }

    private void InitGraphics()
    {
        // 해상도 목록 구성
        resolutions.Clear();
        foreach (var r in Screen.resolutions)
        {
            if (r.refreshRateRatio.value >= 60)
                resolutions.Add(r);
        }

        d_resolutions.options.Clear();
        List<string> options = new();
        foreach (var r in resolutions)
            options.Add($"{r.width} x {r.height}");
        d_resolutions.AddOptions(options);

        // 리스너 등록
        d_resolutions.onValueChanged.AddListener(index =>
        {
            Screen.SetResolution(resolutions[index].width, resolutions[index].height,
                t_fullScreen.isOn ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);

            PlayerSetting.Instance.data.resolution = resolutions[index];
            SoundManager.Instance.Play(SoundType.UIApply);
        });

        t_fullScreen.onValueChanged.AddListener(on =>
        {
            Screen.SetResolution(Screen.width, Screen.height,
                on ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);

            PlayerSetting.Instance.data.fullScreen = on;
            SoundManager.Instance.Play(SoundType.UIApply);
        });
    }

    private void InitSound()
    {
        s_master.onValueChanged.AddListener(value =>
        {
            SoundManager.Instance.SetMasterVolum(value);
            PlayerSetting.Instance.data.masterVol = value;
        });

        s_bgm.onValueChanged.AddListener(value =>
        {
            SoundManager.Instance.SetBGMVolum(value);
            PlayerSetting.Instance.data.bgmVol = value;
        });

        s_effect.onValueChanged.AddListener(value =>
        {
            SoundManager.Instance.SetEffectVolum(value);
            PlayerSetting.Instance.data.effectVol = value;
        });
    }
    #endregion

    // ------------------------------------------------------------
    #region ② ApplyDataToUI (저장된 설정 → UI 적용)
    // ------------------------------------------------------------
    private void ApplyDataToUI()
    {
        var data = PlayerSetting.Instance.data;

        // general
        d_language.value = (int)data.language;

        // graphics
        int index = resolutions.FindIndex(r =>
            r.width == data.resolution.width &&
            r.height == data.resolution.height);

        d_resolutions.value = (index >= 0) ? index : 0;
        t_fullScreen.isOn = data.fullScreen;

        // sounds
        s_master.value = data.masterVol;
        s_bgm.value = data.bgmVol;
        s_effect.value = data.effectVol;
    }
    #endregion

    // ------------------------------------------------------------
    #region Cleanup
    // ------------------------------------------------------------
    private void RemoveListeners()
    {
        b_close.onClick.RemoveAllListeners();
        b_general.onClick.RemoveAllListeners();
        b_graphics.onClick.RemoveAllListeners();
        b_sound.onClick.RemoveAllListeners();

        d_language.onValueChanged.RemoveAllListeners();
        d_resolutions.onValueChanged.RemoveAllListeners();
        t_fullScreen.onValueChanged.RemoveAllListeners();

        s_master.onValueChanged.RemoveAllListeners();
        s_bgm.onValueChanged.RemoveAllListeners();
        s_effect.onValueChanged.RemoveAllListeners();
    }
    #endregion
}
