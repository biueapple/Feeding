using System;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }
    [SerializeField]
    private Language current = Language.Korean;
    public Language Current { get { return current; } set { current = value; OnLanguageChange?.Invoke(value); } }

    public event Action<Language> OnLanguageChange;

    private void Awake()
    {
        Instance = this;
        tables = new();
        foreach(var t in languageTables)
        {
            tables[t.Key] = t;
        }
    }

    [SerializeField]
    private List<LanguageTable> languageTables;
    private Dictionary<string, LanguageTable> tables;

    public string Get(string key)
    {
        if (!tables.ContainsKey(key)) return string.Empty;
        return tables[key].Get(current);
    }
}
