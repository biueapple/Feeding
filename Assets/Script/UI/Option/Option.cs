using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Option : MonoBehaviour
{
    [SerializeField]
    private Button b_close;

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

    [SerializeField]
    private TMP_Dropdown d_language;

    private void OnEnable()
    {
        b_close.onClick.AddListener(GameManager.Instance.OnOptionClose);

        void General()
        {
            p_general.SetActive(true);
            p_graphics.SetActive(false);
            p_sound.SetActive(false);
        }
        void Graphics()
        {
            p_general.SetActive(false);
            p_graphics.SetActive(true);
            p_sound.SetActive(false);
        }
        void Sound()
        {
            p_general.SetActive(false);
            p_graphics.SetActive(false);
            p_sound.SetActive(true);
        }
        b_general.onClick.AddListener(General);
        b_graphics.onClick.AddListener(Graphics);
        b_sound.onClick.AddListener(Sound);

        void OnChangeLanguage(int value)
        {
            LocalizationManager.Instance.Current = (Language)value;
        }

        d_language.ClearOptions();
        List<string> values = Enum.GetNames(typeof(Language)).ToList();
        d_language.AddOptions(values);
        d_language.value = (int)LocalizationManager.Instance.Current;
        d_language.onValueChanged.AddListener(OnChangeLanguage);

        InitUI();
    }

    private void OnDisable()
    {
        b_close.onClick.RemoveAllListeners();
        b_general.onClick.RemoveAllListeners();
        b_graphics.onClick.RemoveAllListeners();
        b_sound.onClick.RemoveAllListeners();
        d_language.onValueChanged.RemoveAllListeners();
        d_resolutions.onValueChanged.RemoveAllListeners();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [SerializeField]
    private TMP_Dropdown d_resolutions;
    [SerializeField]
    private Toggle t_fullScreen;
    List<Resolution> resolutions = new();

    private void InitUI()
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
        for(int i = 0; i < resolutions.Count; i++)
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
    }

    public void DropdownValueChange(int _value)
    {
        Apply(_value);
    }

    private void Apply(int value)
    {
        Screen.SetResolution(resolutions[value].width, resolutions[value].height, 
            t_fullScreen.isOn ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);
        //SoundManager.instance.SFXCreate(SoundManager.Clips.ButtonClip, 1, 0);
    }
}
