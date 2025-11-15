using TMPro;
using UnityEngine;

public class LocalizedText : MonoBehaviour
{
    [SerializeField]
    private string key;

    [SerializeField]
    private TextMeshProUGUI text;

    private void Awake()
    {
        if (text == null) text = GetComponent<TextMeshProUGUI>();
    }

    public void OnEnable()
    {
        Refresh(LocalizationManager.Instance.Current);
        LocalizationManager.Instance.OnLanguageChange += Refresh;
    }

    public void OnDisable()
    {
        if(LocalizationManager.Instance != null)
            LocalizationManager.Instance.OnLanguageChange -= Refresh;
    }

    public void Refresh(Language language)
    {
        if(text != null)
            text.text = LocalizationManager.Instance.Get(key);
    }
}
