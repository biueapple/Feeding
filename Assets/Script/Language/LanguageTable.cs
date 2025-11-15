using System.Collections.Generic;
using UnityEngine;

public enum Language
{
    Korean,
    En
}

[CreateAssetMenu(menuName = "Localization/LanguageTable")]
public class LanguageTable : ScriptableObject
{
    [System.Serializable]
    public class LanguageEntry
    {
        public Language language;
        [TextArea(2, 4)]
        public string text;
    }

    [SerializeField]
    private string key;
    public string Key => key;
    [SerializeField]
    private List<LanguageEntry> entry = new();
    public IReadOnlyList<LanguageEntry> Entry => entry;

    public string Get(Language language)
    {
        foreach(var e in entry)
        {
            if(e.language == language)
                return e.text;
        }
        return string.Empty;
    }
}