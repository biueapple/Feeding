using System.IO;
using UnityEngine;

public class PlayerSetting : MonoBehaviour
{
    public static PlayerSetting Instance { get; private set; }

    public PlayerSettingData data;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        data = Load();
    }


    private string path => Path.Combine(Application.persistentDataPath, "settings.json");

    public void Save(PlayerSettingData data)
    {
        File.WriteAllText(path, JsonUtility.ToJson(data, true));
    }

    public PlayerSettingData Load()
    {
        if (!File.Exists(path)) return new PlayerSettingData();
        return JsonUtility.FromJson<PlayerSettingData>(File.ReadAllText(path));
    }

    private void OnApplicationQuit()
    {
        Save(data);
    }
}

[System.Serializable]
public class PlayerSettingData
{
    //general
    public Language language = Language.En;
    //graphic
    public Resolution resolution = new Resolution();
    public bool fullScreen = false;
    //sound
    public float masterVol = 0.8f;
    public float bgmVol = 0.8f;
    public float effectVol = 0.8f;
}
