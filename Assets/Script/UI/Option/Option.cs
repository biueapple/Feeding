using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Option : MonoBehaviour
{
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
        //d_language.value = 0;
        d_language.onValueChanged.AddListener(OnChangeLanguage);
    }

    private void OnDisable()
    {
        b_general.onClick.RemoveAllListeners();
        b_graphics.onClick.RemoveAllListeners();
        b_sound.onClick.RemoveAllListeners();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
