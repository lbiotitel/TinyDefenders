using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


[System.Serializable]
public class GameSettingData
{
    public int langaugeIndex;
    public int resolutionIndex;
    public float backgroundSoundVolume;
    public float sfxSoundVolume;
    public int targetFrame;
    public int isWindowIndex;
    public int frameIndex;
    public int vSyncIndex;
}

public class GameSettingDefinition
{
    public static List<Vector2> resolutionList = new List<Vector2> {
        new Vector2(1280,720),
        new Vector2(1920,1080),
        new Vector2(2560,1440),
        new Vector2(3840,1440)
    };

    public static List<int> frameList = new List<int> {
        60,
        120,
        144,
        240
    };

    public static List<bool> windowModeList = new List<bool>
    {
        true,
        false
    };

    public static List<bool> vSyncList = new List<bool>
    {
        true,
        false
    };

    public static List<string> languageList = new List<string> {
        "English",
        "ÇÑ±¹¾î"
    };
}


[CreateAssetMenu(fileName = "GameSettingSO", menuName = "Scriptable Objects/GameSettingSO")]
public class GameSettingSO : ScriptableObject
{
    public int langaugeIndex;
    public float backgroundSoundVolume;
    public float sfxSoundVolume;
    public int targetFrame;
    public int resolutionIndex;
    public int isWindowIndex;
    public int frameIndex;
    public int vSyncIndex;

    public void SaveData()
    {
        GameSettingData gameSettingData = new GameSettingData
        {
            langaugeIndex = langaugeIndex,
            backgroundSoundVolume = this.backgroundSoundVolume,
            sfxSoundVolume = this.sfxSoundVolume,
            targetFrame = this.targetFrame,
            resolutionIndex = this.resolutionIndex,
            isWindowIndex = this.isWindowIndex,
            frameIndex = this.frameIndex,
            vSyncIndex = this.vSyncIndex
        };


        string json = JsonUtility.ToJson(gameSettingData);
        try
        {
            string path = Application.persistentDataPath + "/GameSettingData.json";

            if (File.Exists(path)) File.Delete(path);
            File.WriteAllText(path, json);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        Debug.Log(Application.persistentDataPath + "/GameSettingData.json");

    }

    public void LoadData()
    {
        string path = Application.persistentDataPath + "/GameSettingData.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            GameSettingData data = JsonUtility.FromJson<GameSettingData>(json);

            this.langaugeIndex = data.langaugeIndex;
            this.backgroundSoundVolume = data.backgroundSoundVolume;
            this.sfxSoundVolume = data.sfxSoundVolume;
            this.targetFrame = data.targetFrame;
            this.resolutionIndex = data.resolutionIndex;
            this.isWindowIndex = data.isWindowIndex;
            this.frameIndex = data.frameIndex;
            this.vSyncIndex = data.vSyncIndex;
        }
    }



}
